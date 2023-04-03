using DevExpress.XtraSpreadsheet;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartAPS.UI.Helper;
using SmartAPS.Outputs;
using Mozart.Studio.TaskModel.Projects;
using SmartAPS.Inputs;

namespace SmartAPS.UI.Gantts
{
    public class ProductGantt : GanttMaster
    {
        public enum ViewMode
        {
            PRODUCT,
            EQPGROUP,
            EQP,
            STEP,
            EQPMODEL
        }

        public Dictionary<string, GanttInfo> GanttInfos { get; set; }

        public ProductGantt(SpreadsheetControl grid, IExperimentResultItem result)
            : base(grid, GanttType.ProdGantt, result)
        {
            this.GanttInfos = new Dictionary<string, GanttInfo>();
        }

        public void BuildData_Sim(
            string targetLineID,
            IList<string> prodIds,
            string demandId,
            DateTime fromTime,
            DateTime toTime,
            ViewMode viewMode)
        {
            ClearData();

            this.TargetLineID = targetLineID;
            this.FromTime = fromTime;
            this.ToTime = toTime;

            SetValidEqpIDList();
            SetProdColors();

            var eqpIds = new List<string>();
            var eqpArranges = MyHelper.DATASVC.GetEntityData<EQP_ARRANGE>(this.Result);
            foreach (string prodId in prodIds)
            {
                var findArr = eqpArranges.Where(x => x.PRODUCT_ID == prodId);
                foreach (var arr in findArr)
                {
                    if (!eqpIds.Contains(arr.EQP_ID))
                        eqpIds.Add(arr.EQP_ID);
                }
            }

            var planList = GetPlanData(eqpIds, prodIds, demandId);
            FillData_EqpPlan(planList, viewMode, true);

            //no plan eqp
            var idlePlanList = GetPlanData_OnlyIDLE(planList);
            FillData_EqpPlan(idlePlanList, viewMode, false);
        }

        private void FillData_EqpPlan(
            List<EQP_PLAN> planList,
            ViewMode viewMode,
            bool isFilterIDLE)
        {
            if (planList == null)
                return;

            var stepInfos = GetPlanStepList(planList);

            var fromTime = this.FromTime;
            var toTime = this.ToTime;

            foreach (var item in planList)
            {
                EqpState state = Enums.ParseEqpState(item.EQP_STATE_CODE);
                if (IsMatched_Plan(item, state, isFilterIDLE) == false)
                    continue;

                string eqpID = item.EQP_ID;

                EqpMaster.Eqp eqp;
                if (TryGetValidEqp(eqpID, out eqp) == false)
                    continue;

                string eqpGroup = eqp.EqpGroup;
                string eqpModel = eqp.EqpModel;

                string productID = MyHelper.STRING.ToSafeString(item.PRODUCT_ID);
                string processID = MyHelper.STRING.ToSafeString(item.PROCESS_ID);
                string stepID = MyHelper.STRING.ToSafeString(item.STEP_ID);
                string lotID = MyHelper.STRING.ToSafeString(item.LOT_ID);

                DateTime startTime = item.EQP_START_TIME;
                DateTime endTime = item.EQP_END_TIME;
                DateTime arrivalTime = item.STEP_IN_TIME;
                DateTime lpst = item.TARGET_DATE;

                if (startTime < fromTime)
                    startTime = fromTime;

                int qty = (int)item.PROCESS_QTY;
                if (state == EqpState.SETUP)
                    qty = 0;

                bool isOnTime = startTime <= item.TARGET_DATE;
                string origLotID = lotID;
                EQP_DISPATCH_LOG dispInfo = FindDispInfo(eqpID, state, startTime, endTime);
                string detailEqpID = GetDetailEqpID(eqpID);
                List<string> stepList = FindPlanStepList(stepInfos, detailEqpID, stepID, state);

                foreach (string planStepID in stepList)
                {
                    string key = MyHelper.STRING.CreateKey(detailEqpID, planStepID);
                    if (viewMode == ViewMode.EQPGROUP)
                        key = MyHelper.STRING.CreateKey(detailEqpID, eqpGroup);
                    else if (viewMode == ViewMode.EQPMODEL)
                        key = MyHelper.STRING.CreateKey(detailEqpID, eqpModel);
                    else if (viewMode == ViewMode.PRODUCT)
                        key = MyHelper.STRING.CreateKey(productID, planStepID, detailEqpID);

                    AddItem(key,
                            eqp,
                            lotID,
                            origLotID,
                            productID,
                            processID,
                            planStepID,
                            arrivalTime,
                            startTime,
                            endTime,
                            lpst,
                            qty,
                            state,
                            dispInfo,
                            isOnTime,
                            false,
                            this.IsProductInBarTitle
                            );
                }
            }
        }

