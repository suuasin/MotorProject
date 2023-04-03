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
    public partial class EqpEvents
    {
        public void LOADING_STATE_CHANGED0(Mozart.SeePlan.Simulation.AoEquipment aeqp, IHandlingBatch hb, LoadingStates state, ref bool handled)
        {
            SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;
            eqp.LoadingState = state;
            WriteHelper.WriteEqpPlan(aeqp, hb, state);
        }
    }
}