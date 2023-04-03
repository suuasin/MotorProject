using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Mozart.Common;
using Mozart.Collections;
using Mozart.Extensions;
using Mozart.SeePlan;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.Persists;
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class InPlanHelper
    {
        public static void BuildDefaultInPlan(SmartAPSPegPart pp)
        {
            //bool hardPegOpt = OptionHelper.UseHardPegging();

            SmartAPSProduct prod = pp.Product as SmartAPSProduct;
            foreach (SmartAPSPegTarget target in pp.PegTargetList)
            {
                if (target.Qty > 0)
                {
                    //var dt = target.CalcDate.SplitDate();
                    var dt = target.CalcDate;

                    SmartAPSInTarget inTarget = null;

                    //if (hardPegOpt)
                    //    inTarget = InputMart.Instance.SmartAPSInTargetViewByDemand.FindRows(prod, dt, target.DemandID).FirstOrDefault();
                    //else
                    //    inTarget = InputMart.Instance.SmartAPSInTargetView.FindRows(prod, dt).FirstOrDefault();
                    inTarget = InputMart.Instance.SmartAPSInTargetViewByDemand.FindRows(prod, dt, target.DemandID).FirstOrDefault();

                    if (inTarget == null)
                    {
                        inTarget = CreateHelper.CreateInTarget();
                        inTarget.Product = prod;
                        inTarget.TargetDate = dt;
                        //inTarget.DemandID = hardPegOpt ? target.DemandID : string.Empty;
                        inTarget.DemandID = target.DemandID;
                        inTarget.MoProductID = target.Mo.ProductID;
                        InputMart.Instance.SmartAPSInTarget.ImportRow(inTarget);
                    }

                    inTarget.TargetQty += (int)target.CalcQty;
                }
            }
        }

        //public static void BuildBatchSizeInPlan(SmartAPSProduct prod, int inputBatchSize)
        //{
        //    var targets = InputMart.Instance.SmartAPSInTargetViewByProd.FindRows(prod);

        //    if (targets != null && targets.Count() > 0)
        //    {
        //        SplitInPlanByBatchSize(targets.ToList(), inputBatchSize);
        //    }
        //}

        //private static void SplitInPlanByBatchSize(List<SmartAPSInTarget> targets, int inputBatchSize)
        //{
        //    DateTime dt;
        //    int cnt;
        //    int remain;
        //    bool hardPegOpt = OptionHelper.UseHardPegging();

        //    foreach (var it in targets.OrderByDescending(r => r.TargetDate))
        //    {
        //        if (inputBatchSize < it.TargetQty)
        //        {
        //            cnt = 1;
        //            remain = it.TargetQty - inputBatchSize;
        //            it.TargetQty -= remain;

        //            while (remain > 0)
        //            {
        //                dt = it.TargetDate.AddDays(-cnt);

        //                cnt++;

        //                SmartAPSInTarget inTarget = null;

        //                if (hardPegOpt)
        //                    inTarget = InputMart.Instance.SmartAPSInTargetViewByDemand.FindRows(it.Product, dt, it.DemandID).FirstOrDefault();
        //                else
        //                    inTarget = InputMart.Instance.SmartAPSInTargetView.FindRows(it.Product, dt).FirstOrDefault();

        //                if (inTarget == null)
        //                {
        //                    inTarget = CreateHelper.CreateInTarget();
        //                    inTarget.Product = it.Product;
        //                    inTarget.TargetDate = dt;
        //                    inTarget.DemandID = it.DemandID;
        //                    InputMart.Instance.SmartAPSInTarget.ImportRow(inTarget);
        //                }

        //                if (inTarget.TargetQty >= inputBatchSize)
        //                    continue;

        //                var oldTargetQty = inTarget.TargetQty;

        //                inTarget.TargetQty += (remain + oldTargetQty <= inputBatchSize) ? remain - oldTargetQty : inputBatchSize - oldTargetQty;
        //                remain -= (remain + oldTargetQty <= inputBatchSize) ? remain - oldTargetQty : inputBatchSize - oldTargetQty;
        //            }
        //        }
        //    }
        //}
    }
}
