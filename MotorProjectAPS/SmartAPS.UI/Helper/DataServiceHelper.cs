using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Task.Model;
using SmartAPS.Outputs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SmartAPS.UI.Helper
{
    public class DataService
    {
        public static IEnumerable<T> GetEntityData<T>(IExperimentResultItem result, string shopList = null)
        {
            var svc = result;
            if (svc == null)
                return new List<T>();

            var t = typeof(T);

            string nameSpace = t.Namespace;   
            string tableName = t.Name;

            bool isInput = nameSpace.Contains("Inputs");

            IEnumerable<T> entityTable;

            string filter = null;
            if(string.IsNullOrEmpty(shopList) == false)
            {
                var m = t.GetMember("SHOP_ID");
                if (m != null)
                    filter = string.Format("SHOP_ID IN({0})", shopList);
            }

            if(isInput)
                entityTable = svc.LoadInput<T>(tableName, filter);
            else
                entityTable= svc.LoadOutput<T>(tableName, filter);

            return entityTable;
        }

        public static DateTime GetPlanStartTime(IExperimentResultItem result)
        {
            try
            {
                if (result == null)
                    return DateTime.MinValue;

                return MyHelper.DATASVC.GetEntityData<RUN_HISTORY>(result).FirstOrDefault().SIM_START_TIME;
                //return (DateTime)result.Experiment.GetArgument("start-time");
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static int GetPlanPeriod(IExperimentResultItem result)
        {
            int DEFAULT_PERIOD = 7;

            if (result == null)
                return DEFAULT_PERIOD;

            return MyHelper.DATASVC.GetEntityData<RUN_HISTORY>(result).FirstOrDefault().PERIOD;
            //return (int)result.Experiment.GetArgument("period");
        }

        public static DateTime GetPlanEndTime(IExperimentResultItem result)
        {
            if (result == null)
                return DateTime.MinValue;

            var runHist = MyHelper.DATASVC.GetEntityData<RUN_HISTORY>(result).FirstOrDefault();
            //int period = Convert.ToInt32(result.Experiment.GetArgument("period"));

            return runHist.SIM_START_TIME.AddDays(runHist.PERIOD);
            //return ((DateTime)result.Experiment.GetArgument("start-time")).AddDays(period);
        }

        public static int SaveLocal(IExperimentResultItem result, string dataItemName, DataTable dt)
        {
            if (string.IsNullOrEmpty(dataItemName))
                return 0;
                
            ModelEngine engine = result.Model.TargetObject as ModelEngine;
            using (var acc = engine.LocalAccessorFor(dataItemName))
            {
                return acc.Save(dt);
            }
        }
    }
}
