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
    public partial class ToolEvents
    {
        public void ON_SEIZED0(Mozart.SeePlan.Simulation.ToolSettings tool, AoEquipment eqp, ToolSettings last, ref bool handled)
        {
            if (tool.Items[0].ResourceKey != null)
                WriteHelper.WriteToolSeizeLog(tool, eqp, "SEIZE");
        }

        public void ON_RELEASED0(ToolSettings tool, AoEquipment eqp, ToolSettings chg, ref bool handled)
        {
            if (tool.Items[0].ResourceKey != null)
                WriteHelper.WriteToolSeizeLog(tool, eqp, "RELEASE");
        }
    }
}