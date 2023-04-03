using Mozart.SeePlan.Simulation;
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
using System.Diagnostics;
using Mozart.SeePlan;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class CustomEvents_HourChange
    {
        public bool RUN(Mozart.SeePlan.Simulation.ICalendarEvent evt, ICalendarEventManager cm)
        {
            //var grp = InputMart.Instance.Lots.Values.Where(r => r.CurrentStep != null && r.IsFinished == false
            //   ).GroupBy(r =>
            //       new
            //       {
            //           r.CurrentProductID,
            //           r.CurrentProcessID,
            //           r.CurrentStepID,
            //           DemandID = String.IsNullOrWhiteSpace((r.WipInfo as SmartAPSWipInfo).DemandID)
            //           ? r.CurrentPlan.PegInfoList == null
            //               ? ""
            //               : r.CurrentPlan.PegInfoList.FirstOrDefault().StepTarget.CastAs<SmartAPSStepTarget>().MoPlan.DemandID
            //           : (r.WipInfo as SmartAPSWipInfo).DemandID
            //       });

            //foreach (var row in grp)
            //{
            //    Outputs.STEP_WIP wip = CreateHelper.CreateStepWip();

            //    wip.VERSION_NO = ModelContext.Current.VersionNo;
            //    wip.LINE_ID = row.Select(x => x.LineID).FirstOrDefault();
            //    wip.PROCESS_ID = row.Key.CurrentProcessID;
            //    wip.PRODUCT_ID = row.Key.CurrentProductID;
            //    wip.STEP_ID = row.Key.CurrentStepID;
            //    wip.WAIT_UNIT_QTY = row.Where(r => r.CurrentState == EntityState.WAIT).Sum(r => r.UnitQty);
            //    wip.RUN_UNIT_QTY = row.Where(r => r.CurrentState == EntityState.RUN).Sum(r => r.UnitQty);
            //    wip.WAIT_LOT_QTY = row.Where(r => r.CurrentState == EntityState.WAIT).Count();
            //    wip.RUN_LOT_QTY = row.Where(r => r.CurrentState == EntityState.RUN).Count();
            //    wip.TARGET_DATE = AoFactory.Current.NowDT;
            //    wip.STEP_WIP_ID = Guid.NewGuid().ToString();
            //    wip.DEMAND_ID = row.Key.DemandID;

            //    OutputMart.Instance.STEP_WIP.Add(wip);
            //}
            

            Stopwatch watch = InputMart.Instance.SimHourWatch;
            string elapsedTime = watch.Elapsed.ToString().Substring(0, 8);
            int totalHour = GlobalParameters.Instance.period * 24;
            string currentHour = string.Format("{0:00}", ++InputMart.Instance.PlanHours); 
            string head = string.Format("{0}/{1}", currentHour, totalHour);
            string simulationTime = DateUtility.DbToString(AoFactory.Current.NowDT);
            string currentTime = DateUtility.DbToString(DateTime.Now); 

            //Logger.MonitorInfo("{0}. Simulation Time: {1} -  Current Time: {2} - Elapsed Time: {3}", head, simulationTime, currentTime, elapsedTime);

            watch.Reset();
            watch.Start();

            return true;
        }
    }
}