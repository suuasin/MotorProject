using Mozart.SeePlan.DataModel;
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
using System.Text;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class DispatcherControl
    {
        public Type GET_LOT_BATCH_TYPE0(ref bool handled, Type prevReturnValue)
        {
            return typeof(SmartAPSLotBatch);
        }

        public IList<Mozart.SeePlan.Simulation.IHandlingBatch> EVALUATE1(Mozart.SeePlan.Simulation.DispatcherBase db, IList<IHandlingBatch> wips, IDispatchContext ctx, ref bool handled, IList<IHandlingBatch> prevReturnValue)
        {
            // dispatching rule에 의해 정렬해서 넘겨주는 method
            if (db is FifoDispatcher)
            {
                string ft = (db.Eqp.Target as SmartAPSEqp).FifoType;

                switch (ft)
                {
                    case Constants.EDD: // LPST순
                        (wips as List<IHandlingBatch>).Sort(
                            delegate (IHandlingBatch x, IHandlingBatch y)
                            {
                                //return WeightValue.Zero; 
                                SmartAPSLot lotX = x.Sample as SmartAPSLot;
                                SmartAPSLot lotY = y.Sample as SmartAPSLot;

                                DateTime dueDateX = SimHelper.MaxDateTime;
                                DateTime dueDateY = SimHelper.MaxDateTime;

                                // currentPlan : lot의 현재상태 pegInfoList (pegging결과를 object로 가지고있음)
                                if (lotX.CurrentPlan.PegInfoList != null && lotX.CurrentPlan.PegInfoList.Count > 0)
                                {
                                    var peg = lotX.CurrentPlan.PegInfoList.Where(r => r.Qty > 0).OrderBy(r => r.StepTarget.DueDate).FirstOrDefault();
                                    dueDateX = peg == null ? SimHelper.MaxDateTime : peg.StepTarget.DueDate;
                                }

                                if (lotY.CurrentPlan.PegInfoList != null && lotY.CurrentPlan.PegInfoList.Count > 0)
                                {
                                    var peg = lotY.CurrentPlan.PegInfoList.Where(r => r.Qty > 0).OrderBy(r => r.StepTarget.DueDate).FirstOrDefault();
                                    dueDateY = lotY.CurrentPlan.PegInfoList[0].StepTarget.DueDate;
                                }

                                return dueDateX.CompareTo(dueDateY);
                            }
                        );

                        return wips;
                    case Constants.SPT:
                        (wips as List<IHandlingBatch>).Sort(
                           delegate (IHandlingBatch x, IHandlingBatch y)
                           {
                               ProcTimeInfo infoX = TimeHelper.GetProcessTime(x, db.Eqp);
                               ProcTimeInfo infoY = TimeHelper.GetProcessTime(y, db.Eqp);

                               double timeX = infoX.FlowTime.TotalSeconds * x.UnitQty;
                               double timeY = infoY.FlowTime.TotalSeconds * y.UnitQty;

                               return timeX.CompareTo(timeY);
                           }
                       );

                        return wips;
                    case Constants.LPT:
                        (wips as List<IHandlingBatch>).Sort(
                            delegate (IHandlingBatch x, IHandlingBatch y)
                            {
                                ProcTimeInfo infoX = TimeHelper.GetProcessTime(x, db.Eqp);
                                ProcTimeInfo infoY = TimeHelper.GetProcessTime(y, db.Eqp);

                                double timeX = infoX.FlowTime.TotalSeconds * x.UnitQty;
                                double timeY = infoY.FlowTime.TotalSeconds * y.UnitQty;

                                return timeY.CompareTo(timeX);
                            }
                        );

                        return wips;
                    default:
                        return wips;
                }
            }

            if (db.Comparer == null)
                return wips;

            if (db.Eqp.Preset.FactorList.Where(x => x.Name == Constants.MIN_SETUP || x.Name == Constants.MIN_SETUP_PROPERTY).Count() > 0)
            {
                double maxSetupTime = 0f;
                var eqp = db.Eqp as AoEquipment;
                foreach (IHandlingBatch wip in wips)
                {
                    string option = SimRun.Instance.OnGetSetupOption(eqp, wip);
                    if (option != Constants.NONE)
                        maxSetupTime = maxSetupTime > TimeHelper.GetSetupTime(eqp, wip).TotalSeconds
                                    ? maxSetupTime
                                    : TimeHelper.GetSetupTime(eqp, wip).TotalSeconds;
                }
                ctx.Set("maxSetupTime", maxSetupTime);
            }

            return db.WeightEval.Evaluate(wips, ctx);
        }

        public bool IS_WRITE_DISPATCH_LOG0(AoEquipment aeqp, ref bool handled, bool prevReturnValue)
        {
            return true;
        }

        public string ADD_DISPATCH_WIP_LOG1(Mozart.SeePlan.DataModel.Resource eqp, EntityDispatchInfo info, ILot lot, WeightPreset wp, ref bool handled, string prevReturnValue)
        {
            var slot = lot as SmartAPSLot;
            StringBuilder sb = new StringBuilder();

            sb.Append(slot.LotID);
            sb.AppendFormat("/{0}", slot.CurrentProductID);
            sb.AppendFormat("/{0}", slot.CurrentStepID);
            sb.AppendFormat("/{0}", slot.UnitQty);

            if (wp != null)
            {
                foreach (var factor in wp.FactorList)
                {
                    var value = slot.WeightInfo.GetValue(factor);
                    sb.Append("/");
                    sb.Append(value);
                }
            }

            return sb.ToString();
        }

        public void WRITE_DISPATCH_LOG0(DispatchingAgent da, EqpDispatchInfo info, ref bool handled)
        {
            var log = CreateHelper.CreateEqpDispatchLog();

            log.VERSION_NO = ModelContext.Current.VersionNo;
            log.EQP_ID = info.TargetEqp.ResID;
            log.DISPATCHING_TIME = info.DispatchTime.ToString("yyyyMMddHHmmss");
            if (info.TargetEqp.Preset != null)
            {
                log.PRESET_ID = info.TargetEqp.Preset.Name;
            }
            else
            {
                if (info.TargetEqp.DispatcherType == DispatcherType.Fifo)
                {
                    var eqp = info.TargetEqp as SmartAPSEqp;
                    log.PRESET_ID = eqp.FifoType;
                }
            }

            foreach (EntityFilterInfo f in info.FilterInfos.Values)
            {
                log.FILTERED_WIP_CNT += f.FilterWips.Count;
            }

            log.INIT_WIP_CNT = info.Batches.Count + log.FILTERED_WIP_CNT;
            log.FILTERED_WIP_LOG = info.FilteredWipLog;
            log.SELECTED_WIP_CNT = info.SelectedWipLog.Split(';').Length;
            log.SELECTED_WIP = info.SelectedWipLog;
            log.DISPATCH_WIP_LOG = info.DispatchWipLog;

            log.EQP_DISPATCH_LOG_ID = Guid.NewGuid().ToString();

            OutputMart.Instance.EQP_DISPATCH_LOG.Add(log);
        }

        public void ON_DISPATCHED0(DispatchingAgent da, AoEquipment aeqp, IHandlingBatch[] wips, ref bool handled)
        {
            var lot = wips[0].Sample as SmartAPSLot;
            var plan = lot.CurrentPlan as SmartAPSPlanInfo;

            foreach (var mb in plan.MatBom)
            {
                var isInf = mb.MatPlans.Where(r => r.IsInfinite).Count() > 0;
                var compQty = mb.CompQty * lot.UnitQty;

                if (isInf == false && compQty > 0)
                {
                    //자재 중 현재 시간에 사용 가능한 자재를 가져온다.
                    //가능한 자재중 빨리 들어온 자재, 수량이 적은 순으로 정렬 한다.
                    var mats = mb.MatPlans.Where(r => r.ReplenishDate <= da.NowDT && r.Qty > 0)
                        .OrderBy(r => r.Qty)
                        .OrderBy(r => r.ReplenishDate);

                    foreach (var mat in mats)
                    {
                        if (mat.Qty >= compQty)
                        {
                            WriteHelper.WriteMaterialHistory(mb, mat.MaterialID, lot.LotID, mat.Qty, compQty, mat.MatType.ToString());
                            mat.Qty -= compQty;
                            compQty = 0;
                            //자재 사용량이 모두 찼으면 종료
                            break;
                        }
                        else
                        {
                            WriteHelper.WriteMaterialHistory(mb, mat.MaterialID, lot.LotID, mat.Qty, mat.Qty, mat.MatType.ToString());
                            compQty -= mat.Qty;
                            mat.Qty = 0;
                        }
                    }
                }
            }

            //Filter 시 등록 되었던 Filter 정보를 모두 해제해준다.
            MaterialManager.Instance.RemoveFilter(wips[0], lot.CurrentStep);
        }

        public void ON_DISPATCHED1(DispatchingAgent da, AoEquipment aeqp, IHandlingBatch[] wips, ref bool handled)
        {
            if (wips.Count() > 0)
            {
                var lot = wips[0].Sample as SmartAPSLot;

                if (lot.ToolSettings != null && lot.ToolSettings.Items.Count() > 0)
                {
                    var plan = lot.CurrentPlan as SmartAPSPlanInfo;
                    if (!string.IsNullOrEmpty(lot.ToolSettings.Items[0].ResourceKey as string))
                    {
                        plan.ToolID = lot.ToolSettings.Items[0].ResourceKey.ToString();
                    }
                }
            }
        }
    }
}