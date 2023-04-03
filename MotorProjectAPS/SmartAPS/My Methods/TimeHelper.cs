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
using Mozart.SeePlan.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.Simulation.Engine;
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class TimeHelper
    {
        public static TimeSpan GetTAT(SmartAPSStep cstep, SmartAPSProduct prod, DateTime now, bool isRun = true)
        {
            SmartAPSStdStep std = cstep.StdStep;
            TimeSpan tat = TimeSpan.FromMinutes(ConfigHelper.GetConfig<double>(Constants.DEFAULT_STEP_TAT_MINUTES));
            if (ConfigHelper.GetConfig<bool>(Constants.USE_STD_STEP_TAT_INFO))
            {
                tat = TimeSpan.FromMinutes(std.StepTAT/60); 
            }
            else
            {
                var stepTat = InputMart.Instance.STEP_TATView.FindRows(prod.ProductID, cstep.StepID).FirstOrDefault();
                
                if (stepTat != null)
                {
                    if (isRun)
                        tat = TimeSpan.FromMinutes(SAPSUtils.ConvertPeriodUnit(stepTat.RUN_TAT, stepTat.UNIT)/60);
                    else
                        tat = TimeSpan.FromMinutes(SAPSUtils.ConvertPeriodUnit(stepTat.WAIT_TAT, stepTat.UNIT)/60);
                }
                else
                {
                    tat = TimeSpan.FromMinutes(std.StepTAT/60); 
                }
            }

            return tat; 
        }

        public static ProcTimeInfo GetProcessTime(IHandlingBatch hb, AoEquipment aeqp)
        {
            return GetProcessTime(hb, aeqp, aeqp.NowDT);
        }

        public static ProcTimeInfo GetProcessTime(IHandlingBatch hb, AoEquipment aeqp, DateTime curDateTime)
        {
            ProcTimeInfo result = new ProcTimeInfo();
            var lot = hb.Sample as SmartAPSLot;
            var eqp = aeqp.Target as SmartAPSEqp;

            // 확정 계획은 시작 종료 시간이 지정 되어 있어 해당 시간 많큼 작업을 하기 위하여 고정된 시간을 return 한다.
            if (SimHelper.IsFrimPlan(lot))
            {
                var ts = SimHelper.GetFirmPlanSpan(lot);

                SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

                if (wip.WipState == Constants.RUN && lot.CurrentStep.Equals(wip.InitialStep))
                {
                    DateTime stateTime = SimInit.Instance.OnGetStateTime(aeqp, hb);
                    DateTime startTime = SimHelper.GetFirmPlanStartTime(hb);

                    if (startTime < stateTime)
                        ts = ts - (stateTime - startTime);
                }

                result.FlowTime = ts;
                result.TactTime = ts;

                return result;
            }

            var eqpArrange = InputMart.Instance.EQP_ARRANGETimeView.FindRows(lot.CurrentProductID, lot.CurrentProcessID, lot.CurrentStepID, eqp.EqpID).FirstOrDefault();
            var eqpUtilization = InputMart.Instance.EQP_UTILIZATIONView.FindRows(eqp.LineID, eqp.EqpID).Where(x => x.START_TIME <= curDateTime && x.END_TIME >= curDateTime).FirstOrDefault();

            if (eqpArrange != null)
            {
                lot.InputProcTime = eqpArrange.PROC_TIME;
                lot.InputTactTime = eqpArrange.TACT_TIME;
                lot.ApplyProcTime = ApplyEqpUtilization(eqpArrange.PROC_TIME, eqpUtilization, eqpArrange, ref lot); 
                lot.ApplyTactTime = ApplyEqpUtilization(eqpArrange.TACT_TIME, eqpUtilization, eqpArrange, ref lot);
                result.FlowTime = TimeSpan.FromSeconds(lot.ApplyProcTime);
                result.TactTime = TimeSpan.FromSeconds(lot.ApplyTactTime);
            }
            else
            {
                // default ; 
                lot.InputProcTime = SimRun.Instance.OnGetProcTime();
                lot.InputTactTime = SimRun.Instance.OnGetTactTime();
                lot.Utilization = 100;
                lot.Efficiency = 100;
                lot.ApplyProcTime = SimRun.Instance.OnGetProcTime();
                lot.ApplyTactTime = SimRun.Instance.OnGetTactTime();
                result.FlowTime = TimeSpan.FromSeconds(SimRun.Instance.OnGetProcTime());
                result.TactTime = TimeSpan.FromSeconds(SimRun.Instance.OnGetTactTime());
            }
            
            return result; 
        }

        public static double ApplyEqpUtilization(float originalTime, EQP_UTILIZATION eqpUtilization, EQP_ARRANGE eqpArrange, ref SmartAPSLot lot)
        {
            //1순위: EQP_UTILIZATION 테이블에 값이 있으면, 해당 데이터를 활용함. 
            if (eqpUtilization != null)
            {
                lot.Utilization = eqpUtilization.UTILIZATION;
                lot.Efficiency = eqpUtilization.EFFICIENCY;
                if (eqpUtilization.UTILIZATION <= 0 || eqpUtilization.EFFICIENCY <= 0)
                    return 0; 
                
                return originalTime * (100 / eqpUtilization.UTILIZATION) * (100 / eqpUtilization.EFFICIENCY);
            }  
            else
            {
                //2순위: EQP_ARRANGE 테이블에 값이 있으면, 해당 데이터를 사용함. 
                //3순위: EQP_ARRANGE 테이블에도 값이 없으면, EXECUTION_CONFIG_OPTION의 DEFAULT값을 사용함
                //4순위:  EXECUTION_CONFIG_OPTION에도 값이 없으면, 100으로 사용함.
                //3,4순위는 persist단계에서 eqp_arrange에 미리 매핑해둠. 
                lot.Utilization = eqpArrange.UTILIZATION;
                lot.Efficiency = eqpArrange.EFFICIENCY;

                if (lot.Utilization <= 0 || lot.Efficiency <= 0)
                    return 0;
                 
                return originalTime * (100 / eqpArrange.UTILIZATION) * (100 / eqpArrange.EFFICIENCY); 
            }
        } 

        public static bool ContainsTime(DateTime startTime, double period, DateTime compTime)
        {
            DateTime endTime = startTime.AddSeconds(period);

            if (startTime <= compTime && endTime > compTime)
                return true;

            return false;
        }

        public static Time GetSetupTime(AoEquipment aeqp, IHandlingBatch hb)
        {
            Time time = Time.Zero;

            SmartAPSLot lot = hb.Sample as SmartAPSLot;

            if (lot.CurrentStep == null)
                return time;

            SmartAPSStep step = lot.CurrentStep as SmartAPSStep;
            SmartAPSStdStep std = step.StdStep;

            string option = SimRun.Instance.OnGetSetupOption(aeqp, hb);
            double defTime = ConfigHelper.GetConfig<double>(Constants.DEFAULT_SETUP_TIME);

            switch (option)
            {
                case Constants.USE_STEP_SETUP:
                    time = Time.FromMinutes(std.StepSetup);
                    break;
                case Constants.USE_EQP_SETUP:
                    time = SimRun.Instance.OnGetEqpSetupTime(aeqp, hb);
                    break;
                case Constants.CUSTOM:
                    time = SimRun.Instance.OnGetCustomSetupTime(aeqp, hb);
                    break;
                default:
                    time = defTime;
                    break;
            }

            return time;
        }

        public static float GetFlowTime(ISimEntity entity, AoEquipment eqp)
        {
            ProcTimeInfo procTime = TimeHelper.GetProcessTime(entity as IHandlingBatch, eqp);
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;

            float flowTimeSeconds;
            switch (eqp.Target.SimType)
            {
                case SimEqpType.Inline:
                    flowTimeSeconds = Convert.ToSingle(procTime.FlowTime.TotalSeconds + (procTime.TactTime.TotalSeconds * (lot.UnitQty - 1)));
                    break;
                case SimEqpType.Table:
                    flowTimeSeconds = Convert.ToSingle(procTime.TactTime.TotalSeconds) * lot.UnitQty;
                    break;
                default:
                    flowTimeSeconds = Convert.ToSingle(procTime.TactTime.TotalSeconds) * lot.UnitQty;
                    break;
            }

            return flowTimeSeconds;
        }

        public static Time CalcTime(double time, string option)
        {
            Time retValue;
            switch (option)
            {
                case Constants.HOUR:
                    retValue = Time.FromHours(time);
                    break;
                case Constants.MINUTE:
                    retValue = Time.FromMinutes(time);
                    break;
                case Constants.SECOND:
                default:
                    retValue = Time.FromSeconds(time);
                    break;
            }
            return retValue;
        }
    }
}
