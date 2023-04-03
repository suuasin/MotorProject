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
    public partial class PREPARE_TARGET
    {
        public PegPart PREPARE_TARGET0(PegPart pegPart, ref bool handled, PegPart prevReturnValue)
        {
            MergedPegPart mp = pegPart as MergedPegPart;
            
            foreach (SmartAPSMoMaster mm in InputMart.Instance.SmartAPSMoMaster.Rows)
            {
                SmartAPSPegPart pp = CreateHelper.CreatePegPart(mm);
                pp.CurrentStep = mp.CurrentStep;
                pp.DemandID = mm.DemandID;

                

                foreach (SmartAPSMoPlan mo in pp.MoMaster.MoPlanList)
                {
                    SmartAPSPegTarget pt = CreateHelper.CreatePegTarget(pp, mo);
                    pt.DemandID = mo.DemandID;

                    PegInit.Instance.OnCreatePegTarget(pp, mo, pt);
                    pp.AddPegTarget(pt);
                }

                PegInit.Instance.OnCreatePegPart(mm, pp);
                mp.Merge(pp);
            }

            return mp as PegPart;
        }
    }
}