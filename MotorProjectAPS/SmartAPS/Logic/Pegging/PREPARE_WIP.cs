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
    public partial class PREPARE_WIP
    {
        public PegPart PREPARE_WIP0(PegPart pegPart, ref bool handled, PegPart prevReturnValue)
        {
            foreach (SmartAPSWipInfo info in InputMart.Instance.SmartAPSWipInfo.Rows)
            {
                SmartAPSPlanWip wip = CreateHelper.CreatePlanWip(info);
                wip.Qty = info.UnitQty;
                wip.State = info.CurrentState.ToString();
                wip.MapStep = info.InitialStep;

                PegInit.Instance.OnCreatePlanWip(info, wip);
                InputMart.Instance.SmartAPSPlanWip.Rows.Add(wip);
            }

            return pegPart;
        }
    }
}