using Mozart.SeePlan.Simulation;
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
using Mozart.Simulation.Engine;

namespace SmartAPS.Logic.Simulation
{
    [FeatureBind()]
    public partial class FactoryEvents
    {
        public void ON_BEGIN_INITIALIZE0(Mozart.SeePlan.Simulation.AoFactory aoFactory, ref bool handled)
        {
            // material관련부분 작동하기 위한 초기화 코드 (다른애들은 엔진 내부에서 초기화가 되는데, material 관련 코드는 밖으로 빼놓았기 때문에 초기화가 필요)
            MaterialManager.Instance.Initialize(aoFactory);
        }

        public void ON_DONE0(AoFactory aoFactory, ref bool handled)
        {
            foreach (var lot in InputMart.Instance.MergeLots)
            {
                var urw = CreateHelper.CreateUnkitRemainWiplog();
                string[] mergeKey = lot.MergeKey.Split('@');

                urw.VERSION_NO = ModelContext.Current.VersionNo;
                urw.LOT_ID = lot.LotID;
                urw.FROM_PRODUCT_ID = lot.CurrentProductID;
                if (mergeKey.Count() == 2)
                {
                    urw.TO_PRODUCT_ID = mergeKey[0];
                    urw.STEP_ID = mergeKey[1];
                }
                urw.PREP_CMPL_TIME = lot.MergeInTime;
                urw.WAITING_TIME = (AoFactory.Current.NowDT - lot.MergeInTime).TotalMinutes;
                urw.ORG_QTY = lot.OrgQty;
                urw.REMAIN_QTY = lot.UnitQty;
                urw.ASSEMBLY_CNT = lot.MergeCnt;
                urw.UNKIT_REMAIN_WIPLOG_ID = Guid.NewGuid().ToString();

                OutputMart.Instance.UNKIT_REMAIN_WIPLOG.Add(urw);
            }
        }

        public void ON_DONE1(AoFactory aoFactory, ref bool handled)
        {
            if (ConfigHelper.GetConfig<bool>(Constants.APPLY_SHIFT_TIME))
                WriteHelper.WriteJuYaEqpPlan();

            if (aoFactory != null)
            {
                List<ISimEntity> contents;

                foreach (var tag in aoFactory.WipManager.GetTags())
                {
                    foreach (var entity in aoFactory.WipManager.GetWips(tag))
                    {
                        if (entity.Contents == null)
                            contents = new List<ISimEntity> { entity as SmartAPSLot };
                        else
                            contents = entity.Contents as List<ISimEntity>;

                        foreach (var lot in contents)
                        {
                            WriteHelper.WriteUnloadedLot(lot as SmartAPSLot);
                        }
                    }
                }
            }
        }

        public void ON_DONE_PSI(AoFactory aoFactory, ref bool handled)
        {
            foreach (var psiGroup in InputMart.Instance.SmartAPSPsi.Rows.GroupBy(r => r.customerId + "@" + r.productId + "@" + r.productName))
            {
                string[] keys = psiGroup.Key.Split('@');
                string customer = keys[0];
                string product = keys[1];
                string productName = keys[2];
                double pre_iqty = 0;

                //product,customer 같은 값끼리 for문
                foreach (var psi in psiGroup.OrderBy(r => r.date).GroupBy(r => r.date))
                {
                    string date = psi.Key;
                    double pQty = psi.Where(r => r.item == "P").FirstOrDefault() == null ? 0 : psi.Where(r => r.item == "P").FirstOrDefault().qty;
                    double sQty = psi.Where(r => r.item == "S").FirstOrDefault() == null ? 0 : psi.Where(r => r.item == "S").FirstOrDefault().qty;
                    double iQty = pre_iqty + pQty - sQty;
                    double rtfQty = 0;
                    if (iQty >= 0)
                        rtfQty = sQty;
                    else
                        rtfQty = pQty;

                    double rtfRate = pQty / sQty * 100 > 100 ? 100 : pQty / sQty * 100;

                    WriteHelper.writePsi(customer, product, productName, "P", pQty, rtfRate, date);
                    WriteHelper.writePsi(customer, product, productName, "S", sQty, rtfRate, date);
                    WriteHelper.writePsi(customer, product, productName, "I", iQty, rtfRate, date);
                    WriteHelper.writePsi(customer, product, productName, "RTF", rtfQty, rtfRate, date);

                    pre_iqty = iQty;
                }
            }
        }

