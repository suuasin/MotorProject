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
using Mozart.Simulation.Engine;
using Mozart.SeePlan.Simulation;
using Mozart.SeePlan.DataModel;
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class SimHelper
    {
        public static DateTime MinDateTime
        {
            get
            {
                return new DateTime(1753, 1, 1);
            }
        }

        public static DateTime MaxDateTime
        {
            get
            {
                return new DateTime(9999, 12, 31);
            }
        }

        public static bool MergePartLot(SmartAPSLot lot)
        {
            if (string.IsNullOrEmpty(lot.CurrentStepID))
                return false;

            Step step = lot.Process.FindStep(lot.CurrentStepID);
            SmartAPSProduct prod = lot.Product as SmartAPSProduct;

            if (step != null && prod.ProductType == ProductType.PART)
            {
                return MergePartLot(lot, prod, step, step.GetDefaultNextStep(), true);
            }

            return false;
        }

        public static bool IsMergePart(SmartAPSLot lot, SmartAPSProduct prod, Step step, Step next, bool isInit)
        {
            if (next != null)
            {
                // From Product의 마지막 Step이 To Product의 첫 Step일 경우
                // From ProductID와 마지막 StepID로 Route를 찾는다.
                var infos = InputMart.Instance.PRODUCT_ROUTE.Rows.Where(p => (
                        p.FROM_PRODUCT_ID == prod.ProductID &&
                        p.STEP_ID == next.StepID)
                        );

                if (isInit)
                {
                    // WipInit이나 BatchInputInit에서 Merge를 판단할 경우
                    // Next가 있어도 현재 공정을 하고 Merge를 진행해야 하기 때문에 Merge하지 않는다.
                    return false;
                }
                else
                {
                    // NextStep에서 Merge를 판단할 경우
                    return infos.Count() > 0;
                }
            }
            else
            {
                // From Product의 마지막 Step이 null일 경우
                // From ProductID로 Route를 찾는다.
                var infos1 = InputMart.Instance.PRODUCT_ROUTE.Rows.Where(p => (
                        p.FROM_PRODUCT_ID == prod.ProductID)
                        );

                if (isInit)
                {
                    // WipInit이나 BatchInputInit에서 Merge를 판단할 경우
                    if (infos1.Count() > 0)
                    {
                        // From Product의 마지막 Step이 To Product의 첫 Step일 경우만 Merge 한다.
                        // 위 경우가 아니면 마지막 Step을 진행 한 후에 Merge를 수행한다.
                        var infos2 = infos1.Where(p => p.STEP_ID == step.StepID);

                        return infos2.Count() > 0;
                    }

                    return false;
                }
                else
                {
                    var route = infos1.FirstOrDefault();
                    //Next Step이 정의되어 있지 않을 때 ChangeType이 IN인 경우 ErrorLog를 남기고 false를 반환한다.
                    if (route != null && route.CHANGE_TYPE != Constants.WAIT)
                    {
                        string item = $"STEP_ROUTE {nameof(route.STEP_ID)} = {route.STEP_ID}, {nameof(prod.ProductID)} = {prod.ProductID}";
                        ErrorHelper.WriteError(Constants.SI0006, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_INPUT", "STEP data is not defined in STEP_ROUTE",
                                    item, product: prod.ProductID, step: route.STEP_ID, lotid: lot.LotID);
                        return false;
                    }
                    else
                        return infos1.Count() > 0; // NextStep에서 Merge를 판단할 경우
                }
            }
        }

        public static bool MergePartLot(SmartAPSLot lot, SmartAPSProduct prod, Step step, Step next, bool isInit)
        {
            if (IsMergePart(lot, prod, step, next, isInit))
            {
                List<ISimEntity> list = AoFactory.Current.Merge(lot);

                if (list != null && list.Count > 0)
                {
                    foreach (ISimEntity entity in list)
                    {
                        var newLot = entity as SmartAPSLot;

                        if (newLot.CurrentPlan == null)
                            newLot.SetCurrentPlan(EntityControl.Instance.CreateLoadInfo(newLot, newLot.Process.FirstStep));

                        var agent = AoFactory.Current.FindDispatchingAgent(newLot);
                        agent.Factory.In(newLot);
                        agent.ReEnter(newLot, true);
                    }
                }
                return true;
            }

            return false;
        }

        public static bool IsFrimPlan(IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            string key = SAPSUtils.GetFirmPlanKey(lot);
            bool flag = wip.FirmPlans.ContainsKey(key);

            //Logger.Monitor.Info($"IsFirmPlan => {key}, {flag}");

            return flag;
        }

        public static bool IsFrimPlan(IHandlingBatch hb, string lotId, string stepId)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            string key = SAPSUtils.CreateKey(lotId, stepId);
            bool flag = wip.FirmPlans.ContainsKey(key);

            //Logger.Monitor.Info($"IsFirmPlan => {key}, {flag}");

            return flag;
        }

        public static DateTime GetFirmPlanStartTime(IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            if (wip.FirmPlans.TryGetValue(SAPSUtils.GetFirmPlanKey(lot), out SmartAPSFirmPlan plan))
            {
                return plan.StartTime;
            }

            return SimHelper.MinDateTime;
        }

        public static DateTime GetFirmPlanEndTime(IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            if (wip.FirmPlans.TryGetValue(SAPSUtils.GetFirmPlanKey(lot), out SmartAPSFirmPlan plan))
            {
                return plan.EndTime;
            }

            return SimHelper.MinDateTime;
        }

        public static DateTime GetFirmPlanStartTime(IHandlingBatch hb, string lotId, string stepId)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            string key = SAPSUtils.CreateKey(lotId, stepId);
            if (wip.FirmPlans.TryGetValue(key, out SmartAPSFirmPlan plan))
            {
                return plan.StartTime;
            }

            return SimHelper.MinDateTime;
        }

        public static string GetFirmPlanEqpId(IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            if (wip.FirmPlans.TryGetValue(SAPSUtils.GetFirmPlanKey(lot), out SmartAPSFirmPlan plan))
            {
                return plan.EqpID;
            }

            return string.Empty;
        }

        public static TimeSpan GetFirmPlanSpan(IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            if (wip.FirmPlans.TryGetValue(SAPSUtils.GetFirmPlanKey(lot), out SmartAPSFirmPlan plan))
            {
                return plan.EndTime - plan.StartTime;
            }

            return TimeSpan.Zero;
        }

        public static SmartAPSFirmPlan GetFirmPlan(IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            if (wip.FirmPlans.TryGetValue(SAPSUtils.GetFirmPlanKey(lot), out SmartAPSFirmPlan plan))
            {
                return plan;
            }

            return plan;
        }

        public static SmartAPSFirmPlan GetFirmPlan(IHandlingBatch hb, string stepId)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

            if (wip.FirmPlans.TryGetValue(SAPSUtils.GetFirmPlanKey(lot, stepId), out SmartAPSFirmPlan plan))
            {
                return plan;
            }

            return plan;
        }

        public static SmartAPSLot GetFirmPlanLot(AoEquipment aeqp, bool isReserve, FirmType ft = FirmType.DELAY)
        {
            var eqp = aeqp.Target as SmartAPSEqp;

            var firmPlan = eqp.FirmPlans.Values
                .Where(r => (isReserve ? r.StartTime >= aeqp.NowDT : r.StartTime <= aeqp.NowDT)
                    && (ft == FirmType.DELAY ? true : r.FirmType == ft))
                .OrderBy(r => r.StartTime).FirstOrDefault();

            if (firmPlan == null)
                return null;

            var lot = InputMart.Instance.Lots.Values.Where(r => r.IsFinished == false && r.LotID == firmPlan.LotID).FirstOrDefault();

            if (lot != null)
            {
                var eqps = ArrangeHelper.GetLoadableEqpList(lot);

                if (eqps.Contains(aeqp.EqpID) == false)
                    return null;
            }

            return lot;
        }
        public static bool IsSplitLot(IHandlingBatch hb)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;

            // 우선 순위가 생긴다면 아래 FirstOrDefault 부분 변경 필요
            var split = InputMart.Instance.SPLIT_INFOView.FindRows(lot.Product.ProductID, lot.CurrentStepID).FirstOrDefault();
            if (split != null)
            {
                switch (split.CRITERIA)
                {
                    case Constants.QTY:
                    case Constants.PARTIAL_QTY:
                        return split.VALUE < lot.UnitQty;
                    case Constants.CNT:
                        return split.VALUE > 1 && lot.UnitQty / split.VALUE >= 1;
                    default:
                        return false;
                }
            }
            else
                return false;
        }

        public static double GetUnitQty(IHandlingBatch hb, AoEquipment aeqp)
        {
            double unitSize = SeeplanConfiguration.Instance.LotUnitSize;
            SmartAPSLot lot = hb.Sample as SmartAPSLot;

            // 확정 계획의 FIX Type은 정해진 시작 종료 시간을 맞추기 위하여 UnitSize를 1로 계산한다.
            if (SimHelper.IsFrimPlan(lot))
            {
                unitSize = 1;
                return unitSize;
            }

            if (!aeqp.IsBatchType())
            {
                SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

                unitSize = hb.UnitQty - wip.OutQty;
            }

            return unitSize;
        }

        public static DateTime GetIntersectionStartTime(DateTime mainStart, DateTime mainEnd, DateTime intervalStart, DateTime intervalEnd)
        {//main time과 interval time과 겹치기 시작하는 시작 시간
            if (intervalStart >= mainEnd || intervalEnd <= mainStart)
            {
                return SimHelper.MinDateTime;
            }

            if (intervalStart >= mainStart && intervalEnd <= mainEnd)
            {
                return intervalStart;
            }

            DateTime tempStart = intervalStart;
            DateTime tempEnd = intervalEnd;

            if (intervalStart < mainStart) tempStart = mainStart;
            if (intervalEnd > mainEnd) tempEnd = mainEnd;
            // return tempEnd - tempStart;
            return tempStart;
        }

        public static SmartAPSEqp SetEqpBreakList(SmartAPSEqp eqp, DateTime startTime, double period, string unit)
        {
            period = SAPSUtils.ConvertPeriodUnit(period, unit);

            PMSchedule newPm = CreateHelper.CreatePMSchedule(startTime, period);
            if (SimInit.Instance.OnUsePMSchedule(newPm, eqp))
            {
                CreateHelper.CreatePMList(eqp, newPm);
                eqp.BreakList.Add(newPm);
            }

            return eqp;
        }


        public static SmartAPSLot SetSplitLot(AoEquipment aeqp, SmartAPSLot lot, SmartAPSWipInfo wip, int i, int spanQty)
        {
            SmartAPSWipInfo newWip = CreateHelper.CreateWipInfo(lot.Product as SmartAPSProduct, spanQty, wip.DemandID);

            newWip.LotID = lot.LotID + "_" + i;
            newWip.InitialEqp = aeqp.Target as SmartAPSEqp;
            newWip.WipEqpID = aeqp.EqpID;

            SmartAPSLot newLot = CreateHelper.CreateLot(newWip, false);

            newLot.IsCarryOverSplit = true;

            newLot.ParentLot = lot;

            lot.ChildLots.Add(newLot);

            newLot.SetCurrentPlan(EntityControl.Instance.CreateLoadInfo(newLot, lot.CurrentStep));

            newLot.CurrentPlan.AddStepTarget(lot.CurrentPlan.PegInfoList.FirstOrDefault().StepTarget, spanQty);

            return newLot;
        }

        public static bool IsItemChanged(SmartAPSLot lot, SmartAPSPlanInfo info, string item, string itemValue)
        {
            switch (item)
            {
                case Constants.PRODUCT_ID:
                    if (lot.Product.ProductID != info.Lot.Product.ProductID)
                        return true;
                    break;
                case Constants.STEP_ID:
                    if (lot.CurrentStepID != info.StepID)
                        return true;
                    break;
                //case Constants.TOOL_ID:
                //    if (lot.ToolSettings.Items[0].ResourceKey.ToString() != info.ToolID)
                //        return true;  
                //    break; //tool 사용한다고 할 때 처리하는 구간같은데..
                case Constants.PROPERTY:
                    if (lot == null || info ==null)
                        break;

                    string[] PropertyName = itemValue.Split(new[] { '@' }); // PEG CONDITION 의 속성값 1개 or n개 
                    SmartAPSProduct Lotprod = lot.Product as SmartAPSProduct;
                    SmartAPSProduct Infoprod = info.Lot.Product as SmartAPSProduct;
                    string LotProper = null;
                    string InfoProper = null;

                    foreach (var property in PropertyName)
                    {
                        Lotprod.Property.TryGetValue(property, out var LotPropertyValue);
                        Infoprod.Property.TryGetValue(property, out var InfopropertyValue);

                        LotProper += (string)LotPropertyValue + '@';
                        InfoProper += (string)InfopropertyValue + '@';
                    }

                    if (LotProper != InfoProper)
                        return true;
                    break;
                default:
                    return false;
            }
            return false;
        }

        public static DateTime SetReleaseTime(string batchOption, int releaseTimeBuffer, List<int> specificTime, DateTime defaultReleaseTime, SmartAPSInTarget target)
        {
            switch (batchOption)
            {
                case Constants.AT_ENGINE_START:
                    defaultReleaseTime = AoFactory.Current.NowDT;
                    break;
                case Constants.ON_TARGET_TIME:
                    defaultReleaseTime = target.TargetDate.AddHours(releaseTimeBuffer);
                    break;
                case Constants.CUSTOM:
                    if (specificTime != null)
                    {
                        DateTime date = target.TargetDate;
                        int loopValue = 1;
                        if (target.TargetDate < AoFactory.Current.NowDT)
                        {
                            DateTime compareDate = AoFactory.Current.NowDT.Date;
                            while (loopValue <= 2)
                            {
                                foreach (int hour in specificTime.OrderBy(x => x))
                                {
                                    compareDate = compareDate.Date.AddHours(hour);
                                    if (compareDate > AoFactory.Current.NowDT)
                                    {
                                        defaultReleaseTime = compareDate;
                                        loopValue = int.MaxValue - 1;
                                        break;
                                    }
                                }
                                compareDate = compareDate.AddDays(1);
                                loopValue++;
                            }
                        }
                        else
                        {
                            while (date > AoFactory.Current.NowDT && loopValue <= 2)
                            {
                                foreach (int hour in specificTime.OrderByDescending(x => x))
                                {
                                    var compareDate = date.Date.AddHours(hour);
                                    if (compareDate <= target.TargetDate)
                                    {
                                        if (compareDate < AoFactory.Current.NowDT)
                                        {
                                            loopValue = int.MaxValue - 1;
                                            break;
                                        }
                                        else
                                        {
                                            defaultReleaseTime = compareDate;
                                            loopValue = int.MaxValue - 1;
                                            break;
                                        }
                                    }
                                }
                                date = date.AddDays(-1);
                                loopValue++;
                            }

                            if (defaultReleaseTime == DateTime.MinValue)
                            {
                                date = target.TargetDate.Date;

                                loopValue = 1;
                                while (loopValue <= 2)
                                {
                                    foreach (int hour in specificTime.OrderBy(x => x))
                                    {
                                        DateTime compareDate = date.Date.AddHours(hour);
                                        if (compareDate > target.TargetDate)
                                        {
                                            defaultReleaseTime = compareDate;
                                            loopValue = int.MaxValue - 1;
                                            break;
                                        }
                                    }
                                    date = date.AddDays(1);
                                    loopValue++;
                                }
                            }
                        }
                        
                    }
                    break;
                default:
                    break;
            }
            defaultReleaseTime = (defaultReleaseTime < AoFactory.Current.NowDT) ? AoFactory.Current.NowDT : defaultReleaseTime;

            return defaultReleaseTime;
        }
    }
}
