using Mozart.SeePlan.Simulation;
using Mozart.Simulation.Engine;
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
    public partial class Weights
    {
        public WeightValue DEMAND_PRIORITY(Mozart.Simulation.Engine.ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, Mozart.SeePlan.Simulation.IDispatchContext ctx)
        {
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;
            float maxPriority = InputMart.Instance.DEMAND.Rows.Max(x => x.PRIORITY);
            float priority = 0;

            if (lot.CurrentPlan.PegInfoList != null && lot.CurrentPlan.PegInfoList.Count > 0)
            {
                var pegPlan = ((SmartAPSStepTarget)lot.CurrentPlan.PegInfoList.Where(r => r.Qty > 0).OrderBy(r => r.StepTarget.DueDate).FirstOrDefault().StepTarget).MoPlan;
                priority = pegPlan == null ? 0 : float.Parse(pegPlan.Priority.ToString());
            }
            else
                return new WeightValue(0);

            float weight = 1 - (priority / maxPriority);

            return new WeightValue(weight * factor.Factor);
        }

        public WeightValue DUE_DATE(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;
            var presteInfo = InputMart.Instance.PRESET_INFOFactorView.FindRows(factor.Name).FirstOrDefault();
            DateTime dueDate = SimHelper.MinDateTime;

            string[] criteria = InputMart.Instance.PRESET_INFO.Rows.Where(x => x.PRESET_ID == (target as AoEquipment).Preset.Name && x.FACTOR_ID == factor.Name).FirstOrDefault().CRITERIA.Split('/');

            float maxValue = SAPSUtils.CalcOptionValue(criteria[0], float.Parse(criteria[1]));
            float termValue = SAPSUtils.CalcOptionValue(criteria[2], float.Parse(criteria[3]));

            //if (lot.CurrentPlan.PegInfoList != null && lot.CurrentPlan.PegInfoList.Count > 0)
            //{
            //    var pegPlan = ((SmartAPSStepTarget)lot.CurrentPlan.PegInfoList.Where(r => r.Qty > 0).OrderBy(r => r.StepTarget.DueDate).FirstOrDefault().StepTarget).MoPlan;
            //    dueDate = pegPlan == null ? SimHelper.MaxDateTime : pegPlan.DueDate;
            //}
            //else
            //    return new WeightValue(0);

            var a = InputMart.Instance.DEMANDIDView.FindRows((lot.WipInfo as SmartAPSWipInfo).DemandID);
            if (a.Count() > 0)
                dueDate = a.FirstOrDefault().DUE_DATE;
            else
                return new WeightValue(0);

            TimeSpan diff = dueDate - now;

            float diffSeconds = Convert.ToSingle(diff.TotalSeconds);
            float section = Convert.ToSingle(Math.Truncate(maxValue / termValue));
            float weight = SAPSUtils.SetWeight(maxValue, diffSeconds, section, Constants.DESC);

            return new WeightValue(weight * factor.Factor);
        }

        public WeightValue DUE_LPST(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;
            string[] criteria = InputMart.Instance.PRESET_INFO.Rows.Where(x => x.PRESET_ID == (target as AoEquipment).Preset.Name && x.FACTOR_ID == factor.Name).FirstOrDefault().CRITERIA.Split('/');

            float maxValue = SAPSUtils.CalcOptionValue(criteria[0], float.Parse(criteria[1]));
            float termValue = SAPSUtils.CalcOptionValue(criteria[2], float.Parse(criteria[3]));

            DateTime dueDate = SimHelper.MinDateTime;
            if (lot.CurrentPlan.PegInfoList != null && lot.CurrentPlan.PegInfoList.Count > 0)
            {
                var peg = lot.CurrentPlan.PegInfoList.Where(r => r.Qty > 0).OrderBy(r => r.StepTarget.DueDate).FirstOrDefault();
                dueDate = peg == null ? SimHelper.MaxDateTime : peg.StepTarget.DueDate;
            }
            else
                return new WeightValue(0);

            TimeSpan diff = dueDate - now;

            float diffSeconds = Convert.ToSingle(diff.TotalSeconds);
            float section = Convert.ToSingle(Math.Truncate(maxValue / termValue));
            float weight = SAPSUtils.SetWeight(maxValue, diffSeconds, section, Constants.DESC);

            return new WeightValue(weight * factor.Factor);
        }

        public WeightValue FIFO(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;
            string[] criteria = InputMart.Instance.PRESET_INFO.Rows.Where(x => x.PRESET_ID == (target as AoEquipment).Preset.Name && x.FACTOR_ID == factor.Name).FirstOrDefault().CRITERIA.Split('/');

            float maxValue = SAPSUtils.CalcOptionValue(criteria[0], float.Parse(criteria[1]));
            float termValue = SAPSUtils.CalcOptionValue(criteria[2], float.Parse(criteria[3]));

            TimeSpan waitTime = now - lot.DispatchInTime;
            float waitSeconds = Convert.ToSingle(waitTime.TotalSeconds);
            float weight = 0f;

            float section = Convert.ToSingle(Math.Truncate(maxValue / termValue));
            weight = SAPSUtils.SetWeight(maxValue, waitSeconds, section, Constants.ASC, false);

            return new WeightValue(weight * factor.Factor);
        }

        public WeightValue IS_SETUP(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;
            AoEquipment eqp = target as AoEquipment;

            if (SimRun.Instance.OnIsNeedSetup(eqp, entity as IHandlingBatch))
                return new WeightValue(0);
            else
                return new WeightValue(factor.Factor);
        }

        public WeightValue LIFO(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;
            string[] criteria = InputMart.Instance.PRESET_INFO.Rows.Where(x => x.PRESET_ID == (target as AoEquipment).Preset.Name && x.FACTOR_ID == factor.Name).FirstOrDefault().CRITERIA.Split('/');

            float maxValue = SAPSUtils.CalcOptionValue(criteria[0], float.Parse(criteria[1]));
            float termValue = SAPSUtils.CalcOptionValue(criteria[2], float.Parse(criteria[3]));

            TimeSpan waitTime = now - lot.DispatchInTime;

            float waitSeconds = Convert.ToSingle(waitTime.TotalSeconds);
            float weight = 0f;

            float section = Convert.ToSingle(Math.Truncate(maxValue / termValue));
            weight = SAPSUtils.SetWeight(maxValue, waitSeconds, section, Constants.DESC, false);

            return new WeightValue(weight * factor.Factor);
        }

        public WeightValue LPT(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;
            AoEquipment eqp = target as AoEquipment;

            string[] criteria = InputMart.Instance.PRESET_INFO.Rows.Where(x => x.PRESET_ID == (target as AoEquipment).Preset.Name && x.FACTOR_ID == factor.Name).FirstOrDefault().CRITERIA.Split('/');

            float maxValue = SAPSUtils.CalcOptionValue(criteria[0], float.Parse(criteria[1]));
            float termValue = SAPSUtils.CalcOptionValue(criteria[2], float.Parse(criteria[3]));

            float flowTimeSeconds = TimeHelper.GetFlowTime(entity, eqp);

            float section = Convert.ToSingle(Math.Truncate(maxValue / termValue));
            float weight = SAPSUtils.SetWeight(maxValue, flowTimeSeconds, section, Constants.ASC);

            return new WeightValue(weight * factor.Factor);
        }

        public WeightValue MIN_SETUP(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            var eqp = target as AoEquipment;
            var lastPlan = (Mozart.SeePlan.General.DataModel.PlanInfo)eqp.LastPlan;
            string curProdID = (entity as SmartAPSLot).CurrentProductID;
            float setupTime = Convert.ToSingle(TimeHelper.GetSetupTime(eqp, entity as IHandlingBatch).TotalSeconds);
            double maxSetupTime = Double.Parse(ctx["maxSetupTime"].ToString());
            float weight = 0f;

            if (lastPlan == null)
                return new WeightValue(weight * factor.Factor);

            if (curProdID == lastPlan.ProductID)
                return new WeightValue(factor.Factor);

            if (maxSetupTime == 0f)
                weight = 0f;
            else
                weight = 1 - Convert.ToSingle(setupTime / maxSetupTime);

            return new WeightValue(weight * factor.Factor);
        }

        public WeightValue SPT(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            SmartAPSLot lot = (entity as IHandlingBatch).Sample as SmartAPSLot;
            AoEquipment eqp = target as AoEquipment;

            string[] criteria = InputMart.Instance.PRESET_INFO.Rows.Where(x => x.PRESET_ID == (target as AoEquipment).Preset.Name && x.FACTOR_ID == factor.Name).FirstOrDefault().CRITERIA.Split('/');

            float maxValue = SAPSUtils.CalcOptionValue(criteria[0], float.Parse(criteria[1]));
            float termValue = SAPSUtils.CalcOptionValue(criteria[2], float.Parse(criteria[3]));

            float flowTimeSeconds = TimeHelper.GetFlowTime(entity, eqp);

            float section = Convert.ToSingle(Math.Truncate(maxValue / termValue));
            float weight = SAPSUtils.SetWeight(maxValue, flowTimeSeconds, section, Constants.DESC);

            return new WeightValue(weight * factor.Factor);
        }

        public WeightValue MIN_SETUP_PROPERTY(ISimEntity entity, DateTime now, ActiveObject target, WeightFactor factor, IDispatchContext ctx)
        {
            var eqp = target as AoEquipment;

            var lastPlan = (Mozart.SeePlan.General.DataModel.PlanInfo)eqp.LastPlan;
            string curProdID = (entity as SmartAPSLot).CurrentProductID;
            String[] PropertyName = InputMart.Instance.PRODUCTView.FindRows(curProdID).FirstOrDefault().PROPERTY_NAME.Split(new[] { '@' });

            float setupTime = Convert.ToSingle(TimeHelper.GetSetupTime(eqp, entity as IHandlingBatch).TotalSeconds);
            double maxSetupTime = Double.Parse(ctx["maxSetupTime"].ToString());
            float weight = 0f;

            if (lastPlan == null)
                return new WeightValue(weight * factor.Factor);

            SmartAPSProduct CurProd = InputMart.Instance.SmartAPSProductView.FindRows(curProdID).FirstOrDefault();
            SmartAPSProduct LastProd = InputMart.Instance.SmartAPSProductView.FindRows(lastPlan.ProductID).FirstOrDefault();

            string CurProper = null;
            string LastProper = null;

            foreach (var property in PropertyName)
            {
                CurProd.Property.TryGetValue(property, out var LotPropertyValue);
                LastProd.Property.TryGetValue(property, out var InfopropertyValue);

                CurProper += (string)LotPropertyValue + '@';
                LastProper += (string)InfopropertyValue + '@';
            }

            if (CurProper == LastProper) //Priority 로 변경해줘야함 
                return new WeightValue(factor.Factor);

            if (maxSetupTime == 0f)
                weight = 0f;
            else
                weight = 1 - Convert.ToSingle(setupTime / maxSetupTime);

            return new WeightValue(weight * factor.Factor);
        }
    }
}