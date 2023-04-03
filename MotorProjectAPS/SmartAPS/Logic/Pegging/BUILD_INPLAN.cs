using Mozart.SeePlan.Pegging;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Inputs;
using System.Collections.Generic;
using System.Linq;

namespace SmartAPS.Logic.Pegging
{
    [FeatureBind()]
    public partial class BUILD_INPLAN
    {
        public PegPart BUILD_IN_PLAN0(PegPart pegPart, ref bool handled, PegPart prevReturnValue)
        {
            MergedPegPart mPart = pegPart as MergedPegPart;
            List<PegPart> pegParts = new List<PegPart>();
            if (mPart == null)
                pegParts.Add(pegPart);
            else
                pegParts = mPart.Items;

            if (PegRun.Instance.OnGetInPlanOption())
            {
                SmartAPSInTarget inTarget;

                foreach (IN_PLAN inPlan in InputMart.Instance.IN_PLAN.Rows)
                {
                    if (string.IsNullOrEmpty(inPlan.PRODUCT_ID))
                    {
                        string item = $"BUILD_IN_PLAN0";
                        ErrorHelper.WriteError(Constants.PE0002, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_IN_PLAN", "product_id is empty!",
                            item, null, null);
                        continue;
                    }

                    SmartAPSProduct prod = InputMart.Instance.SmartAPSProductView.FindRows(inPlan.PRODUCT_ID).FirstOrDefault();
                    if (prod == null)
                    {
                        string item = $"BUILD_IN_PLAN0";
                        ErrorHelper.WriteError(Constants.PE0003, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_IN_PLAN", "product_id is not matching!",
                            item, null, null);
                        continue;
                    }

                    inTarget = CreateHelper.CreateInTarget();
                    inTarget.Product = prod;
                    inTarget.TargetDate = inPlan.PLAN_DATE;
                    inTarget.DemandID = inPlan.DEMAND_ID;
                    inTarget.TargetQty = inPlan.PLAN_QTY;
                    InputMart.Instance.SmartAPSInTarget.ImportRow(inTarget);
                }
            }
            else
            {
                foreach (SmartAPSPegPart pp in pegParts)
                {
                    InPlanHelper.BuildDefaultInPlan(pp);

                    // INPUT_BATCH_SIZE 주석 처리

                    //SmartAPSProduct prod = pp.Product as SmartAPSProduct;

                    //var inputBatchSize = PegPost.Instance.OnGetInputBatchSize(prod);

                    //if (inputBatchSize > 0)
                    //{
                    //    var lotSize = PegPost.Instance.OnGetLotSize(prod);
                    //    if (lotSize > 0)
                    //        inputBatchSize = Convert.ToInt32(Math.Floor(inputBatchSize / (double)lotSize) * lotSize);

                    //    InPlanHelper.BuildBatchSizeInPlan(prod, inputBatchSize);
                    //}
                }
            }


            foreach (SmartAPSInTarget it in InputMart.Instance.SmartAPSInTarget.Rows)
            {
                WriteHelper.WriteInputPlan(it);
            }

            foreach (SmartAPSPlanWip planWip in InputMart.Instance.SmartAPSPlanWip.Rows)
            {
                SmartAPSWipInfo wipinfo = planWip.Wip as SmartAPSWipInfo;
                if (InputMart.Instance.TempWipInfo.TryGetValue(wipinfo.LotID, out object value))
                {
                    List<SmartAPSWipInfo> tempWip = value as List<SmartAPSWipInfo>;

                    foreach (SmartAPSWipInfo temp in tempWip)
                    {
                        if (temp.DemandID != wipinfo.DemandID)
                        {
                            temp.LotID += "_" + CreateHelper.CreateSeq(temp.LotID);
                            temp.SplitCheck = true;
                            temp.ParentsLot = wipinfo;
                            InputMart.Instance.SmartAPSWipInfo.Rows.Add(temp);
                        }
                    }
                }
            }

            return pegPart;
        }
    }
}