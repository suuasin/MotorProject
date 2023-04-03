using SmartAPS.Persists;
using SmartAPS.Outputs;
using SmartAPS.Inputs;
using Mozart.Common;
using Mozart.Extensions;
using Mozart.SeePlan.Simulation;
using Mozart.Simulation.Engine;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class MergeControl
    {
        public object GET_MERGEABLE_KEY0(Mozart.Simulation.Engine.ISimEntity entity, ref bool handled, object prevReturnValue)
        {
            SmartAPSLot lot = entity as SmartAPSLot;
            SmartAPSWipInfo wipInfo = lot.WipInfo as SmartAPSWipInfo;

            if (lot == null)
                return null;

            if (lot.IsFirmPlanSplit)
                return lot.ParentLot;
            else
            {
                // TODO: Key를 Multi Key로 보내서 Lot이 다수의 Bom을 가지고 있을 경우를 처리 할 수 있도록 해야 한다.
                var info = InputMart.Instance.PRODUCT_ROUTE.Rows.Where(p => (
                        p.FROM_PRODUCT_ID == lot.CurrentProductID)
                        ).FirstOrDefault();

                if (info == null)
                    return null;

                return $"{info.TO_PRODUCT_ID}@{info.STEP_ID}@{wipInfo.DemandID}";
            }
        }

        public List<ISimEntity> MERGE0(object key, List<ISimEntity> entitys, ref bool handled, List<ISimEntity> prevReturnValue)
        {
            if (key == null)
                return null;
            if (key is string)
            {
                List<ISimEntity> result = new List<ISimEntity>();

                foreach (var entity in entitys)
                {
                    var lot = entity as SmartAPSLot;

                  

                    if (string.IsNullOrEmpty(lot.MergeKey))
                    {
                        lot.MergeKey = key.ToString();
                        lot.MergeInTime = AoFactory.Current.NowDT;
                        lot.OrgQty = lot.UnitQty;
                    }

                    if (InputMart.Instance.MergeLots.Contains(lot) == false)
                        InputMart.Instance.MergeLots.Add(lot);
                }

                string[] keys = key.ToString().Split('@');

                if (keys.Length < 3)
                    return null;

                string keyProdId = keys[0];
                string keyStepId = keys[1];
                string keyDemandId = keys[2];

                bool isMerge = SimRun.Instance.OnIsAddLotsAssyConstraint();

                var info = InputMart.Instance.PRODUCT_ROUTE.Rows.Where(p => (p.TO_PRODUCT_ID == keyProdId && p.STEP_ID == keyStepId));

                var prods = entitys.Select(r => (r as SmartAPSLot).Product.ProductID).Distinct();

                if (info.All(x => prods.Contains(x.FROM_PRODUCT_ID)) == false)
                    return null;

                var prod = InputMart.Instance.SmartAPSProductView.FindRows(keyProdId).FirstOrDefault();

                if (prod == null)
                    return null;

                var lotSize = SimInit.Instance.OnGetLotSize(prod);

                if (lotSize < 0)
                    return null;

                //IsMerge false
                //각 반제품별 가장 오래 머물던 Lot을 선택하여 조합을 꾸린다.
                //3개의 반제품 중 할 수 있는 최소값을 찾는다.
                List<ISimEntity> partEntitys = new List<ISimEntity>();
                if (isMerge)
                {
                    foreach (var route in info)
                    {
                        var entity = entitys.Where(r => (r as SmartAPSLot).Product.ProductID == route.FROM_PRODUCT_ID).OrderBy(r => (r as SmartAPSLot).MergeInTime).FirstOrDefault();

                        //만약 1개의 반제품이라도 없으면 Merge할 수 없다.
                        if (entity == null)
                            return null;

                        partEntitys.Add(entity);
                    }
                }
                else
                {
                    partEntitys.AddRange(entitys);
                }

                int minQty = int.MaxValue;

                

                //각 반제품 별 수량을 변환 비율에 맞게 구하여
                //Merge 할 수 있는 최소 값을 찾는다.
                foreach (var route in info)
                {
                    var fents = partEntitys.Where(r => (r as SmartAPSLot).Product.ProductID == route.FROM_PRODUCT_ID);

                    //만약 1개의 반제품이라도 없으면 Merge할 수 없다.
                    if (fents == null || fents.Count() == 0)
                        return null;

                    var rate = route.OUT_QTY / (double)route.IN_QTY;
                    var qty = Convert.ToInt32(Math.Floor(fents.Sum(r => (r as SmartAPSLot).UnitQty) * rate));

                    //만약 1개의 반제품이라도 없으면 Merge할 수 없다.
                    if (qty == 0)
                        return null;

                    if (minQty > qty)
                        minQty = qty;
                }

                // minQty / lotSize로 부모 Lot의 수를 구하여 부모Lot을 생성한다.

                int lotCnt = 1; //Convert.ToInt32(Math.Ceiling(minQty / (double)lotSize));
                for (int i = 0; i < lotCnt; i++)
                {
                    var wip = CreateHelper.CreateWipInfo(prod, minQty, keyDemandId);
                    // TODO: HardPegMode일 경우 DemaindID를 추가해야 한다.
                    SmartAPSLot plot = CreateHelper.CreateLot(wip, false);
                    plot.SetCurrentPlan(EntityControl.Instance.CreateLoadInfo(plot, plot.Process.FindStep(keyStepId)));
                    result.Add(plot);
                }

                //생성된 부모 Lot의 크기에 맞도록 자식 Lot을 가장 오래된 Lot부터 차감한다.
                foreach (var route in info)
                {
                    var fents = partEntitys.Where(r => (r as SmartAPSLot).Product.ProductID == route.FROM_PRODUCT_ID).OrderBy(r => (r as SmartAPSLot).MergeInTime);

                    var rate = route.IN_QTY / (double)route.OUT_QTY;
                    var rate2 = route.OUT_QTY / (double)route.IN_QTY;

                    foreach (var plot in result)
                    {
                        var splot = plot as SmartAPSLot;

                        var qty = Convert.ToInt32(Math.Ceiling(splot.UnitQty * rate));

                        foreach (var fent in fents)
                        {
                            var sfent = fent as SmartAPSLot;

                            if (sfent.UnitQty <= 0)
                                continue;

                            if (sfent.UnitQty >= qty)
                            {
                                WriteHelper.WriteProdChangeLog(sfent, splot, qty, splot.UnitQty, route.ROUTE_TYPE);
                                sfent.UnitQty -= qty;
                                sfent.MergeCnt++;
                                break;
                            }
                            else
                            {
                                WriteHelper.WriteProdChangeLog(sfent, splot, sfent.UnitQty, splot.UnitQty, route.ROUTE_TYPE);
                                qty -= sfent.UnitQty;
                                sfent.UnitQty = 0;
                                sfent.MergeCnt++;
                            }
                        }
                    }
                }

                return result;
            }

            return null;
        }

        public List<ISimEntity> MERGE1(object key, List<ISimEntity> entitys, ref bool handled, List<ISimEntity> prevReturnValue)
        {
            if (key is SmartAPSLot)
            {
                var plot = key as SmartAPSLot;

                List<ISimEntity> result = prevReturnValue == null ? new List<ISimEntity>() : prevReturnValue;

                if (entitys.Contains(plot) == false)
                    return null;

                if (plot.ChildLots.All(x => entitys.Contains(x)) == false)
                    return null;

                //모든 Child Lot이 같은 공정인지 검사
                int seq = 0;
                foreach (var child in plot.ChildLots)
                {
                    if (seq == 0)
                    {
                        seq = child.CurrentStep.Sequence;
                        continue;
                    }
                    else
                    {
                        if (seq != child.CurrentStep.Sequence)
                            return null;
                    }
                }

                foreach (var child in plot.ChildLots)
                {
                    if (entitys.Contains(child) == false)
                        continue;

                    if (child.Equals(plot))
                        continue;

                    plot.UnitQty += child.UnitQty;
                    child.UnitQty = 0;
                    InputMart.Instance.Lots.Remove(child.LotID);
                }

                result.Add(plot);
                plot.IsFirmPlanSplit = false;
                plot.ParentLot = null;
                plot.ChildLots.Clear();

                return result;
            }

            return prevReturnValue;
        }

        public List<ISimEntity> DISPOSE_ENTITIES0(List<ISimEntity> entitys, ref bool handled, List<ISimEntity> prevReturnValue)
        {
            List<ISimEntity> list = null;

            if (entitys.Where(r => (r as SmartAPSLot).IsFirmPlanSplit).Count() > 0)
            {
                list = entitys.Where(r => (r as SmartAPSLot).UnitQty == 0).ToList();
            }
            else
            {
                bool isSplit = SimRun.Instance.OnIsPartProdSplit();

                if (isSplit == false)
                    list = entitys.Where(r => (r as SmartAPSLot).UnitQty == 0 && (r as SmartAPSLot).MergeCnt > 0).ToList();
                else
                    list = entitys.Where(r => (r as SmartAPSLot).UnitQty == 0).ToList();

                foreach (var entity in list)
                {
                    var lot = entity as SmartAPSLot;

                    if (lot.UnitQty == 0 && InputMart.Instance.MergeLots.Contains(lot))
                        InputMart.Instance.MergeLots.Remove(lot);
                }
            }

            return list;
        }
    }
}