        private Dictionary<string, List<string>> GetPlanStepList(List<EQP_PLAN> planList)
        {
            Dictionary<string, List<string>> infos = new Dictionary<string, List<string>>();

            bool isFilterIDLE = true;
            foreach (var item in planList)
            {
                EqpState state = Enums.ParseEqpState(item.EQP_STATE_CODE);

                if (state == EqpState.DOWN || state == EqpState.PM)
                    continue;

                if (IsMatched_Plan(item, state, isFilterIDLE) == false)
                    continue;

                string eqpID = item.EQP_ID;

                EqpMaster.Eqp eqp;
                if (TryGetValidEqp(eqpID, out eqp) == false)
                    continue;

                string detailEqpID = GetDetailEqpID(eqpID);

                string stepID = MyHelper.STRING.ToSafeString(item.STEP_ID);

                List<string> list;
                if (infos.TryGetValue(detailEqpID, out list) == false)
                {
                    list = new List<string>();
                    infos.Add(detailEqpID, list);
                }

                if (list.Contains(stepID))
                    continue;

                list.Add(stepID);
            }

            return infos;
        }

        private bool IsMatched_Plan(EQP_PLAN item, EqpState state, bool isFilterIDLE)
        {
            var fromTime = this.FromTime;
            var toTime = this.ToTime;

            //상태시작시간                        
            DateTime startTime = item.EQP_START_TIME;
            if (startTime >= toTime)
                return false;

            //상태종료시간
            DateTime endTime = item.EQP_END_TIME;
            if (endTime <= fromTime)
                return false;

            //PlanStartTime보다 EqpPlanStart가 작을 경우 (초기Run재공)
            if (startTime < fromTime)
                startTime = fromTime;

            //PlanEndTime보다 Lot의 EndTime이 작을 경우
            if (endTime > toTime)
                endTime = toTime;

            //ProcessedTime is zero
            if (startTime >= endTime)
                return false;

            if (isFilterIDLE)
            {
                //IDLE, IDLERUN
                if ((state == EqpState.IDLE || state == EqpState.IDLERUN))
                    return false;
            }

            return true;
        }

        private List<EQP_PLAN> GetPlanData_OnlyIDLE(List<EQP_PLAN> planList)
        {
            if (planList == null || planList.Count == 0)
                return null;

            List<EQP_PLAN> list = new List<EQP_PLAN>();

            var groups = planList.GroupBy(t => t.EQP_ID);
            foreach (var it in groups)
            {
                var find = it.FirstOrDefault(t => IsIDLE(t.EQP_STATE_CODE) == false);
                if (find == null)
                {
                    var sample = it.FirstOrDefault();
                    if (sample != null)
                        list.Add(sample);
                }
            }

            return list;
        }

        private bool IsIDLE(string eqpStatus)
        {
            if (string.IsNullOrEmpty(eqpStatus))
                return false;

            EqpState state = Enums.ParseEqpState(eqpStatus);
            if (state == EqpState.IDLE || state == EqpState.IDLERUN)
                return true;

            return false;
        }

