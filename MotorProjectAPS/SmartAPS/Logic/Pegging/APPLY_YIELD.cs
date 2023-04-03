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
    public partial class APPLY_YIELD
    {
        public double GET_YIELD0(Mozart.SeePlan.Pegging.PegPart pegPart, ref bool handled, double prevReturnValue)
        {
            SmartAPSPegPart pp = pegPart as SmartAPSPegPart;
            SmartAPSProduct prod = pp.Product as SmartAPSProduct;
            SmartAPSStep step = pegPart.CurrentStep as SmartAPSStep;
            SmartAPSStdStep std = step.StdStep;

            double yield = ConfigHelper.GetConfig<double>(Constants.DEFAULT_STEP_YIELD);
            if (ConfigHelper.GetConfig<bool>(Constants.USE_STD_STEP_YIELD_INFO))
            {
                yield = std.StepYield;
            }
            else
            {
                var stepYield = InputMart.Instance.STEP_YIELDView.FindRows(prod.ProductID, step.StepID).FirstOrDefault();

                if (stepYield != null)
                    yield = stepYield.YIELD_VALUE;
                else
                    yield = std.StepYield;
            }

            if (yield < 0 || yield > 1)
                return 1;

            return yield;
        }

        public bool USE_TARGET_YIELD0(PegPart pegPart, PegStage stage, ref bool handled, bool prevReturnValue)
        {
            return false;
        }
    }
}