using Mozart.SeePlan.DataModel;
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
using Mozart.Simulation.Engine;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class DownControl
    {
        public IEnumerable<Mozart.SeePlan.DataModel.PMSchedule> GET_PMLIST_FROM_PM(PMEvents fe, AoEquipment aeqp, ref bool handled, IEnumerable<Mozart.SeePlan.DataModel.PMSchedule> prevReturnValue)
        {
            SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;

            List<PMSchedule> list = new List<PMSchedule>();
            DateTime startTime = InputMart.Instance.GlobalParameters.start_time;
            int period = InputMart.Instance.GlobalParameters.period;

            foreach (var pm in InputMart.Instance.PM_PLAN.Rows
                .OrderBy(r => r.START_TIME).ThenByDescending(r => r.PERIOD))
            {
                if (aeqp.EqpID == pm.EQP_ID)
                {
                    PMSchedule newPm = CreateHelper.CreatePMSchedule(pm.START_TIME, pm.PERIOD);
                    if (SimInit.Instance.OnUsePMSchedule(newPm, eqp))
                        list.Add(newPm);
                }
            }

            list.AddRange(eqp.BreakList);

            return list.OrderBy(r => r.StartTime).ThenByDescending(r => r.Duration);
        }

        //public IEnumerable<Mozart.SeePlan.DataModel.PMSchedule> GET_PMLIST_FROM_FACTORY_BREAK_INIT(PMEvents fe, AoEquipment aeqp, ref bool handled, IEnumerable<Mozart.SeePlan.DataModel.PMSchedule> prevReturnValue)
        //{
        //    SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;
        //    DateTime startTime = InputMart.Instance.GlobalParameters.start_time;
        //    int period = InputMart.Instance.GlobalParameters.period;

        //    DateTime pmStDt;
        //    DateTime beforDate;
        //    foreach (var fb in InputMart.Instance.FACTORY_BREAK.Rows
        //        .OrderBy(r => r.START_TIME).ThenByDescending(r => r.PERIOD))
        //    {
        //        if (fb.START_TIME < startTime)
        //        {
        //            pmStDt = startTime.Date.AddSeconds(fb.START_TIME.TimeOfDay.TotalSeconds);
        //            switch (fb.REPEAT_CYCLE)
        //            {
        //                case "DAY":
        //                    beforDate = pmStDt.AddDays(-1);
        //                    SAPSUtils.SetBeforePMSchedule(eqp, startTime, beforDate, fb);
        //                    break;
        //                case "WEEK":
        //                    int dayOfWeek = (int)fb.START_TIME.DayOfWeek;
        //                    beforDate = pmStDt.AddDays(dayOfWeek - (int)(pmStDt.DayOfWeek));
        //                    SAPSUtils.SetBeforePMSchedule(eqp, startTime, beforDate, fb);
        //                    break;
        //                case "MONTH":
        //                    beforDate = new DateTime(startTime.Year, startTime.Month, fb.START_TIME.Day).AddSeconds(fb.START_TIME.TimeOfDay.TotalSeconds);
        //                    SAPSUtils.SetBeforePMSchedule(eqp, startTime, beforDate, fb);
        //                    break;
        //                case "YEAR":
        //                    beforDate = new DateTime(startTime.Year, fb.START_TIME.Month, fb.START_TIME.Day).AddSeconds(fb.START_TIME.TimeOfDay.TotalSeconds);
        //                    SAPSUtils.SetBeforePMSchedule(eqp, startTime, beforDate, fb);
        //                    break;
        //                default:
        //                    if (fb.START_TIME < startTime && startTime <= fb.START_TIME.AddSeconds(fb.PERIOD))
        //                    {
        //                        PMSchedule newPm = CreateHelper.CreatePMSchedule(startTime, fb.PERIOD - (startTime - fb.START_TIME).TotalSeconds);
        //                        if (SimInit.Instance.OnUsePMSchedule(newPm, eqp))
        //                            eqp.BreakList.Add(newPm);
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //    return prevReturnValue;
        //}

        public IEnumerable<Mozart.SeePlan.DataModel.PMSchedule> GET_PMLIST_FROM_FACTORY_BREAK(PMEvents fe, AoEquipment aeqp, ref bool handled, IEnumerable<Mozart.SeePlan.DataModel.PMSchedule> prevReturnValue)
        {
            SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;

            List<PMSchedule> list = prevReturnValue.ToList();
            DateTime startTime = InputMart.Instance.GlobalParameters.start_time;
            int period = InputMart.Instance.GlobalParameters.period;

            DateTime pmStDt;
            for (DateTime tt = startTime; tt < startTime.AddDays(period); tt = tt.AddDays(1))
            {
                foreach (var fb in InputMart.Instance.FACTORY_BREAK.Rows
                    .OrderByDescending(r => r.START_TIME).ThenByDescending(r => r.PERIOD))
                {
                    if (fb.LINE_ID != eqp.LineID)
                        continue;

                    pmStDt = tt.Date.AddSeconds(fb.START_TIME.TimeOfDay.TotalSeconds);
                    if (pmStDt >= fb.START_TIME)
                    {
                        switch (fb.REPEAT_CYCLE)
                        {
                            case "DAY":
                                eqp = SimHelper.SetEqpBreakList(eqp, pmStDt, fb.PERIOD, fb.PERIOD_UNIT);
                                break;
                            case "WEEK":
                                if (tt.DayOfWeek == fb.START_TIME.DayOfWeek)
                                    eqp = SimHelper.SetEqpBreakList(eqp, pmStDt, fb.PERIOD, fb.PERIOD_UNIT);
                                break;
                            case "MONTH":
                                if (tt.ToString("dd") == fb.START_TIME.ToString("dd"))
                                    eqp = SimHelper.SetEqpBreakList(eqp, pmStDt, fb.PERIOD, fb.PERIOD_UNIT);
                                break;
                            case "YEAR":
                                if (tt.ToString("MMdd") == fb.START_TIME.ToString("MMdd"))
                                    eqp = SimHelper.SetEqpBreakList(eqp, pmStDt, fb.PERIOD, fb.PERIOD_UNIT);
                                break;
                            default:
                                if (tt.ToString("yyyyMMdd") == fb.START_TIME.ToString("yyyyMMdd"))
                                    eqp = SimHelper.SetEqpBreakList(eqp, fb.START_TIME, fb.PERIOD, fb.PERIOD_UNIT);
                                break;
                        }
                    }
                }
            }

            list.AddRange(eqp.BreakList);

            return list.OrderBy(r => r.StartTime).ThenByDescending(r => r.Duration);
        }

        public void ON_PMEVENT3(AoEquipment aeqp, Mozart.SeePlan.DataModel.PMSchedule fs, DownEventType det, ref bool handled)
        {
            if (aeqp.SetParallelChamberPM(fs, det))
                return;

            if (aeqp.SetChamberPM(fs, det))
                return;

            SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;

            if (det == DownEventType.Start)
            {
                aeqp.WriteHistory(LoadingStates.PM);
                aeqp.Loader.Block();

                if (eqp.Automation == Constants.MANUAL)
                {
                    var t = Time.FromHours(fs.Duration.TotalHours);
                    foreach (var proc in aeqp.Processes)
                    {
                        if (!proc.IsBlocked())
                        {
                            proc.Block();
                            proc.Break(t);
                        }
                    }
                }
                WriteHelper.WriteEqpDownPlan(aeqp, fs, eqp.BreakList.Contains(fs) ? "BREAK" : LoadingStates.PM.ToString());
            }
            else
            {
                aeqp.Loader.Unblock();
                aeqp.WriteHistoryAfterBreak();

                if (eqp.Automation == Constants.MANUAL)
                {
                    foreach (var proc in aeqp.Processes)
                    {
                        proc.Unblock();
                        proc.SetModified();
                    }
                }

                aeqp.SetModified();
            }
        }
    }
}