using DevExpress.XtraEditors.Repository;
using DevExpress.XtraSpreadsheet;
using Mozart.Collections;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Properties;
using SmartAPS.UserLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace SmartAPS.UI.Gantts
{
	public enum GanttType
    {
        EqpGantt,
        ProdGantt
    }

    //public enum EqpState
    //{
    //    PM = 0,
    //    SETUP = 1,
    //    BUSY = 2,
    //    IDLE = 3,
    //    IDLERUN = 4,
    //    DOWN = 5,
    //    BREAK = 6,
    //    NONE = 7
    //}

    public enum MouseSelectType
    {
        Product,
        PB,
        PO,
        POB,
        Pattern
    }

    public enum MesEqpStatus
    {
        PM,
        DOWN,
        OFF,
        RUN,
        E_RUN,
        IDLE,
        Set_Up,
        W_CST,
        NONE,
    }

    public class GanttMaster : GanttView
    {
        private GanttType _ganttType;

        public string TargetVersionNo { get; set; }
        protected DateTime PlanStartTime { get; set; }

        private bool SelectMode { get; set; }
        //private ColorGenerator ColorGen { get; set; }
        private BrushInfo BrushEmpth = new BrushInfo(Color.Transparent);
        private Dictionary<string, Color> ProdColors { get; set; }

        public MouseSelectType MouseSelType { get; set; }        

        private Dictionary<string, int> _jobChgCntByHour;
        private Dictionary<string, int> _jobChgCntByShift;
        private HashSet<string> _visibleItems;
        private ColorGenerator _colorGen;
        protected EqpMaster EqpMgr { get; set; }
        protected DoubleDictionary<string, DateTime, EQP_DISPATCH_LOG> DispInfos { get; set; }
        protected Dictionary<string, STD_STEP_INFO> StdSteps { get; set; }
        protected Dictionary<string, EqpMaster.Eqp> ValidEqps { get; set; }
		protected Dictionary<string, EQUIPMENT> EqpInfos { get; set; }
		public bool IsProductInBarTitle { get; private set; }
        public bool IsCheckTargetComp { get; set; }
        public Dictionary<string, float> _duplicatedQty;
        public string TargetLineID { get; set; }
        public IList<string> EqpGroups { get; set; }
        protected IExperimentResultItem Result { get; set; }

        public GanttType GanttType
        {
            get { return this._ganttType; }
            set { this._ganttType = value; }
        }

        new public bool EnableSelect
        {
            get { return this.SelectMode; }
        }

        private bool IsUsedProdColor { get; set; }
        
        public GanttMaster(SpreadsheetControl grid,
                           GanttType type, IExperimentResultItem result)
            : base(grid)
        {
            this.GanttType = type;
            this.Result = result;
            this._colorGen = new ColorGenerator();
        }

        public void Reset(DateTime planStartTime, EqpMaster eqpMgr)
        {
            this.PlanStartTime = planStartTime;
            this.EqpMgr = eqpMgr;

            this.SelectMode = false;
            //this.ColorGen = new ColorGenerator();

            _visibleItems = new HashSet<string>();

            _jobChgCntByHour = new Dictionary<string, int>();
            _jobChgCntByShift = new Dictionary<string, int>();
            _duplicatedQty = new Dictionary<string, float>();

            this.ValidEqps = new Dictionary<string, EqpMaster.Eqp>();
            this.StdSteps = new Dictionary<string, STD_STEP_INFO>();
            this.EqpInfos = new Dictionary<string, EQUIPMENT>();

            this.DispInfos = new DoubleDictionary<string, DateTime, EQP_DISPATCH_LOG>();

            this.ClearData();
            this.ResetWorksheet();
        }

        #region GanttOption

        public void TurnOnSelectMode() { this.SelectMode = true; }

        public void TurnOffSelectMode() { this.SelectMode = false; }

        public BrushInfo GetBrushInfo(GanttBar bar, string patternOfProdID)
        {
            BrushInfo brushinfo = null;

            if (bar.State == EqpState.SETUP)
            {
                brushinfo = new BrushInfo(Color.Red);
            }
            else if (bar.State == EqpState.IDLE || bar.State == EqpState.IDLERUN)
            {
                brushinfo = new BrushInfo(Color.White);
            }
            else if (bar.State == EqpState.PM)
            {
                brushinfo = new BrushInfo(Color.Black);
            }
            else if (bar.State == EqpState.DOWN)
            {
                brushinfo = new BrushInfo(HatchStyle.Percent30, Color.Gray, Color.Black);
            }
            else if (bar.IsGhostBar)
            {
                brushinfo = new BrushInfo(HatchStyle.Percent30, Color.LightGray, Color.White);
            }
            else
            {
                var color = GetBarColorByProductID(bar.ProductID);

                //if (bar.WipInitRun == "Y")
                //    brushinfo = new BrushInfo(HatchStyle.Percent30, Color.Black, color);
                //else
                    brushinfo = new BrushInfo(color);
            }

            var selBar = this.SelectedBar;

            if (!this.EnableSelect || selBar == null)
            {
                bar.BackColor = brushinfo.BackColor;
                return brushinfo;
            }

            if (!CompareToSelectedBar(bar, patternOfProdID))
            {
                bar.BackColor = this.BrushEmpth.BackColor;

                return this.BrushEmpth;
            }

            bar.BackColor = brushinfo.BackColor;
            return brushinfo;
        }

        public Color GetBarColorByProductID(string productID)
        {
            Color color = this.BrushEmpth.BackColor;
            if (string.IsNullOrEmpty(productID))
                return color;

            if (this.IsUsedProdColor)
            {
                Color prodColor;
                if (this.ProdColors.TryGetValue(productID, out prodColor))
                    color = prodColor;
            }
            else
            {
                return this._colorGen.GetColor(productID);
            }

            return color;
        }

        public bool CompareToSelectedBar(GanttBar bar, string patternOfProdID)
        {
            if (bar.IsGhostBar)
                return true;

            if (bar.State == EqpState.PM || bar.State == EqpState.DOWN)
                return true;

            if (this.MouseSelType == MouseSelectType.Pattern)
            {
                if (string.IsNullOrEmpty(patternOfProdID))
                    return true;

                bool isLike = MyHelper.STRING.Like(bar.ProductID, patternOfProdID);

                return isLike;
            }

            var selBar = this.SelectedBar as GanttBar;

            if (selBar == null)
                return true;

            if (this.MouseSelType == MouseSelectType.Product)
            {
                return selBar.ProductID == bar.ProductID;
            }
            //else if (this.MouseSelType == MouseSelectType.PB)
            //{
            //    return selBar.ProductID + selBar.ProductVersion == bar.ProductID + bar.ProductVersion;
            //}
            else if (this.MouseSelType == MouseSelectType.PO)
            {
                return selBar.ProductID + selBar.StepID == bar.ProductID + bar.StepID;
            }
            //else if (this.MouseSelType == MouseSelectType.POB)
            //{
            //    return selBar.ProductID + selBar.StepID + selBar.ProductVersion
            //        == bar.ProductID + bar.StepID + bar.ProductVersion;
            //}

            return false;
        }

        #endregion

        #region Initialize

        protected virtual void ClearData()
        {
            this.Clear();

            _visibleItems.Clear();
            _jobChgCntByHour.Clear();
            _jobChgCntByShift.Clear();
        }

        public void PrepareData(bool isProductInBarTitle, bool isUsedProdColor)
        {
            this.IsProductInBarTitle = isProductInBarTitle;
            this.IsUsedProdColor = isUsedProdColor;

            PrepareData_Eqp();
            PrepareData_StdStep();
            PrepareDispatchingData();
        }

		private void PrepareData_Eqp()
		{
			if (this.EqpInfos == null)
				this.EqpInfos = new Dictionary<string, EQUIPMENT>();

			var table = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(this.Result);

			if (table == null)
				return;

			foreach (var it in table)
			{
				if (this.EqpInfos.ContainsKey(it.EQP_ID) == false)
					this.EqpInfos[it.EQP_ID] = it;
			}
		}

        private void PrepareData_StdStep()
        {
            if (this.StdSteps == null)
                this.StdSteps = new Dictionary<string, STD_STEP_INFO>();

            var table = MyHelper.DATASVC.GetEntityData<STD_STEP_INFO>(this.Result);

            if (table == null)
                return;

            foreach (var it in table)
            {
                if (this.StdSteps.ContainsKey(it.STD_STEP_ID) == false)
                    this.StdSteps[it.STD_STEP_ID] = it;
            }
        }

        private void PrepareDispatchingData()
        {
            var table = MyHelper.DATASVC.GetEntityData<EQP_DISPATCH_LOG>(this.Result);
            if (table == null)
                return; 

            var infos = this.DispInfos;
            foreach (var it in table)
            {
                string eqpID = it.EQP_ID;
             
                DateTime dispatchingTime = MyHelper.DATE.StringToDateTime(it.DISPATCHING_TIME);

                Dictionary<DateTime, EQP_DISPATCH_LOG> dic;
                if (infos.TryGetValue(eqpID, out dic) == false)
                    infos[eqpID] = dic = new Dictionary<DateTime, EQP_DISPATCH_LOG>();

                if (dic.ContainsKey(dispatchingTime) == false)
                    dic[dispatchingTime] = it;
            }
        }

        #endregion

        #region Bind Controls

        public void BindChkListEqpGroup(RepositoryItemCheckedComboBoxEdit control, string targetLineID)
        {
            control.Items.Clear();

            SortedSet<string> list = new SortedSet<string>();

            var allEqps = this.EqpMgr.EqpAll;
            foreach (var eqp in allEqps.Values)
            {
                string eqpGroup = eqp.EqpGroup;
                if (string.IsNullOrEmpty(eqpGroup))
                    continue;

				string lineID = eqp.LineID;
				if (targetLineID != Consts.ALL)
				{
					bool isAdd = false;

					if (isAdd == false)
						isAdd = lineID == targetLineID;

					if (isAdd == false)
						continue;
				}

				if (list.Contains(eqpGroup) == false)
                    list.Add(eqpGroup);
            }

            foreach (var eqpGroup in list)
                control.Items.Add(eqpGroup);
        }

        public void BindChkListEqpId(RepositoryItemCheckedComboBoxEdit control, IList<string> eqpGrps)
        {
            control.Items.Clear();

            SortedSet<string> list = new SortedSet<string>();

            var allEqps = this.EqpMgr.EqpAll;
            foreach (var eqp in allEqps.Values)
            {
                string eqpId = eqp.EqpID;
                if (string.IsNullOrEmpty(eqpId))
                    continue;

                string eqpGrp = eqp.EqpGroup;

                if (string.IsNullOrEmpty(eqpGrp))
                    continue;

                if (eqpGrps.Contains(eqpGrp) == false)
                    continue;

                if (list.Contains(eqpId) == false)
                    list.Add(eqpId);
            }

            foreach (var eqpGroup in list)
                control.Items.Add(eqpGroup);
        }

        #endregion

        #region Job Change

        public void AddJobChange(DateTime startTime)
        {
            string chgTime = startTime.ToString(this.DateKeyPattern);
            string shiftTime = ShopCalendar.ShiftStartTimeOfDayT(startTime).ToString(this.DateGroupPattern);

            if (_jobChgCntByHour.ContainsKey(chgTime) == false)
                _jobChgCntByHour.Add(chgTime, 0);

            if (_jobChgCntByShift.ContainsKey(shiftTime) == false)
                _jobChgCntByShift.Add(shiftTime, 0);

            _jobChgCntByHour[chgTime]++;
            _jobChgCntByShift[shiftTime]++;
        }

        public string GetJobChgHourCntFormat(DateTime targetTime)
        {
            string hourString = targetTime.Hour.ToString();

            return string.Format("{0}", hourString);
        }

        public string GetJobChgShiftCntFormat(DateTime shiftTime)
        {
            string shift = shiftTime.ToString(this.DateGroupPattern);

            return string.Format("{0}", shift);
        }

        #endregion


        public DataTable listToDT(List<EQP_PLAN> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("STEP_TYPE");
            dt.Columns.Add("DEMAND_ID");
            dt.Columns.Add("TARGET_DATE", typeof(DateTime));
            dt.Columns.Add("EQP_STATE_CODE");
            dt.Columns.Add("EQP_END_TIME", typeof(DateTime));
            dt.Columns.Add("EQP_START_TIME", typeof(DateTime));
            dt.Columns.Add("STEP_IN_TIME", typeof(DateTime));
            dt.Columns.Add("PROCESS_QTY");
            dt.Columns.Add("EQP_PLAN_ID");
            dt.Columns.Add("STEP_ID");
            dt.Columns.Add("PROCESS_ID");
            dt.Columns.Add("PRODUCT_ID");
            dt.Columns.Add("LOT_ID");
            dt.Columns.Add("EQP_ID");
            dt.Columns.Add("LINE_ID");
            dt.Columns.Add("VERSION_NO");
            dt.Columns.Add("DUE_DATE", typeof(DateTime));

            foreach (var item in list)
            {
                DataRow row = dt.NewRow();

                row["STEP_TYPE"] = item.STEP_TYPE;
                row["DEMAND_ID"] = item.DEMAND_ID;
                row["TARGET_DATE"] = item.TARGET_DATE;
                row["EQP_STATE_CODE"] = item.EQP_STATE_CODE;
                row["EQP_START_TIME"] = item.EQP_START_TIME;
                row["EQP_END_TIME"] = item.EQP_END_TIME;
                row["PROCESS_QTY"] = item.PROCESS_QTY;
                row["STEP_IN_TIME"] = item.STEP_IN_TIME;
                row["EQP_PLAN_ID"] = item.EQP_PLAN_ID;
                row["STEP_ID"] = item.STEP_ID;
                row["PROCESS_ID"] = item.PROCESS_ID;
                row["PRODUCT_ID"] = item.PRODUCT_ID;
                row["LOT_ID"] = item.LOT_ID;
                row["EQP_ID"] = item.EQP_ID;
                row["LINE_ID"] = item.LINE_ID;
                row["VERSION_NO"] = item.VERSION_NO;
                row["DUE_DATE"] = item.DUE_DATE;

                dt.Rows.Add(row);
            }

            return dt;
        }


        #region BindData

        protected List<EQP_PLAN> GetPlanData(IList<string> eqpIds, IList<string> prodIds, string demandId = null)
        {
            var table = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(this.Result);
            
            var finds = table.Where(t => IsMatchedEqpPlan(t, this.FromTime, this.ToTime, this.EqpGroups, this.TargetLineID, eqpIds, prodIds, demandId));

            var list = finds.ToList();
            list.Sort(ComparerEqpPlan);
            DataTable dttt = listToDT(list);

            int cnt = list.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (i == cnt - 1)
                    break;

                // only for BUSY & SETUP
                if (list[i].EQP_STATE_CODE != "BUSY" && list[i].EQP_STATE_CODE != "SETUP")
                    continue;

                if (list[i].EQP_ID != list[i + 1].EQP_ID)
                    continue;

                // 다음 item이 몇 번이나 연속된 BREAK인지 체크
                int nextCnt = 0;
                while (list[i + nextCnt + 1].EQP_STATE_CODE == "BREAK" && list[i].EQP_ID == list[i + nextCnt + 1].EQP_ID)
                {
                    // 그 다음 item도 break인데, 중간에 lot item이 없는 경우 (break 끼리의 간격이 긴 경우)
                    if (list[i + nextCnt].EQP_END_TIME < list[i + nextCnt + 1].EQP_START_TIME)
                        break;

                    nextCnt++;

                    if (list.Count == i + nextCnt + 1)
                        break;
                }

                if (list[i].AUTOMATION == "MANUAL")
                {
                    // BREAK 바로 이전의 item이 BUSY이면서 item이 BREAK이 끝난 이후까지 이어진 경우 2개의 item으로 Split
                    if (list[i + 1].EQP_STATE_CODE == "BREAK"
                        && (list[i].EQP_END_TIME > list[i + nextCnt].EQP_END_TIME || list[i + nextCnt].EQP_END_TIME >= this.ToTime))
                    {
                        EQP_PLAN newItem = list[i].Clone() as EQP_PLAN;
                        newItem.EQP_START_TIME = list[i + nextCnt].EQP_END_TIME;
                        newItem.EQP_END_TIME = list[i].EQP_END_TIME;
                        newItem.EQP_PLAN_ID = Guid.NewGuid().ToString();
                        list[i].EQP_END_TIME = list[i + 1].EQP_START_TIME;
                        
                        list.Insert(i + nextCnt + 1, newItem);
                        
                        // split으로 인해 중복된 값
                        if (_duplicatedQty.ContainsKey(list[i].EQP_ID))
                            _duplicatedQty[list[i].EQP_ID] += list[i].PROCESS_QTY;
                        else
                            _duplicatedQty[list[i].EQP_ID] = list[i].PROCESS_QTY;

                        cnt++;
                    }
                    // BREAK 바로 이전의 item이 SETUP인 경우 혹은 eqp가 auto인 경우 BREAK의 START_TIME을 수정
                    else if (list[i].EQP_STATE_CODE == "SETUP" && list[i + 1].EQP_STATE_CODE == "BREAK"
                             && list[i].EQP_END_TIME > list[i + 1].EQP_START_TIME)
                    {
                        list[i + 1].EQP_START_TIME = list[i].EQP_END_TIME;
                    }
                }
                else // AUTO
                {
                    // BREAK 바로 이전의 item이 SETUP인 경우 혹은 eqp가 auto인 경우 BREAK의 START_TIME을 수정
                    if (list[i + 1].EQP_STATE_CODE == "BREAK" && list[i].EQP_END_TIME > list[i + 1].EQP_START_TIME)
                    {
                        list[i + 1].EQP_START_TIME = list[i].EQP_END_TIME;
                    }
                    // SETUP으로 인해 BUSY item이 BREAK 사이에 할당이 됐을 때
                    else if (i > 0 && list[i - 1].EQP_STATE_CODE == "BREAK" 
                             && list[i].EQP_START_TIME >= list[i - 1].EQP_START_TIME && list[i].EQP_END_TIME <= list[i - 1].EQP_END_TIME)
                    {
                        list[i - 1].EQP_START_TIME = list[i].EQP_END_TIME;
                    }
                }
            }

            // sort again
            if (this.GanttType == GanttType.EqpGantt)
                list.Sort(ComparerEqpPlan);
            else
                list.Sort(ComparerProductEqpPlan);

            return list;
        }

        private bool IsMatchedEqpPlan(EQP_PLAN info, DateTime fromTime, DateTime toTime, IList<string> eqpGroupList,
                                      string targetLineID, IList<string> eqpIdList, IList<string> prodIdList, string demandId)
        {
            if (info == null)
                return false;

            if (info.EQP_START_TIME >= toTime)
                return false;

            if (info.EQP_END_TIME <= fromTime)
                return false;

            if (targetLineID.ToUpper().Trim() != "ALL" && info.LINE_ID != targetLineID)
                return false;

            if (this.GanttType == GanttType.EqpGantt)
            {
                EQUIPMENT eqpInfo;
                if (eqpGroupList != null && EqpInfos.TryGetValue(info.EQP_ID, out eqpInfo))
                {
                    if (eqpGroupList.Contains(eqpInfo.EQP_GROUP) == false)
                        return false;
                }

                if (eqpIdList != null && eqpIdList.Contains(info.EQP_ID) == false)
                    return false;
            }
            else if (this.GanttType == GanttType.ProdGantt)
            {
                if (eqpIdList != null && info.EQP_STATE_CODE == "BREAK" && eqpIdList.Contains(info.EQP_ID) == true)
                    return true;

                if (prodIdList != null && prodIdList.Contains(info.PRODUCT_ID) == false)
                    return false;

                if (info.PRODUCT_ID == "-" || info.PRODUCT_ID == null)
                    return false;

                if (demandId != "ALL" && !demandId.Equals(info.DEMAND_ID))
                    return false;
            }

            return true;
        }

        public int ComparerEqpPlan(EQP_PLAN x, EQP_PLAN y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            int cmp;

            cmp = string.Compare(x.EQP_ID, y.EQP_ID);

            if (cmp == 0)
            {
                DateTime x_START_TIME = x.EQP_START_TIME;
                DateTime y_START_TIME = y.EQP_START_TIME;

                cmp = DateTime.Compare(x_START_TIME, y_START_TIME);

                if (cmp == 0)
                {
                    cmp = string.Compare(y.EQP_STATE_CODE, x.EQP_STATE_CODE);
                }
            }

            return cmp;
        }

        public int ComparerProductEqpPlan(EQP_PLAN x, EQP_PLAN y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            int cmp;

            cmp = string.Compare(x.PRODUCT_ID, y.PRODUCT_ID);
            if (cmp == 0)
            {
                cmp = string.Compare(x.STEP_ID, y.STEP_ID);
                if (cmp == 0)
                {
                    cmp = string.Compare(x.EQP_ID, y.EQP_ID);
                    if (cmp == 0)
                    {
                        DateTime x_START_TIME = x.EQP_START_TIME;
                        DateTime y_START_TIME = y.EQP_START_TIME;

                        cmp = DateTime.Compare(x_START_TIME, y_START_TIME);

                        if (cmp == 0)
                        {
                            cmp = string.Compare(y.EQP_STATE_CODE, x.EQP_STATE_CODE);
                        }
                    }
                }
            }

            return cmp;
        }

        //protected string GetLayer(string shopID, string stepID)
        //{
        //    string key = MyHelper.STRING.CreateKey(shopID, stepID);

        //    STD_STEP_INFO info;
        //    if (this.StdSteps.TryGetValue(key, out info))
        //        return string.Format("{0} ({1})", info.STD_STEP_ID, info.LAYER_ID ?? string.Empty);

        //    return string.Empty;
        //}

        //protected int GetSortSeq(string shopID, string stepID)
        //{
        //    string key = MyHelper.STRING.CreateKey(shopID, stepID);

        //    STD_STEP_INFO info;
        //    if (this.StdSteps.TryGetValue(key, out info))
        //        return Convert.ToInt32(info.STEP_SEQ);

        //    return int.MaxValue;
        //}

        protected void SetValidEqpIDList(string selectedLineId = null, string eqpPattern = null)
        {
            if (this.ValidEqps.Count > 0)
                this.ValidEqps.Clear();

            Dictionary<string, EqpMaster.Eqp> eqps = EqpMgr.GetEqpsByLine(selectedLineId);
            if (eqps == null)
                return;

            foreach (var eqp in eqps.Values)
            {
                if (string.IsNullOrEmpty(eqpPattern) == false && eqp.EqpID.Contains(eqpPattern) == false)
                    continue;

                if (this.ValidEqps.ContainsKey(eqp.EqpID) == false)
                    this.ValidEqps.Add(eqp.EqpID, eqp);
            }
        }

        public bool TryGetValidEqp(string eqpId, out EqpMaster.Eqp eqp)
        {
            if (this.ValidEqps.TryGetValue(eqpId, out eqp))
                return true;

            return false;
        }

        protected void SetProdColors()
        {
            //var dic = this.ProdColors = new Dictionary<string, Color>();
            
            //var table = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.TargetVersionNo);
            //if (table == null)
            //    return;

            //foreach (var it in table)
            //{
            //    string key = it.PRODUCT_ID;
            //    if (string.IsNullOrEmpty(key) || dic.ContainsKey(key))
            //        continue;

            //    int a, r, g, b;
            //    if (TryGetArgb(it.VIEW_COLOR, out a, out r, out g, out b) == false)
            //        continue;

            //    Color color = Color.FromArgb(a, r, g, b);
            //    dic.Add(key, color);
            //}
        }

        private bool TryGetArgb(string viewColor, out int a, out int r, out int g, out int b)
        {
            a = -1;
            r = -1;
            g = -1;
            b = -1;

            if (string.IsNullOrEmpty(viewColor))
                return false;

            var arr = viewColor.Split(',');

            int count = arr.Length;
            if (count < 4)
                return false;

            if (int.TryParse(arr[0], out a) == false)
                return false;

            if (int.TryParse(arr[1], out r) == false)
                return false;

            if (int.TryParse(arr[2], out g) == false)
                return false;

            if (int.TryParse(arr[3], out b) == false)
                return false;


            return true;
        }

        #endregion

        #region Inner Class

        private class DownItemInfo
        {
            public string EqpID;
            public string EqpGroup;
            public DateTime LoadTime;
            public DateTime EndTime;
            public DateTime StateEndTime;
            public EqpState State;
            public DateTime ToTime;
            public EqpMaster.Eqp Eqp;

            public DownItemInfo(string eqpID, string eqpGroup, DateTime loadTime, DateTime endTime, DateTime stateEndTime, EqpState state, DateTime toTime, EqpMaster.Eqp eqp)
            {
                this.EqpID = eqpID;
                this.EqpGroup = eqpGroup;
                this.LoadTime = loadTime;
                this.EndTime = endTime;
                this.StateEndTime = stateEndTime;
                this.State = state;
                this.ToTime = toTime;
                this.Eqp = eqp;
            }
        }

        public class CompareMBarList : IComparer<LinkedBarNode>
        {
            public int Compare(LinkedBarNode x, LinkedBarNode y)
            {
                Bar a = x.LinkedBarList[0].BarList[0];
                Bar b = y.LinkedBarList[0].BarList[0];

                int cmp = a.TkinTime.CompareTo(b.TkinTime);

                if (cmp == 0)
                    cmp = y.LinkedBarList.Count.CompareTo(x.LinkedBarList.Count);

                return cmp;
            }
        }

        #endregion
    }
}
