using Mozart.Common;
using Mozart.SeePlan.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.Simulation.Engine;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class WriteHelper
    {
        public static void WriteInputPlan(SmartAPSInTarget it)
        {
            INPUT_PLAN ip = CreateHelper.CreateInputPlan();

            ip.VERSION_NO = ModelContext.Current.VersionNo;
            ip.BATCH_ID = $"{it.DemandID}_{it.Product.ProductID}_{it.TargetDate.ToString("yyyy-MM-dd HH:mm:ss")}";
            ip.PRODUCT_ID = it.Product.ProductID;
            ip.PLAN_DATE = it.TargetDate;
            ip.PLAN_QTY = it.TargetQty;
            ip.INPUT_PLAN_ID = Guid.NewGuid().ToString();

            OutputMart.Instance.INPUT_PLAN.Table.ImportRow(ip);
        }

        public static void WriteReleaseHistory(SmartAPSLot lot, SmartAPSInTarget target)
        {
            RELEASE_HISTORY rh = CreateHelper.CreateReleaseHistory();

            rh.VERSION_NO = ModelContext.Current.VersionNo;
            rh.BATCH_ID = $"{target.DemandID}_{lot.CurrentProductID}_{target.TargetDate.ToString("yyyy-MM-dd HH:mm:ss")}";
            rh.LOT_ID = lot.LotID;
            rh.PRODUCT_ID = lot.CurrentProductID;
            rh.RELEASE_DATE = lot.ReleaseTime;
            rh.INPUT_STEP_ID = lot.CurrentStepID;
            rh.QTY = lot.UnitQty;
            rh.MO_DEMAND_ID = target.DemandID;
            rh.MO_PRODUCT_ID = target.MoProductID;
            rh.RELEASE_HISTORY_ID = Guid.NewGuid().ToString();

            OutputMart.Instance.RELEASE_HISTORY.Table.ImportRow(rh);
        }

        public static void WriteWipHis(WIP entity)
        {
            WIP_HIS ep = CreateHelper.CreateWipHis();

            ep.WIP_HIS_ID = Guid.NewGuid().ToString();
            ep.VERSION_NO = ModelContext.Current.VersionNo;
            ep.LOT_ID = entity.LOT_ID;
            ep.LINE_ID = entity.LINE_ID;
            ep.PRODUCT_ID = entity.PRODUCT_ID;
            ep.PROCESS_ID = entity.PROCESS_ID;
            ep.STEP_ID = entity.STEP_ID;
            ep.UNIT_QTY = entity.UNIT_QTY;
            ep.EQP_ID = entity.EQP_ID;
            ep.LINE_IN_TIME = entity.LINE_IN_TIME;
            ep.STEP_ARRIVE_TIME = entity.STEP_IN_TIME;
            ep.STATE = entity.STATE;
            ep.STATE_TIME = entity.STATE_CHANGE_TIME;
            ep.OUT_QTY = entity.OUT_QTY;
            ep.DEMAND_ID = entity.DEMAND_ID;

            OutputMart.Instance.WIP_HIS.Add(ep);
        }

        public static void WritePresetHis(PRESET_INFO entity)
        {
            PRESET_INFO_HIS pi = CreateHelper.CreatePresetInfoHis();

            pi.VERSION_NO = ModelContext.Current.VersionNo;
            pi.PRESET_ID = entity.PRESET_ID;
            pi.FACTOR_ID = entity.FACTOR_ID;
            pi.FACTOR_TYPE = entity.FACTOR_TYPE;
            pi.FACTOR_WEIGHT = entity.FACTOR_WEIGHT;
            pi.FACTOR_DETAIL = entity.FACTOR_DETAIL;
            pi.ORDER_TYPE = entity.ORDER_TYPE;
            pi.SEQUENCE = entity.SEQUENCE;
            pi.CRITERIA = entity.CRITERIA;
            pi.DESCRIPTION = entity.DESCRIPTION;

            OutputMart.Instance.PRESET_INFO_HIS.Add(pi);
        }

        public static void WriteDemandHis(DEMAND entity)
        {
            DEMAND_HIS ep = CreateHelper.CreateDemandHis();

            ep.DEMAND_HIS_ID = Guid.NewGuid().ToString();
            ep.VERSION_NO = ModelContext.Current.VersionNo;
            ep.DEMAND_VER = entity.DEMAND_VER;
            ep.DEMAND_ID = entity.DEMAND_ID;
            ep.PRODUCT_ID = entity.PRODUCT_ID;
            ep.CUSTOMER_ID = entity.CUSTOMER_ID;
            ep.DUE_DATE = entity.DUE_DATE;
            ep.DEMAND_QTY = entity.DEMAND_QTY;
            ep.PRIORITY = entity.PRIORITY;
            ep.LINE_ID = entity.LINE_ID;

            OutputMart.Instance.DEMAND_HIS.Add(ep);
        }


        public static void WriteEqpPlan(AoEquipment aeqp, IHandlingBatch hb, LoadingStates state, DateTime? endTime = null)
        {
               
            if (state == LoadingStates.IDLE || state == LoadingStates.IDLERUN)
                return;

            IList<ISimEntity> contents;
            if (hb.Contents == null)
                contents = new List<ISimEntity> { hb.Sample as SmartAPSLot };
            else
                contents = hb.Contents;

            foreach (SmartAPSLot lot in contents)
            {

                EQP_PLAN ep = CreateHelper.CreateEqpPlan();

                ep.VERSION_NO = ModelContext.Current.VersionNo;
                ep.LINE_ID = lot.LineID;
                ep.LOT_ID = lot.LotID;
                ep.EQP_STATE_CODE = state.ToString();
                ep.PRODUCT_ID = lot.Product.ProductID;
                ep.PROCESS_ID = lot.Process.ProcessID;
                ep.PROCESS_QTY = lot.UnitQty;
                ep.STEP_IN_TIME = lot.DispatchInTime;
                ep.STEP_ID = lot.CurrentStep == null ? string.Empty : lot.CurrentStep.StepID;
                ep.STEP_TYPE = lot.CurrentStep.StepType;
                ep.EQP_ID = aeqp == null ? "BUCKET" : aeqp.EqpID;
                ep.TOOL_ID = lot.CurrentPlan.ToolID;
                ep.AUTOMATION = aeqp == null ? "" : InputMart.Instance.EQUIPMENT.Rows.Where(x => x.EQP_ID == ep.EQP_ID).FirstOrDefault().AUTOMATION;
                if (lot.CurrentPlan == null || lot.CurrentPlan.PegInfoList == null || lot.CurrentPlan.PegInfoList.Count == 0)
                {
                    ep.TARGET_DATE = SimHelper.MinDateTime;
                    var id = (lot.WipInfo as SmartAPSWipInfo).DemandID;
                    if (string.IsNullOrEmpty(id) == false)
                    {
                        ep.DEMAND_ID = id;
                        ep.DUE_DATE = InputMart.Instance.DEMAND.Rows.Where(x => x.DEMAND_ID == id).FirstOrDefault().DUE_DATE;
                    }
                    else
                    {
                        ep.DEMAND_ID = string.Empty;
                        ep.DUE_DATE = SimHelper.MinDateTime;
                    }
                }
                else
                {
                    var peg = lot.CurrentPlan.PegInfoList.Where(r => r.Qty > 0).OrderBy(r => r.StepTarget.DueDate).FirstOrDefault();
                    ep.TARGET_DATE = peg == null ? SimHelper.MinDateTime : peg.StepTarget.DueDate;
                    ep.DEMAND_ID = peg == null ? string.Empty : (peg.StepTarget as SmartAPSStepTarget).MoPlan.DemandID;
                    ep.DUE_DATE = peg == null ? SimHelper.MinDateTime : (peg.StepTarget as SmartAPSStepTarget).MoPlan.DueDate;
                }

                if (state == LoadingStates.BUSY)
                {

                    ep.EQP_START_TIME = aeqp == null ? lot.CurrentPlan.StartTime : aeqp.NowDT;

                    if (aeqp != null)
                    {
                        ep.EQP_END_TIME = (DateTime)aeqp.GetNextInTime();
                    }
                    else if (endTime != null)
                    {
                        ep.EQP_END_TIME = endTime.Value;
                    }
                }
                else if (state == LoadingStates.SETUP)
                {
                    ep.EQP_START_TIME = aeqp.NowDT;
                    ep.EQP_END_TIME = aeqp.NowDT.AddSeconds(aeqp.SetupTime.TotalSeconds);
                }
                else if (state == LoadingStates.WAIT_SETUP)
                {
                    ep.EQP_START_TIME = aeqp.NowDT;
                    ep.EQP_END_TIME = aeqp.NowDT.AddSeconds(aeqp.WaitSetupTime.TotalSeconds);
                }

                ep.INPUT_PROC_TIME = lot.InputProcTime;
                ep.INPUT_TACT_TIME = lot.InputTactTime;
                ep.UTILIZATION = lot.Utilization;
                ep.EFFICIENCY = lot.Efficiency;
                ep.APPLY_PROC_TIME = lot.ApplyProcTime;
                ep.APPLY_TACT_TIME = lot.ApplyTactTime;
                ep.EQP_PLAN_ID = Guid.NewGuid().ToString();

                OutputMart.Instance.EQP_PLAN.AddBuffer(ep);

                if (!string.IsNullOrEmpty(ep.DEMAND_ID) && ep.STEP_TYPE == "OUT")
                {
                    WritePsiObj(ep.DEMAND_ID, ep.PRODUCT_ID, "P", (double)ep.PROCESS_QTY, ep.EQP_END_TIME);
                }


            }
        }

        public static void WriteEqpDownPlan(AoEquipment aeqp, PeriodSection ps, string state)
        {
            EQP_PLAN ep = CreateHelper.CreateEqpPlan();
            SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;

            SmartAPSLot lot = null;

            if (lot == null && aeqp.LastPlan != null)
            {
                var info = aeqp.LastPlan as SmartAPSPlanInfo;
                lot = info.Lot as SmartAPSLot;
            }

            int breakSeq = 0;
            for (int i = 0; i < eqp.BreakList.Count; i++)
            {
                if (eqp.BreakList[i].StartTime == ps.StartTime)
                {
                    breakSeq = i;
                    break;
                }
            }

            ep.VERSION_NO = ModelContext.Current.VersionNo;
            ep.LOT_ID = state + breakSeq;
            ep.EQP_STATE_CODE = state;
            ep.PRODUCT_ID = "-";
            ep.PROCESS_ID = "-";
            ep.PROCESS_QTY = 0;
            ep.STEP_IN_TIME = lot == null ? SimHelper.MinDateTime : lot.DispatchInTime;
            ep.STEP_ID = "-";
            ep.EQP_ID = aeqp == null ? string.Empty : aeqp.EqpID;
            ep.EQP_START_TIME = ps.StartTime;
            ep.EQP_END_TIME = ps.EndTime;
            ep.EQP_PLAN_ID = Guid.NewGuid().ToString();
            ep.LINE_ID = "-";

            OutputMart.Instance.EQP_PLAN.AddBuffer(ep);
        }

        public static void WritePsiObj(string demandId, string productId, string item, double qty, DateTime date)
        {

            DEMAND demand = null;
            demand = InputMart.Instance.DEMANDViewById.FindRows(demandId, productId).FirstOrDefault();
            if (demand == null) return;

            string inputDate = date.ToString("yyyy-MM-dd");
            string inputCustomer = demand.CUSTOMER_ID;

            var psi = InputMart.Instance.SmartAPSPsiView.FindRows(inputCustomer, productId, item, inputDate).FirstOrDefault();
            if (psi == null)
            {
                SmartAPSPsi tmp = SAPSObjectMapper.Create<SmartAPSPsi>();
                tmp.customerId = inputCustomer;
                tmp.productId = productId;
                // var product = InputMart.Instance.SmartAPSProductView.FindRows(productId).FirstOrDefault();
                var product = InputMart.Instance.PRODUCTView.FindRows(productId).FirstOrDefault();
                tmp.productName = product.PRODUCT_NAME;
                tmp.item = item;
                tmp.date = inputDate;
                tmp.qty = qty;

                InputMart.Instance.SmartAPSPsi.ImportRow(tmp);
            }
            else
            {
                psi.qty += qty;
            }
        }

        public static void writePsi(string customerId, string productId, string productName, string item, double qty, double rtfRate, string date)
        {
            ANALYSIS_PSI inPsi = CreateHelper.CreatePsi();
            inPsi.ANALYSIS_PSI_ID = Guid.NewGuid().ToString();
            inPsi.VERSION_NO = ModelContext.Current.VersionNo;
            inPsi.CUSTOMER_ID = customerId;
            inPsi.PRODUCT_ID = productId;
            inPsi.PRODUCT_NAME = productName;
            inPsi.ITEM = item;
            inPsi.QTY = qty;
            if (Double.IsNaN(rtfRate))
                inPsi.RTF_RATE = null;
            else
                inPsi.RTF_RATE = rtfRate;
            inPsi.PSI_DATE = date;

            OutputMart.Instance.ANALYSIS_PSI.Add(inPsi);
        }

        public static void WriteProdChangeLog(SmartAPSLot fl, SmartAPSLot tl, double fromQty, double toQty, string routeType)
        {
            PRODUCT_ROUTE_LOG prl = CreateHelper.CreateMergeWipLog();

            prl.VERSION_NO = ModelContext.Current.VersionNo;
            prl.FROM_LOT_ID = fl.LotID;
            prl.TO_LOT_ID = tl.LotID;
            prl.FROM_PRODUCT_ID = fl.Product.ProductID;
            prl.TO_PRODUCT_ID = tl.Product.ProductID;
            prl.STEP_ID = tl.CurrentStepID;
            prl.FROM_UNIT_QTY = fromQty;
            prl.TO_UNIT_QTY = toQty;
            prl.CMPL_TIME = AoFactory.Current.NowDT;
            prl.PREP_CMPL_TIME = fl.MergeInTime;
            prl.PRODUCT_ROUTE_LOG_ID = Guid.NewGuid().ToString();
            prl.ROUTE_TYPE = routeType;

            OutputMart.Instance.PRODUCT_ROUTE_LOG.Add(prl);
        }

        public static void WriteMaterialHistory(IMatBom bom, string matId, string lotId, double useQty, string useType, double qty = 0)
        {
            MATERIAL_HISTORY mh = CreateHelper.CreateMaterialHistory();

            mh.VERSION_NO = ModelContext.Current.VersionNo;
            mh.MAT_ID = matId;
            mh.MAT_TYPE = bom.MaterialType;
            mh.PRODUCT_ID = bom.Product.ProductID;
            mh.STEP_ID = bom.Step.StepID;
            mh.LOT_ID = lotId;
            mh.QTY = qty;
            mh.USE_QTY = useQty;
            mh.USE_DATE = AoFactory.Current.NowDT;
            mh.USE_TYPE = useType;
            mh.MATERIAL_HISTORY_ID = Guid.NewGuid().ToString();

            OutputMart.Instance.MATERIAL_HISTORY.Add(mh);
        }

        public static void WriteUnloadedLot(SmartAPSLot lot)
        {
            UNLOADED_LOTLOG ul = CreateHelper.CreateUnloadedLotLog();

            ul.VERSION_NO = ModelContext.Current.VersionNo;
            ul.LOT_ID = lot.LotID;
            ul.LAST_STEP_ID = lot.CurrentStepID;
            ul.PRODUCT_ID = lot.CurrentProductID;
            ul.UNIT_QTY = lot.UnitQty;
            ul.UNLOADED_LOTLOG_ID = Guid.NewGuid().ToString();

            OutputMart.Instance.UNLOADED_LOTLOG.Add(ul);
        }

        public static void WriteInitWipLog(WIP wip, string orgState, string orgStepId = "")
        {
            INIT_WIP_LOG iwl = CreateHelper.CreateInitWipLog();

            iwl.VERSION_NO = ModelContext.Current.VersionNo;
            iwl.LOT_ID = wip.LOT_ID;
            iwl.PRODUCT_ID = wip.PRODUCT_ID;
            iwl.ORG_STEP_ID = orgStepId;
            iwl.REV_STEP_ID = wip.STEP_ID;
            iwl.ORG_STATUS = orgState;
            iwl.REV_STATUS = wip.STATE;
            iwl.AVAILABLE_TIME = wip.STATE_CHANGE_TIME;
            iwl.CATEGORY = Constants.WAITING_LOT_AT_ASSEMBLY_STEP;

            OutputMart.Instance.INIT_WIP_LOG.Add(iwl);
        }

        public static void WriteUnpegHistory(SmartAPSPlanWip wip, string category, string reason)
        {

            UNPEG_HISTORY hist = CreateHelper.CreateUnpegHistory();

            hist.VERSION_NO = ModelContext.Current.VersionNo;
            hist.LINE_ID = (wip.Wip as SmartAPSWipInfo).LineID;
            hist.LOT_ID = wip.Wip.LotID;
            hist.PRODUCT_ID = wip.Wip.Product.ProductID;
            hist.STEP_ID = wip.Wip.WipStepID;
            hist.UNIT_QTY = wip.Wip.UnitQty;
            hist.UNPEG_QTY = wip.Qty;
            hist.UNPEG_CATEGORY = category;
            hist.UNPEG_REASON = reason;
            hist.STATE = wip.State;

            hist = PegOutputMapper.Instance.OnWriteUnPegHistory(hist, wip);
            hist.UNPEG_HISTORY_ID = Guid.NewGuid().ToString();

            OutputMart.Instance.UNPEG_HISTORY.Add(hist);
        }

        public static void WriteUnpegHistory(WIP wip, string category, string reason)
        {
            UNPEG_HISTORY hist = CreateHelper.CreateUnpegHistory();

            hist.VERSION_NO = ModelContext.Current.VersionNo;
            hist.LINE_ID = wip.LINE_ID;
            hist.LOT_ID = wip.LOT_ID;
            hist.PRODUCT_ID = wip.PRODUCT_ID;
            hist.STEP_ID = wip.STEP_ID;
            hist.UNIT_QTY = wip.UNIT_QTY;
            hist.UNPEG_QTY = wip.UNIT_QTY;
            hist.UNPEG_CATEGORY = category;
            hist.UNPEG_REASON = reason;
            hist.STATE = wip.STATE;
            hist.UNPEG_HISTORY_ID = Guid.NewGuid().ToString();
            //hist = PegOutputMapper.Instance.OnWriteUnPegHistory(hist, wip);

            OutputMart.Instance.UNPEG_HISTORY.Add(hist);
        }

        public static void WriteToolSeizeLog(ToolSettings tool, AoEquipment eqp, string state)
        {
            SmartAPSTool t = tool.Data as SmartAPSTool;
            var pool = eqp.Factory.GetResourcePool("Tools");
            ToolItem toolItem = tool.Items[0];
            SecondResource sres = pool.GetResource(toolItem.ResourceKey.ToString(), eqp);
            TOOL_SEIZE_LOG tsl = CreateHelper.CreateToolSeizeLog();
            tsl.VERSION_NO = ModelContext.Current.VersionNo;
            tsl.EQP_ID = eqp.EqpID;
            tsl.TOOL_ID = toolItem.ResourceKey.ToString();
            tsl.TOOL_LIST = string.Join(";", t.Tools);
            if (state == "SEIZE")
                tsl.SEIZE_TIME = eqp.NowDT;
            else if (state == "RELEASE")
                tsl.RELEASE_TIME = eqp.NowDT;

            tsl.AVAILABLES = sres.Availables;
            tsl.CAPACITY = sres.Capacity;

            OutputMart.Instance.TOOL_SEIZE_LOG.Add(tsl);
        }

        public static void WriteJuYaEqpPlan()
        {
            //var shiftTime = InputMart.Instance.SHIFT_TIME_CONFIG.Rows;

            //DateTime SPLIT_JU_START_TIME = DateTime.Parse(shiftTime.Where(p => p.SHIFT_NAME == "주간").Select(x => x.SPLIT_START_TIME).FirstOrDefault());
            //DateTime SPLIT_JU_END_TIME = DateTime.Parse(shiftTime.Where(p => p.SHIFT_NAME == "주간").Select(x => x.SPLIT_END_TIME).FirstOrDefault());
            //DateTime SPLIT_YA_START_TIME = DateTime.Parse(shiftTime.Where(p => p.SHIFT_NAME == "야간").Select(x => x.SPLIT_START_TIME).FirstOrDefault());
            //DateTime SPLIT_YA_END_TIME = DateTime.Parse(shiftTime.Where(p => p.SHIFT_NAME == "야간").Select(x => x.SPLIT_END_TIME).FirstOrDefault());

            //var eqpPlans = OutputMart.Instance.EQP_PLAN.Buffer.Rows;
            //int decimalPoint = ConfigHelper.GetConfig<int>(Constants.DECIMAL_POINT);

            //foreach (EQP_PLAN eqpPlan in eqpPlans)
            //{
            //    int cnt = 1;
            //    TimeSpan beforeTime = TimeSpan.Zero;
            //    TimeSpan afterTime = TimeSpan.Zero;
            //    string juyaCheck = null;
            //    List<SmartAPSPMList> pmList = new List<SmartAPSPMList>();
            //    SmartAPSPMList FirstPL = new SmartAPSPMList();
            //    SmartAPSPMList LastPL = new SmartAPSPMList();

            //    var eqpStartTime = eqpPlan.EQP_START_TIME;
            //    var eqpEndTime = eqpPlan.EQP_END_TIME;
            //    var startdays = eqpStartTime.Day - SPLIT_JU_END_TIME.Day;
            //    var endDays = eqpEndTime.Day - SPLIT_JU_END_TIME.Day;

            //    double totalSecound = 0;
            //    double beforeQty = 0;
            //    double afterQty = 0;
            //    double pmresultQty = 0;

            //    TimeSpan pmTime = TimeSpan.Zero;
            //    if (eqpPlan.EQP_STATE_CODE == "BUSY")
            //    {
            //        if (eqpStartTime < SPLIT_JU_END_TIME.AddDays(startdays) && eqpEndTime > SPLIT_YA_START_TIME.AddDays(startdays)) //주간에 나뉘어지는 경우 
            //        {
            //            pmList = InputMart.Instance.SmartAPSPMList.Rows.Where(x => x.LineID.Equals(eqpPlan.LINE_ID) &&
            //                                                                            x.PM_Start_Time >= eqpStartTime &&
            //                                                                            x.PM_End_Time <= eqpEndTime).OrderBy(x => x.PM_Start_Time).ToList();

            //            foreach (SmartAPSPMList pm in pmList)
            //            {
            //                var Count = InputMart.Instance.SmartAPSPMList.Rows.Where(x => x.LineID.Equals(pm.LineID) &&
            //                                                                        x.PM_Start_Time <= pm.PM_Start_Time &&
            //                                                                        x.PM_End_Time >= pm.PM_End_Time).ToList().Count();

            //                if (Count > 1)
            //                    continue;

            //                pmTime += pm.PM_Run_Time;
            //            }

            //            juyaCheck = "주간";
            //            cnt++;
            //        }
            //        else if (eqpStartTime < SPLIT_YA_END_TIME.AddDays(startdays) && eqpEndTime > SPLIT_JU_START_TIME.AddDays(startdays)) //야간에 나뉘어 지는 경우 
            //        {
            //            pmList = InputMart.Instance.SmartAPSPMList.Rows.Where(x => x.LineID.Equals(eqpPlan.LINE_ID) &&
            //                                                                            x.PM_Start_Time >= eqpStartTime &&
            //                                                                            x.PM_End_Time <= eqpEndTime).OrderBy(x => x.PM_Start_Time).ToList();
            //            foreach (SmartAPSPMList pm in pmList)
            //            {
            //                var Count = InputMart.Instance.SmartAPSPMList.Rows.Where(x => x.LineID.Equals(pm.LineID) &&
            //                                                                        x.PM_Start_Time <= pm.PM_Start_Time &&
            //                                                                        x.PM_End_Time >= pm.PM_End_Time).ToList().Count();
            //                if (Count > 1)
            //                    continue;

            //                pmTime += pm.PM_Run_Time;
            //            }

            //            juyaCheck = "야간";
            //            cnt++;
            //        }
            //    }

            //    if (juyaCheck != null)
            //    {
            //        float qtyTemp = 0;

            //        if (pmList.Count() > 0)
            //        {
            //            FirstPL = pmList.OrderBy(x => x.PM_Start_Time).FirstOrDefault();
            //            LastPL = pmList.OrderByDescending(x => x.PM_End_Time).FirstOrDefault();
                        
            //            if (endDays - startdays > 1)
            //            {
            //                LastPL = pmList.Where(t => t.PM_Start_Time < SPLIT_JU_START_TIME.AddDays(endDays)).OrderByDescending(t => t.PM_End_Time).FirstOrDefault();
            //                if(pmList.OrderByDescending(x => x.PM_End_Time).FirstOrDefault().PM_Start_Time != LastPL.PM_Start_Time)
            //                {
            //                    pmTime -= pmList.OrderByDescending(x => x.PM_End_Time).FirstOrDefault().PM_Run_Time;
            //                }
            //            }

            //            beforeTime = LastPL.PM_End_Time - eqpStartTime;
            //            afterTime = eqpEndTime - LastPL.PM_End_Time;

            //            if (pmTime != TimeSpan.Zero)
            //            {
            //                beforeTime = beforeTime - pmTime;
            //                pmresultQty = Math.Abs(beforeTime.TotalSeconds) / eqpPlan.APPLY_PROC_TIME;
            //            }
            //            totalSecound = beforeTime.TotalMilliseconds + afterTime.TotalMilliseconds;
            //            beforeQty = beforeTime.TotalMilliseconds / totalSecound;
            //            afterQty = afterTime.TotalMilliseconds / totalSecound;

            //        }

            //        for (int i = 0; i < cnt; i++)
            //        {
            //            EQP_PLAN ep = CreateHelper.CreateEqpPlan();
            //            ep.VERSION_NO = ModelContext.Current.VersionNo;
            //            ep.LINE_ID = eqpPlan.LINE_ID;
            //            ep.LOT_ID = juyaCheck == "주간" ? eqpPlan.LOT_ID + "_1" : juyaCheck == "야간" ? eqpPlan.LOT_ID + "_2" : eqpPlan.LOT_ID;
            //            if (eqpPlan.EQP_STATE_CODE == "BUSY")
            //            {
            //                if (i == 1)
            //                    ep.EQP_START_TIME = LastPL.PM_End_Time;
            //                else
            //                    ep.EQP_START_TIME = eqpPlan.EQP_START_TIME;

            //                if (i == 1)
            //                    ep.EQP_END_TIME = eqpPlan.EQP_END_TIME;
            //                else
            //                    ep.EQP_END_TIME = juyaCheck != null ? LastPL.PM_End_Time : eqpPlan.EQP_END_TIME;
            //            }

            //            if (i == 1)
            //                ep.LOT_ID = juyaCheck == "주간" ? eqpPlan.LOT_ID + "_2" : eqpPlan.LOT_ID + "_1";

            //            ep.EQP_STATE_CODE = eqpPlan.EQP_STATE_CODE;
            //            ep.PRODUCT_ID = eqpPlan.PRODUCT_ID;
            //            ep.PROCESS_ID = eqpPlan.PROCESS_ID;
            //            ///
            //            if (i == 1)
            //            {
            //                ep.PROCESS_QTY = eqpPlan.PROCESS_QTY - qtyTemp;
            //                qtyTemp = 0;
            //            }
            //            else
            //            {
            //                if (pmresultQty != 0)
            //                {
            //                    ep.PROCESS_QTY = (float)ConfigHelper.DecimalFormatHalper(pmresultQty, decimalPoint);
            //                    qtyTemp = ep.PROCESS_QTY;
            //                }
            //                else
            //                {
            //                    ep.PROCESS_QTY = juyaCheck == null ? eqpPlan.PROCESS_QTY : i == 1
            //                        ? (float)ConfigHelper.DecimalFormatHalper((eqpPlan.PROCESS_QTY * afterQty), decimalPoint)
            //                        : (float)ConfigHelper.DecimalFormatHalper((eqpPlan.PROCESS_QTY * beforeQty), decimalPoint);
            //                    qtyTemp = ep.PROCESS_QTY;
            //                }
            //            }
            //            ///
            //            ep.STEP_IN_TIME = eqpPlan.STEP_IN_TIME;
            //            ep.STEP_ID = eqpPlan.STEP_ID;
            //            ep.STEP_TYPE = eqpPlan.STEP_TYPE;
            //            ep.EQP_ID = eqpPlan.EQP_ID;
            //            ep.TOOL_ID = eqpPlan.TOOL_ID;
            //            ep.AUTOMATION = eqpPlan.AUTOMATION;
            //            ep.TARGET_DATE = eqpPlan.TARGET_DATE;
            //            ep.DEMAND_ID = eqpPlan.DEMAND_ID;
            //            ep.DUE_DATE = eqpPlan.DUE_DATE;
            //            ep.INPUT_PROC_TIME = eqpPlan.INPUT_PROC_TIME;
            //            ep.INPUT_TACT_TIME = eqpPlan.INPUT_TACT_TIME;
            //            ep.UTILIZATION = eqpPlan.UTILIZATION;
            //            ep.EFFICIENCY = eqpPlan.EFFICIENCY;
            //            ep.APPLY_PROC_TIME = eqpPlan.APPLY_PROC_TIME;
            //            ep.APPLY_TACT_TIME = eqpPlan.APPLY_TACT_TIME;
            //            ep.EQP_PLAN_ID = eqpPlan.EQP_PLAN_ID;
            //            InputMart.Instance.WriteEqpPlans.Add(ep);
            //        }
            //        InputMart.Instance.RemoveEqpPlan.Add(eqpPlan);
            //    }
            //}
            //foreach (EQP_PLAN eqpPlan in InputMart.Instance.RemoveEqpPlan)
            //    OutputMart.Instance.EQP_PLAN.RemoveBuffer(eqpPlan);

            //foreach (EQP_PLAN ep in InputMart.Instance.WriteEqpPlans)
            //    OutputMart.Instance.EQP_PLAN.AddBuffer(ep);

            //var orderbyBuffer = OutputMart.Instance.EQP_PLAN.Buffer.Rows.OrderBy(x => x.EQP_START_TIME).ThenBy(x => x.EQP_ID);
            //foreach (EQP_PLAN obBuffer in orderbyBuffer)
            //{
            //    OutputMart.Instance.EQP_PLAN.RemoveBuffer(obBuffer);
            //    OutputMart.Instance.EQP_PLAN.AddBuffer(obBuffer);
            //}

            //var pmlist = InputMart.Instance.SmartAPSPMList.Rows;
        }

        public static void WriteSmartAPSProductRoute(PRODUCT_ROUTE st, SmartAPSPegPart pp)
        {
            SmartAPSProductRoute pr = CreateHelper.CreateSmartAPSProductRoute();

            pr.FromProductId = st.FROM_PRODUCT_ID;
            pr.ToProductId = st.TO_PRODUCT_ID;
            pr.MoDemandId = pp.DemandID;
            //pr.MoProductId = pp.MoMaster.MoPlanList;
            pr.StepId = st.STEP_ID;
            InputMart.Instance.SmartAPSProductRoute.Rows.Add(pr);

        }
    }
}
