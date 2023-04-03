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
    public partial class WRITE_UNPEG
    {
        public void WRITE_UNPEG0(Mozart.SeePlan.Pegging.PegPart pegPart, ref bool handled)
        {
            foreach (SmartAPSPlanWip planWip in InputMart.Instance.SmartAPSPlanWip.Rows)
            {
                if (planWip.Qty == 0)
                    continue;

                SmartAPSWipInfo winfo = planWip.Wip as SmartAPSWipInfo;
                //이 부분 수정해야한다.REASON사유 수정 

                if (planWip.MapCount == 0)
                {
                    WriteHelper.WriteUnpegHistory(planWip, Constants.NO_TARGET, Constants.NO_TARGET);
                }
                else if (planWip.Qty > 0)
                {
                    WriteHelper.WriteUnpegHistory(planWip, Constants.EXCESS, Constants.EXCESS);
                }
            }
        }
    }
}