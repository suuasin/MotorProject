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
    public partial class CustomEvents_DayChange
    {
        public bool RUN(Mozart.SeePlan.Simulation.ICalendarEvent evt, ICalendarEventManager cm)
        {
            Stopwatch watch = InputMart.Instance.SimDayWatch;
            string elapsedTime = watch.Elapsed.ToString().Substring(0, 8);
            
            int totalDay = GlobalParameters.Instance.period;
            string simulationDay = string.Format("{0:00}", ++InputMart.Instance.PlanDays);
            string head = string.Format("{0}/{1}", simulationDay, totalDay);
            
            string simulationTime = DateUtility.DbToString(AoFactory.Current.NowDT);
            string currentTime = DateUtility.DbToString(DateTime.Now);
            Logger.MonitorInfo("{0}. Simulation Time: {1} -  Current Time: {2} - Elapsed Time: {3}", head, simulationTime, currentTime, elapsedTime);

            watch.Reset();
            watch.Start();

            return true;
        }
    }
}