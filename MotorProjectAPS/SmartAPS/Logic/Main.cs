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

namespace SmartAPS.Logic
{
    [FeatureBind()]
    public partial class Main
    {
        string _ApsLogID;
        object _objRequestText;
        public void RUN1(ModelContext context, ref bool handled)
        {
            var handler = ServiceLocator.Resolve<TaskControl>();
            var modules = context.GetOrderedExecutionModules().ToArray();
            Logger.StartHandler(context.GetLog(MConstants.LoggerExecution));
            try
            {
                SAPSUtils.ResisterType();

                int count = modules.Length;
                for (int i = 0; i < count; i++)
                {
                    var module = modules[i];
                    if (!handler.CanExecuteModule(module, context))
                        continue;
                    if (!handler.IsContinueExecution(module, context))
                        break;
                    Logger.MonitorInfo(module.Name + " Start.");
                    module.Execute(context);
                    if (context.HasErrors)
                        break;
                    Logger.MonitorInfo(module.Name + " End.");
                }
            }
            finally
            {
                Logger.EndHandler();
            }
        }

        public void SETUP_QUERY_ARGS1(ModelTask task, ModelContext context, ref bool handled)
        {
            //모니터링으로 실행할때만 db에 저장 
            if (GlobalParameters.Instance.is_anomaly)
            {
                ModelContext.Current.Outputs.UseDatabase = true;
            }
            //monitor 트리거에서 들어오는 값 체크 
            if (ModelContext.Current.Arguments.ContainsKey("LOG_TEXT"))
                _objRequestText = ModelContext.Current.Arguments["LOG_TEXT"];
            else
                _objRequestText = null;
            //List<APS_LOG_HISTORY> alhTest = CreateHelper.CreateApsLogHistory();

            List<APS_LOG_HISTORY> alhList = new List<APS_LOG_HISTORY>();
            List<APS_STATUS_MASTER> asmList;
            APS_LOG_HISTORY alh = CreateHelper.CreateApsLogHistory();
            DateTime beginDateTime = DateTime.Now;
            _ApsLogID = AnomalyDetectionHelper.GetLogID(beginDateTime, AnomalyDetectionHelper.COMPANY, AnomalyDetectionHelper.SITE, AnomalyDetectionHelper.SYSTEM);
            string logText = AnomalyDetectionHelper.GetLogText(_objRequestText as string, AnomalyDetectionHelper.BEGIN);

            //version_no 셋팅 
            context.QueryArgs = AnomalyDetectionHelper.SetVersionNo(_ApsLogID, task);

            //APS_LOG_HISTORY output mart에 추가 
            alh.LOG_ID = _ApsLogID;
            alh.LOG_DATETIME = beginDateTime;
            alh.LOG_TEXT = logText;
            OutputMart.Instance.APS_LOG_HISTORY.Add(alh);
            alhList.Add(alh);

            //APS_LOG_HISTORY DB에 저장  (코드가 실행되는 시점의 시간을 로그로 기록) 
            var dtItem = ModelContext.Current.Outputs.GetItem("APS_LOG_HISTORY");
            dtItem.SetActiveAction("Insert");
            Dictionary<object, object> insertArgs = AnomalyDetectionHelper.GetLogArgs(_ApsLogID, beginDateTime, logText);
            int insert;
            try
            {
                insert = ModelContext.Current.Outputs.Save<Outputs.APS_LOG_HISTORY>("APS_LOG_HISTORY", alhList, insertArgs);
                dtItem.ActiveAction = null;
            }
            catch (Exception)
            {
                return;
            }

            //APS_STATUS_MASTER DB에 저장 및 OUTPUT MART에 추가 
            asmList = AnomalyDetectionHelper.CreateApsStatusMaster(AnomalyDetectionHelper.BEGIN, _ApsLogID, beginDateTime, AnomalyDetectionHelper.SUCCESS);
            var dtItem2 = ModelContext.Current.Outputs.GetItem("APS_STATUS_MASTER");
            List<APS_STATUS_MASTER> asmListDelete = new List<APS_STATUS_MASTER>();
            asmListDelete.Add(asmList[0]);
            dtItem2.SetActiveAction("Delete");
            int delete = ModelContext.Current.Outputs.Save<Outputs.APS_STATUS_MASTER>("APS_STATUS_MASTER", asmListDelete, null);
            dtItem2.ActiveAction = null;


            dtItem2.SetActiveAction("Insert");
            foreach (var item in asmList)
            {
                OutputMart.Instance.APS_STATUS_MASTER.Add(item);
                List<APS_STATUS_MASTER> asmList2 = new List<APS_STATUS_MASTER>();

                asmList2.Add(item);
                Dictionary<object, object> insertStatusArgs = AnomalyDetectionHelper.GetStatusArgs(item.APS_CODE, item.APS_VALUE);
                insert = ModelContext.Current.Outputs.Save<Outputs.APS_STATUS_MASTER>("APS_STATUS_MASTER", asmList2, insertStatusArgs);
            }
            dtItem2.ActiveAction = null;

            /*
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("version_no", ModelContext.Current.Version.ToString());
                context.QueryArgs = dict;
            */
        }

