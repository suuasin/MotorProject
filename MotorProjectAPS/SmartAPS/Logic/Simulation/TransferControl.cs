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
using Mozart.SeePlan.DataModel;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class TransferControl
    {
        public Time GET_TRANSFER_TIME1(Mozart.SeePlan.Simulation.IHandlingBatch hb, ref bool handled, Time prevReturnValue)
        {
            var lot = hb.Sample as SmartAPSLot;
            var step = lot.CurrentStep as SmartAPSStep;

            if (step.StdStep.TransferTime == 0)
            {
                SeeplanConfiguration conf = ServiceLocator.Resolve<SeeplanConfiguration>();

                return Time.FromMinutes(conf.TransferTimeMinutes);
            }

            return Time.FromMinutes(step.StdStep.TransferTime);
        }
    }
}