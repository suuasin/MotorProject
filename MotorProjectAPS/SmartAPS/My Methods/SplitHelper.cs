using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Mozart.Common;
using Mozart.Collections;
using Mozart.Extensions;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.Persists;
using Mozart.SeePlan.Simulation;
using Mozart.SeePlan.DataModel;

namespace SmartAPS
{
    [FeatureBind()]
    public static partial class SplitHelper
    {
        public static List<IHandlingBatch> GetSplitLots(AoEquipment aeqp, IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            List<IHandlingBatch> lots = new List<IHandlingBatch>();

            SPLIT_INFO info = InputMart.Instance.SPLIT_INFOView.FindRows(lot.Product.ProductID, lot.CurrentStepID).FirstOrDefault();

            int unitQty = lot.UnitQty;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            switch (info.CRITERIA)
            {
                case Constants.QTY:
                    for (int i = 1; 0 < unitQty; i++)
                    {
                        int spanQty = unitQty > info.VALUE ? info.VALUE : unitQty;

                        SmartAPSLot newLot = SimHelper.SetSplitLot(aeqp, lot, wip, i, spanQty);
                        lots.Add(newLot);

                        unitQty -= spanQty;
                    }
                    InputMart.Instance.Lots.Remove(lot.LotID);
                    break;

                case Constants.CNT:
                    int splitValue = info.VALUE;
                    int seq = 1;

                    for (; 0 < splitValue; splitValue--)
                    {
                        int spanQty = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(unitQty / splitValue)));

                        SmartAPSLot newLot = SimHelper.SetSplitLot(aeqp, lot, wip, seq, Convert.ToInt32(spanQty));
                        lots.Add(newLot);

                        unitQty -= spanQty;
                        seq++;
                    }

                    InputMart.Instance.Lots.Remove(lot.LotID);
                    break;

                case Constants.PARTIAL_QTY:
                    SmartAPSLot partialLot = SimHelper.SetSplitLot(aeqp, lot, wip, 1, info.VALUE);
                    lots.Add(partialLot);

                    SmartAPSLot mainLot = SimHelper.SetSplitLot(aeqp, lot, wip, 2, unitQty - info.VALUE);
                    lots.Add(mainLot);

                    InputMart.Instance.Lots.Remove(lot.LotID);
                    break;

                default:
                    break;
            }

            return lots;
        }

        public static List<IHandlingBatch> GetFirmSplitLots(AoEquipment aeqp, IHandlingBatch hb, DateTime startTime)
        {
            List<IHandlingBatch> lots = new List<IHandlingBatch>();

            var span = startTime - AoFactory.Current.NowDT;
            var pti = TimeHelper.GetProcessTime(hb, aeqp);

            int spanQty = Convert.ToInt32(Math.Floor(span.TotalSeconds / pti.TactTime.TotalSeconds));

            if (spanQty > 0)
            {
                var lot = hb.Sample as SmartAPSLot;
                var lwip = lot.WipInfo as SmartAPSWipInfo;
                var wip = CreateHelper.CreateWipInfo(lot.Product as SmartAPSProduct, lot.UnitQty - spanQty, lwip.DemandID);
                wip.LotID = lot.LotID + "_SP";
                wip.InitialEqp = aeqp.Target as SmartAPSEqp;
                wip.WipEqpID = aeqp.EqpID;
                var nlot = CreateHelper.CreateLot(wip, false);

                lot.UnitQty = spanQty;
                lot.UnitQtyDouble = spanQty;

                lot.IsFirmPlanSplit = true;
                nlot.IsFirmPlanSplit = true;

                lot.ParentLot = lot;
                nlot.ParentLot = lot;

                lot.ChildLots.Add(lot);
                lot.ChildLots.Add(nlot);

                nlot.SetCurrentPlan(EntityControl.Instance.CreateLoadInfo(nlot, lot.CurrentStep));

                lots.Add(lot);
                lots.Add(nlot);
            }
            else
            {
                lots.Add(hb);
            }

            return lots;
        }
    }
}
