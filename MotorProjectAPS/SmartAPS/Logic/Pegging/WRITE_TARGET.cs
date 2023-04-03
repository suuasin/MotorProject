using Mozart.SeePlan.DataModel;
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
    public partial class WRITE_TARGET
    {
        public void WRITE_TARGET0(Mozart.SeePlan.Pegging.PegPart pegPart, bool isOut, ref bool handled)
        {
            SmartAPSStep step = pegPart.CurrentStep as SmartAPSStep;

            foreach (PegTarget pt in pegPart.PegTargetList)
            {
                SmartAPSPegTarget target = pt as SmartAPSPegTarget;

                // CREATE 
                Outputs.STEP_TARGET st = CreateHelper.CreateStepTarget();

                st.VERSION_NO = ModelContext.Current.VersionNo;
                st.PRODUCT_ID = (pegPart as SmartAPSPegPart).Product.ProductID;
                st.STEP_ID = step.StepID;

                if (isOut)
                    st.OUT_QTY = pt.Qty;
                else
                    st.IN_QTY = pt.Qty;

                if (isOut) //step_target in_out 넣기
                    st.IN_OUT = "OUT";

                else
                    st.IN_OUT = "IN";

                    st.TARGET_DATE = pt.DueDate;
                st.MO_DEMAND_ID = target.Mo.DemandID;
                st.MO_PRODUCT_ID = target.Mo.MoMaster.Product.ProductID;
                st.MO_DUE_DATE = target.Mo.DueDate;

                st = PegOutputMapper.Instance.OnWriteStepTarget(st, target);

                st.STEP_TARGET_ID = Guid.NewGuid().ToString();
                OutputMart.Instance.STEP_TARGET.Add(st);
                
            }
        }

        public object GET_STEP_PLAN_KEY0(PegPart pegPart, ref bool handled, object prevReturnValue)
        {
            bool hardPegOpt = OptionHelper.UseHardPegging();

            if (hardPegOpt)
                return (pegPart as SmartAPSPegPart).DemandID;
            else
                return (pegPart as SmartAPSPegPart).Product;
        }

        public StepTarget CREATE_STEP_TARGET0(PegTarget pegTarget, object stepPlanKey, Step step, bool isRun, ref bool handled, StepTarget prevReturnValue)
        {
            var pt = pegTarget as SmartAPSPegTarget;
            var st = CreateHelper.CreateStepTarget(stepPlanKey, step, pt.Qty, pt.DueDate, isRun);
            st.MoPlan = pegTarget.MoPlan as SmartAPSMoPlan;

            return st;
        }
    }
}