        public void ON_DONE_STEP_MOVE(AoFactory aoFactory, ref bool handled)
        {
            Dictionary<string, Dictionary<string, float>> stepMoveDic = new Dictionary<string, Dictionary<string, float>>();

            var eqpPlanGrp = from eqp in OutputMart.Instance.EQP_PLAN.Table.Rows
                             where eqp.EQP_STATE_CODE == Constants.BUSY
                             group eqp by eqp.LINE_ID + "@" + eqp.DEMAND_ID + "@" + eqp.PRODUCT_ID + "@" + eqp.PROCESS_ID + "@" + eqp.STEP_ID + "@" + eqp.EQP_ID into grp
                             select new { grp.Key, Value = grp };

            foreach (var eqpPlan in eqpPlanGrp)
            {
                foreach (var plan in eqpPlan.Value)
                {
                    DateTime startTime = plan.EQP_START_TIME.Date.AddHours(plan.EQP_START_TIME.Hour);
                    DateTime endTime = plan.EQP_END_TIME.Date.AddHours(plan.EQP_END_TIME.Hour);

                    string startKey = eqpPlan.Key + "@" + startTime.ToString();
                    string endKey = eqpPlan.Key + "@" + endTime.ToString();

                    if (stepMoveDic.ContainsKey(startKey))
                    {
                        if (!stepMoveDic[startKey].ContainsKey(Constants.IN))
                            stepMoveDic[startKey].Add(Constants.IN, plan.PROCESS_QTY);
                        else
                            stepMoveDic[startKey][Constants.IN] += plan.PROCESS_QTY;
                    }
                    else
                        stepMoveDic.Add(startKey, new Dictionary<string, float>() { { Constants.IN, plan.PROCESS_QTY } });

                    if (stepMoveDic.ContainsKey(endKey))
                    {
                        if (!stepMoveDic[endKey].ContainsKey(Constants.OUT))
                            stepMoveDic[endKey].Add(Constants.OUT, plan.PROCESS_QTY);
                        else
                            stepMoveDic[endKey][Constants.OUT] += plan.PROCESS_QTY;
                    }
                    else
                        stepMoveDic.Add(endKey, new Dictionary<string, float>() { { Constants.OUT, plan.PROCESS_QTY } });
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, float>> stepMove in stepMoveDic)
            {
                String[] splitArray = stepMove.Key.Split(new String[] { "@" }, StringSplitOptions.None);
                STEP_MOVE sm = CreateHelper.CreateStepMove();
                sm.VERSION_NO = ModelContext.Current.VersionNo;
                sm.LINE_ID = splitArray[0];
                sm.DEMAND_ID = splitArray[1];
                sm.PRODUCT_ID = splitArray[2];
                sm.PROCESS_ID = splitArray[3];
                sm.STEP_ID = splitArray[4];
                sm.EQP_ID = splitArray[5];
                sm.PLAN_DATE = splitArray[6].ToDateTime();
                sm.IN_QTY = stepMove.Value.ContainsKey(Constants.IN) ? stepMove.Value[Constants.IN] : 0;
                sm.OUT_QTY = stepMove.Value.ContainsKey(Constants.OUT) ? stepMove.Value[Constants.OUT] : 0;

                OutputMart.Instance.STEP_MOVE.Add(sm);
            }
        }

        public void ON_START0(AoFactory aoFactory, ref bool handled)
        {
            InputMart.Instance.SimHourWatch = new System.Diagnostics.Stopwatch();
            InputMart.Instance.SimDayWatch = new System.Diagnostics.Stopwatch();

            InputMart.Instance.SimHourWatch.Start();
            InputMart.Instance.SimDayWatch.Start();

            //InputMart.Instance.FactoryShiftName = Mozart.SeePlan.FactoryConfiguration.Current.ShiftNames[InputMart.Instance.FactoryShiftIndex++]; 

        }

        public void ON_DAY_CHANGED0(AoFactory aoFactory, ref bool handled)
        {

        }

        public void ON_SHIFT_CHANGE0(AoFactory aoFactory, ref bool handled)
        {
            
            /*string[] shiftNames = Mozart.SeePlan.FactoryConfiguration.Current.ShiftNames; 
            if (shiftNames.Length == InputMart.Instance.FactoryShiftIndex)
                InputMart.Instance.FactoryShiftIndex = 0; 
                
            InputMart.Instance.FactoryShiftName = shiftNames[InputMart.Instance.FactoryShiftIndex++];*/
            
        }
    }
}