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

namespace SmartAPS
{
    [FeatureBind()]
    public static partial class AnomalyDetectionHelper
    {
        public const string BEGIN = "0%";
        public const string RUN = "50%";
        public const string DONE = "100%";
        public static string COMPANY = InputMart.Instance.GlobalParameters.company;
        public static string SITE = InputMart.Instance.GlobalParameters.site;
        public static string SYSTEM = InputMart.Instance.GlobalParameters.system;
        public const string SUCCESS = "성공";
        public const string FAIL = "실패";
        public const string EXECUTION_RESULT_CODE = "마지막 실행 결과코드";
        public const string EXECUTION_RESULT_CODE_SET = "마지막 실행 결과 셋";
        public const string EXECUTION_DATE = "마지막 실행 일시";
        public const string EXECUTION_RESULT_HISTORY = "마지막 실행 결과 이력";
        public const string STATUS = "상태";
        public static string GetLogID(DateTime insertDateTime, string company, string site, string system)
        {
            string[] logIdArr = { company, site, system, insertDateTime.ToString("yyyyMMddHHmmss") };
            string insertLogID = string.Join("-", logIdArr);

            return insertLogID;
        }

        public static string GetLogText(string text, string state)
        {
            string returnLogText;
            if (string.IsNullOrEmpty(text))
            {
                returnLogText = state;
            }
            else
            {
                returnLogText = "[" + text + "] 로 인한 스케쥴 재동작 : " + state;
            }

            return returnLogText;
        }

        public static Dictionary<object, object> GetLogArgs(string LOG_ID, DateTime LOG_DATETIME, string LOG_TEXT)
        {
            Dictionary<object, object> insert_args = new Dictionary<object, object>();
            insert_args.Add("@LOG_ID", LOG_ID);
            insert_args.Add("@LOG_DATETIME", LOG_DATETIME);
            insert_args.Add("@LOG_TEXT", LOG_TEXT);

            return insert_args;
        }

        public static Dictionary<object, object> GetStatusArgs(string APS_CODE, string APS_VALUE)
        {
            Dictionary<object, object> insert_args = new Dictionary<object, object>();
            insert_args.Add("@APS_CODE", APS_CODE);
            insert_args.Add("@APS_VALUE", APS_VALUE);

            return insert_args;
        }

        public static List<APS_STATUS_MASTER> CreateApsStatusMaster(string flag, string logID, DateTime executionTime, string isSuccess)
        {
            List<APS_STATUS_MASTER> asmList = new List<APS_STATUS_MASTER>();

            asmList.Add(CreateHelper.CreateApsStatusMaster(EXECUTION_RESULT_CODE, isSuccess));
            asmList.Add(CreateHelper.CreateApsStatusMaster(EXECUTION_RESULT_CODE_SET, logID));
            asmList.Add(CreateHelper.CreateApsStatusMaster(EXECUTION_DATE, executionTime.ToString("yyyy-MM-dd HH:mm:ss")));
            //asmList.Add(new APS_STATUS_MASTER() { APS_CODE = EXECUTION_RESULT_CODE, APS_VALUE = isSuccess });
            //asmList.Add(new APS_STATUS_MASTER() { APS_CODE = EXECUTION_RESULT_CODE_SET, APS_VALUE = logID });
            //asmList.Add(new APS_STATUS_MASTER() { APS_CODE = EXECUTION_DATE, APS_VALUE = executionTime.ToString("yyyy-MM-dd HH:mm:ss") });

            if (isSuccess == SUCCESS)
            {
                APS_STATUS_MASTER asm = CreateHelper.CreateApsStatusMaster();
                APS_STATUS_MASTER asm2 = CreateHelper.CreateApsStatusMaster();
                asm.APS_CODE = EXECUTION_RESULT_HISTORY;
                asm2.APS_CODE = STATUS;
                switch (flag)
                {
                    case BEGIN:
                        asm.APS_VALUE = "스케줄이 정상적으로 실행되었습니다.";
                        asm2.APS_VALUE = "시작";
                        break;
                    case RUN:
                        asm.APS_VALUE = "스케줄이 실행 중입니다.";
                        asm2.APS_VALUE = "실행 중";
                        break;
                    case DONE:
                        asm.APS_VALUE = "스케줄 실행이 완료되었습니다.";
                        asm2.APS_VALUE = "실행 완료";
                        break;
                    default:
                        break;
                }
                asmList.Add(asm);
                asmList.Add(asm2);
            }

            return asmList;
        }

        public static Dictionary<string, object> SetVersionNo(string versionNo, Mozart.Task.Execution.ModelTask task)
        {
            task.Context.Version.VersionNo = versionNo;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@VERSION_NO", versionNo);

            return dict;
        }
    }
}