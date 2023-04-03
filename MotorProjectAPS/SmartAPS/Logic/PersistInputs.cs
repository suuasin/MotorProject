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
using SmartAPS.Persists;
using Mozart.SeePlan.General;
using Mozart.SeePlan;
using Mozart.SeePlan.General.DataModel;
using SmartAPS.Outputs;
using Mozart.Task.Execution.Persists;
using Mozart.SeePlan.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.Data.Entity;

namespace SmartAPS.Logic
{
    [FeatureBind()]
    public partial class PersistInputs
    {
        public bool OnAfterLoad_PROCESS(PROCESS entity)
        {
            var stepRoutes = InputMart.Instance.STEP_ROUTEView.FindRows(entity.PROCESS_ID);
            if (stepRoutes.Count() == 0)
            {
                string item = $"{nameof(entity.PROCESS_ID)} = '{entity.PROCESS_ID}'";
                ErrorHelper.WriteError(Constants.IN0023, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_PROCESS", "PROCESS must be defined in STEP_ROUTE", item);
                return true;
            }

            // BOP Builder 를 Single Process 타입으로 생성
            BopBuilder bb = new BopBuilder(BopType.SINGLE_FLOW);

            // Delegate 함수 지정
            // Step 이전 Step의 Path 우선순위 결정 함수 지정
            bb.ComparePrevSteps = BopHelper.ComparePrevSteps;
            // 연결된 Step 의 소팅 순서 결정함수 지정
            bb.CompareSteps = BopHelper.CompareSteps;

            // SampleProcess 생성, 필요시 My Method 를 사용하여 Process 속성을 설정하도록 구현합니다.
            SmartAPSProcess proc = CreateHelper.CreateProcess(entity);

            // 해당 Process 를 구성할 Step 정보 Dictionary 구성
            Dictionary<string, GeneralStep> stepDict = BopHelper.LoadSteps(proc);

            // 해당 Process 를 구성할 Step간 링크정보(PrpInfo) Dictionary 구성
            Dictionary<string, PrpInfo> prpDict = BopHelper.LoadPrpInfo(proc);

            // BOP 구성 함수 호출
            bb.BuildBop(proc, stepDict, prpDict);

            // Site Project OnAfterLoad 호출
            DerivedHelper.CallAfterLoadHandler(entity, proc);

            // 생성된 Process 등록
            InputMart.Instance.SmartAPSProcess.ImportRow(proc);

            return true;
        }

        public bool OnAfterLoad_PRODUCT(PRODUCT entity)
        {
            SmartAPSProduct prod = CreateHelper.CreateProduct(entity);
            if (prod != null)
            {
                InputMart.Instance.SmartAPSProduct.ImportRow(prod);
                return true; 
            }

            return false;
        }

        public bool OnAfterLoad_PRODUCT_ROUTE(PRODUCT_ROUTE entity)
        {
            entity.CHANGE_TYPE = Constants.WAIT;
            //if (!Enum.TryParse<ProductChangeType>(entity.CHANGE_TYPE, out ProductChangeType changeType))
            //    entity.CHANGE_TYPE = Constants.WAIT;

            return true;
        }

        public bool OnAfterLoad_STD_STEP_INFO(STD_STEP_INFO entity)
        {
            SmartAPSStdStep std = CreateHelper.CreateStdStep(entity);

            // Site Project OnAfterLoad 호출
            DerivedHelper.CallAfterLoadHandler(entity, std);

            // 생성된 StdStep 등록
            InputMart.Instance.SmartAPSStdStep.ImportRow(std);

            //return false;
            return true; 
        }

        public bool OnAfterLoad_EQUIPMENT(EQUIPMENT entity)
        {
            // 장비 생성 Method 를 만들어 장비를 생성합니다.
            SmartAPSEqp eqp = CreateHelper.CreateEqp(entity);

            // 장비 Collection 에 데이터를 추가합니다. 이후 장비 시뮬레이션 모델 초기화시 사용합니다.
            InputMart.Instance.SmartAPSEqp.Rows.Add(eqp);

            return true;
        }

        public bool OnAfterLoad_MATERIAL(MATERIAL entity)
        {
            MatPlan mat = CreateHelper.CreateMatPlan(entity);

            if (mat == null)
                return false;

            MaterialManager.Instance.Materials.Add(mat);

            return true;
        }

        public bool OnAfterLoad_MATERIAL_BOM(MATERIAL_BOM entity)
        {
            MatBom bom = CreateHelper.CreateMatBom(entity);

            if (bom == null)
                return false;

            MaterialManager.Instance.MatBoms.Add(bom);

            return true;
        }

        public bool OnAfterLoad_PRESET_INFO(PRESET_INFO entity)
        {
            WriteHelper.WritePresetHis(entity);

            return true;
        }

        public void OnAction_PRESET_INFO(IPersistContext context)
        {
            InputMart.Instance.PRESET_INFO.DefaultView.Sort = $"{nameof(Inputs.PRESET_INFO.PRESET_ID)} ASC, {nameof(Inputs.PRESET_INFO.FACTOR_ID)} ASC";

            foreach (Inputs.PRESET_INFO pi in InputMart.Instance.PRESET_INFO.DefaultView)
            {
                if (Enum.IsDefined(typeof(UseCriteriaWeightFactor), pi.FACTOR_ID))
                    pi.CRITERIA = SAPSUtils.ValidationCriteria(pi.CRITERIA);

                SmartAPSWeightPreset preset = InputMart.Instance.SmartAPSWeightPresetView.FindRows(pi.PRESET_ID).FirstOrDefault();
                if (preset == null)
                {
                    preset = CreateHelper.CreateWeightPreset(pi);

                    // Site Project OnAfterLoad 호출
                    DerivedHelper.CallAfterLoadHandler(pi, preset);

                    // 생성된 WeightPreset 등록
                    InputMart.Instance.SmartAPSWeightPreset.Rows.Add(preset);
                }

                WeightFactor factor = new WeightFactor(pi.FACTOR_ID, pi.FACTOR_WEIGHT, pi.SEQUENCE,
                                             (FactorType)Enum.Parse(typeof(FactorType), pi.FACTOR_TYPE), pi.ORDER_TYPE == OrderType.DESC.ToString() ? OrderType.DESC : OrderType.ASC);

                preset.FactorList.Add(factor);
            }
        }

        public bool OnAfterLoad_REPLENISH_PLAN(REPLENISH_PLAN entity)
        {
            MatPlan plan = CreateHelper.CreateMatPlan(entity);

            if (plan == null)
                return false;

            MaterialManager.Instance.Materials.Add(plan);

            return true;
        }

        public bool OnAfterLoad_SPLIT_INFO(SPLIT_INFO entity)
        {
            if (String.IsNullOrWhiteSpace(entity.PRODUCT_ID) || String.IsNullOrWhiteSpace(entity.STEP_ID) || String.IsNullOrWhiteSpace(entity.CRITERIA) || entity.VALUE == 0)
            {
                string item = $"{nameof(entity.PRODUCT_ID)} = '{entity.PRODUCT_ID}', {nameof(entity.STEP_ID)} = '{entity.STEP_ID}'";

                ErrorHelper.WriteError(Constants.IN0025, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_SPLIT_INFO", "All values of SPLIT_INFO cannot be NULL", item, product: entity.PRODUCT_ID, step: entity.STEP_ID);
                return false;
            }

            if (!Enum.IsDefined(typeof(SplitType), entity.CRITERIA.ToUpper()))
            {
                string item = $"{nameof(entity.PRODUCT_ID)} = '{entity.PRODUCT_ID}', {nameof(entity.STEP_ID)} = '{entity.STEP_ID}'";

                ErrorHelper.WriteError(Constants.IN0026, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_SPLIT_INFO", "CRITERIA data is missing", item, product: entity.PRODUCT_ID, step: entity.STEP_ID);
                return false;
            }
            entity.CRITERIA = entity.CRITERIA.ToUpper();

            SmartAPSProduct prod = BopHelper.GetProduct(entity.PRODUCT_ID);
            if (prod == null)
            {
                string item = $"{nameof(entity.PRODUCT_ID)} = '{entity.PRODUCT_ID}', {nameof(entity.STEP_ID)} = '{entity.STEP_ID}'";

                ErrorHelper.WriteError(Constants.IN0027, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_SPLIT_INFO", "PRODUCT data is missing", item, product: entity.PRODUCT_ID, step: entity.STEP_ID);
                return false;
            }
            SmartAPSStep step = prod.Process.FindStep(entity.STEP_ID) as SmartAPSStep;
            if (step == null)
            {
                string item = $"{nameof(entity.PRODUCT_ID)} = '{entity.PRODUCT_ID}', {nameof(entity.STEP_ID)} = '{entity.STEP_ID}'";

                ErrorHelper.WriteError(Constants.IN0028, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_SPLIT_INFO", "STEP data does not exist in STEP_ROUTE", item, product: entity.PRODUCT_ID, step: entity.STEP_ID);
                return false;
            }

            return true;
        }

        public bool OnAfterLoad_DEMAND(DEMAND entity)
        {
            WriteHelper.WriteDemandHis(entity);

            return true;
        }

        public void OnAction_DEMAND(IPersistContext context)
        {
            bool useHardPeg = OptionHelper.UseHardPegging();

            // PLAN SELECT
            InputMart.Instance.DEMANDView.Sort = useHardPeg ? "DEMAND_ID, DUE_DATE ASC" : "DEMAND_ID, DUE_DATE ASC";//"PRODUCT_ID, DUE_DATE ASC";

            SmartAPSMoMaster lastMM = null;

            foreach (DEMAND demand in InputMart.Instance.DEMAND.DefaultView)
            {
                SmartAPSProduct prod = BopHelper.GetProduct(demand.PRODUCT_ID);

                double qty = demand.DEMAND_QTY;

                //완제품 창고의 재고가 있을 경우 Demand에서 차감해준다.
                var stocks = InputMart.Instance.SmartAPSOutActView.FindRows(prod);
                if (stocks != null && stocks.Count() > 0)
                {
                    foreach (var stock in stocks)
                    {
                        //psi write (outStock은 inDate의 당일생산량으로 친다)
                        WriteHelper.WritePsiObj(demand.DEMAND_ID, prod.ProductID, "P", stock.InQty, stock.InDate);

                        if (qty > stock.InQty)
                        {
                            qty -= stock.InQty;
                            stock.InQty = 0;
                        }
                        else
                        {
                            stock.InQty -= qty;
                            qty = 0;
                            break;
                        }
                    }

                    // 이미 모두 만들어진 Demand는 더이상 생산하지 않는다.
                    if (qty == 0)
                        continue;
                }

                var dueDate = demand.DUE_DATE;

                if (prod == null)
                {
                    string item = $"{nameof(demand.DEMAND_ID)} = '{demand.DEMAND_ID}'";
                    ErrorHelper.WriteError(Constants.IN0001, ErrorSeverity.WARNING, context.ModelContext.StartTime, "INVALID_DEMAND", "PRODUCT data is missing", item, demandid: demand.DEMAND_ID, product: demand.PRODUCT_ID);
                    continue;
                }

                // CREATE
                if (useHardPeg)
                {
                    if (lastMM == null || lastMM.DemandID != demand.DEMAND_ID)
                    {
                        SmartAPSMoMaster mm = CreateHelper.CreateMoMaster(demand);
                        mm.Product = prod;
                        mm.DemandID = demand.DEMAND_ID;
                        lastMM = mm;

                        InputMart.Instance.SmartAPSMoMaster.ImportRow(lastMM);
                    }
                }
                else
                {
                    if (lastMM == null || lastMM.DemandID != demand.DEMAND_ID)//lastMM.Product != prod)
                    {
                        SmartAPSMoMaster mm = CreateHelper.CreateMoMaster(demand);
                        mm.Product = prod;
                        mm.DemandID = demand.DEMAND_ID;
                        lastMM = mm;

                        InputMart.Instance.SmartAPSMoMaster.ImportRow(lastMM);
                    }
                }

                SmartAPSMoPlan mp = CreateHelper.CreateMoPlan(lastMM, (float)qty, dueDate);
                mp.CustomerID = demand.CUSTOMER_ID; //추가항목
                mp.DemandID = demand.DEMAND_ID;
                mp.Priority = demand.PRIORITY;
                mp.WeekNo = demand.DUE_DATE.GetIso8601WeekOfYear().ToString();
                DerivedHelper.CallAfterLoadHandler(demand, mp);
                lastMM.AddMoPlan(mp);

                //psi demand write
                WriteHelper.WritePsiObj(demand.DEMAND_ID, demand.PRODUCT_ID, "S", demand.DEMAND_QTY, demand.DUE_DATE);
            }
        }

        public bool OnAfterLoad_FIRM_PLAN(FIRM_PLAN entity)
        {
            SmartAPSFirmPlan firm = CreateHelper.CreateFirmPlan(entity);
            if (firm == null)
                return false;


            if (firm.EndTime < firm.StartTime)
            {
                string item = $"{nameof(firm.LotID)} = '{firm.LotID}'";
                ErrorHelper.WriteError(Constants.IN0019, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_FIRMPLAN", "FIRM_PLAN END_TIME cannot be eariler than START_TIME", item, lotid: firm.LotID);
                return true;
            }

            var wips = InputMart.Instance.SmartAPSWipInfoView.FindRows(firm.LotID);
            SmartAPSWipInfo wip;

            if (wips != null && wips.Count() > 0)
            {
                wip = wips.FirstOrDefault();
                if (wip.FirmPlans.ContainsKey(firm.Key))
                {
                    string item = $"{nameof(firm.LotID)} = '{firm.LotID}', {nameof(firm.StepID)} = '{firm.StepID}'";
                    ErrorHelper.WriteError(Constants.IN0020, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_FIRMPLAN", "FIRM_PLAN cannot overlap LOT_ID and STEP_ID", item, step: firm.StepID, lotid: firm.LotID);
                    return true;
                }
                else
                {
                    if (wip.CurrentState.ToString() == Constants.RUN)
                    {
                        if (wip.WipStepID == firm.StepID)
                        {
                            string item = $"{nameof(firm.LotID)} = '{firm.LotID}', {nameof(firm.StepID)} = '{firm.StepID}'";
                            ErrorHelper.WriteError(Constants.IN0021, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_FIRMPLAN", "STEP of WIP and FIRM_PLAN cannot be overlapped", item, step: firm.StepID, lotid: firm.LotID);
                            return true;
                        }
                    }
                    //설비에 대한 validation 진행 후 wip의 확정계획에 추가
                    //wip.FirmPlans.Add(firm.Key, firm); 
                }
            }
            else
                return true;

            if (!string.IsNullOrWhiteSpace(firm.EqpID))
            {
                var eqps = InputMart.Instance.SmartAPSEqpView.FindRows(firm.EqpID);

                if (eqps != null && eqps.Count() > 0)
                {
                    var eqp = eqps.FirstOrDefault();

                    eqp.FirmPlans.Add(firm.Key, firm);
                    wip.FirmPlans.Add(firm.Key, firm);
                }
                else if (eqps.Count() == 0)
                {
                    string item = $"{nameof(firm.LotID)} = '{firm.LotID}', {nameof(firm.EqpID)} = '{firm.EqpID}'";
                    ErrorHelper.WriteError(Constants.IN0022, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_FIRMPLAN", "Loaded EQUIPMENT data is mismatched",
                        item, step: firm.StepID, equipment: firm.EqpID, lotid: firm.LotID);

                    return true;
                }
            }

            return true;
        }

        public bool OnAfterLoad_OUT_ACT(OUT_ACT entity)
        {
            SmartAPSOutAct stock = CreateHelper.CreateOutAct(entity);

            if (stock == null)
                return false;

            InputMart.Instance.SmartAPSOutAct.Rows.Add(stock);

            return true;
        }

        public bool OnAfterLoad_WIP(WIP entity)
        {
            if (entity.STATE == Constants.PREP)
            {
                var eqp = InputMart.Instance.SmartAPSEqpView.FindRows(entity.EQP_ID);
                var eqpArr = InputMart.Instance.EQP_ARRANGEStepView.FindRows(entity.PRODUCT_ID, entity.PROCESS_ID, entity.STEP_ID).Where(x => x.EQP_ID == entity.EQP_ID).FirstOrDefault();
                if (!string.IsNullOrEmpty(entity.EQP_ID) && eqp != null && eqpArr != null)
                {
                    entity.STATE = Constants.WAIT;
                    entity.PREP_EFF_END_TIME = entity.PREP_EFF_END_TIME == DateTime.MinValue
                                              ? DateTime.MaxValue
                                              : entity.PREP_EFF_END_TIME;
                }
                else
                {
                    string item = $"{nameof(entity.EQP_ID)} = '{entity.EQP_ID}'";
                    ErrorHelper.WriteError(Constants.IN0029, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_WIP", "PREP WITH INVALID EQP",
                        item, product: entity.PRODUCT_ID, step: entity.STEP_ID, equipment: entity.EQP_ID, lotid: entity.LOT_ID);
                    entity.STATE = Constants.WAIT;
                    entity.PREP_EFF_END_TIME = DateTime.MinValue;
                }
            }

            var info = InputMart.Instance.PRODUCT_ROUTE.Rows.Where(p => (p.FROM_PRODUCT_ID == entity.PRODUCT_ID && p.STEP_ID == entity.STEP_ID));

            if (entity.STATE == Constants.WAIT && info.Count() > 0)
            {
                var stepRoute = InputMart.Instance.STEP_ROUTEView.FindRows(entity.PROCESS_ID);
                if (stepRoute.Count() > 0)
                {
                    string stepId = stepRoute.OrderByDescending(x => x.STEP_SEQ).FirstOrDefault().STEP_ID;

                    string ordStepId = entity.STEP_ID;

                    entity.STEP_ID = stepId;
                    entity.OUT_QTY = entity.UNIT_QTY;
                    entity.STATE = Constants.RUN;
                    entity.EQP_ID = null;

                    WriteHelper.WriteInitWipLog(entity, Constants.WAIT, ordStepId);
                }

            }

            SmartAPSWipInfo wip = CreateHelper.CreateWipInfo(entity);

            WriteHelper.WriteWipHis(entity);

            if (wip == null)
                return false;

            // Site Project OnAfterLoad 호출
            if (DerivedHelper.CallAfterLoadHandler(entity, wip))
            {
                // 생성된 WipInfo 등록
                InputMart.Instance.SmartAPSWipInfo.Rows.Add(wip);
                return true;
            }
            else
                return false;
        }

        public bool OnAfterLoad_EQP_ARRANGE(EQP_ARRANGE entity)
        {
            var eqp = InputMart.Instance.SmartAPSEqpView.FindRows(entity.EQP_ID).FirstOrDefault();
            string simType = null;

            if (eqp != null)
                simType = eqp.SimType.ToString();

            if (simType == "Inline")
            {
                if (entity.TACT_TIME <= 0 || entity.PROC_TIME <= 0)
                {
                    string errorReason = $"TactTime and ProcTime are required for {eqp.SimType.ToString()} type equipment";
                    string item = $"{nameof(entity.TACT_TIME)} = '{entity.TACT_TIME}', {nameof(entity.PROC_TIME)} = '{entity.PROC_TIME}'"; 
                    ErrorHelper.WriteError(Constants.IN0029, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_EQP_ARRANGE", errorReason,
                    item, product: entity.PRODUCT_ID, step: entity.STEP_ID, equipment: entity.EQP_ID);
                }
            } else if (simType == "Table" || simType == "Chamber" || simType == "LotBatch" || simType == "BatchInline" || simType == "UnitBatch")
            {
                if (entity.TACT_TIME <= 0)
                {
                    string item = $"{nameof(entity.TACT_TIME)} = '{entity.TACT_TIME}'";
                    ErrorHelper.WriteError(Constants.IN0029, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_EQP_ARRANGE", "TACT_TIME or PROC_TIME is missing",
                    item, product: entity.PRODUCT_ID, step: entity.STEP_ID, equipment: entity.EQP_ID);
                }
            }

            if (entity.UTILIZATION == null || entity.UTILIZATION <= 0)
                entity.UTILIZATION = SimRun.Instance.OnGetDefaultUtilization();
            if (entity.EFFICIENCY == null || entity.EFFICIENCY <= 0)
                entity.EFFICIENCY = SimRun.Instance.OnGetDefaultEfficiency();

            return true;
        }

        public void OnAction_FIRM_PLAN(IPersistContext context)
        {   
            //region 연속된 공정의 뒷 공정이 앞공정보다 확정 계획 시간이 빠를때가 존재함 (삭제대상)
            foreach (var firmGroup in InputMart.Instance.FIRM_PLAN.Rows.GroupBy(g => g.LOT_ID)
                                                                      .Select(r => new
                                                                      {
                                                                          COUNT = r.Count(),
                                                                          firmList = r.ToList()
                                                                      }))
            {
                foreach (FIRM_PLAN entity in firmGroup.firmList)
                {
                    List<FIRM_PLAN> firmList = firmGroup.firmList.Where(w => w.LOT_ID == entity.LOT_ID
                                                                && w.STEP_ID != entity.STEP_ID
                                                                && w.START_TIME < entity.START_TIME)
                                                        .ToList();
                    if (firmList.Count() > 0)
                    {

                        STEP_ROUTE entityStep = InputMart.Instance.STEP_ROUTE.Where(w => w.PROCESS_ID == entity.LOT_ID && w.STEP_ID == entity.STEP_ID).FirstOrDefault();
                        SmartAPSWipInfo wipinfo = new SmartAPSWipInfo();
                        wipinfo = InputMart.Instance.SmartAPSWipInfo.Where(x => x.LotID == entity.LOT_ID && x.WipStepID == entity.STEP_ID && x.WipEqpID == entity.EQP_ID).FirstOrDefault();

                        foreach (FIRM_PLAN fp in firmList) //검증 후 삭제해야하는 부분 or 에러처리해야하는 부분 
                        {
                            if (wipinfo == null)
                            {
                                wipinfo = new SmartAPSWipInfo();
                                wipinfo.WipProcessID = fp.LOT_ID;
                            }

                            if (InputMart.Instance.STEP_ROUTE.Where(w => w.PROCESS_ID == wipinfo.WipProcessID && w.STEP_ID == fp.STEP_ID && w.STEP_SEQ > entityStep.STEP_SEQ).Any())
                            {
                                SmartAPSFirmPlan firm = CreateHelper.CreateFirmPlan(fp);
                                var wip = InputMart.Instance.SmartAPSWipInfoView.FindRows(firm.LotID).FirstOrDefault();

                                if (wip == null) continue;

                                wip.FirmPlans.Remove(firm.Key);
                                InputMart.Instance.FIRM_PLAN.Rows.Remove(fp);
                            }
                        }
                    }
                }
            }
        }

        public void OnAction_STEP_ROUTE(IPersistContext context)
        {
            //2021-01-20 부분 적용 불가로 개발 대기

            //foreach (STEP_ROUTE sr in InputMart.Instance.STEP_ROUTE.Rows.AsEnumerable().Where(w=> w.PROCESS_ID.Equals("S2011-0052-1|SRF80-S09-04271R")))
            //{
            //    SPLIT_INFO si = new SPLIT_INFO();
            //    si.PRODUCT_ID = sr.PROCESS_ID;
            //    si.STEP_ID = sr.STEP_ID;
            //    //옵션1 : 각 Child Lot의 수량 지정(QTY)
            //    //옵션2 : Child Lot의 수 지정(CNT)
            //    //옵션3 : 특정 수량을 가지는 하나의 Child Lot의 수량 지정(PARTIAL_QTY)
            //    si.CRITERIA = "QTY";
            //    si.VALUE = 1;
            //    InputMart.Instance.SPLIT_INFO.Rows.Add(si);
            //}

            #region 연속된 공정중 동시가공시작과 끝을 입력(LOT SPLIT 기능으로 MERGE 되는 LOT 의 STEP 을 지정)
            foreach (var srGroupList in InputMart.Instance.STEP_ROUTE.Rows
                                                            .AsEnumerable()
                                                            .GroupBy(g => g.PROCESS_ID)
                                                            .Select(r => new { PROCESS_ID = r.Key, SRList = r.OrderBy(o => o.STEP_SEQ).ToList() }))
            {
                bool iSameProc = false;
                SPLIT_INFO si = null;


                foreach (STEP_ROUTE sr in srGroupList.SRList)
                {
                    if (iSameProc != sr.IS_SAME_PROC)
                    {
                        if (sr.PROCESS_TYPE == "BUCKETING" && si != null)
                        {
                            si.MERGE_STEP_ID = sr.STEP_ID;
                            InputMart.Instance.SPLIT_INFO.Rows.Add(si);
                            si = null;
                        }
                        else
                        {
                            if (sr.IS_SAME_PROC)
                            {
                                si = new SPLIT_INFO();
                                si.PRODUCT_ID = srGroupList.PROCESS_ID;
                                si.STEP_ID = sr.STEP_ID;
                                si.CRITERIA = "QTY";
                                si.VALUE = 1;
                            }
                            else
                            {
                                if (si != null)
                                {
                                    si.MERGE_STEP_ID = sr.STEP_ID;
                                    InputMart.Instance.SPLIT_INFO.Rows.Add(si);
                                    si = null;
                                }
                            }
                        }
                        iSameProc = sr.IS_SAME_PROC;
                    }
                }
            }
            #endregion
        }

        public bool OnAfterLoad_FACTORY_BREAK(FACTORY_BREAK entity)
        {
            var period = SAPSUtils.ConvertPeriodUnit(entity.PERIOD, entity.PERIOD_UNIT);
            if (entity.IS_APPLY != true)
                return false;

            DateTime startTime = entity.START_TIME;
            DateTime EndTime = InputMart.Instance.GlobalParameters.start_time.AddDays(InputMart.Instance.GlobalParameters.period);
            switch (entity.REPEAT_CYCLE) // FACTORY BREAK
            {
                case "DAY":
                    for (DateTime tt = startTime; tt < EndTime; tt = tt.AddDays(1))
                    {
                        SmartAPSFactoryBreakTime fb = new SmartAPSFactoryBreakTime();
                        fb.StartTime = tt;
                        fb.EndTime = tt.AddSeconds(period);
                        InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Add(fb);
                    }
                    break;
                case "WEEK":
                    for (DateTime tt = startTime; tt < EndTime; tt = tt.AddDays(7))
                    {
                        SmartAPSFactoryBreakTime fb = new SmartAPSFactoryBreakTime();
                        fb.StartTime = tt;
                        fb.EndTime = tt.AddSeconds(period);
                        InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Add(fb);
                    }
                    break;
                case "MONTH":
                    for (DateTime tt = startTime; tt < EndTime; tt = tt.AddMonths(1))
                    {
                        SmartAPSFactoryBreakTime fb = new SmartAPSFactoryBreakTime();
                        fb.StartTime = tt;
                        fb.EndTime = tt.AddSeconds(period);
                        InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Add(fb);
                    }
                    break;
                case "YEAR":
                    for (DateTime tt = startTime; tt < EndTime; tt = tt.AddYears(1))
                    {
                        SmartAPSFactoryBreakTime fb = new SmartAPSFactoryBreakTime();
                        fb.StartTime = tt;
                        fb.EndTime = tt.AddSeconds(period);
                        InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Add(fb);
                    }
                    break;
                default: 
                    {
                        SmartAPSFactoryBreakTime fb = new SmartAPSFactoryBreakTime();
                        fb.StartTime = entity.START_TIME;
                        fb.EndTime = entity.START_TIME.AddSeconds(period);
                        InputMart.Instance.SmartAPSFactoryBreakTime.Rows.Add(fb);
                        break;
                    }
            }
            return true;
        }

        public bool OnAfterLoad_EQP_UTILIZATION(EQP_UTILIZATION entity)
        {
            var eqp = InputMart.Instance.SmartAPSEqpView.FindRows(entity.EQP_ID).FirstOrDefault();

            if (eqp == null)
            {
                string errorReason = $"EQUIPMENT data is missing";
                string item = $"{nameof(entity.EQP_ID)} = '{entity.EQP_ID}'";
                ErrorHelper.WriteError(Constants.IN0029, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_EQUIPMENT", errorReason,
                item, equipment: entity.EQP_ID);
            }
            if (entity.START_TIME > entity.END_TIME)
            {
                string errorReason = $"The end time is earlier than the start time";
                string item = $"{nameof(entity.START_TIME)} = '{entity.START_TIME}', {nameof(entity.END_TIME)} = '{entity.END_TIME}'";
                ErrorHelper.WriteError(Constants.IN0034, ErrorSeverity.WARNING, ModelContext.Current.StartTime, "INVALID_EQP_UTILIZATION_TIME", errorReason,
                item);
            }

            return true;
        }

        public void OnAction_PEG_CONDITION(IPersistContext context)
        {
            if (InputMart.Instance.PEG_CONDITION.Rows.FirstOrDefault() == null)
            {
                var NewPeg = new PEG_CONDITION();
                NewPeg.STEP_ID = "ALL";
                NewPeg.ITEM = "PRODUCT";

                InputMart.Instance.PEG_CONDITION.ImportRow(NewPeg);
            }
        }
    }
}