        private List<string> FindPlanStepList(Dictionary<string, List<string>> infos, string detailEqpID, string stepID, EqpState state)
        {
            List<string> list = new List<string>();

            if (state == EqpState.DOWN || state == EqpState.PM)
            {
                if (infos != null && infos.Count > 0)
                {
                    List<string> stepList;
                    if (detailEqpID != null && infos.TryGetValue(detailEqpID, out stepList))
                    {
                        list.AddRange(stepList);
                    }
                }
            }

            if (list.Count == 0)
                list.Add(stepID);

            return list;
        }

        private string GetDetailEqpID(string eqpID, string subEqpID = null)
        {
            if (string.IsNullOrEmpty(subEqpID))
                return eqpID;

            return string.Format("{0}-{1}", eqpID ?? "null", subEqpID);
        }

        private void FilterDownEqp(bool isFilterDownEqp)
        {
            if (isFilterDownEqp == false)
                return;

            var infos = this.GanttInfos;
            var keys = infos.Keys.ToList();

            foreach (string key in keys)
            {
                GanttInfo info;
                if (infos.TryGetValue(key, out info) == false)
                    continue;

                bool isRemove = info.Items == null || info.Items.Count == 0;
                if (isRemove == false)
                {
                    bool existNonDown = false;
                    foreach (var barList in info.Items.Values)
                    {
                        //BUSY, SETUP, PM이 전혀 없는 설비 제외
                        var find = barList.Find(t => t.State == EqpState.SETUP
                            || t.State == EqpState.BUSY || t.State == EqpState.PM);

                        if (find != null)
                        {
                            existNonDown = true;
                            break;
                        }
                    }

                    if (existNonDown == false)
                        isRemove = true;
                }

                if (isRemove)
                    infos.Remove(key);
            }
        }

        private EQP_DISPATCH_LOG FindDispInfo(string eqpID, EqpState state, DateTime startTime, DateTime endTime)
        {
            EQP_DISPATCH_LOG find;
            if (this.DispInfos.TryGetValue(eqpID, startTime, out find))
                return find;

            //AheadSetup인 경우 EndTime 기준으로 추가 체크
            if (state == EqpState.SETUP)
            {
                if (this.DispInfos.TryGetValue(eqpID, endTime, out find))
                    return find;
            }

            return null;
        }

        protected override void ClearData()
        {
            base.ClearData();

            this.GanttInfos.Clear();
        }

        private void AddItem(
            string key,
            EqpMaster.Eqp eqpInfo,
            string lotID,
            string origLotID,
            string productID,
            string processID,
            string stepID,
            DateTime arrivalTime,
            DateTime startTime,
            DateTime endTime,
            DateTime lpst,
            int inQty,
            EqpState state,
            EQP_DISPATCH_LOG dispInfo,
            bool isOnTime,
            bool isGhostBar = false,
            bool isProductInBarTitle = true)
        {
            string eqpModel = eqpInfo.EqpModel;
            string eqpGroup = eqpInfo.EqpGroup;
            string eqpID = eqpInfo.EqpID;

            GanttInfo info;
            if (this.GanttInfos.TryGetValue(key, out info) == false)
                this.GanttInfos.Add(key, info = new GanttInfo(productID, eqpModel, eqpGroup, eqpID, stepID));

            //int sortSeq = GetSortSeq(shopID, stepID);

            //SortSeq MIN 기준으로 변경
            //info.SortSeq = Math.Min(info.SortSeq, sortSeq);

            if (!this.IsCheckTargetComp)
                isOnTime = true;

            GanttBar bar = new GanttBar(eqpGroup,
                                        eqpID,
                                        lotID,
                                        origLotID,
                                        productID,
                                        processID,
                                        stepID,
                                        arrivalTime,
                                        startTime,
                                        endTime,
                                        lpst,
                                        inQty,
                                        state,
                                        eqpInfo,
                                        dispInfo,
                                        this,
                                        isOnTime,
                                        isGhostBar,
                                        isProductInBarTitle);

            var barKey = state != EqpState.DOWN ? bar.BarKey : "DOWN";

            if (barKey != string.Empty)
            {
                //SETUP 다음 BUSY로 이어진 BUSY Bar는 SETUP과 동일한 값 설정 하도록 함.
                if (bar.DispInfo == null && bar.State == EqpState.BUSY)
                    bar.DispInfo = GetDispInfoBySetup(info, barKey);

                info.AddItem2(barKey, bar, this.IsProductInBarTitle);
            }

            //collect job change 
            if (state == EqpState.SETUP)
                AddJobChange(startTime);
        }

