using DevExpress.XtraSpreadsheet;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
//using Template.Lcd.Scheduling.Inputs;
//using Template.Lcd.Scheduling.Outputs;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Gantts;
using SmartAPS.UI.Helper;
using Mozart.Studio.TaskModel.Projects;

namespace SmartAPS.UI.LotGantts
{
    public enum GanttType
    {
        Default
    }

    public enum MouseSelectType
    {
        Product,
        Lot,
        Process,
        Pattern
    }

    public class LotGanttMaster : GanttView
    {
        public bool IsOnlyLineMode { get; private set; }
       
        public MouseSelectType MouseSelType { get; set; }

        protected DateTime _planStartTime;

        private EqpMaster _eqpMgr;
        public ColorGenerator ColorGen { get; set; }

		private HashedSet<string> LotList { get; set; }

		public string TargetLineID { get; set; }

        public string TargetVersionNo { get; set; }

        public HashSet<string> SelectedProdList { get; set; }
        public HashSet<string> SelectedStepList { get; set; }
        public IList<string> SelectedLotList { get; set; }

        public List<string> _visibleItems;

        IExperimentResultItem Result { get; set; }

        public LotGanttMaster(
            SpreadsheetControl grid,
            string targetLineID,
            DateTime planStartTime,
            EqpMaster eqpMgr,
            IExperimentResultItem result
        )
            : base(grid)
        {
            this.TargetLineID = targetLineID;

            _planStartTime = planStartTime;
            _eqpMgr = eqpMgr;

            this.EnableSelect = false;

            this.ColorGen = new ColorGenerator();
            _visibleItems = new List<string>();

            this.Result = result;

            this.LotList = GetLotList(this.TargetLineID);
		}

        #region from GanttOption

        public void TurnOnSelectMode() { this.EnableSelect = true; }

        public void TurnOffSelectMode() { this.EnableSelect = false; }

        BrushInfo brushEmpth = new BrushInfo(Color.Transparent);
        public BrushInfo GetBrushInfo(LotBar bar, string patternOfProdID)
        {
            BrushInfo brushinfo = null;

            if (bar.State == EqpState.SETUP)
                brushinfo = new BrushInfo(Color.Red);
            else if (bar.State == EqpState.PM)
                brushinfo = new BrushInfo(HatchStyle.Divot, Color.Black, Color.OrangeRed);
            else if (bar.State == EqpState.DOWN)
                brushinfo = new BrushInfo(HatchStyle.Percent30, Color.Gray, Color.Black);
            else if (bar.IsGhostBar)
                brushinfo = new BrushInfo(HatchStyle.Percent30, Color.LightGray, Color.White);
            else
            {
                var color = ColorGen.GetColor(bar.LotID);

                if (bar.State == EqpState.IDLERUN)
                    brushinfo = new BrushInfo(HatchStyle.Percent30, Color.Black, color);
                else
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
                bar.BackColor = brushEmpth.BackColor;
                return brushEmpth;
            }

            bar.BackColor = brushinfo.BackColor;
            return brushinfo;
        }

        public bool CompareToSelectedBar(LotBar bar, string patternOfProdID)
        {
            if (bar.IsGhostBar)
                return true;

            if (this.MouseSelType == MouseSelectType.Pattern)
            {
                if (string.IsNullOrEmpty(patternOfProdID))
                    return true;

                bool isLike = MyHelper.STRING.Like(bar.ProductId, patternOfProdID);

                return isLike;
            }

            var selBar = this.SelectedBar as LotBar;

            if (selBar == null)
                return true;

            if (this.MouseSelType == MouseSelectType.Product)
            {
                return selBar.ProductId == bar.ProductId;
            }
            else if (this.MouseSelType == MouseSelectType.Process)
            {
                return selBar.ProcessId == bar.ProcessId;
            }
            else if (this.MouseSelType == MouseSelectType.Lot)
            {
                return selBar.LotID == bar.LotID;
            }

            return false;
        }

        private string GetLotId(string lotId)
        {
            int idx = lotId.IndexOf('_');
            if (idx > 0)
                return lotId.Substring(0, idx);

            return lotId;
        }

		#endregion

		private HashedSet<string> GetLotList(string targetLineID)
		{
			HashedSet<string> list = new HashedSet<string>();

            var table = GetPlanData();
			if (table == null)
				return list;

			foreach (var item in table)
			{
				if (item.LINE_ID != targetLineID)
					continue;

				string lotID = item.LOT_ID;
				if (string.IsNullOrEmpty(lotID))
					continue;

				list.Add(lotID);
			}

			return list;
		}

