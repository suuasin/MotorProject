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
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class ErrorHelper
    {
        public static void WriteError(string errCode, ErrorSeverity severity, DateTime logtime, string category, string reason, string item ,
            string demandid = null, string product = null, string step = null, string equipment = null, string lotid = null)
        {
            ERROR_LOG log = CreateHelper.CreateErrorLog();

            log.ERR_CODE = errCode;
            log.VERSION_NO = ModelContext.Current.VersionNo; 
            log.SEVERITY = severity.ToString();            
            log.LOG_TIME = logtime; 
            log.CATEGORY = category;
            log.REASON = reason;
            log.ITEM = item;
            log.PRODUCT_ID = product;
            log.STEP_ID = step;
            log.EQP_ID = equipment;
            log.LOT_ID = lotid;
            log.DEMAND_ID = demandid; 
            log.ERROR_LOG_ID = Guid.NewGuid().ToString();

            OutputMart.Instance.ERROR_LOG.Add(log);

        }

        internal static void WriteError(string iN0001, ErrorSeverity wARNING, object startTime, string v1, string v2, string item, object demandid, object product)
        {
            throw new NotImplementedException();
        }
    }
}
