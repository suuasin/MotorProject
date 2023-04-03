using Mozart.SeePlan.DataModel;
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

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class EqpInit
    {
        public IEnumerable<Mozart.SeePlan.DataModel.Resource> GET_EQP_LIST0(ref bool handled, IEnumerable<Mozart.SeePlan.DataModel.Resource> prevReturnValue)
        {
            // Persist 과정에서 생성해둔 장비 Collection 을 사용합니다.
            return InputMart.Instance.SmartAPSEqp.Rows.ToList<Resource>();
        }

        public DateTime GET_EQP_UP_TIME0(Mozart.SeePlan.DataModel.Resource resource, DateTime stateChangeTime, ref bool handled, DateTime prevReturnValue)
        {
            SmartAPSEqp eqp = resource as SmartAPSEqp;

            if (string.IsNullOrWhiteSpace(eqp.StateCode))
                return ModelContext.Current.EndTime;

            if (eqp.StateChangeTime <= SimHelper.MinDateTime || eqp.StateChangeTime >= SimHelper.MaxDateTime)
                return ModelContext.Current.EndTime;

            var fb = InputMart.Instance.FACTORY_BREAKView.FindRows(eqp.StateCode).FirstOrDefault();

            double period = 0;

            if (fb != null)
            {
                period = SAPSUtils.ConvertPeriodUnit(fb.PERIOD, fb.PERIOD_UNIT);
            }
            else
            {
                var pm = InputMart.Instance.PM_PLANView.FindRows(eqp.StateCode).FirstOrDefault();

                if (pm != null)
                    period = pm.PERIOD;
            }

            DateTime upTime = eqp.StateChangeTime.AddSeconds(period);

            return ModelContext.Current.StartTime > upTime ? ModelContext.Current.StartTime : upTime;
        }
    }
}