        public void RUN_ANOMALY(ModelContext context, ref bool handled)
        {
            List<APS_LOG_HISTORY> alhList = new List<APS_LOG_HISTORY>();
            APS_LOG_HISTORY alh = CreateHelper.CreateApsLogHistory();
            List<APS_STATUS_MASTER> asmList;
            DateTime runDateTime = DateTime.Now;
            string logText = AnomalyDetectionHelper.GetLogText(_objRequestText as string, AnomalyDetectionHelper.RUN);
            int insert;


            alh.LOG_ID = _ApsLogID;
            alh.LOG_DATETIME = runDateTime;
            alh.LOG_TEXT = logText;
            OutputMart.Instance.APS_LOG_HISTORY.Add(alh);
            alhList.Add(alh);


            var dtItem = ModelContext.Current.Outputs.GetItem("APS_LOG_HISTORY");
            dtItem.SetActiveAction("Insert");
            Dictionary<object, object> insertArgs = AnomalyDetectionHelper.GetLogArgs(_ApsLogID, runDateTime, logText);

            try
            {
                insert = ModelContext.Current.Outputs.Save<Outputs.APS_LOG_HISTORY>("APS_LOG_HISTORY", alhList, insertArgs);
                dtItem.ActiveAction = null;
            }
            catch (Exception)
            {
                return;
            }


            var dtItem2 = ModelContext.Current.Outputs.GetItem("APS_STATUS_MASTER");
            asmList = AnomalyDetectionHelper.CreateApsStatusMaster(AnomalyDetectionHelper.RUN, _ApsLogID, runDateTime, AnomalyDetectionHelper.SUCCESS);

            List<APS_STATUS_MASTER> asmListDelete = new List<APS_STATUS_MASTER>();
            asmListDelete.Add(asmList[0]);


            dtItem2.SetActiveAction("Delete");
            int delete = ModelContext.Current.Outputs.Save<Outputs.APS_STATUS_MASTER>("APS_STATUS_MASTER", asmListDelete, null);
            dtItem2.ActiveAction = null;


            dtItem2.SetActiveAction("Insert");
            foreach (var item in asmList)
            {
                OutputMart.Instance.APS_STATUS_MASTER.Add(item);
                List<APS_STATUS_MASTER> asmList2 = new List<APS_STATUS_MASTER>();

                asmList2.Add(item);
                Dictionary<object, object> insertStatusArgs = AnomalyDetectionHelper.GetStatusArgs(item.APS_CODE, item.APS_VALUE);
                insert = ModelContext.Current.Outputs.Save<Outputs.APS_STATUS_MASTER>("APS_STATUS_MASTER", asmList2, insertStatusArgs);
            }
            dtItem2.ActiveAction = null;
        }

