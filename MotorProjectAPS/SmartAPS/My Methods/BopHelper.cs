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
using SmartAPS.Persists;
using Mozart.SeePlan.General.DataModel;
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class BopHelper
    {
        public static int ComparePrevSteps(GeneralStep x, GeneralStep y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;
            return x.Sequence.CompareTo(y.Sequence);
        }

        public static int CompareSteps(GeneralStep x, GeneralStep y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;
            return x.Sequence.CompareTo(y.Sequence);
        }

        public static Dictionary<string, GeneralStep> LoadSteps(SmartAPSProcess proc)
        {
            // Process별 StepID정보는 ProcStep에 있고, 해당 StepID의 Step정보는 ProcStep에서 가져옴
            var stepRoutes = InputMart.Instance.STEP_ROUTEView.FindRows(proc.ProcessID);
            Dictionary<string, GeneralStep> stepDict = new Dictionary<string, GeneralStep>();

            foreach (STEP_ROUTE sr in stepRoutes)
            {
                SmartAPSStep s = CreateHelper.CreateStep(sr);

                if (s != null)
                {
                    if (DerivedHelper.CallAfterLoadHandler(sr, s))
                        stepDict.Add(s.StepID, s); 
                }
            }

            return stepDict;
        }

        public static Dictionary<string, PrpInfo> LoadPrpInfo(SmartAPSProcess proc)
        {
            Dictionary<string, PrpInfo> dict = new Dictionary<string, PrpInfo>();

            // 해당 Process 에 해당하는 ProcStep 정보를 검색, Prp 정보를 생성하여 Dictionary에 저장합니다.
            var prps = InputMart.Instance.STEP_ROUTEView.FindRows(proc.ProcessID);
            if (prps == null || prps.Count() == 0)
            {
                Logger.Info($"Step-Path not found for Process with ID : {proc.ProcessID}");
                return dict;
            }

            List<STEP_ROUTE> list = prps.OrderBy(a => a.STEP_SEQ).Where(x => InputMart.Instance.SmartAPSStdStepView.FindRows(x.STEP_ID).FirstOrDefault() != null).ToList();

            string stepID;
            
            for (int i = 0; i < list.Count; i++)
            {
                stepID = list[i].STEP_ID;

                if (dict.TryGetValue(stepID, out PrpInfo prp) == false)
                    dict.Add(stepID, prp = new PrpInfo(stepID));

                if (i + 1 < list.Count)
                    prp.AddPrpTo(list[i + 1].STEP_ID);
            }

            return dict;
        }

        public static SmartAPSProcess GetProcess(string processid)
        {
            SmartAPSProcess proc =  InputMart.Instance.SmartAPSProcessView.FindRows(processid).FirstOrDefault();
            return proc; 
        }


        public static SmartAPSProduct GetProduct(string productid)
        {
            SmartAPSProduct prod = InputMart.Instance.SmartAPSProductView.FindRows(productid).FirstOrDefault();
            return prod; 
        }
    }
}
