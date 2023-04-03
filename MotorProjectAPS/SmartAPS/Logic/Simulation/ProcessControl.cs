using SmartAPS.Persists;
using SmartAPS.Inputs;
using Mozart.SeePlan.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.Simulation.Engine;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class ProcessControl
    {
        public void ON_CUSTOM_LOAD0(Mozart.SeePlan.Simulation.AoEquipment aeqp, IHandlingBatch hb, ref bool handled)
        {//misc에서 useCustomLoad체크 하면 타기

            //Split 확정 계획이 있을 경우 계획을 진행할 수 있는 많큼 Split하여 로딩한다.
            var lot = SimHelper.GetFirmPlanLot(aeqp, true, FirmType.SPLIT);

            if (lot != null)
            {
                var plan = SimHelper.GetFirmPlan(lot);

                foreach (var sl in SplitHelper.GetFirmSplitLots(aeqp, hb, SimHelper.GetFirmPlanStartTime(lot)))
                {
                    if (sl.Equals(hb.Sample))
                    {
                        aeqp.Loader.Take(sl);
                    }
                    else
                    {
                        //Split 된 계획은 확정계획에 등록했다가 확정계획이 끝나면 바로 시작할 수 있도록 한다.
                        plan.SplitLots.Add(sl);
                    }
                }
            }
            else if (SimHelper.IsSplitLot(hb))
            {
                foreach (var item in SplitHelper.GetSplitLots(aeqp, hb))
                {
                    aeqp.Loader.Take(item);
                }
            }
        }

        public bool IS_NEED_SETUP0(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, bool prevReturnValue)
        {
            string option = SimRun.Instance.OnGetSetupOption(aeqp, hb);

            if (option == Constants.NONE)
                return false;
            else
                return SimRun.Instance.OnIsNeedSetup(aeqp, hb);
        }

        public ProcTimeInfo GET_PROCESS_TIME0(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, ProcTimeInfo prevReturnValue)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            return TimeHelper.GetProcessTime(hb, aeqp);
        }

        public double GET_PROCESS_UNIT_SIZE1(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, double prevReturnValue)
        {
            return SimHelper.GetUnitQty(hb, aeqp);
        }

        public void ON_TRACK_OUT0(AoEquipment aeqp, IHandlingBatch hb, ref bool handled)
        {
            SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            IList<ISimEntity> contents;
            if (hb.Contents == null)
                contents = new List<ISimEntity> { lot };
            else
                contents = hb.Contents;

            foreach (SmartAPSLot slot in contents)
            {
                EQP_PLAN plan = OutputMart.Instance.EQP_PLAN.FindBuffer(ModelContext.Current.VersionNo, eqp.EqpID, slot.LotID, slot.CurrentStep.StepID, Constants.BUSY);

                if (plan != null)
                    plan.EQP_END_TIME = AoFactory.Current.NowDT;
            }

            lot.PrepEffEndTime = DateTime.MinValue;
        }

        public IHandlingBatch[] INTERCEPT_MOVE0(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, IHandlingBatch[] prevReturnValue)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            AoFactory aoFactory = aeqp.Factory;

            var fromprod = InputMart.Instance.PRODUCT_ROUTEViewFromProductID.FindRows(lot.CurrentProductID);
            var toprod = InputMart.Instance.PRODUCT_ROUTEViewToProductID.FindRows(fromprod.FirstOrDefault() != null ? fromprod.FirstOrDefault().TO_PRODUCT_ID : string.Empty);
            IHandlingBatch[] lots = new IHandlingBatch[1];
            lots[0] = hb;

            if (toprod.Count() <= 1)
                return lots;

            List<ISimEntity> simList = aoFactory.Merge(lot);
            if (simList == null)
                return null;

            foreach (ISimEntity item in simList)
            {
                var agent =aoFactory.FindDispatchingAgent(aeqp);
                agent.ReEnter(item);
            }

            return null;
        }
    }
}