        public void DONE_ANOMALY(ModelContext context, ref bool handled)
        {
            List<APS_LOG_HISTORY> alhList = new List<APS_LOG_HISTORY>();
            List<APS_LOG_HISTORY> alhList2 = new List<APS_LOG_HISTORY>();
            List<APS_STATUS_MASTER> asmList;
            APS_LOG_HISTORY alh = CreateHelper.CreateApsLogHistory();
            APS_LOG_HISTORY alh2 = CreateHelper.CreateApsLogHistory();
            DateTime doneDateTime = DateTime.Now;
            Random rand = new Random();
            double number = rand.NextDouble();
            doneDateTime = doneDateTime.AddSeconds(number);
            string logText = AnomalyDetectionHelper.GetLogText(_objRequestText as string, AnomalyDetectionHelper.DONE);
            string logText2 = "[ " + _ApsLogID + " ] 완료";
            int insert;

            alh.LOG_ID = _ApsLogID;
            alh.LOG_DATETIME = doneDateTime;
            alh.LOG_TEXT = logText;
            OutputMart.Instance.APS_LOG_HISTORY.Add(alh);
            alhList.Add(alh);

            alh2.LOG_ID = _ApsLogID;
            alh2.LOG_DATETIME = doneDateTime.AddSeconds(0.002d);
            alh2.LOG_TEXT = logText2;
            OutputMart.Instance.APS_LOG_HISTORY.Add(alh2);
            alhList2.Add(alh2);

            var dtItem = ModelContext.Current.Outputs.GetItem("APS_LOG_HISTORY");
            dtItem.SetActiveAction("Insert");
            Dictionary<object, object> insertArgs = AnomalyDetectionHelper.GetLogArgs(_ApsLogID, doneDateTime, logText);
            Dictionary<object, object> insertArgs2 = AnomalyDetectionHelper.GetLogArgs(_ApsLogID, alh2.LOG_DATETIME, logText2);

            try
            {
                insert = ModelContext.Current.Outputs.Save<Outputs.APS_LOG_HISTORY>("APS_LOG_HISTORY", alhList, insertArgs);
                insert = ModelContext.Current.Outputs.Save<Outputs.APS_LOG_HISTORY>("APS_LOG_HISTORY", alhList2, insertArgs2);
            }
            catch (Exception)
            {
                return;
            }

            dtItem.ActiveAction = null;


            var dtItem2 = ModelContext.Current.Outputs.GetItem("APS_STATUS_MASTER");
            asmList = AnomalyDetectionHelper.CreateApsStatusMaster(AnomalyDetectionHelper.DONE, _ApsLogID, doneDateTime, AnomalyDetectionHelper.SUCCESS);

            List<APS_STATUS_MASTER> asmListDelete = new List<APS_STATUS_MASTER>();
            asmListDelete.Add(asmList[0]);
            dtItem2.SetActiveAction("Delete");
            int delete = ModelContext.Current.Outputs.Save<Outputs.APS_STATUS_MASTER>("APS_STATUS_MASTER", asmListDelete, null);
            dtItem2.ActiveAction = null;

            dtItem2.SetActiveAction("Insert");
            foreach (var item in asmList)
            {
                OutputMart.Instance.APS_STATUS_MASTER.Add(item);
                List<APS_STATUS_MASTER> asmList2 = new List<APS_STATUS_MASTER>();

                asmList2.Add(item);
                Dictionary<object, object> insertStatusArgs = AnomalyDetectionHelper.GetStatusArgs(item.APS_CODE, item.APS_VALUE);
                insert = ModelContext.Current.Outputs.Save<Outputs.APS_STATUS_MASTER>("APS_STATUS_MASTER", asmList2, insertStatusArgs);
            }
            dtItem2.ActiveAction = null;
        }

