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
    public partial class Misc
    {
        public bool USE_CUSTOM_LOAD0(Mozart.SeePlan.Simulation.AoEquipment aeqp, IHandlingBatch hb, ref bool handled, bool prevReturnValue)
        {
            //Split 확정 계획이 있을 경우 계획을 Split할 수 있다.
            var lot = SimHelper.GetFirmPlanLot(aeqp, true, FirmType.SPLIT);

            if (lot != null)
            {
                TimeSpan span = SimHelper.GetFirmPlanStartTime(lot) - aeqp.NowDT;
                ProcTimeInfo pti = TimeHelper.GetProcessTime(hb, aeqp);
                if ((pti.FlowTime.TotalSeconds * hb.UnitQty) > span.TotalSeconds)
                    return true;
                else
                    return false;
            }

            return SimHelper.IsSplitLot(hb);
        }
    }
}