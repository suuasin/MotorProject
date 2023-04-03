using SmartAPS.Persists;
using SmartAPS.Outputs;
using SmartAPS.Inputs;
using Mozart.Common;
using Mozart.SeePlan.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class Route
    {
        public void PRE_PEGGING0(Mozart.SeePlan.Simulation.IHandlingBatch hb, List<Mozart.SeePlan.DataModel.PeggingInfo> infos, ref bool handled)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            if (lot.CurrentPlan != null && lot.CurrentPlan.PegInfoList.Count > 0)
            {
                string demandId = ((SmartAPSStepTarget)lot.CurrentPlan.PegInfoList.FirstOrDefault().StepTarget).MoPlan.DemandID;
                if (!string.IsNullOrWhiteSpace(demandId))
                    lot.OriginLineID = lot.LineID = InputMart.Instance.DEMAND.Rows.Where(x => x.DEMAND_ID == demandId).FirstOrDefault().LINE_ID;
            }
        }

        public IList<string> GET_LOADABLE_EQP_LIST0(DispatchingAgent da, IHandlingBatch hb, ref bool handled, IList<string> prevReturnValue)
        {
            return ArrangeHelper.GetLoadableEqpList(hb);
        }

        public IHandlingBatch[] STEP_CHANGE0(IHandlingBatch hb, DateTime now, ref bool handled, IHandlingBatch[] prevReturnValue)
        {
            var lot = hb as SmartAPSLot;

            var infos = InputMart.Instance.PRODUCT_ROUTE.Rows.Where(p => (
                p.FROM_PRODUCT_ID == lot.CurrentProductID &&
                p.STEP_ID == lot.CurrentStepID)
                );

            lot.Product = BopHelper.GetProduct(infos.FirstOrDefault().TO_PRODUCT_ID);

            return null;
        }

        public Step GET_NEXT_STEP1(ILot lot, LoadInfo loadInfo, Step step, DateTime now, ref bool handled, Step prevReturnValue)
        {
            Step next = step.GetDefaultNextStep();

            SmartAPSLot slot = lot as SmartAPSLot;
            SmartAPSWipInfo wip = slot.WipInfo as SmartAPSWipInfo;
            SmartAPSProduct prod = slot.Product as SmartAPSProduct;

            if (wip.OutQty > 0)
            {
                // 한 번 사용한 OutQty는 Reset 시켜준다. (다음번에 사용되지 않도록)
                if (next == null || next.Equals(wip.InitialStep) == false)
                    wip.OutQty = 0;
            }
            if (prod.ProductType == ProductType.PART && ConfigHelper.GetConfig<bool>(Constants.USE_PART_MERGELOT))
            {
                if (SimHelper.MergePartLot(slot, prod, step, next, false))
                    return null;
            }

            if (next != null)
            {
                if (SimHelper.IsFrimPlan(lot, lot.LotID, next.StepID))
                {
                    var firmPlan = SimHelper.GetFirmPlan(lot, next.StepID);
                    if (!string.IsNullOrEmpty(firmPlan.EqpID))
                    {
                        var aeqp = AoFactory.Current.GetEquipment(firmPlan.EqpID);

                        var firm_stime = firmPlan.StartTime;
                        var firm_etime = firmPlan.EndTime;

                        PeriodSection prePs = null;
                        foreach (var st in aeqp.DownManager.ScheduleTable)   //모든 Down되는 List
                        {
                            var ps = st.Tag as PeriodSection;

                            if (prePs == ps) continue;
                            if (prePs == null || prePs != ps) prePs = ps;

                            var ps_stime = ps.StartTime;
                            var ps_etime = ps.EndTime;

                            DateTime ts = SimHelper.GetIntersectionStartTime(firm_stime, firm_etime, ps_stime, ps_etime);
                            if (ts >= SimHelper.MinDateTime)
                            { // errlog
                                string item = $"Firm Plan {nameof(lot.LotID)} = {lot.LotID}, {nameof(next.StepID)} = {next.StepID}, StartTime = {ts}";
                                ErrorHelper.WriteError(Constants.SI0005, ErrorSeverity.WARNING, ts, "INVALID_FIRMPLAN", "Firm plan intersects with PM plan",
                                    item, null, null, null, null, lot.LotID);
                            }
                        }
                    }

                    var dt = SimHelper.GetFirmPlanStartTime(lot, lot.LotID, next.StepID);
                    if (AoFactory.Current.NowDT > dt)
                    {
                        string item = $"Firm Plan {nameof(lot.LotID)} = {lot.LotID}, {nameof(next.StepID)} = {next.StepID}, StartTime = {dt}";
                        ErrorHelper.WriteError(Constants.SI0003, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_FIRMPLAN", "Time out to firm plan start time",
                            item, null, null, null, null, lot.LotID);

                        return null;
                    }
                }
            }

            if (slot.LotID == "10W210825D001-005_2440_25")
            {

            }

            if (ConfigHelper.GetConfig<bool>(Constants.FW_PART_CHANGE))
            {
                if (next != null)
                {
                    var infos = InputMart.Instance.SmartAPSProductRouteFromStepDemandView.FindRows(slot.CurrentProductID, next.StepID, wip.DemandID);

                    if (infos != null && infos.Count() > 0)
                    {
                        slot.Product = BopHelper.GetProduct(infos.FirstOrDefault().ToProductId);
                    }
                }
                else
                {

                }
            }
            return next;
        }

        public LoadInfo CREATE_LOAD_INFO0(ILot lot, Step task, ref bool handled, LoadInfo prevReturnValue)
        {
            return CreateHelper.CreatePlanInfo(lot as SmartAPSLot, task as SmartAPSStep);
        }
    }
}