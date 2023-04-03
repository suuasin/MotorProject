using Mozart.Simulation.Engine;
using Mozart.SeePlan.Simulation;
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
using Mozart.SeePlan.DataModel;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class FilterControl
    {
        public bool IS_PREVENT_DISPATCHING0(Mozart.SeePlan.Simulation.AoEquipment aeqp, IList<IHandlingBatch> wips, Mozart.Simulation.Engine.Time waitDownTime, ref bool handled, bool prevReturnValue)
        {
            // dispatch이전에 prevent
            var lot = SimHelper.GetFirmPlanLot(aeqp, true, FirmType.NORMAL);
            var prep = wips.Where(x => (x as SmartAPSLot).PrepEffEndTime >= AoFactory.Current.NowDT);

            if (lot != null)
            {
                TimeSpan span = SimHelper.GetFirmPlanStartTime(lot) - aeqp.NowDT;
                ProcTimeInfo pti;
                IHandlingBatch hb;

                if (prep.Count() == 0)
                {
                    for (int i = wips.Count - 1; i >= 0; i--)
                    {
                        hb = wips[i];

                        if (SimHelper.IsFrimPlan(hb))
                            continue;

                        pti = TimeHelper.GetProcessTime(hb, aeqp);

                        if (pti.FlowTime.TotalSeconds * hb.UnitQty > span.TotalSeconds)
                            wips.Remove(hb);
                    }
                    return wips.Count == 0;
                }
                else
                {
                    for (int i = wips.Count - 1; i >= 0; i--)
                    {
                        hb = wips[i];

                        if (SimHelper.IsFrimPlan(hb))
                            continue;

                        pti = TimeHelper.GetProcessTime(hb, aeqp);

                        if (pti.FlowTime.TotalSeconds * hb.UnitQty > span.TotalSeconds)
                            wips.Remove(hb);

                        if ((hb as SmartAPSLot).PrepEffEndTime < AoFactory.Current.NowDT)
                            wips.Remove(hb);
                    }
                    return wips.Count == 0;
                }
            }
            else if (prep.Count() != 0)
            {
                IHandlingBatch hb;
                for (int i = wips.Count - 1; i >= 0; i--)
                {
                    hb = wips[i];
                    if ((hb as SmartAPSLot).PrepEffEndTime < AoFactory.Current.NowDT)
                        wips.Remove(hb);
                }
                return wips.Count() == 0;
            }

            return false;
        }

        public IHandlingBatch[] CHECK_RESERVATION0(DispatchingAgent da, AoEquipment aeqp, ref bool handled, IHandlingBatch[] prevReturnValue)
        {
            //확정 계획이 있으면 확정계획을 예약된 Lot으로 등록하여 예약된 Lot부터 진행하도록 한다.
            var lot = SimHelper.GetFirmPlanLot(aeqp, false);

            if (lot != null)
            {
                var plan = SimHelper.GetFirmPlan(lot);

                if (plan != null)
                {
                    List<IHandlingBatch> list = new List<IHandlingBatch>();

                    list.Add(lot);

                    //Split 된 계획은 확정계획이 끝나면 바로 시작할 수 있도록 확정계획 뒤에 예약해준다.
                    foreach (var splitLot in plan.SplitLots)
                    {
                        list.Add(splitLot);
                    }

                    return list.ToArray();
                }
            }

            return null;
        }

        public bool IS_LOADABLE0(AoEquipment eqp, IHandlingBatch hb, IDispatchContext ctx, ref bool handled, bool prevReturnValue)
        {
            // material 관련 filter

            var lot = hb.Sample as SmartAPSLot;
            var plan = lot.CurrentPlan as SmartAPSPlanInfo;

            //필요한 모든 자재가 있는지 확인한다.
            foreach (var mb in plan.MatBom)
            {
                //자재가 무재한 인 것이 있으면 필터하지 않아도 된다.
                var isInf = mb.MatPlans.Where(r => r.IsInfinite).Count() > 0;

                if (isInf == false)
                {
                    //자재 수량 확인
                    var matSum = mb.MatPlans.Where(r => r.ReplenishDate <= eqp.NowDT).Sum(r => r.Qty);

                    //사용 가능한 자재 수량이 매칭되는 MatBom의 수량보다 적으면 Filter
                    if (matSum < mb.CompQty * lot.UnitQty)
                    {
                        //Filter 정보를 Material Manager에 저장하여 추후 계획 자재가 있을 경우 재 배치 되도록 한다.
                        MaterialManager.Instance.AddMatFilter(mb, hb, eqp);
                        //Eqp Dispatch Log에 Filter Log를 추가해준다.
                        eqp.EqpDispatchInfo.AddFilteredWipInfo(hb, "NO_MATERIAL");

                        return false;
                    }
                }
            }
            return true;
        }

        public bool IS_LOADABLE1(AoEquipment eqp, IHandlingBatch hb, IDispatchContext ctx, ref bool handled, bool prevReturnValue)
        {          
            if (prevReturnValue)
            {
                var lot = hb.Sample as SmartAPSLot;
                var plan = lot.CurrentPlan as SmartAPSPlanInfo;
                STEP_TAT sTat = InputMart.Instance.STEP_TAT.Rows.Where(w => w.PRODUCT_ID.Equals(lot.CurrentProductID) && w.STEP_ID.Equals(lot.CurrentStepID)).FirstOrDefault();
                if (sTat == null)
                    return prevReturnValue;

                DateTime wipStartTime = eqp.NowDT;
                DateTime wipEndTime = eqp.NowDT.AddMinutes(sTat.TOTAL_TAT);

                foreach (PM_PLAN pp in InputMart.Instance.PM_PLAN.Rows.Where(w => w.EQP_ID.Equals(eqp.EqpID) && wipStartTime <= w.START_TIME.AddSeconds(w.PERIOD) && wipEndTime >= w.START_TIME))
                {
                    DateTime ppEndTime = pp.START_TIME.AddSeconds(pp.PERIOD);
                    List<FACTORY_BREAK> fList = InputMart.Instance.FACTORY_BREAK.Rows.Where(w => w.START_TIME < ppEndTime && w.START_TIME.AddSeconds(w.PERIOD) > pp.START_TIME).ToList();

                    if (fList.Any())
                    {
                        foreach (FACTORY_BREAK fb in fList)
                        {
                            DateTime fbEndTime = fb.START_TIME.AddSeconds(fb.PERIOD);
                            if (pp.START_TIME < fb.START_TIME)
                                wipEndTime = wipEndTime.AddSeconds((fb.START_TIME - pp.START_TIME).TotalSeconds);

                            if (ppEndTime > fbEndTime)
                                wipEndTime = wipEndTime.AddSeconds((ppEndTime - fbEndTime).TotalSeconds);
                        }
                    }
                    else
                        wipEndTime = wipEndTime.AddSeconds(pp.PERIOD);
                }

                foreach (FACTORY_BREAK fb in InputMart.Instance.FACTORY_BREAK.Rows.Where(w => wipStartTime <= w.START_TIME.AddSeconds(w.PERIOD)
                                                                            && wipEndTime >= w.START_TIME))
                    wipEndTime = wipEndTime.AddSeconds(fb.PERIOD); 

                if (InputMart.Instance.FIRM_PLAN.Rows.Where(w =>
                                                            w.EQP_ID == eqp.EqpID
                                                            && w.START_TIME < wipEndTime
                                                            && w.END_TIME > wipStartTime).Any())
                    return false;
            }

            return prevReturnValue;
        }

        public IList<IHandlingBatch> DO_FILTER1(AoEquipment eqp, IList<IHandlingBatch> wips, IDispatchContext ctx, ref bool handled, IList<IHandlingBatch> prevReturnValue)
        {
            var filterControl = DispatchFilterControl.Instance;

            filterControl.SetFilterContext(eqp, wips, ctx);

            //if (eqp.EqpID == "IB-01")
            //    Console.Write("A");

            for (int i = wips.Count - 1; i >= 0; i--)
            {
                var hb = wips[i];

                filterControl.SetLotCondition(eqp, hb, ctx);

                if (filterControl.CheckSecondResouce(eqp, hb, ctx) == false)
                {
                    wips.RemoveAt(i);
                    continue;
                }

                if (filterControl.CheckSetupCrew(eqp, hb, ctx) == false)
                {
                    wips.RemoveAt(i);
                    continue;
                }

                var filterKey = filterControl.GetFilterSetKey(eqp, hb, ctx);
                if (string.IsNullOrEmpty(filterKey))
                {
                    if (filterControl.IsLoadable(eqp, hb, ctx) == false)
                    {
                        wips.RemoveAt(i);
                        continue;
                    }
                }
                else
                {
                    if (AoFactory.Current.Filters.Filter(filterKey, hb, AoFactory.Current.NowDT, eqp, ctx))
                    {
                        wips.RemoveAt(i);
                        continue;
                    }
                }
            }

            return wips;
        }
    }
}