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
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class ArrangeHelper
    {
        public static IList<string> GetLoadableEqpList(IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;

            //확정 계획은 지정된 장비에서만 할 수 있다.
            if (SimHelper.IsFrimPlan(hb))
                return new List<string> { SimHelper.GetFirmPlanEqpId(lot) };

            //확정 계획으로 인하여 Split된 Lot은 같은 장비에서 해야 한다.
            if (lot.IsFirmPlanSplit && lot.WipInfo.InitialEqp != null)
                return new List<string> { lot.WipInfo.InitialEqp.EqpID };

            //var arrs = InputMart.Instance.EQP_ARRANGEStepView.FindRows(lot.CurrentProductID, lot.CurrentProcessID, lot.CurrentStepID);
            var arrs = InputMart.Instance.EQP_ARRANGEProdStep.FindRows(lot.CurrentProductID, lot.CurrentStepID);

            //foreach (var arr in arrs)
            //{
            //    if (arr.EQP_ID == "IB-01")
            //        return arrs.Select(r => r.EQP_ID).Distinct().ToList();
            //}

            return arrs.Select(r => r.EQP_ID).Distinct().ToList();

        }
    }
}
