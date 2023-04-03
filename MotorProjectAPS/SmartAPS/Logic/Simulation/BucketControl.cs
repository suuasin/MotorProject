using Mozart.SeePlan.Simulation;
using Mozart.Simulation.Engine;
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
    public partial class BucketControl
    {
        public Time GET_BUCKET_TIME0(Mozart.SeePlan.Simulation.IHandlingBatch hb, AoBucketer bucketer, ref bool handled, Time prevReturnValue)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;

            if (lot.CurrentStep == null)
                return Time.Zero;

            // 계획이 확정 계획 일 경우
            if (SimHelper.IsFrimPlan(hb))
            {
                var plan = SimHelper.GetFirmPlan(hb);
                DateTime startTime = bucketer.NowDT > plan.StartTime ? bucketer.NowDT : plan.StartTime;
                DateTime endTime = bucketer.NowDT > plan.EndTime ? bucketer.NowDT : plan.EndTime;
                Time time = Time.FromSeconds((endTime - startTime).TotalSeconds);

                double totalTime = time.TotalSeconds;
                if (plan.EqpID == "BUCKET")
                {
                    //확정 계획이지만 시작시간(AoFactory.Current.NowDT)이 확정된 시간과 달라졌으면 휴일 영향 받게 구현
                    if (Math.Abs(AoFactory.Current.NowDT.Subtract(plan.StartTime).TotalMinutes) > 0)
                    {
                        DateTime endDateTime = AoFactory.Current.NowDT.AddSeconds(totalTime);
                        //BUCKET 공정의 끝나는 시간을 구해서 이 시간이 공장휴무(FACTORY BREAK) 에 해당하면 FACTORY BREAK 끝날때로 넣어준다.

                        while(InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Where(x => x.StartTime < endDateTime && endDateTime < x.EndTime).FirstOrDefault() != null)
                        {
                            var et = InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Where(x => x.StartTime < endDateTime && endDateTime < x.EndTime).FirstOrDefault();
                            var timedff = (et.EndTime - et.StartTime).TotalSeconds;
                            endDateTime = endDateTime.AddSeconds(timedff);
                            totalTime += timedff;
                        }
                    }
                } 
                WriteHelper.WriteEqpPlan(null, hb, LoadingStates.BUSY, AoFactory.Current.NowDT.AddSeconds(totalTime));

                return Time.FromSeconds(totalTime);
            }
            else
            {
                SmartAPSProduct prod = lot.Product as SmartAPSProduct;
                SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

                if (lot.UnitQty - wip.OutQty == 0)
                    return Time.Zero;

                Time time = Time.FromMinutes(TimeHelper.GetTAT(lot.CurrentStep as SmartAPSStep, prod, bucketer.NowDT, true).TotalMinutes);

                double totalTime = time.TotalSeconds;
                DateTime endDateTime = AoFactory.Current.NowDT.AddSeconds(totalTime);

                while (InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Where(x => x.StartTime < endDateTime && endDateTime < x.EndTime).FirstOrDefault() != null)
                {
                    var et = InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Where(x => x.StartTime < endDateTime && endDateTime < x.EndTime).FirstOrDefault();
                    var timedff = (et.EndTime - et.StartTime).TotalSeconds;
                    endDateTime = endDateTime.AddSeconds(timedff);
                    totalTime += timedff;
                }

                WriteHelper.WriteEqpPlan(null, hb, LoadingStates.BUSY, AoFactory.Current.NowDT.AddSeconds(totalTime));

                return Time.FromSeconds(totalTime);
            }
        }
    }
}