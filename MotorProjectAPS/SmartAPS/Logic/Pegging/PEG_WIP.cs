using Mozart.SeePlan.Pegging.Rule;
using Mozart.SeePlan.Pegging;
using SmartAPS.Persists;
using SmartAPS.Outputs;
using SmartAPS.Inputs;
using SmartAPS.DataModel;
using Mozart.Task.Execution;
using Mozart.Extensions;
using Mozart.Collections;
using Mozart.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace SmartAPS.Logic.Pegging
{
    [FeatureBind()]
    public partial class PEG_WIP
    {
        public IList<Mozart.SeePlan.Pegging.IMaterial> GET_WIPS0(Mozart.SeePlan.Pegging.PegPart pegPart, bool isRun, ref bool handled, IList<IMaterial> prevReturnValue)
        {
            List<IMaterial> result = new List<IMaterial>();

            SmartAPSStep step = pegPart.CurrentStep as SmartAPSStep;
            //SmartAPSPegPart pp = pegPart as SmartAPSPegPart;
            

            bool hardPegOpt = OptionHelper.UseHardPegging();
            //InputMart.Instance.SmartAPSPlanWipView.Sort = "MapStep";
            var wips = InputMart.Instance.SmartAPSPlanWipView.FindRows(step);

            foreach (SmartAPSPlanWip wip in wips)
            {
                if (isRun && wip.Wip.CurrentState != Mozart.SeePlan.Simulation.EntityState.RUN)
                    continue;

                if (!isRun && wip.Wip.CurrentState == Mozart.SeePlan.Simulation.EntityState.RUN)
                    continue;

                if (hardPegOpt)
                {
                    if ((wip.Wip as SmartAPSWipInfo).DemandID != (pegPart as SmartAPSPegPart).DemandID)
                        continue;
                }
                else
                {
                    PEG_CONDITION PegCondition = null;
                    if (InputMart.Instance.PEG_CONDITIONView.FindRows("ALL").FirstOrDefault() != null || InputMart.Instance.PEG_CONDITION.Rows == null)
                        PegCondition = InputMart.Instance.PEG_CONDITIONView.FindRows("ALL").FirstOrDefault();
                    else
                        PegCondition = InputMart.Instance.PEG_CONDITIONView.FindRows(wip.Wip.WipStepID).FirstOrDefault();

                    if (PegCondition == null)
                    {
                        string item = $"{nameof(wip.Wip.Product.ProductID)} = '{wip.Wip.Product.ProductID}', {nameof(wip.Wip.WipStepID)} = '{wip.Wip.WipStepID}', {nameof(wip.Wip.LotID)} = '{wip.Wip.LotID}'";
                        ErrorHelper.WriteError(Constants.IN0032, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_PEG_CONDITION", "PEG CONDITION is empty", item,"", wip.Wip.Product.ProductID, wip.Wip.WipStepID,"", wip.Wip.LotID);
                        continue; 
                    }

                    if (PegCondition.ITEM.Equals(Constants.PRODUCT, StringComparison.OrdinalIgnoreCase) || PegCondition.ITEM == null)
                    {
                        if (wip.Wip.Product.ProductID != (pegPart as SmartAPSPegPart).Product.ProductID)
                            continue;
                    }
                    else if (PegCondition.ITEM.Equals(Constants.PROPERTY, StringComparison.OrdinalIgnoreCase))
                    {
                        string[] PropertyName = PegCondition.ITEM_DETAIL.Split(new[] { '@' }); // PEG CONDITION 의 속성값 1개 or n개 
                        SmartAPSProduct prod = wip.Wip.Product as SmartAPSProduct;
                        SmartAPSPegPart pp = pegPart as SmartAPSPegPart;
                        SmartAPSProduct pegPartProduct = pp.Product as SmartAPSProduct;

                        foreach (var property in PropertyName)
                        {
                            bool hasDetail = prod.Property.TryGetValue(property, out var WipPropertyValue); //wip product 의 속성값
                            if (hasDetail)
                            {
                                pegPartProduct.Property.TryGetValue(property, out var PegPartPropertyValue);

                                if (WipPropertyValue != PegPartPropertyValue)
                                    continue;
                            }
                        }
                    }
                }
                wip.MapCount++;
                result.Add(wip);
            }
            return result;
        }

        public int SORT_WIP0(Mozart.SeePlan.Pegging.Rule.MaterialInfo x, MaterialInfo y, ref bool handled, int prevReturnValue)
        {
            DateTime xTime = ((x.Material as SmartAPSPlanWip).Wip as SmartAPSWipInfo).PrepEffEndTime;
            DateTime yTime = ((y.Material as SmartAPSPlanWip).Wip as SmartAPSWipInfo).PrepEffEndTime;

            int retValue = xTime > yTime
                           ? -1
                           : xTime < yTime
                             ? 1
                             : 0;

            return retValue;
        }

        public bool CAN_PEG_MORE0(PegTarget target, IMaterial m, bool isRun, ref bool handled, bool prevReturnValue)
        {
            SmartAPSPlanWip info = m as SmartAPSPlanWip;
            if (info.Qty == 0)
                return false;

            return true;
        }

        public void WRITE_PEG0(PegTarget target, IMaterial m, double qty, ref bool handled)
        {
            PEG_HISTORY ph = CreateHelper.CreatePegHistory();
            SmartAPSStep step = target.PegPart.CurrentStep as SmartAPSStep;
            SmartAPSPlanWip info = m as SmartAPSPlanWip;
            SmartAPSWipInfo wipinfo = info.Wip as SmartAPSWipInfo;   ////DemandID 넘겨주는 코드 작성 1 (우연선임)
            SmartAPSMoMaster mm = target.PegPart.MoMaster as SmartAPSMoMaster;
            SmartAPSMoPlan mp = target.MoPlan as SmartAPSMoPlan;

            wipinfo.DemandID = mp.DemandID;
            SmartAPSWipInfo newWip = wipinfo.Clone() as SmartAPSWipInfo;
            newWip.UnitQty = qty;

            if(InputMart.Instance.TempWipInfo.TryGetValue(wipinfo.LotID, out object value))
            {
                List<SmartAPSWipInfo> tempWip = value as List<SmartAPSWipInfo>;
                tempWip.Add(newWip);
            }
            else
            {
                List<SmartAPSWipInfo> tempWip = new List<SmartAPSWipInfo>();
                tempWip.Add(newWip);
                InputMart.Instance.TempWipInfo.Add(wipinfo.LotID, tempWip);
            }

            ph.VERSION_NO = ModelContext.Current.VersionNo;
            ph.LINE_ID = (info.Wip as SmartAPSWipInfo).LineID;
            ph.STEP_ID = step.StepID;
            ph.LOT_ID = info.Wip.LotID;
            ph.PRODUCT_ID = info.Wip.Product.ProductID;
            if (InputMart.Instance.PEG_CONDITIONView.FindRows("ALL").FirstOrDefault() != null || InputMart.Instance.PEG_CONDITION.Rows == null)
                ph.ITEM_DETAIL = "PRODUCT";
            else
                ph.ITEM_DETAIL = InputMart.Instance.PEG_CONDITIONView.FindRows(step.StepID).FirstOrDefault().ITEM_DETAIL;

            ph.UNIT_QTY = info.Wip.UnitQty;
            ph.PEG_QTY = qty;
            ph.STATE = info.State;

            if (info.State == "WAIT" | info.State == "HOLD") //peg_history in_out 넣기
                ph.IN_OUT = "IN";
            else
                ph.IN_OUT = "OUT";


            ph.MO_PRODUCT_ID = (target.PegPart.MoMaster as SmartAPSMoMaster).Product.ProductID;
            ph.MO_DEMAND_ID = mp.DemandID;
            ph.MO_DUE_DATE = target.MoPlan.DueDate;

            ph = PegOutputMapper.Instance.OnWritePegHistory(ph, target, info);

            ph.PEG_HISTORY_ID = Guid.NewGuid().ToString();

            OutputMart.Instance.PEG_HISTORY.Add(ph);
        }
    }
}