		public virtual void ClearData()
        {
            this.Clear();
        }

        #region Job Change

        public string GetJobChgHourCntFormat(DateTime targetTime)
        {
            string hourString = targetTime.Hour.ToString();
            string chgTime = targetTime.ToString(this.DateKeyPattern);

            return string.Format("{0}", hourString);
        }

        public string GetJobChgShiftCntFormat(DateTime shiftTime)
        {
            string shift = shiftTime.ToString(this.DateGroupPattern);

            return string.Format("{0}", shift);
        }

        #endregion

        #region Visible Item

        // 특정 pattern으로 조회 했을 때
        // pattern 과 일치하는 prodCode 를 진행한 설비를 collection에 보관하고,
        // EqpGantt --> Expand() 에서 Filtering하여 화면에 보이지 않게 한다.
        public virtual void AddVisibleItem(string productID, string prodCode, string pattern)
        {
        }

        public void AddVisibleItem(string item)
        {
            if (_visibleItems.Contains(item) == false)
                _visibleItems.Add(item);
        }

        protected bool IsVisibleItem(string item)
        {
            if (_visibleItems == null || _visibleItems.Count == 0)
                return true;

            return _visibleItems.Contains(item);
        }

        #endregion
                                
        public void BuildGantt(
            bool isOnlyLineMode,
            string selectedLineId,
            HashSet<string> selectedProdList,
            HashSet<string> selectedStepList,
            //IList<string> selectedToolList,
            DateTime fromTime,
            DateTime toTime,
            DateTime planStartTime,
            string eqpPattern
        )
        {
            ClearData();

            this.IsOnlyLineMode = isOnlyLineMode;

            this.FromTime = fromTime;
            this.ToTime = toTime;

            this.SelectedProdList = selectedProdList;
            this.SelectedStepList = selectedStepList;
            //this.SelectedToolList = selectedToolList;

            var planList = GetPlanData();
            FillGantt(planList, selectedLineId, fromTime, toTime);
        }

        protected List<EQP_PLAN> GetPlanData()
        {
            var eqpPlan = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(this.Result);
            if (eqpPlan == null)
                return new List<EQP_PLAN>();

            var finds = eqpPlan.Where(t => IsMatchedEqpPlan(t, this.FromTime, this.ToTime, this.SelectedProdList, this.SelectedStepList, this.SelectedLotList));

            var list = finds.ToList();
            list.Sort(ComparerEqpPlan);

            return list;
        }

        private bool IsMatchedEqpPlan(EQP_PLAN info, DateTime fromTime, DateTime toTime, HashSet<string> prodList, HashSet<string> stepList, IList<string> toolList)
        {
            if (info == null)
                return false;

            if (info.EQP_START_TIME >= toTime)
                return false;

            if (info.EQP_END_TIME <= fromTime)
                return false;

            if (prodList != null)
            {
                if (prodList.Contains(info.PRODUCT_ID) == false)
                    return false;
            }

            if (stepList != null)
            {
                if (stepList.Contains(info.STEP_ID) == false)
                    return false;
            }

            //if (toolList != null)
            //{
            //    if (toolList.Contains(info.TOOL_ID) == false)
            //        return false;
            //}

            return true;
        }

        public static int ComparerEqpPlan(EQP_PLAN x, EQP_PLAN y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            int cmp = string.Compare(x.EQP_ID, y.EQP_ID);

            if (cmp == 0)
            {
                DateTime x_START_TIME = x.EQP_START_TIME;
                DateTime y_START_TIME = y.EQP_START_TIME;

                cmp = DateTime.Compare(x_START_TIME, y_START_TIME);
            }

            return cmp;
        }