        private EQP_DISPATCH_LOG GetDispInfoBySetup(GanttInfo info, string barKey)
        {
            BarList barList;
            if (info.Items.TryGetValue(barKey, out barList) == false)
                return null;

            var prev = barList.LastOrDefault() as GanttBar;
            if (prev != null && prev.State == EqpState.SETUP)
                return prev.DispInfo;

            return null;
        }

        public List<ProductGantt.GanttInfo> Expand(bool showLayerBar, ProductGantt.ViewMode selectViewMode, bool isDefault = true)
        {
            var infos = this.GanttInfos.Values.ToList();

            if (showLayerBar)
            {
                if (selectViewMode != ViewMode.EQPGROUP)
                    ExpandLayer(infos);
            }

            foreach (GanttInfo info in this.GanttInfos.Values)
            {
                info.Expand(isDefault);
                info.LinkBar(this, isDefault);
            }

            return infos;
        }

        private void ExpandLayer(List<ProductGantt.GanttInfo> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var info = list[i];

                string eqpGroup = info.EqpGroup;
                string eqpID = info.EqpID;
                string stepID = info.StepID;

                foreach (BarList barList in info.Items.Values)
                {
                    foreach (GanttBar bar in barList)
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            ProductGantt.GanttInfo info2 = list[j];

                            bool condition = info2.StepID != stepID && info2.EqpID == eqpID && bar.IsGhostBar == false && bar.State != EqpState.DOWN;

                            if (condition)
                            {
                                var ghostBar = new GanttBar(eqpGroup,
                                                            eqpID,
                                                            bar.LotID,
                                                            bar.OrigLotID,
                                                            bar.ProductID,
                                                            bar.ProcessID,
                                                            bar.StepID,
                                                            bar.DispatchInTime,
                                                            bar.StartTime,
                                                            bar.EndTime,
                                                            bar.TargetDate,
                                                            0,
                                                            (bar.State == EqpState.PM || bar.State == EqpState.DOWN) ? bar.State : EqpState.IDLERUN,
                                                            bar.EqpInfo,
                                                            bar.DispInfo,
                                                            this,
                                                            true,
                                                            true);

                                info2.AddItem2("ghost", ghostBar, this.IsProductInBarTitle);
                            }
                        }
                    }
                }

                if (info.StepID == "PM")
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        public class GanttInfo : GanttItem
        {
            public string ProductID { get; set; }
            public string EqpModel { get; set; }
            public string EqpGroup { get; set; }
            public string EqpID { get; set; }
            public string StepID { get; set; }

            public int SortSeq { get; set; }

            public GanttInfo(string productID, string eqpModel, string eqpGroup, string eqpID, string stepId, int sortSeq = int.MaxValue)
                : base()
            {
                this.ProductID = productID;
                this.EqpModel = eqpModel;
                this.EqpGroup = eqpGroup;
                this.EqpID = eqpID;
                this.StepID = stepId;
                this.SortSeq = sortSeq;
            }

            public override void AddLinkedNode(Bar bar, LinkedBarNode lnkBarNode)
            {
                base.AddLinkedNode((bar as GanttBar).BarKey, lnkBarNode);
            }

            protected override bool CheckConflict(bool isDefault, Bar currentBar, Bar prevBar)
            {
                return isDefault && (currentBar as GanttBar).BarKey != (prevBar as GanttBar).BarKey;
            }

            public void AddItem2(string key, GanttBar bar, bool isProductInBarTitle, bool isSplitA = false)
            {
                BarList list;
                if (this.Items.TryGetValue(key, out list) == false)
                {
                    this.Items.Add(key, list = new BarList());
                    list.Add(bar);
                    return;
                }

                foreach (GanttBar it in list)
                {
                    if (it.State != bar.State)
                        continue;

                    if (it.IsOnTime != bar.IsOnTime)
                        continue;

                    if (it.IsShiftSplit || (bar.IsShiftSplit && isSplitA == false) && it.State.Equals(bar.State))
                        return;

                    if (it.Merge(bar))
                        return;
                }

                list.Add(bar);
            }

            public Bar GetBarItem(string key, string lotID)
            {
                BarList list;
                if (this.Items.TryGetValue(key, out list) == false)
                    return null;

                return list.FindLast(t => (t as GanttBar).LotID == lotID);
            }

        }
        
