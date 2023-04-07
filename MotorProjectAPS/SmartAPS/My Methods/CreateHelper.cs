using Mozart.Common;
using Mozart.SeePlan.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Inputs;
using SmartAPS.Logic;
using SmartAPS.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartAPS
{
    [FeatureBind()]
    public static partial class CreateHelper
    {
        #region Common Input

        public static SmartAPSStep CreateStep(STEP_ROUTE entity)
        {
            SmartAPSStep step = SAPSObjectMapper.Create<SmartAPSStep>();

            step.Sequence = entity.STEP_SEQ;
            step.StepID = entity.STEP_ID;
            step.Key = step.StepID;
            step.StdStepID = entity.STD_STEP_ID;
            step.StepType = "MAIN";
            step.StepType = entity.STEP_TYPE;

            //표준Step 을 찾아서 연결
            step.StdStep = InputMart.Instance.SmartAPSStdStepView.FindRows(step.StepID).FirstOrDefault();

            if (step.StdStep == null)
            {
                string item = $"{nameof(step.StepID)} = '{step.StepID}'";
                ErrorHelper.WriteError(Constants.IN0018, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_PROCESS", "STD_STEP data is missing", item, step: step.StepID);
                return null;
            }

            return step;
        }

        public static SmartAPSProcess CreateProcess(PROCESS entity)
        {
            SmartAPSProcess proc = SAPSObjectMapper.Create<SmartAPSProcess>();
            proc.ProcessID = entity.PROCESS_ID;
            return proc;
        }

        public static SmartAPSProduct CreateProduct(PRODUCT entity)
        {
            SmartAPSProduct prod = SAPSObjectMapper.Create<SmartAPSProduct>();
            prod.ProductID = entity.PRODUCT_ID;
            prod.LotSize = entity.LOT_SIZE;
            Dictionary<string, object> PropertyDic = new Dictionary<string, object>();

            if (entity.PROPERTY_NAME != null)
            {
                string[] PropertyName = entity.PROPERTY_NAME.Split(new[] { '@' });
                string[] PropertyValue = entity.PROPERTY_VALUE.Split(new[] { '@' });

                foreach (var property in PropertyName.Select((value, index) => new { value = value, index = index }))
                {
                    PropertyDic.Add(property.value, PropertyValue[property.index]);
                }

                prod.Property = PropertyDic;
            }

            if (Enum.TryParse<ProductType>(entity.PRODUCT_TYPE, out ProductType prodType))
                prod.ProductType = prodType;
            else
                prod.ProductType = ProductType.FG;

            SmartAPSProcess proc = BopHelper.GetProcess(entity.PROCESS_ID);

            if (proc != null)
            {
                prod.Process = proc;
            }
            else
            {
                string item = $"{nameof(entity.PROCESS_ID)} = '{entity.PROCESS_ID}'";
                ErrorHelper.WriteError(Constants.IN0002, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_PRODUCT", "PROCESS data is missing",
                    item, product: prod.ProductID);
                return null;
            }

            return prod;
        }

        public static SmartAPSStdStep CreateStdStep(STD_STEP_INFO entity)
        {
            SmartAPSStdStep std = SAPSObjectMapper.Create<SmartAPSStdStep>();

            std.StdStepID = entity.STD_STEP_ID;
            std.StepName = entity.STD_STEP_NAME;
            std.StepTAT = SAPSUtils.ConvertPeriodUnit(entity.STEP_TAT, entity.UNIT);
            std.StepYield = entity.STEP_YIELD;
            std.StepSetup = SAPSUtils.ConvertPeriodUnit(entity.STEP_SETUP, entity.UNIT);
            std.Capacity = entity.BUCKET_CAPACITY​;
            std.TransferTime = SAPSUtils.ConvertPeriodUnit(entity.TRANSFER_TIME, entity.UNIT);
            std.StepSeq = entity.STEP_SEQ;

            return std;
        }

        public static SmartAPSWipInfo CreateWipInfo(WIP entity)
        {

            // 중복된 Wip이 있을 경우 가장 처음 로딩된 Wip을 기준으로 생산 
            //   => 우선순위 
            //      1. FIRM_PLAN
            //      2. WIP
            SmartAPSWipInfo fwip = InputMart.Instance.SmartAPSWipInfoView.FindRows(entity.LOT_ID).FirstOrDefault();
            if (fwip != null)
            {
                string item = $"{nameof(fwip.LotID)} = {fwip.LotID}";
                ErrorHelper.WriteError(Constants.IN0003, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "CreateWipInfo", "Duplicated Wip Info!",
                    item, null, fwip.WipProductID);
                WriteHelper.WriteUnpegHistory(entity, Constants.INVALID_INPUT, Constants.DUPLICATED_WIP_INFO);
                return null;
            }

            SmartAPSWipInfo wip = SAPSObjectMapper.Create<SmartAPSWipInfo>();

            wip.LotID = entity.LOT_ID;

            if (wip.LotID == "10W210825D001-006_2441_27")
                Console.Write("A");

            wip.WipProductID = entity.PRODUCT_ID;
            wip.WipProcessID = entity.PROCESS_ID;
            wip.WipStepID = entity.STEP_ID;
            wip.WipEqpID = entity.EQP_ID;
            wip.WipState = entity.STATE;
            wip.WipStateTime = entity.STATE_CHANGE_TIME;
            wip.LineID = entity.LINE_ID;
            wip.LineInTime = entity.LINE_IN_TIME;
            wip.StepArrivalTime = entity.STEP_IN_TIME;
            wip.UnitQty = entity.UNIT_QTY;
            wip.OutQty = entity.OUT_QTY;
            wip.DemandID = entity.DEMAND_ID;
            wip.PrepEffEndTime = entity.PREP_EFF_END_TIME;

            SmartAPSProduct prod = BopHelper.GetProduct(wip.WipProductID);
            if (prod == null)
            {
                string item = $"{nameof(wip.LotID)} = '{wip.LotID}'";
                ErrorHelper.WriteError(Constants.IN0004, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_WIP", "PRODUCT data is missing",
                    item, null, wip.WipProductID, lotid: wip.LotID);
                WriteHelper.WriteUnpegHistory(entity, Constants.INVALID_INPUT, Constants.NOT_FOUND_PRODUCT_ID);

                return null;
            }


            SmartAPSStep step = prod.Process.FindStep(entity.STEP_ID) as SmartAPSStep;
            if (step == null)
            {
                if (string.IsNullOrWhiteSpace(entity.STEP_ID))
                {
                    step = prod.Process.FirstStep as SmartAPSStep;
                }
                else
                {
                    string item = $"{nameof(wip.LotID)} = '{wip.LotID}', {nameof(entity.STEP_ID)} = '{entity.STEP_ID}'";
                    ErrorHelper.WriteError(Constants.IN0005, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_WIP", "STEP data does not exist in STEP_ROUTE",
                        item, null, wip.WipProductID, entity.STEP_ID, null, wip.LotID);
                    WriteHelper.WriteUnpegHistory(entity, Constants.INVALID_INPUT, Constants.NOT_EXIST_STEP_ROUTE);

                    return null;
                }
            }

            var stdStep = InputMart.Instance.SmartAPSStdStepView.FindRows(step.StepID);

            if (stdStep == null)
            {
                string item = $"{nameof(wip.LotID)} = '{wip.LotID}', {nameof(step.StepID)} = '{step.StepID}'";
                ErrorHelper.WriteError(Constants.IN0006, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_WIP", "STD_STEP data is missing",
                    item, product: wip.WipProductID, lotid: wip.LotID);
                WriteHelper.WriteUnpegHistory(entity, Constants.INVALID_INPUT, Constants.NOT_FOUND_STEP_ID);

                return null;
            }

            // 장비 로딩 상태인 경우 장비정보에 대한 Validation 처리
            SmartAPSEqp eqp = null;
            if (string.IsNullOrEmpty(entity.EQP_ID) == false)
            {
                eqp = InputMart.Instance.SmartAPSEqpView.FindRows(entity.EQP_ID).SingleOrDefault();
                if (eqp == null)
                {
                    string item = $"{nameof(wip.LotID)} = '{wip.LotID}'";
                    ErrorHelper.WriteError(Constants.IN0007, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_WIP", "Loaded EQUIPMENT data is mismatched",
                        item, product: wip.WipProductID, step: wip.WipStepID, equipment: wip.WipEqpID, lotid: wip.LotID);
                    WriteHelper.WriteUnpegHistory(entity, Constants.INVALID_INPUT, Constants.NOT_FOUND_EQP_ID);

                    // 장비 정보가 맞지 않으면 Wip 정보 사용하지 않음
                    return null;
                }
            }

            if (Enum.TryParse<EntityState>(wip.WipState, out EntityState es) == false)
            {
                string item = $"{nameof(wip.WipState)} = '{wip.WipState}'";
                ErrorHelper.WriteError(Constants.IN0008, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_WIP", "WIP state is not defined",
                    item, product: wip.WipProductID, step: wip.WipStepID, equipment: wip.WipEqpID, lotid: wip.LotID);
                WriteHelper.WriteUnpegHistory(entity, Constants.INVALID_INPUT, Constants.NOT_DEFINED_WIP_STATE);
                return null;
            }

            wip.CurrentState = es;
            wip.Product = prod;
            wip.Process = prod.Process;
            wip.InitialEqp = eqp;
            wip.InitialStep = step;

            return wip;
        }

        internal static SmartAPSOutAct CreateOutAct(OUT_ACT entity)
        {
            SmartAPSOutAct stock = SAPSObjectMapper.Create<SmartAPSOutAct>();

            SmartAPSProduct prod = BopHelper.GetProduct(entity.PRODUCT_ID);
            if (prod == null)
            {
                string item = $"{nameof(entity.PRODUCT_ID)} = '{entity.PRODUCT_ID}'";
                ErrorHelper.WriteError(Constants.IN0015, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_OUTACT", "PRODUCT data is missing",
                    item, product: entity.PRODUCT_ID);

                return null;
            }

            stock.Product = prod;
            stock.InQty = entity.ACT_QTY;
            stock.InDate = entity.OUT_DATE;

            return stock;
        }

        public static MatBom CreateMatBom(MATERIAL_BOM entity)
        {
            MatBom bom = SAPSObjectMapper.Create<MatBom>();

            bom.MaterialType = entity.MAT_TYPE;

            SmartAPSProduct prod = BopHelper.GetProduct(entity.PRODUCT_ID);
            if (prod == null)
            {
                string item = $"{nameof(bom.MaterialType)} = '{bom.MaterialType}'";
                ErrorHelper.WriteError(Constants.IN0016, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_MATBOM", "PRODUCT data is missing",
                    item, product: entity.PRODUCT_ID);

                return null;
            }

            SmartAPSStep step = prod.Process.FindStep(entity.STEP_ID) as SmartAPSStep;
            if (step == null)
            {
                string item = $"{nameof(bom.MaterialType)} = '{bom.MaterialType}', {nameof(entity.STEP_ID)} = '{entity.STEP_ID}'";
                ErrorHelper.WriteError(Constants.IN0017, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_MATBOM", "STEP data is missing",
                    item, product: entity.PRODUCT_ID, step: entity.STEP_ID);

                return null;
            }

            var stdStep = InputMart.Instance.SmartAPSStdStepView.FindRows(entity.STEP_ID);
            if (stdStep.Count() == 0)
            {
                string item = $"{nameof(bom.MaterialType)} = '{bom.MaterialType}', {nameof(entity.STEP_ID)} = '{entity.STEP_ID}'";
                ErrorHelper.WriteError(Constants.IN0024, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_MATBOM", "STD_STEP data is missing", item, product: entity.PRODUCT_ID, step: entity.STEP_ID);

                return null;
            }

            bom.Product = prod;
            bom.Step = step;
            bom.CompQty = entity.COMP_QTY;

            var mats = MaterialManager.Instance.FindMatPlans(bom.MaterialType);
            if (mats != null && mats.Count() > 0)
                bom.MatPlans.AddRange(mats);

            return bom;
        }

        public static SmartAPSFirmPlan CreateFirmPlan(FIRM_PLAN entity)
        {
            SmartAPSFirmPlan plan = SAPSObjectMapper.Create<SmartAPSFirmPlan>();

            plan.LotID = entity.LOT_ID;
            plan.StepID = entity.STEP_ID;
            plan.EqpID = string.IsNullOrEmpty(entity.EQP_ID) ? string.Empty : entity.EQP_ID;
            plan.StartTime = entity.START_TIME;

            var wipinfo = InputMart.Instance.SmartAPSWipInfo.Rows.Where(w => w.LotID == entity.LOT_ID).FirstOrDefault();
            if (wipinfo == null)
            {
                string item = $"{nameof(entity.LOT_ID)} = '{entity.LOT_ID}'";
                ErrorHelper.WriteError(Constants.IN0033, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_FIRMPLAN", "FIRM_PLAN's LOT ID is mismatched", item, lotid: entity.LOT_ID);
                return null;

                //plan.EndTime = entity.END_TIME;
            }

            else
            {
                var wipErrange = InputMart.Instance.EQP_ARRANGETimeView.FindRows(wipinfo.WipProductID, wipinfo.WipProcessID, entity.STEP_ID, entity.EQP_ID).FirstOrDefault();
                var wipEqp = InputMart.Instance.EQUIPMENT.Rows.Where(x => x.EQP_ID == plan.EqpID).FirstOrDefault();

                if (wipEqp == null || wipErrange == null)
                {

                    string item = $"{nameof(entity.LOT_ID)} = '{entity.LOT_ID}'";
                    ErrorHelper.WriteError(Constants.IN0033, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_FIRMPLAN", "FIRM_PLAN data does not exist in EQP ", item, lotid: entity.LOT_ID, equipment: entity.EQP_ID);
                    return null;

                    //plan.EndTime = entity.END_TIME;
                }
                else
                {
                    float flowTimeSeconds;
                    switch (wipEqp.SIM_TYPE)
                    {
                        case "INLINE":
                            flowTimeSeconds = Convert.ToSingle(wipErrange.PROC_TIME + (wipErrange.PROC_TIME * (wipinfo.UnitQty - wipinfo.OutQty - 1)));
                            break;
                        case "TABLE":
                            flowTimeSeconds = Convert.ToSingle((wipErrange.PROC_TIME) * wipinfo.UnitQty);
                            break;
                        default:
                            flowTimeSeconds = Convert.ToSingle((wipErrange.PROC_TIME) * wipinfo.UnitQty);
                            break;
                    }
                    if (flowTimeSeconds != 0)
                        plan.EndTime = entity.START_TIME.AddSeconds(flowTimeSeconds);
                    else
                        plan.EndTime = entity.END_TIME;
                }
            }

            plan.Key = SAPSUtils.CreateKey(entity.LOT_ID, entity.STEP_ID);

            if (Enum.TryParse<FirmType>(entity.FIRM_TYPE, out FirmType firmType))
                plan.FirmType = firmType;
            else
                plan.FirmType = FirmType.NORMAL;

            return plan;
        }

        public static MatPlan CreateMatPlan(REPLENISH_PLAN entity)
        {
            MatPlan plan = SAPSObjectMapper.CreateMatPlan();

            plan.MaterialID = entity.MAT_ID + Guid.NewGuid().ToString();
            plan.MaterialType = entity.MAT_TYPE;
            plan.Qty = entity.MAT_QTY;
            plan.IsInfinite = false;
            plan.ReplenishDate = entity.REPLENISH_DATE;
            plan.MatType = MatType.Plan;

            return plan;
        }

        public static MatPlan CreateMatPlan(MATERIAL entity)
        {
            MatPlan plan = SAPSObjectMapper.CreateMatPlan();

            plan.MaterialID = entity.MAT_ID;
            plan.MaterialType = entity.MAT_TYPE;
            plan.Qty = entity.MAT_QTY;
            plan.IsInfinite = SAPSUtils.BoolYN(entity.IS_INFINITY);
            plan.ReplenishDate = InputMart.Instance.GlobalParameters.start_time;
            plan.MatType = MatType.Inv;

            return plan;
        }

        public static SmartAPSWipInfo CreateWipInfo(SmartAPSProduct prod, int unitQty, string demandId = "")
        {
            SmartAPSWipInfo wip = SAPSObjectMapper.Create<SmartAPSWipInfo>();

            wip.LotID = CreateLotID(prod.ProductID);

            //if (wip.LotID == "10W210825D001-006_2441_27")
            //    Console.Write("A");

            wip.WipProductID = prod.ProductID;
            wip.WipProcessID = prod.Process.ProcessID;
            wip.WipStepID = null;
            wip.WipEqpID = null;
            wip.WipState = Constants.WAIT;
            wip.WipStateTime = SimHelper.MinDateTime;
            wip.LineID = prod.LineID == null
                ? String.IsNullOrWhiteSpace(demandId)
                    ? prod.LineID
                    : InputMart.Instance.DEMAND.Rows.Where(x => x.DEMAND_ID == demandId).FirstOrDefault().LINE_ID
                : prod.LineID;
            wip.LineInTime = SimHelper.MinDateTime;
            wip.StepArrivalTime = SimHelper.MinDateTime;
            wip.UnitQty = unitQty;
            wip.OutQty = 0;
            wip.DemandID = demandId;

            wip.CurrentState = EntityState.WAIT;
            wip.Product = prod;
            wip.Process = prod.Process;

            SmartAPSStep step = prod.Process.FirstStep as SmartAPSStep;

            if (step == null)
            {
                string item = $"Wip {nameof(wip.LotID)} = {wip.LotID}, {nameof(prod.Process.ProcessID)} = {prod.Process.ProcessID}";
                ErrorHelper.WriteError(Constants.SI0002, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_INPUT", "Process Step data is missing",
                    item, null, wip.WipProductID, null, null, wip.LotID);

                return null;
            }

            wip.InitialStep = prod.Process.FirstStep;

            return wip;
        }

        public static string CreateLotID(string prodID)
        {
            string lotId = string.Empty;

            if (InputMart.Instance.LotIdCnt.TryGetValue(prodID, out int cnt) == false)
                InputMart.Instance.LotIdCnt[prodID] = 0;

            do
            {
                InputMart.Instance.LotIdCnt[prodID] = ++cnt;
                lotId = $"LOT_{prodID}_{cnt}";
            } while (InputMart.Instance.Lots.ContainsKey(lotId));

            return lotId;
        }

        #endregion
        //SmartAPS  
        #region Pegging Input

        public static SmartAPSMoMaster CreateMoMaster(DEMAND entity)
        {
            SmartAPSMoMaster mm = SAPSObjectMapper.Create<SmartAPSMoMaster>();
            return mm;

        }

        public static SmartAPSMoPlan CreateMoPlan(SmartAPSMoMaster mm, float qty, DateTime dueDate)
        {
            SmartAPSMoPlan mo = SAPSObjectMapper.Create<SmartAPSMoPlan>(mm, qty, dueDate);
            return mo;
        }

        public static SmartAPSPegPart CreatePegPart(SmartAPSMoMaster mm)
        {
            SmartAPSPegPart pp = SAPSObjectMapper.Create<SmartAPSPegPart>(mm, mm.Product);
            return pp;
        }

        public static SmartAPSPegTarget CreatePegTarget(SmartAPSPegPart pp, SmartAPSMoPlan mp)
        {
            SmartAPSPegTarget pt = SAPSObjectMapper.Create<SmartAPSPegTarget>(pp, mp);
            return pt;
        }

        public static SmartAPSPlanWip CreatePlanWip(SmartAPSWipInfo info)
        {
            SmartAPSPlanWip wip = SAPSObjectMapper.Create<SmartAPSPlanWip>(info);
            return wip;
        }

        public static SmartAPSStepTarget CreateStepTarget(object stepPlanKey, Step step, double qty, DateTime dueDate, bool isRun)
        {
            SmartAPSStepTarget st = SAPSObjectMapper.Create<SmartAPSStepTarget>(stepPlanKey, step, qty, dueDate, isRun);
            return st;
        }

        #endregion

        #region Simulation Input

        public static SmartAPSInTarget CreateInTarget()
        {
            SmartAPSInTarget inTarget = SAPSObjectMapper.Create<SmartAPSInTarget>();

            return inTarget;
        }

        public static SmartAPSWeightPreset CreateWeightPreset(PRESET_INFO pi)
        {
            SmartAPSWeightPreset wp = SAPSObjectMapper.Create<SmartAPSWeightPreset>(pi.PRESET_ID);

            return wp;
        }

        public static SmartAPSLot CreateLot(SmartAPSWipInfo wip, bool isWip)
        {
            bool useUnpegOption = SimInit.Instance.OnGetUseUnpegWipOption();
            double wipQty = wip.UnitQty;
            PEG_HISTORY pegHis = null;

            if(wip.SplitCheck)
                pegHis = OutputMart.Instance.PEG_HISTORY.Table.Rows.Where(x => x.LOT_ID == wip.ParentsLot.LotID && x.MO_DEMAND_ID == wip.DemandID).FirstOrDefault();
            else
                pegHis = OutputMart.Instance.PEG_HISTORY.Table.Rows.Where(x => x.LOT_ID == wip.LotID && x.MO_DEMAND_ID == wip.DemandID).FirstOrDefault();

            if (isWip)
            {
                if (!useUnpegOption)
                {
                    if (pegHis == null)
                        return null;
                    else
                    {
                        if (!wip.SplitCheck)
                            wipQty = pegHis.PEG_QTY;
                    }

                }
            }
            SmartAPSLot lot = SAPSObjectMapper.Create<SmartAPSLot>(wip);


            //if (lot.LotID == "10W210825D001-006_2441_27")
            //    Console.Write("A");

            lot.WipInfo = wip;
            lot.Product = wip.Product;
            lot.Route = wip.Process;

            lot.OriginLineID = wip.LineID != null ? wip.LineID : "-";
            lot.LineID = wip.LineID != null ? wip.LineID : "-";
            lot.UnitQty = (int)wipQty;
            lot.UnitQtyDouble = wipQty;

            if (isWip)
                lot.LotState = LotState.WIP;
            else
                lot.LotState = LotState.CREATE;

            lot.CurrentState = wip.CurrentState;
            lot.StateTime = wip.WipStateTime;
            lot.DispatchInTime = wip.WipStateTime <= SimHelper.MinDateTime ? AoFactory.Current.NowDT : wip.WipStateTime;
            lot.PrepEffEndTime = wip.PrepEffEndTime;
      

            InputMart.Instance.Lots[lot.LotID] = lot;

            return lot;
        }

        public static SmartAPSEqp CreateEqp(EQUIPMENT entity)
        {
            SmartAPSEqp eqp = SAPSObjectMapper.Create<SmartAPSEqp>();

            eqp.LineID = entity.LINE_ID;
            eqp.EqpID = entity.EQP_ID;
            eqp.ResID = entity.EQP_ID;
            eqp.Key = entity.EQP_ID;
            eqp.ResGroup = entity.EQP_GROUP;

            if (Enum.TryParse<SimEqpType>(entity.SIM_TYPE, true, out SimEqpType simType))
                eqp.SimType = simType;
            else
                eqp.SimType = SimEqpType.Table;

            if (string.IsNullOrWhiteSpace(entity.DISPATCHER_TYPE) == false)
            {
                string dt = entity.DISPATCHER_TYPE.ToUpper();

                switch (dt)
                {
                    case Constants.WEIGHTSORTED:
                        eqp.DispatcherType = DispatcherType.WeightSorted;
                        break;
                    case Constants.WEIGHTSUM:
                        eqp.DispatcherType = DispatcherType.WeightSum;
                        break;
                    case Constants.FIFO:
                    case Constants.EDD:
                    case Constants.LPT:
                    case Constants.SPT:
                        eqp.DispatcherType = DispatcherType.Fifo;
                        eqp.FifoType = dt;
                        break;
                    default:
                        eqp.DispatcherType = DispatcherType.Fifo;
                        break;
                }
            }
            else
            {
                eqp.DispatcherType = DispatcherType.Fifo;
            }


            if (eqp.DispatcherType != DispatcherType.Fifo)
            {
                SmartAPSWeightPreset defPreset = InputMart.Instance.SmartAPSWeightPreset.Rows.FirstOrDefault();
                SmartAPSWeightPreset preset = null;
                if (!string.IsNullOrEmpty(entity.PRESET_ID))
                {
                    preset = InputMart.Instance.SmartAPSWeightPresetView.FindRows(entity.PRESET_ID).FirstOrDefault();
                    if (preset == null)
                        preset = defPreset;
                }
                eqp.Preset = preset;
            }

            if (Enum.TryParse<LoadingStates>(entity.EQP_STATE, out LoadingStates ls))
            {
                switch (ls)
                {
                    case LoadingStates.DOWN:
                    case LoadingStates.PM:
                        eqp.State = ResourceState.Down;
                        eqp.StateChangeTime = entity.STATE_CHANGE_TIME;
                        eqp.StateCode = entity.EQP_STATE_CODE;
                        break;
                    default:
                        eqp.State = ResourceState.Up;
                        break;
                }
            }
            else
            {
                eqp.State = ResourceState.Up;
            }

            if (string.IsNullOrWhiteSpace(entity.AUTOMATION) == false)
                eqp.Automation = entity.AUTOMATION.ToUpper();

            eqp.SetupInfo = InputMart.Instance.SETUP_INFOView.FindRows(entity.EQP_ID).ToList();

            return eqp;
        }

        public static SmartAPSPlanInfo CreatePlanInfo(SmartAPSLot lot, SmartAPSStep step)
        {   
            
            SmartAPSPlanInfo plan = SAPSObjectMapper.Create<SmartAPSPlanInfo>(step);
            plan.Lot = lot;
            plan.ProductID = lot.Product.ProductID;
            plan.ProcessID = lot.Product.Process.ProcessID;
            if(lot.Process.LastStep.StepID == lot.CurrentStepID)
                plan.ProcessID = lot.CurrentProductID;
            plan.LotID = lot.LotID;

            var mbs = MaterialManager.Instance.FindMatBoms(lot.Product, step);

            foreach (var mb in mbs)
                plan.MatBom.Add(mb);
        
            return plan;
        }

        public static PMSchedule CreatePMSchedule(DateTime startTime, double period)
        {
            PMSchedule ps = SAPSObjectMapper.Create<PMSchedule>(startTime, Convert.ToInt32(period));

            return ps;
        }

        #endregion

        #region Output

        public static Outputs.STEP_TARGET CreateStepTarget()
        {
            Outputs.STEP_TARGET st = SAPSObjectMapper.Create<Outputs.STEP_TARGET>();

            return st;
        }

        public static Outputs.PEG_HISTORY CreatePegHistory()
        {
            Outputs.PEG_HISTORY ph = SAPSObjectMapper.Create<Outputs.PEG_HISTORY>();

            return ph;
        }

        public static Outputs.UNPEG_HISTORY CreateUnpegHistory()
        {
            Outputs.UNPEG_HISTORY uh = SAPSObjectMapper.Create<Outputs.UNPEG_HISTORY>();

            return uh;
        }

        public static Outputs.EQP_DISPATCH_LOG CreateEqpDispatchLog()
        {
            Outputs.EQP_DISPATCH_LOG dl = SAPSObjectMapper.Create<Outputs.EQP_DISPATCH_LOG>();

            return dl;
        }

        public static Outputs.INPUT_PLAN CreateInputPlan()
        {
            Outputs.INPUT_PLAN ip = SAPSObjectMapper.Create<Outputs.INPUT_PLAN>();

            return ip;
        }

        public static Outputs.RELEASE_HISTORY CreateReleaseHistory()
        {
            Outputs.RELEASE_HISTORY rh = SAPSObjectMapper.Create<Outputs.RELEASE_HISTORY>();

            return rh;
        }

        public static Outputs.WIP_HIS CreateWipHis()
        {
            Outputs.WIP_HIS el = SAPSObjectMapper.Create<Outputs.WIP_HIS>();

            return el;
        }

        public static Outputs.PRESET_INFO_HIS CreatePresetInfoHis()
        {
            PRESET_INFO_HIS pi = SAPSObjectMapper.Create<Outputs.PRESET_INFO_HIS>();

            return pi;
        }

        public static Outputs.DEMAND_HIS CreateDemandHis()
        {
            Outputs.DEMAND_HIS el = SAPSObjectMapper.Create<Outputs.DEMAND_HIS>();

            return el;
        }

        public static INIT_WIP_LOG CreateInitWipLog()
        {
            Outputs.INIT_WIP_LOG iwl = SAPSObjectMapper.Create<Outputs.INIT_WIP_LOG>();

            return iwl;
        }

        public static Outputs.ERROR_LOG CreateErrorLog()
        {
            Outputs.ERROR_LOG el = SAPSObjectMapper.Create<Outputs.ERROR_LOG>();

            return el;
        }

        public static Outputs.RUN_HISTORY CreateRunHistory()
        {
            Outputs.RUN_HISTORY el = SAPSObjectMapper.Create<Outputs.RUN_HISTORY>();

            return el;
        }

        public static Outputs.UNKIT_REMAIN_WIPLOG CreateUnkitRemainWiplog()
        {
            Outputs.UNKIT_REMAIN_WIPLOG urw = SAPSObjectMapper.Create<Outputs.UNKIT_REMAIN_WIPLOG>();

            return urw;
        }

        public static Outputs.STEP_WIP CreateStepWip()
        {
            Outputs.STEP_WIP sw = SAPSObjectMapper.Create<Outputs.STEP_WIP>();

            return sw;
        }

        public static STEP_MOVE CreateStepMove()
        {
            STEP_MOVE sm = SAPSObjectMapper.Create<STEP_MOVE>();
            return sm;
        }

        public static Outputs.EQP_PLAN CreateEqpPlan()
        {
            Outputs.EQP_PLAN ep = SAPSObjectMapper.Create<Outputs.EQP_PLAN>();

            return ep;
        }
        public static Outputs.ANALYSIS_PSI CreatePsi()
        {
            Outputs.ANALYSIS_PSI ep = SAPSObjectMapper.Create<Outputs.ANALYSIS_PSI>();

            return ep;
        }

        public static MATERIAL_HISTORY CreateMaterialHistory()
        {
            Outputs.MATERIAL_HISTORY mh = SAPSObjectMapper.Create<Outputs.MATERIAL_HISTORY>();

            return mh;
        }

        public static PRODUCT_ROUTE_LOG CreateMergeWipLog()
        {
            Outputs.PRODUCT_ROUTE_LOG mw = SAPSObjectMapper.Create<Outputs.PRODUCT_ROUTE_LOG>();

            return mw;
        }

        public static UNLOADED_LOTLOG CreateUnloadedLotLog()
        {
            Outputs.UNLOADED_LOTLOG ul = SAPSObjectMapper.Create<Outputs.UNLOADED_LOTLOG>();

            return ul;
        }

        public static TOOL_SEIZE_LOG CreateToolSeizeLog()
        {
            Outputs.TOOL_SEIZE_LOG tsl = SAPSObjectMapper.Create<Outputs.TOOL_SEIZE_LOG>();

            return tsl;
        }

        public static APS_STATUS_MASTER CreateApsStatusMaster()
        {
            Outputs.APS_STATUS_MASTER asm = SAPSObjectMapper.Create<Outputs.APS_STATUS_MASTER>();

            return asm;
        }
        public static APS_STATUS_MASTER CreateApsStatusMaster(string APS_CODE, string APS_VALUE)
        {
            Outputs.APS_STATUS_MASTER asm = SAPSObjectMapper.Create<Outputs.APS_STATUS_MASTER>();
            asm.APS_CODE = APS_CODE;
            asm.APS_VALUE = APS_VALUE;

            return asm;
        }
        //public static List<APS_STATUS_MASTER> CreateListApsStatusMaster()
        //{
        //    Outputs.APS_STATUS_MASTER asm = SAPSObjectMapper.Create<Outputs.APS_STATUS_MASTER>();
        //
        //    //List<Outputs.APS_STATUS_MASTER> asm1;
        //    //asm1.Add(asm);
        //
        //    return asm;
        //}


        public static APS_LOG_HISTORY CreateApsLogHistory()
        {
            Outputs.APS_LOG_HISTORY alh = SAPSObjectMapper.Create<Outputs.APS_LOG_HISTORY>();

            return alh;
        }
        #endregion

        public static void CreatePMList(SmartAPSEqp eqp, PMSchedule newPm)
        {
            var shiftTimeConfigs = InputMart.Instance.SHIFT_TIME_CONFIG.Rows;

            bool juyaCheck = false;
            int cnt = 0;
            foreach (SHIFT_TIME_CONFIG stc in shiftTimeConfigs)
            {
                DateTime SPLIT_START_TIME = DateTime.Parse(stc.SPLIT_START_TIME);
                DateTime SPLIT_END_TIME = DateTime.Parse(stc.SPLIT_END_TIME);

                SmartAPSPMList pmList = SAPSObjectMapper.Create<SmartAPSPMList>();
                pmList.LineID = eqp.LineID;

                var startdays = newPm.StartTime.Day - SPLIT_END_TIME.Day;

                if (newPm.StartTime < SPLIT_END_TIME.AddDays(startdays) && newPm.EndTime > SPLIT_END_TIME.AddDays(startdays))
                {
                    pmList.PM_Start_Time = newPm.StartTime;
                    pmList.PM_End_Time = SPLIT_END_TIME.AddDays(startdays);
                    pmList.PM_Run_Time = pmList.PM_End_Time - pmList.PM_Start_Time;

                    juyaCheck = true;

                    var ContinuePM = InputMart.Instance.SmartAPSPMListLineView.FindRows(pmList.LineID).Where(x => x.PM_End_Time == pmList.PM_Start_Time).FirstOrDefault();
                    if (ContinuePM != null)
                    {
                        ContinuePM.PM_End_Time = pmList.PM_End_Time;
                        ContinuePM.PM_Run_Time = ContinuePM.PM_End_Time - ContinuePM.PM_Start_Time;
                    }
                    else
                        InputMart.Instance.SmartAPSPMList.ImportRow(pmList);

                    cnt++;
                }

                if (cnt != 0)
                {
                    pmList = SAPSObjectMapper.Create<SmartAPSPMList>();

                    pmList.LineID = eqp.LineID;
                    pmList.PM_Start_Time = SPLIT_END_TIME.AddDays(startdays);
                    pmList.PM_End_Time = newPm.EndTime;
                    pmList.PM_Run_Time = pmList.PM_End_Time - pmList.PM_Start_Time;

                    var ContinuePM = InputMart.Instance.SmartAPSPMListLineView.FindRows(pmList.LineID).Where(x => x.PM_End_Time == pmList.PM_Start_Time).FirstOrDefault();
                    if (ContinuePM != null)
                    {
                        ContinuePM.PM_End_Time = pmList.PM_End_Time;
                        ContinuePM.PM_Run_Time = ContinuePM.PM_End_Time - ContinuePM.PM_Start_Time;
                    }
                    else
                        InputMart.Instance.SmartAPSPMList.ImportRow(pmList);
                    cnt = 0;
                }
            }

            if (!juyaCheck)
            {
                SmartAPSPMList pmList = SAPSObjectMapper.Create<SmartAPSPMList>();
                pmList.LineID = eqp.LineID;
                pmList.PM_Start_Time = newPm.StartTime;
                pmList.PM_End_Time = newPm.EndTime;
                pmList.PM_Run_Time = pmList.PM_End_Time - pmList.PM_Start_Time;

                var ContinuePM = InputMart.Instance.SmartAPSPMListLineView.FindRows(pmList.LineID).Where(x => x.PM_End_Time == pmList.PM_Start_Time).FirstOrDefault();
                if (ContinuePM != null)
                {
                    ContinuePM.PM_End_Time = pmList.PM_End_Time;
                    ContinuePM.PM_Run_Time = ContinuePM.PM_End_Time - ContinuePM.PM_Start_Time;
                }
                else
                    InputMart.Instance.SmartAPSPMList.ImportRow(pmList);
            }
        }

        public static SmartAPSProductRoute CreateSmartAPSProductRoute()
        {
            SmartAPSProductRoute pr = SAPSObjectMapper.Create<SmartAPSProductRoute>();

            return pr;
        }

        public static int CreateSeq(string str)
        {
            string temp = string.Empty;

            if (InputMart.Instance.strCnt.TryGetValue(str, out int cnt) == false)
                InputMart.Instance.strCnt[str] = 0;

            InputMart.Instance.strCnt[str] = ++cnt;

            return cnt;
        }
    }
}
