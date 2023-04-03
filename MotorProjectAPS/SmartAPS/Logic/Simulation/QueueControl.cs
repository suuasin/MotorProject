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

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class QueueControl
    {
        public bool IS_HOLD0(Mozart.SeePlan.Simulation.DispatchingAgent da, IHandlingBatch hb, ref bool handled, bool prevReturnValue)
        {
            var lot = hb.Sample as SmartAPSLot;

            // 확정 계획이 확정된 시간 전에 도착하면 확정 시간이 될 때까지 Hold 시킨다.
            if (SimHelper.IsFrimPlan(hb))
            {
                DateTime dt = SimHelper.GetFirmPlanStartTime(hb);
                DateTime newDt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                DateTime newNowDt = new DateTime(da.NowDT.Year, da.NowDT.Month, da.NowDT.Day, da.NowDT.Hour, da.NowDT.Minute, da.NowDT.Second);

                if (newDt > newNowDt)
                    return true;
            }
            else
            {
                var stepRoute = InputMart.Instance.STEP_ROUTEView.FindRows(lot.CurrentProcessID);
                string processType = stepRoute.Where(w => w.STEP_ID == lot.CurrentStepID).Select(r => r.PROCESS_TYPE).FirstOrDefault();

                if (processType.ToUpper() == "BUCKETING") //버켓 공정의 경우에도 FACTORY BREAK 를 고려해야한다. 
                {
                    SmartAPSFactoryBreakTime Bt = InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Where(x => x.StartTime < da.NowDT && x.EndTime > da.NowDT).FirstOrDefault();
                    if (Bt != null)
                        return true;
                    else
                    {
                        FACTORY_BREAK fb = InputMart.Instance.FACTORY_BREAK.Rows.Where(w => w.START_TIME < da.NowDT && w.START_TIME.AddSeconds(w.PERIOD) > da.NowDT).FirstOrDefault();
                        if (fb != null)
                            return true;
                    }
                }
            }
            return false;
        }

        public Time GET_HOLD_TIME0(DispatchingAgent da, IHandlingBatch hb, ref bool handled, Time prevReturnValue)
        { 
            //외주 공정(Bucket) 처리를 위한 로직. Bucket이 FactoryBreak(공장휴무) 끝나는 날까지 Hold 시켜준다. 

            DateTime dt = SimHelper.GetFirmPlanStartTime(hb);
            Time ht = Time.FromSeconds((dt - da.NowDT).TotalSeconds); //확정계획이 있을경우 확정계획까지 Hold 시킨 후 Release 한다. 
            
            return ht;
        }

        public bool INTERCEPT_IN0(DispatchingAgent da, IHandlingBatch hb, ref bool handled, bool prevReturnValue)
        {
            // wip 상태가 hold이면 intercept
            var lot = hb.Sample as SmartAPSLot;
            var wip = lot.WipInfo as SmartAPSWipInfo;

            if (wip.WipState == Constants.HOLD)
                return true;

            return false;
        }

        public bool INTERCEPT_IN1(DispatchingAgent da, IHandlingBatch hb, ref bool handled, bool prevReturnValue)
        {
            // merge intercept
            if (prevReturnValue)
                return prevReturnValue;

            var lot = hb.Sample as SmartAPSLot;

            if (lot.IsFirmPlanSplit)
            {
                List<ISimEntity> list = AoFactory.Current.Merge(hb);

                if (list != null)
                {
                    foreach (ISimEntity entity in list)
                    {
                        var agent = AoFactory.Current.FindDispatchingAgent(hb);
                        agent.ReEnter(entity, true);
                    }
                }

                return true;
            }

            return false;
        }

        public IList<string> FILTER_LOADABLE_EQP_LIST0(DispatchingAgent da, IList<string> src, IHandlingBatch hb, ref bool handled, IList<string> prevReturnValue)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;

            if (lot.PrepEffEndTime > AoFactory.Current.NowDT)
            {
                return new List<string> { lot.WipInfo.InitialEqp.EqpID };
            }

            return src;
        }

        public void ON_NOT_FOUND_DESTINATION0(DispatchingAgent da, IHandlingBatch hb, int destCount, ref bool handled)
        {
            string notFoundDestinationOption = SimRun.Instance.OnGetNotFoundDestOption(da, hb);

            if (notFoundDestinationOption == Constants.DUMMY)
            {
                da.Factory.AddToBucketer(hb);
            }
            else
            {
                SmartAPSLot lot = hb.Sample as SmartAPSLot;

                ErrorHelper.WriteError(Constants.SI0001, ErrorSeverity.WARNING, da.NowDT, "NO_EQP_ARRANGE", "There is no available EQP on EQP_ARRANGE", $"{nameof(lot.LotID)} = {lot.LotID}", (lot.WipInfo as SmartAPSWipInfo).DemandID, lot.Product.ProductID, lot.CurrentStep.StepID, null, lot.LotID);
            }
        }

        public Time GET_HOLD_TIME1(DispatchingAgent da, IHandlingBatch hb, ref bool handled, Time prevReturnValue)
        {
            if (!SimHelper.IsFrimPlan(hb)) //확정계획이 아닐경우 Factory Break 시간을 고려하여 Hold 시켜줘야한다. 
            {
                var Bt = InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Where(x => x.StartTime < da.NowDT && x.EndTime > da.NowDT).FirstOrDefault();
                var TimeA = Bt.EndTime - da.NowDT;
                return TimeA;
            }
            else
                return prevReturnValue;

        }
    }
}