        public void SHUTDOWN0(ModelTask task, ref bool handled)
        {
/*
            RUN_HISTORY rh = CreateHelper.CreateRunHistory();
            rh.VERSION_NO = ModelContext.Current.VersionNo;
            rh.INPUT_VER = "-";
            rh.SIM_START_TIME = InputMart.Instance.GlobalParameters.start_time;
            rh.ENG_START_TIME = ModelContext.Current.Version.RunTime;
            rh.ENG_END_TIME = DateTime.Now;
            rh.FACTORY_START_TIME = Mozart.SeePlan.FactoryConfiguration.Current.StartTime.ToString();
            rh.DATA_COLLECTION_BY_FST = InputMart.Instance.GlobalParameters.data_collection_by_fst ? 'Y' : 'N';
            rh.STATE = task.HasErrors ? "FAIL" : "SUCCESS";
            rh.PERIOD = InputMart.Instance.GlobalParameters.period;
            OutputMart.Instance.RUN_HISTORY.Add(rh);
*/
        }

        public void ON_DONE1(ModelContext context, ref bool handled)
        {
            RUN_HISTORY rh = CreateHelper.CreateRunHistory();

            rh.VERSION_NO = ModelContext.Current.VersionNo;
            rh.INPUT_VER = "-";
            rh.SIM_START_TIME = InputMart.Instance.GlobalParameters.start_time;
            rh.ENG_START_TIME = ModelContext.Current.Version.RunTime;
            rh.ENG_END_TIME = DateTime.Now;
            rh.FACTORY_START_TIME = Mozart.SeePlan.FactoryConfiguration.Current.StartTime.ToString();
            rh.DATA_COLLECTION_BY_FST = InputMart.Instance.GlobalParameters.data_collection_by_fst ? 'Y' : 'N';
            rh.STATE = context.HasErrors ? "FAIL" : "SUCCESS";
            rh.PERIOD = InputMart.Instance.GlobalParameters.period;
            OutputMart.Instance.RUN_HISTORY.Add(rh);
        }

        public void ON_INITIALIZE0(ModelContext context, ref bool handled)
        {
            var inputFactoryTime = InputMart.Instance.EXECUTION_OPTION_CONFIG.Rows.Where(x => x.OPTION_ID ==Constants.DEFAULT_FACTORY_TIME ).Select(y => y.OPTION_VALUE).FirstOrDefault();
            var inputShiftNames = InputMart.Instance.EXECUTION_OPTION_CONFIG.Rows.Where(x => x.OPTION_ID == Constants.DEFAULT_SHIFT_NAMES).Select(y => y.OPTION_VALUE).FirstOrDefault();
            string[] shiftNamesArr;
            int shiftHour;
            int dayHour = Constants.DayValue2; 
            Mozart.SeePlan.FactoryConfiguration currentFactory = Mozart.SeePlan.FactoryConfiguration.Current;
            Mozart.SeePlan.FactoryTimeInfo timeInfo = new Mozart.SeePlan.FactoryTimeInfo();
            try
            {
                timeInfo.StartOffset = inputFactoryTime != null ? TimeSpan.Parse(inputFactoryTime.ToString()) : currentFactory.StartOffset;

                if (inputShiftNames != null)
                {
                    shiftNamesArr = inputShiftNames.ToString().Split(',');
                    shiftHour = dayHour / shiftNamesArr.Count();

                    timeInfo.ShiftHours = shiftHour;
                    timeInfo.ShiftNames = shiftNamesArr;
                } else
                {
                    timeInfo.ShiftHours = currentFactory.ShiftHours;
                    timeInfo.ShiftNames = currentFactory.ShiftNames;
                }

                if (inputFactoryTime != null || inputShiftNames != null)
                {
                    currentFactory.TimeInfo = timeInfo;
                }
            }
            catch (Exception)
            {
                string item = "";
                ErrorHelper.WriteError(Constants.IN0031, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_EXECUTION_OPTION_CONFIG", "factory time or shift names is empty!",
                    item, null, null);
            }
        }
    }
}