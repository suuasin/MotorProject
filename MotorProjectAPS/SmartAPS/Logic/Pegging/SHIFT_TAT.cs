using Mozart.SeePlan.Pegging;
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

namespace SmartAPS.Logic.Pegging
{
    [FeatureBind()]
    public partial class SHIFT_TAT
    {
        public TimeSpan GET_TAT0(Mozart.SeePlan.Pegging.PegPart pegPart, bool isRun, ref bool handled, TimeSpan prevReturnValue)
        {
            SmartAPSProduct prod = (pegPart as SmartAPSPegPart).Product as SmartAPSProduct;
            SmartAPSStep cstep = pegPart.CurrentStep as SmartAPSStep;

            return TimeHelper.GetTAT(cstep, prod, ModelContext.Current.StartTime, isRun);
        }

        public bool USE_TARGET_TAT0(PegPart pegPart, PegStage stage, bool isRun, ref bool handled, bool prevReturnValue)
        {
            return false;
        }
    }
}