        private void FillGantt(List<EQP_PLAN> planList, string selectedLineId, DateTime fromTime, DateTime toTime)
        {
            Dictionary<string, List<LotSchedInfo>> lotSchedListDic = new Dictionary<string, List<LotSchedInfo>>();

            foreach (var item in planList)
            {
                if (selectedLineId.ToUpper().Equals("ALL") == false && item.LINE_ID != selectedLineId)
                    continue;

                string lotID = item.LOT_ID;
                if (string.IsNullOrEmpty(lotID))
                    continue;

                if (this.LotList.Contains(lotID))
                    this.LotList.Remove(lotID);

                List<LotSchedInfo> lotSchedList;
                if (lotSchedListDic.TryGetValue(lotID, out lotSchedList) == false)
                    lotSchedListDic.Add(lotID, lotSchedList = new List<LotSchedInfo>());
                
                var eqp = _eqpMgr.FindEqp(item.EQP_ID);
                if(eqp == null)
                    continue;

                //상태시작시간                        
                DateTime startTime = item.EQP_START_TIME;
                if (startTime >= toTime)
                    continue;
                else if (startTime < fromTime)
                { 
                    startTime = fromTime;
                }

                //상태종료시간
                DateTime endTime = item.EQP_END_TIME;
                if (endTime <= fromTime)
                    continue;
                else if (endTime > toTime)
                { 
                    endTime = toTime;
                }

                DateTime tkInTime = startTime;
				DateTime tkInEndTime = endTime;

				int qty = Convert.ToInt32(item.PROCESS_QTY);

                LotSchedInfo lotSched = new LotSchedInfo(item.LINE_ID,
                    item.EQP_ID, item.PRODUCT_ID, item.PROCESS_ID, item.STEP_ID, 
                    item.LOT_ID, startTime, endTime, tkInTime, tkInEndTime, qty, EqpState.BUSY, eqp);

                lotSchedList.Add(lotSched);
            }

            foreach (List<LotSchedInfo> list in lotSchedListDic.Values)
            {
                List<LotSchedInfo> sortedList = list.OrderBy(x => x.TkInTime).ToList();

                int count = sortedList.Count;
                for (int i = 0; i < count; i++)
                {
                    var info = sortedList[i];

                    bool isLast = i == count - 1;
                    var next = isLast ? null : sortedList[i + 1];

                    DateTime nextStateStartTime = next != null ? next.StartTime : toTime;
                                           
                    AddItem(info.EqpInfo,
                            info.LineID, 
                            info.ProductId,
                            info.ProcessId, 
                            info.StepId,
                            info.LotId,
                            info.StartTime,
                            info.EndTime,
                            info.TkInTime,
                            info.TkOutTime,
                            nextStateStartTime,
                            info.Qty,
                            info.State);
                }
            }

            var dummyEqp = EqpMaster.Eqp.CreateDummy(this.TargetLineID, "-");
            foreach (var lotID in this.LotList)
            {
                if (string.IsNullOrEmpty(lotID))
                    continue;

                if (this.SelectedLotList.Contains(lotID) == false)
                    continue;

                AddItem(dummyEqp, this.TargetLineID, lotID, 
                    string.Empty, string.Empty, string.Empty,
                    _planStartTime, _planStartTime, _planStartTime, _planStartTime, _planStartTime, 0, EqpState.IDLE);
            }
        }

        public virtual void AddItem(
            EqpMaster.Eqp eqpInfo,
            string lineID,
            string lotId,
            string productId,
            string processId,
            string stepId,
            DateTime startTime,
            DateTime endTime,
            DateTime tkInTime,
            DateTime tkOutTime,
            DateTime nextStateStartTime,
            int qty,
            EqpState state,
            DataRow dispatchingInfo = null
        )
        {
        }
    }

    public class LotSchedInfo
    {
        public string LineID { get; private set; }
        public string EqpId { get; private set; }
        public string ProductId { get; private set; }
        public string ProcessId { get; private set; }
        public string StepId { get; private set; }
        public string LotId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public DateTime TkInTime { get; private set; }
        public DateTime TkOutTime { get; private set; }
        public int Qty { get; private set; }
        public EqpState State { get; private set; }
        public EqpMaster.Eqp EqpInfo { get; private set; }

        public LotSchedInfo(
            string lineID,
            string eqpId,
            string productId,
            string processId,
            string stepId,
            string lotId,
            DateTime startTime,
            DateTime endTime,
            DateTime tkInTime,
            DateTime tkOutTime,
            int qty,
            EqpState state,
            EqpMaster.Eqp eqpInfo)
        {
            this.LineID = lineID;
            this.EqpId = eqpId;
            this.ProductId = productId;
            this.ProcessId = processId;
            this.StepId = stepId;
            this.LotId = lotId;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.TkInTime = tkInTime;
            this.TkOutTime = tkOutTime;
            this.Qty = qty;
            this.State = state;
            this.EqpInfo = eqpInfo;
        }
    }

    public class CompareMBarList : IComparer<LinkedBarNode>
    {
        #region IComparer<List<LinkedBar>> 멤버

        public int Compare(LinkedBarNode x, LinkedBarNode y)
        {
            Bar a = x.LinkedBarList[0].BarList[0];
            Bar b = y.LinkedBarList[0].BarList[0];

            int cmp = a.TkinTime.CompareTo(b.TkinTime);

            if (cmp == 0)
                cmp = y.LinkedBarList.Count.CompareTo(x.LinkedBarList.Count);

            return cmp;
        }
        #endregion
    }
}