        public enum SortOptions
        {
            PRODUCT,
            EQP_GROUP,
            EQP,
            STEP,
            EQP_MODEL
        }

        public class CompareGanttInfo : IComparer<GanttInfo>
        {
            private SortOptions[] SortList { get; set; }
            private GanttType GanttType { get; set; }
            private EqpMaster EqpMst { get; set; }
            private string TargetLineID { get; set; }

            public CompareGanttInfo(GanttType gType, EqpMaster eqpMst, string targetLineID, params SortOptions[] sortList)
            {
                this.SortList = sortList;
                this.GanttType = gType;
                this.EqpMst = eqpMst;
                this.TargetLineID = targetLineID;
            }

            public int Compare(GanttInfo x, GanttInfo y)
            {
                int cmp = 0;
                foreach (var sort in SortList)
                {
                    if (cmp != 0)
                        break;

                    cmp = Compare(x, y, sort);
                }

                return cmp;
            }

            private int Compare(GanttInfo x, GanttInfo y, SortOptions sort)
            {
                if (sort == SortOptions.PRODUCT)
                {
                    int xSeq = x.SortSeq;
                    int ySeq = y.SortSeq;

                    if (xSeq == ySeq)
                    {
                        xSeq = x.StepID.CompareTo(y.ProductID);
                        ySeq = y.StepID.CompareTo(x.ProductID);
                    }

                    return xSeq.CompareTo(ySeq);
                }

                if (sort == SortOptions.EQP_GROUP)
                {
                    int seq_x = this.EqpMst.GetDspGroupSeq(this.TargetLineID, x.EqpGroup);
                    int seq_y = this.EqpMst.GetDspGroupSeq(this.TargetLineID, y.EqpGroup);

                    int cmp = seq_x.CompareTo(seq_y);
                    if (cmp == 0)
                    {
                        cmp = string.Compare(x.EqpGroup, y.EqpGroup);
                    }

                    return cmp;
                }

                if (sort == SortOptions.EQP)
                {
                    int seq_x = this.EqpMst.GetEqpSeq(x.EqpID);
                    int seq_y = this.EqpMst.GetEqpSeq(y.EqpID);

                    int cmp = seq_x.CompareTo(seq_y);
                    if (cmp == 0)
                    {
                        cmp = string.Compare(x.EqpID, y.EqpID);
                    }

                    return cmp;
                }

                if (sort == SortOptions.STEP)
                {
                    int xSeq = x.SortSeq;
                    int ySeq = y.SortSeq;

                    if (xSeq == ySeq)
                    {
                        xSeq = x.StepID.CompareTo(y.StepID);
                        ySeq = y.StepID.CompareTo(x.StepID);
                    }

                    return xSeq.CompareTo(ySeq);
                }

                if (sort == SortOptions.EQP_MODEL)
                {
                    int seq_x = this.EqpMst.GetEqpModelSeq(this.TargetLineID, x.EqpModel);
                    int seq_y = this.EqpMst.GetEqpModelSeq(this.TargetLineID, y.EqpModel);

                    int cmp = seq_x.CompareTo(seq_y);
                    if (cmp == 0)
                    {
                        cmp = string.Compare(x.EqpModel, y.EqpModel);
                    }

                    return cmp;
                }

                return 0;
            }
        }
    }
}
