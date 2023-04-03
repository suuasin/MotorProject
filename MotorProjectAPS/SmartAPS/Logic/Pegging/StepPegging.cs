using Mozart.SeePlan.Pegging;
using Mozart.SeePlan.DataModel;
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
    public partial class StepPegging
    {
        public Step GETLASTPEGGINGSTEP(Mozart.SeePlan.Pegging.PegPart pegPart)
        {
            SmartAPSMoMaster mm = pegPart.MoMaster as SmartAPSMoMaster;

            return mm.Product.Process.LastStep;
        }

        public Step GETPREVPEGGINGSTEP(PegPart pegPart, Step currentStep)
        {
            SmartAPSPegPart pp = pegPart as SmartAPSPegPart;
            if (pp.IsPartStepChanged)
            {
                pp.IsPartStepChanged = false;
                return pegPart.CurrentStep;
            }
            else
            {
                return pegPart.CurrentStep.GetDefaultPrevStep();
            }
        }
    }
}