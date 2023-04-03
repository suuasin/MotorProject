using Mozart.SeePlan.Simulation;
using Mozart.Simulation.Engine;
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
    public partial class SetupControl
    {
        public Time GET_SETUP_TIME0(Mozart.SeePlan.Simulation.AoEquipment aeqp, IHandlingBatch hb, ref bool handled, Time prevReturnValue)
        {
            return TimeHelper.GetSetupTime(aeqp, hb);
        }
    }
}