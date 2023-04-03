using DevExpress.XtraCharts;
using DevExpress.XtraPivotGrid;
using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.Projects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using DevExpress.XtraBars;
using DevExpress.Utils;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors.Repository;
using DevExpress.Office.Drawing;
using DevExpress.XtraCharts.Design;
using DevExpress.Data.Extensions;
using SmartAPS.UserLibrary.Extensions;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;

namespace SmartAPS.UI.Analysis
{   
    public partial class TargetMoveAnalysisView : MyXtraPivotGridTemplate
    {
        private SortedSet<string> _dayHourList;
		private Dictionary<string, double> _stepIndexs;
		private List<int> _allRowsList;
        private List<string> _productList;
        private bool firstChart = true;
        private Dictionary<string, string> _lineDic;
        private Dictionary<string, ResultItem> _processDic;
        private Dictionary<string, double> _categoryIndexs;
        private List<string> _disPlayList;
        private IEnumerable<STEP_TARGET> _stepTarget;
        private IEnumerable<STEP_MOVE> _stepMove;
        private IEnumerable<STEP_WIP> _stepWip;

        private bool setTree = true;
        private Dictionary<string, string> demandEndSteps = new Dictionary<string, string>();

        private string TargetProductID
        {
            get
            {
                return this.editProdId.EditValue as string;
            }
        }

        private List<string> TargetDemandID
        {
            get
            {
                return this.repoDemandID.GetCheckedItemsToList();
            }
        }

        private bool SyncChart
        {
            get
            {
                if (this.CheckSyncChart.Checked == true)
                    return true;
                return false;
            }
        }

        private bool Stacked
        {
            get
            {
                if (this.CheckStacked.Checked == true)
                    return true;
                return false;
            }
        }

        private DateTime QueryStartTime
        {
            get
            {
                return Convert.ToDateTime(this.editDateTime.EditValue);
            }
        }

        private DateTime QueryEndTime
        {
            get
            {
                return this.QueryStartTime.AddHours(Convert.ToInt32(this.TargetAddDays) * ShopCalendar.ShiftHours);
            }
        }

        private int TargetAddDays
        {
            get
            {
                return Convert.ToInt32(this.editDateSpin.EditValue);
            }
        }

        private DateTime GetPlanDate(DateTime t)
        {
            DateTime planDate = t;

            if (MyHelper.ENUM.Equals(this.radioPlanDate.EditValue, "D"))
                planDate = ShopCalendar.StartTimeOfDayT(t);

            return planDate;
        }

        private DateTime GetRptDate_1Hour(DateTime t, int baseMinute)
        {
            //1시간 단위
            int baseHours = 1;

            //ex) HH:30:00
            DateTime rptDate = new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0).AddMinutes(baseMinute); //DateTime.Trim(t,"HH")

            //baseMinute(ex.30분) 이상인 경우 이후 시간대 baseMinute의 실적
            //07:30 = 06:30(초과) ~ 07:30(이하)인경우, 06:40 --> 07:30, 07:30 --> 07:30, 07:40 --> 08:30
            //if (t.Minute > baseMinute)
            //{
            //    rptDate = rptDate.AddHours(baseHours);
            //}

            return rptDate;
        }

        private DateTime PlanStartTime
        {
            get
            {
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
            }
        }

        #region Internal Class : ResultItem

        internal class ResultItem
        {
            public string LineID { get; private set; }
            public string ProcessID { get; private set; }
            public string ProductID { get; private set; }
            public string StepID { get; private set; }
            public double StepSeq { get; private set; }
            public string Category { get; private set; }
            public string TimeInfo { get; set; }
            public string TotalQty { get; set; }

            public ResultItem(string lineID, string processID, string productID, string stepID, double stepSeq, string categry, string timeInfo)
            {
                this.LineID = lineID;
                this.ProcessID = processID;
                this.ProductID = productID;
                this.StepID = stepID;
                this.StepSeq = stepSeq;
                this.Category = categry;
                this.TimeInfo = timeInfo;
            }

            public ResultItem(string lineID, string processID, string productID, string stepID, double stepSeq, string category, string timeInfo, string totalQty)
            {
                this.LineID = lineID;
                this.ProcessID = processID;
                this.ProductID = productID;
                this.StepID = stepID;
                this.StepSeq = stepSeq;
                this.Category = category;
                this.TimeInfo = timeInfo;
                this.TotalQty = totalQty;
            }

            public ResultItem(string processID, string lineID)
            {
                this.ProcessID = processID;
                this.LineID = lineID;
            }

            public void UpdateQty(string totalQty)
            {
                double total = Convert.ToInt32(this.TotalQty) + Convert.ToInt32(totalQty);
                this.TotalQty = Convert.ToString(total);
            }
        }

        #endregion

        public TargetMoveAnalysisView()
        {
            InitializeComponent();
        }

        public TargetMoveAnalysisView(IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            InitializeComponent();
        }

        public TargetMoveAnalysisView(IExperimentResultItem result)
        {
            InitializeComponent();
            LoadDocument();
            this.Result = result;
            
            this.ribbonControl1.Hide();
            this.dockPanel2.Hide();
            this.hideContainerLeft.Hide();            
            this.setTree = false;
            SetProductEndStep();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetMainPivotGrid(this.pivotGridControl1);

            this.AddExtendPivotGridMenu(this.pivotGridControl1);

            SetControls();
        }

        protected override void Query()
        {
            if (this.setTree)
                SetTreeView();

            var dt = GetData();
            BindData(dt);
        }

        public void Query(List<string> productID, DateTime startTime, int days, bool cumulative)
        {
            this.CheckStacked.Checked = cumulative;
            this.editDateTime.EditValue = startTime;
            this.editDateSpin.EditValue = days;
            this.editProdId.EditValue = string.Join(",", productID);
            this._productList = productID;

            Query();

            this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
        }

        private void SetTreeView()
        {
            this.treeList1.BeginInit();
            this.treeList1.BeginUpdate();

            this.treeList1.ClearNodes();
            this.treeList1.Columns.Clear();
            if (this._productList != null && this._productList.Count > 0)
                this._productList.Clear();

            if (this.dockPanel2.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Hidden)
                this.dockPanel2.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;

            TreeListColumn column = this.treeList1.Columns.Add();
            column.FieldName = "PRODUCT";
            column.Caption = "PRODUCT";
            column.Visible = true;

            var routeList = MyHelper.DATASVC.GetEntityData<PRODUCT_ROUTE>(this.Result);

            var productTable = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result);
            List<object> productList = new List<object>();

            foreach (var proDt in productTable)
            {
                if (!productList.Contains(proDt.PRODUCT_ID))
                    productList.Add(proDt.PRODUCT_ID);
            }

            productList.Sort();
            foreach (var prod in productList)
                this.treeList1.AppendNode(new object[] { prod }, null);

            foreach (var route in routeList.OrderBy(r => r.FROM_PRODUCT_ID))
            {
                TreeListNode parentNodes = null;
                TreeListNode childNodes = null;
                if (this.treeList1.Nodes.Count > 0)
                {
                    parentNodes = this.treeList1.GetNodeList().FirstOrDefault(n => n.GetDisplayText("PRODUCT").Equals(route.TO_PRODUCT_ID));
                    childNodes = this.treeList1.GetNodeList().FirstOrDefault(n => n.GetDisplayText("PRODUCT").Equals(route.FROM_PRODUCT_ID));
                }

                if (parentNodes == null)
                    parentNodes = this.treeList1.AppendNode(new object[] { route.TO_PRODUCT_ID }, null);

                if (childNodes == null)
                    childNodes = this.treeList1.AppendNode(new object[] { route.FROM_PRODUCT_ID }, parentNodes);
                else
                    this.treeList1.MoveNode(childNodes, parentNodes);
            }

            if (this.TargetProductID.ToUpper().Trim() != "ALL")
            {
                this.dockPanel2.ShowSliding();

                TreeListNode searchNode = this.treeList1.GetNodeList().FirstOrDefault(n => n.GetDisplayText("PRODUCT").Equals(this.TargetProductID));
           
                if (searchNode != null)
                {
                    this.treeList1.MoveNode(searchNode, null);
                    List<TreeListNode> selectedNodes = new List<TreeListNode>();
                    GetChildNodes(searchNode, selectedNodes, false);

                    this.treeList1.GetNodeList().ForEach(node =>
                    {
                        if (selectedNodes.Contains(node) == false)
                            node.Remove();
                        else
                            node.ExpandAll();
                    });

                    this._productList = selectedNodes.Select(item => item.GetDisplayText("PRODUCT")).ToList();
                }
                else
                    this._productList.Add(this.TargetProductID);
            }

            this.treeList1.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeView;
            this.treeList1.OptionsBehavior.Editable = false;
            this.treeList1.OptionsView.ShowColumns = true;
            this.treeList1.OptionsView.ShowFilterPanelMode = DevExpress.XtraTreeList.ShowFilterPanelMode.Never;

            this.treeList1.EndUpdate();
            this.treeList1.EndInit();

            //_prodIndexs = new Dictionary<string, double>();
            //this.SetProductSortIndex(this.treeList1.Nodes);
        }

        private void SetControls()
        {
            SetControl_ProductID(this.editProdId);

            SetControl_DemandID(this.editDemandID);

            ShowTotal(this.pivotGridControl1);

            this.radioPlanDate.EditValue = "D";
            this.radioInOut.EditValue = "IN";

            int baseMinute = ShopCalendar.StartTime.Minutes; //timespan

            this.editDateTime.EditValue = GetRptDate_1Hour(this.PlanStartTime, baseMinute);
            this.editDateSpin.EditValue = MyHelper.DATASVC.GetPlanPeriod(this.Result);

            this.treeList1.OptionsSelection.MultiSelect = true;
            this.treeList1.OptionsSelection.MultiSelectMode = DevExpress.XtraTreeList.TreeListMultiSelectMode.CellSelect;

            SetChart();

            GetStepTable();
        }

        public PivotGridField GetColumnField()
        {
            return this.pivotGridControl1.Fields.GetFieldByName("field_TARGET_DATE");
        }

        private void SetControl_ProductID(BarEditItem control)
        {
            control.BeginUpdate();

            var cbEdit = control.Edit as RepositoryItemComboBox;
            if (cbEdit == null)
                return;

            cbEdit.Items.Clear();

            List<object> list = GetList_ProductID();

            cbEdit.Items.AddRange(list);

            if (!cbEdit.Items.Contains(control.EditValue))
            {
                if (cbEdit.Items.Count > 0)
                    control.EditValue = cbEdit.Items[0];
                else
                    control.EditValue = null;
            }

            control.EndUpdate();
            control.Refresh();
        }


        private void SetControl_DemandID(BarEditItem control)
        {
            if (control == null)
                return;

            control.BeginUpdate();
            try
            {
                var cbEdit = control.Edit as RepositoryItemCheckedComboBoxEdit;
                if (cbEdit == null)
                    return;

                cbEdit.Items.Clear();
                List<object> list = GetList_DemandID();
                list.Sort();
                list.ForEach(item => cbEdit.Items.Add(item));

                cbEdit.Items.ToList<CheckedListBoxItem>().ForEach(item => item.CheckState = CheckState.Checked);
                control.EditValue = cbEdit.GetCheckedItems();
            }
            catch
            {
                control.EditValue = null;
            }
            finally
            {
                control.EndUpdate();
                control.Refresh();
            }
        }


        private List<object> GetList_ProductID()
        {
            List<object> productIdList = new List<object>();

            var demand = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result);
            productIdList.Add("ALL");
            foreach (var item in demand)
            {
                string value = item.PRODUCT_ID;

                if (string.IsNullOrEmpty(value) == false && productIdList.Contains(item.PRODUCT_ID) == false)
                    productIdList.Add(value);
            }

            productIdList.Sort();
            return productIdList;
        }

        private List<object> GetList_DemandID()
        {
            List<object> demandIdList = new List<object>();

            var demand = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result);

            foreach (var item in demand)
            {
              
                if (this.TargetProductID != "ALL" && item.PRODUCT_ID != this.TargetProductID)
                    continue;

                string value = item.DEMAND_ID;

                if (string.IsNullOrEmpty(value) == false && demandIdList.Contains(item.DEMAND_ID) == false)
                    demandIdList.Add(value);
            }

            demandIdList.Sort();
            return demandIdList;
        }

        private void SetChart()
        {
            this.chartControl.Hide();
            this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            //List<int> rows = new List<int>();
            //var count = pivotGridControl1.Cells.RowCount;
            //for (int i = 0; i < count; i++)
            //{
            //    rows.Add(i);
            //}
            //_allRowsList = rows;
            //FillChart(pivotGridControl1, _allRowsList);
        }

        new private XtraPivotGridHelper.DataViewTable GetData()
        {
            SetDateRanges();

            XtraPivotGridHelper.DataViewTable table = CreateDataViewSchema();
            FillData(table);

            return table;
        }

        private void FillData(XtraPivotGridHelper.DataViewTable dt)
        {
            LineProcessDic();

            var items = LoadData();

            var stepData = items.Values;
            Dictionary<string, ResultItem> dic = new Dictionary<string, ResultItem>();

			_stepIndexs = new Dictionary<string, double>();

			var table = dt.DataTable;
         
            Dictionary<string, int> qtyDic = new Dictionary<string, int>();
            _disPlayList = new List<string>();
            var sortStepData = stepData.OrderBy(t => t.TimeInfo);

            foreach (var item in sortStepData)
            {
                string lineID = item.LineID;

                if (item.LineID == null)
                    lineID = "-";
                string displayKey = /*item.LineID + */ item.ProcessID + item.ProductID + item.StepID + item.Category + item.TimeInfo;

                if (item.ProductID == null)
                    continue;

                if (item.TotalQty == null)
                {
                    if (!_disPlayList.Contains(displayKey))
                        _disPlayList.Add(displayKey);
                }

                if (this._productList != null && this._productList.Count > 0 && this._productList.Contains(item.ProductID) == false)
                    continue;

                int stackedQty = Convert.ToInt32(item.TotalQty);

                if (item.Category != "WIP" && Stacked == true)
                {
                    string key = /*item.LineID +*/ item.ProcessID + item.ProductID + item.StepID + item.StepSeq + item.Category;

                    if (qtyDic.ContainsKey(key) == false)
                        qtyDic.Add(key, stackedQty);
                    else
                    {
                        qtyDic[key] += stackedQty;
                        stackedQty = qtyDic[key];
                    }
                }

                table.Rows.Add(
                    //lineID,
                    item.ProcessID,
                    item.ProductID ?? string.Empty,
                    item.StepID ?? string.Empty,
                    item.Category,
                    item.TimeInfo,
                    stackedQty
                );

				string stepKey = item.StepID;
				_stepIndexs[stepKey] = item.StepSeq;
			}
  
        }

        private void GetStepTable()
        {
            _stepTarget = MyHelper.DATASVC.GetEntityData<STEP_TARGET>(this.Result);
            _stepMove = MyHelper.DATASVC.GetEntityData<STEP_MOVE>(this.Result);
            _stepWip = MyHelper.DATASVC.GetEntityData<STEP_WIP>(Result);

        }

        //private ResultItem GetSampleItem(string areaID, Dictionary<string,ResultItem> item)
        //{
        //    var sw = item.Values;
        //    if (sw == null)
        //        return null;

        //    var sample = sw.Where(t => t.AreaID == areaID).FirstOrDefault();
        //    if (sample != null)
        //        return sample;

        //    return null;
        //}

        private void BindData(XtraPivotGridHelper.DataViewTable table)
        {
            this.pivotGridControl1.BeginInit();
            this.pivotGridControl1.BeginUpdate();

            this.pivotGridControl1.ClearPivotGridFields();
            this.pivotGridControl1.CreatePivotGridFields(table);

            this.pivotGridControl1.DataSource = table.DataTable;

            pivotGridControl1.CustomCellDisplayText += pivotGridControl1_CellDisplayText;
            
            pivotGridControl1.Fields["QTY"].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields["QTY"].CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.Fields["STEP"].SortMode = PivotSortMode.Custom;

            this.pivotGridControl1.Fields["CATEGORY"].SortMode = PivotSortMode.Custom;

            pivotGridControl1.CustomDrawCell += pivotGridControl1_CustomDrawCell;

            this.pivotGridControl1.EndUpdate();
            this.pivotGridControl1.EndInit();

            pivotGridControl1.BestFit();
            pivotGridControl1.BestFitColumnArea();
        }

        private string GetDateString(string value, bool withTime = true)
        {
            DateTime primary;

            if (value == null)
            {
                primary = DateTime.MinValue;
                return primary.ToString("yyyyMMddHHmm");
            }

            int start = 0;
            int num = 0;//중간 띄어쓰기 위치
            string tmp = value;
            while (tmp.IndexOf(" ") > 0)
            {
                num = tmp.IndexOf(" ");
                string tmp1 = tmp.Substring(0, num);
                start = num + 1;
                tmp1 += tmp.Substring(num + 1);
                tmp = tmp1;
            }

            value = tmp; //MyHelper.STRING.Trim(value);
            int length = value.Length;

            if (length < 8)
            {
                primary = DateTime.MinValue;
                return primary.ToString("yyyyMMddHHmm");
            }

            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            try
            {
                year = int.Parse(value.Substring(0, 4));
                month = int.Parse(value.Substring(4, 2));
                day = int.Parse(value.Substring(6, 2));
                if (withTime)
                {
                    int t = 8;

                    if (length >= 10)
                    {
                        if (value[8] == ' ')
                            t++;

                        hour = int.Parse(value.Substring(t + 0, 2));
                    }

                    if (length >= 12)
                    {
                        if (value[8] == ' ')
                            t++;

                        minute = int.Parse(value.Substring(t + 2, 2));
                    }

                    if (length >= 14)
                    {
                        second = int.Parse(value.Substring(t + 4, 2));
                    }
                }
            }
            catch
            {
            }
            primary = new DateTime(year, month, day, hour, minute, second);

            if (this.radioPlanDate.EditValue.Equals("D"))
                return primary.ToString("yyyyMMdd");
            else
                return primary.ToString("yyyyMMddHHmm");
        }

        private void SetDateRanges()
        {
            _dayHourList = new SortedSet<string>();

            float interval = 1f;
            if (this.radioPlanDate.EditValue.Equals("D"))
                interval = 24f;

            DateTime st = GetPlanDate(this.QueryStartTime);
            if (st != this.QueryStartTime)
                this.editDateTime.EditValue = st;
            DateTime et = this.QueryEndTime;

            int baseMinute = ShopCalendar.StartTime.Minutes;
            DateTime baseT = GetRptDate_1Hour(st, baseMinute); //test MyHelper.DateHelper

            for (DateTime t = baseT; t < et; t = t.AddHours(interval))
            {
                string str = t.ToString("yyyyMMddHHmm"); //GetDateString(t);

                _dayHourList.Add(str);
            }
        }

        private void DateTimeFormatChange(DefaultBoolean calendar, string format)
        {
            this.repoDateTime.CalendarTimeEditing = calendar;
            this.repoDateTime.DisplayFormat.FormatString = format;
            this.repoDateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.repoDateTime.EditFormat.FormatString = format;
            this.repoDateTime.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.repoDateTime.Mask.EditMask = format;
        }

        private void LineProcessDic()
        {
            _processDic = new Dictionary<string, ResultItem>();
            _lineDic = new Dictionary<string, string>();

            var eqpArr = MyHelper.DATASVC.GetEntityData<EQP_ARRANGE>(this.Result);
            var eqp = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(this.Result);
            foreach(var ep in eqp)
            {
                string key = ep.EQP_ID;
                string line;
                if (_lineDic.TryGetValue(key, out line) == false)
                    _lineDic.Add(key, ep.LINE_ID);
            }

            foreach (EQP_ARRANGE ea in eqpArr)
            {
                string k = ea.PRODUCT_ID + ea.STEP_ID;
                ResultItem lineProInfo;
                if (_processDic.TryGetValue(k, out lineProInfo) == false)
                {
                    var processID = ea.PROCESS_ID;
                    var eqpID = ea.EQP_ID;
                    string lineID;
                    _lineDic.TryGetValue(eqpID, out lineID);
                    _processDic.Add(k, new ResultItem(processID, lineID));
                }
            }
        }

        private void makeCategoryDic()
        {
            _categoryIndexs = new Dictionary<string, double>();
            _categoryIndexs.Add("DEMAND", 0);
            _categoryIndexs.Add("PLAN", 1);
            //_categoryIndexs.Add("WIP", 2);
        }

        private Dictionary<string, ResultItem> LoadData()
        {
            Dictionary<string, ResultItem> items = new Dictionary<string, ResultItem>();
          
            makeCategoryDic();

            bool isFirst = true;

            var stdStepDic = GetStepSeqDic();
            string[] category = new string[] { "DEMAND", "PLAN" /*, "WIP" */};
            
            var stepTarget = MyHelper.DATASVC.GetEntityData<STEP_TARGET>(this.Result).Where(t => t.TARGET_DATE >= this.QueryStartTime && t.TARGET_DATE < this.QueryEndTime);

            foreach (STEP_TARGET target in stepTarget)
            {
                if (!this.TargetDemandID.Contains(target.MO_DEMAND_ID))
                    continue;

                if (CheckEndStep(target.MO_DEMAND_ID, target.STEP_ID) == false)
                    continue;

                var std_info = stdStepDic.TryGetValue(target.STEP_ID, out double re);
                var std_seq = std_info == true ? re : 0;

                var qty = Convert.ToString(target.IN_QTY);
                if (radioInOut.EditValue.Equals("OUT"))
                    qty = Convert.ToString(target.OUT_QTY);

                string infoKey = target.PRODUCT_ID + target.STEP_ID;

                if (!_processDic.ContainsKey(infoKey))
                    continue;

                var lineId = _processDic[infoKey].LineID;
                var processId = _processDic[infoKey].ProcessID;

                if (isFirst)
                {
                    foreach (string dayHour in _dayHourList)
                    {
                        string dateString = GetDateString(dayHour);

                        foreach (string cate in category)
                        {
                            string k = lineId + processId + target.PRODUCT_ID + target.STEP_ID + cate + dateString;

                            ResultItem padding;
                            if (items.TryGetValue(k, out padding) == false)
                            {
                                padding = new ResultItem(lineId, processId, target.PRODUCT_ID, target.STEP_ID, std_seq, cate, dateString);

                                items.Add(k, padding);
                            }
                        }
                    }
                    //isFirst = false;
                }

                DateTime DateChange = GetRptDate_1Hour(target.TARGET_DATE, ShopCalendar.StartTime.Minutes);
                DateTime planDate = GetPlanDate(DateChange);
                string dateStr = GetDateString(planDate.ToString("yyyyMMddHHmm")); //GetDateString(planDate);
                string staget = target.PRODUCT_ID + target.STEP_ID + dateStr;

                foreach (string cate in category)
                {
                    string key = lineId + processId + target.PRODUCT_ID + target.STEP_ID + cate + dateStr;

                    ResultItem ri = null;
                    if (items.TryGetValue(key, out ri) == false)
                        items.Add(key, ri = new ResultItem(lineId, processId, target.PRODUCT_ID, target.STEP_ID, std_seq, cate, dateStr));
                    if (cate == "DEMAND")
                        ri.UpdateQty((string)qty);
                }
            }

            var stepMove = MyHelper.DATASVC.GetEntityData<STEP_MOVE>(this.Result).Where(t => t.PLAN_DATE >= this.QueryStartTime && t.PLAN_DATE < this.QueryEndTime);

            foreach (STEP_MOVE move in stepMove)
            {
                if (!this.TargetDemandID.Contains(move.DEMAND_ID))
                    continue;

                if (CheckEndStep(move.DEMAND_ID, move.STEP_ID) == false)
                    continue;

                var std_info = stdStepDic.TryGetValue(move.STEP_ID, out double re);
                var std_seq = std_info == true ? re : 0;

                double stepSeq = std_seq;

                var qty = Convert.ToString(move.IN_QTY);
                if (radioInOut.EditValue.Equals("OUT"))
                    qty = Convert.ToString(move.OUT_QTY);

                if (isFirst)
                {
                    foreach (string dayHour in _dayHourList)
                    {
                        string dateString = GetDateString(dayHour);

                        foreach (string cate in category)
                        {
                            string k = move.LINE_ID + move.PROCESS_ID + move.PRODUCT_ID + move.STEP_ID + cate + dateString;

                            ResultItem padding;
                            if (items.TryGetValue(k, out padding) == false)
                            {
                                padding = new ResultItem(move.LINE_ID, move.PROCESS_ID, move.PRODUCT_ID, move.STEP_ID, stepSeq, cate, dateString);

                                items.Add(k, padding);
                            }
                        }
                    }
                    //isFirst = false;
                }

                DateTime DateChange = GetRptDate_1Hour(move.PLAN_DATE, ShopCalendar.StartTime.Minutes);
                DateTime planDate = GetPlanDate(DateChange);
                string dateStr = GetDateString(planDate.ToString("yyyyMMddHHmm")); //GetDateString(planDate);
                foreach (string cate in category)
                {
                    string key = move.LINE_ID + move.PROCESS_ID + move.PRODUCT_ID + move.STEP_ID + cate + dateStr;

                    ResultItem ri = null;
                    if (items.TryGetValue(key, out ri) == false)
                        items.Add(key, ri = new ResultItem(move.LINE_ID, move.PROCESS_ID, move.PRODUCT_ID, move.STEP_ID, stepSeq, cate, dateStr));
                    if (cate == "PLAN")
                        ri.UpdateQty((string)qty);
                }
            }

            var stepWip = MyHelper.DATASVC.GetEntityData<STEP_WIP>(this.Result).Where(t => t.TARGET_DATE >= this.QueryStartTime && t.TARGET_DATE < this.QueryEndTime);

            foreach (STEP_WIP wip in stepWip)
            {
                if (!this.TargetDemandID.Contains(wip.DEMAND_ID))
                    continue;

                if (CheckEndStep(wip.DEMAND_ID, wip.STEP_ID) == false)
                    continue;

                var std_info = stdStepDic.TryGetValue(wip.STEP_ID, out double re);
                var std_seq = std_info == true ? re : 0;

                double stepSeq = std_seq;

                var qty = Convert.ToString(wip.WAIT_UNIT_QTY);
                if (radioInOut.EditValue.Equals("OUT"))
                    qty = Convert.ToString(wip.WAIT_UNIT_QTY + wip.RUN_UNIT_QTY);

                if (isFirst)
                {
                    foreach (string dayHour in _dayHourList)
                    {
                        string dateString = GetDateString(dayHour);

                        foreach (string cate in category)
                        {
                            string k = wip.LINE_ID + wip.PROCESS_ID + wip.PRODUCT_ID + wip.STEP_ID + cate + dateString;

                            ResultItem padding;
                            if (items.TryGetValue(k, out padding) == false)
                            {
                                padding = new ResultItem(wip.LINE_ID, wip.PROCESS_ID, wip.PRODUCT_ID, wip.STEP_ID, stepSeq, cate, dateString);

                                items.Add(k, padding);
                            }
                        }
                    }
                }

                DateTime DateChange = GetRptDate_1Hour(wip.TARGET_DATE, ShopCalendar.StartTime.Minutes);
                DateTime planDate = GetPlanDate(DateChange);
                string dateStr = GetDateString(planDate.ToString("yyyyMMddHHmm")); //GetDateString(planDate);
                foreach (string cate in category)
                {
                    string key = wip.LINE_ID + wip.PROCESS_ID + wip.PRODUCT_ID + wip.STEP_ID + cate + dateStr;

                    ResultItem ri = null;
                    if (items.TryGetValue(key, out ri) == false)
                        items.Add(key, ri = new ResultItem(wip.LINE_ID, wip.PROCESS_ID, wip.PRODUCT_ID, wip.STEP_ID, stepSeq, cate, dateStr));
                    if (cate == "WIP")
                        ri.UpdateQty((string)qty);
                }
            }
            return items;
        }

        private bool CheckEndStep(string demandId, string stepId)
        {
            if (this.demandEndSteps.Count == 0)
                return true;
            string endStep;
            if (this.demandEndSteps.TryGetValue(demandId, out endStep) == false)
                return true;

            if (endStep == stepId)
                return true;

            return false;
        }

        private void SetProductEndStep()
        {
            Dictionary<string, string> productProcessId = new Dictionary<string, string>();

            var products = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result);

            foreach (var product in products)
                productProcessId[product.PRODUCT_ID] = product.PROCESS_ID;

            var steps = MyHelper.DATASVC.GetEntityData<STEP_ROUTE>(this.Result);
            Dictionary<string, List<STEP_ROUTE>> processSteps = new Dictionary<string, List<STEP_ROUTE>>();

            foreach (var step in steps)
            {
                if (processSteps.ContainsKey(step.PROCESS_ID) == false)
                    processSteps[step.PROCESS_ID] = new List<STEP_ROUTE>();

                processSteps[step.PROCESS_ID].Add(step);
            }

            var demands = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result);

            foreach (var demand in demands)
            {
                string processId;
                if (productProcessId.TryGetValue(demand.PRODUCT_ID, out processId) == false)
                    continue;

                if (processSteps.ContainsKey(processId) == false)
                    continue;

                this.demandEndSteps[demand.DEMAND_ID] = processSteps[processId].OrderByDescending(x => x.STEP_SEQ).FirstOrDefault().STEP_ID;
            }
        }

        private Dictionary<string, double> GetStepSeqDic()
        {
            var stdStep = MyHelper.DATASVC.GetEntityData<STD_STEP_INFO>(this.Result);

            Dictionary<string, double> list = new Dictionary<string, double>();

            var finds = stdStep.OrderBy(t => t.STEP_SEQ);
            if (finds == null || finds.Count() == 0)
                return list;

            foreach (var item in finds)
            {
                string key = item.STD_STEP_ID;

                if (!list.ContainsKey(key))
                    list.Add(key, item.STEP_SEQ);
            }

            return list;
        }

        private XtraPivotGridHelper.DataViewTable CreateDataViewSchema()
        {
            XtraPivotGridHelper.DataViewTable dt = new XtraPivotGridHelper.DataViewTable();

            //dt.AddColumn("LINE_ID", "LINE ID", typeof(string), PivotArea.RowArea, null, null);            
            dt.AddColumn("PROCESS_ID", "PROCESS ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("PRODUCT_ID", "PRODUCT ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("STEP", "STEP", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("CATEGORY", "CATEGORY", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("TARGET_DATE", "DEMAND DATE", typeof(string), PivotArea.ColumnArea, null, null);
            dt.AddColumn("QTY", "QTY", typeof(float), PivotArea.DataArea, null, null);

            dt.AddDataTablePrimaryKey(
                    new DataColumn[]
                    {
                        //dt.Columns["LINE_ID"],
                        dt.Columns["PROCESS_ID"],
                        dt.Columns["PRODUCT_ID"],
                        dt.Columns["STEP"],
                        dt.Columns["CATEGORY"],
                        dt.Columns["TARGET_DATE"]
                    }
                );

            return dt;
        }

        private string[] GetDataAreaList(PivotGridControl pivotGrid)
        {
            List<string> list = new List<string>();

            var dataAreaList = pivotGrid.GetFieldsByArea(PivotArea.DataArea);
            foreach (var field in dataAreaList)
            {
                if (field.DataType == typeof(string))
                    continue;

                list.Add(field.FieldName);

                //ValueDataMembers에 복수개을 설정해도 첫번째 Field만 기록됨(그래서 한개만 추가 하도록 함)
            }

            return list.ToArray();
        }

        private List<string> GetColNameList(PivotGridControl pivotGrid)
        {
            List<string> list = new List<string>();

            List<PivotGridField> rowList = pivotGrid.GetFieldsByArea(PivotArea.RowArea);
            for (int i = 0; i < rowList.Count; i++)
            {
                string colName = pivotGrid.GetFieldByArea(PivotArea.RowArea, i).FieldName;
                list.Add(colName);
            }

            return list;
        }

        private List<string> GetRowValueList(PivotGridControl pivotGrid, int rowindex)
        {
            List<string> list = new List<string>();

            List<PivotGridField> rowList = pivotGrid.GetFieldsByArea(PivotArea.RowArea);

            for (int i = 0; i < rowList.Count; i++)
            {
                string value = pivotGrid.GetFieldValue(pivotGrid.GetFieldByArea(PivotArea.RowArea, i), rowindex) as string;
                list.Add(value ?? string.Empty);
            }

            return list;
        }

        private void ShowTotal(PivotGridControl pivot, bool isCheck = false)
        {
            pivot.OptionsView.ShowRowTotals = false;
            pivot.OptionsView.ShowRowGrandTotals = false;
            pivot.OptionsView.ShowColumnTotals = isCheck;
            pivot.OptionsView.ShowColumnGrandTotals = isCheck;
        }

        private DataView SummaryData(DataTable dt, List<string> colNameList, List<string> valueList)
        {
            DataTable result = CreateSummaryTable(colNameList);
            FillRowZero(result, valueList);

            int keyColCount = colNameList.Count;

            foreach (DataRow row in dt.Rows)
            {
                object[] pks = new object[keyColCount + 1];

                for (int i = 0; i < keyColCount; i++)
                {
                    string colName = colNameList[i];
                    pks[i] = row.GetString(colName);
                }

                string dayHour = row.GetString("TARGET_DATE");
                pks[keyColCount] = dayHour;

                DataRow findRow = result.Rows.Find(pks);
                if (findRow == null)
                    continue;

                int inQty = row.GetInt32("QTY");
                if (inQty > 0)
                    findRow["QTY"] = findRow.GetInt32("QTY") + inQty;

            }

            return new DataView(result, string.Empty, "TARGET_DATE", DataViewRowState.CurrentRows);
        }

        private DataTable CreateSummaryTable(List<string> colNameList)
        {
            DataTable result = new DataTable();

            List<DataColumn> pkList = new List<DataColumn>();
            foreach (var colName in colNameList)
            {
                var col = result.Columns.Add(colName, typeof(string));
                pkList.Add(col);
            }

            var tcol = result.Columns.Add("TARGET_DATE", typeof(string));
            pkList.Add(tcol);

            List<string> qtyColList = new List<string> { "QTY" };
            foreach (var qtyCol in qtyColList)
            {
                if (!result.Columns.Contains(qtyCol))
                {
                    result.Columns.Add(qtyCol, typeof(int));
                }
            }

            result.PrimaryKey = pkList.ToArray();

            return result;
        }

        private void FillRowZero(DataTable result, List<string> valueList)
        {
            int count = valueList.Count;
            foreach (var dayHour in _dayHourList)
            {
                DataRow row = result.NewRow();

                for (int i = 0; i < count; i++)
                    row[i] = valueList[i];

                row["TARGET_DATE"] = dayHour;
                row["QTY"] = 0;

                if (this.radioPlanDate.EditValue.Equals("D"))
                    row["TARGET_DATE"] = GetDateString(dayHour);

                result.Rows.Add(row);
            }
        }

        private double ConvertLayerIndex(string stepId)
        {
            if (stepId == null)
                return 999;

            double seq = 0;
            if (_stepIndexs.TryGetValue(stepId, out seq))
                return seq;

            return 999;
        }

        private double ConvertCategoryIndex(string category)
        {
            if (category == null)
                return 999;
            double seq = 0;
            if (_categoryIndexs.TryGetValue(category, out seq))
                return seq;
            return 999;
        }

        private void FillChart(PivotGridControl pivotGrid, List<int> rows)
        {
            XYDiagram xyDiagram = (XYDiagram)chartControl.Diagram;
            if (xyDiagram != null)
            {
                xyDiagram.AxisX.Label.Angle = 45;
                xyDiagram.AxisX.Label.ResolveOverlappingOptions.AllowHide = false;
                xyDiagram.AxisX.NumericScaleOptions.AutoGrid = true;
                xyDiagram.EnableAxisXScrolling = false;
                xyDiagram.EnableAxisYScrolling = true;
                xyDiagram.AxisX.Label.Font = new System.Drawing.Font("Tahoma", 7F);
            }

            chartControl.Series.Clear();

            DataTable dt = (DataTable)pivotGrid.DataSource;
            var colNameList = GetColNameList(pivotGrid);

            var dataAreaList = GetDataAreaList(pivotGrid);

            //bool onlyOne = rows.Count == 1;
            
            foreach (int rowIdx in rows)
            {
                var valueList = GetRowValueList(pivotGrid, rowIdx);
                DataView dv = SummaryData(dt, colNameList, valueList);

                string seriesName = Concat('@', valueList.ToArray()); //MyHelper.STRING.ConcatKey
                if (dataAreaList.Length > 1)
                {
                    foreach (string a in dataAreaList)
                    {
                        AddSeries(seriesName + "@" + a, ViewType.Line, new string[] { a }, dv);
                    }
                }
                else
                    AddSeries(seriesName, ViewType.Line, dataAreaList, dv);
            }
        }

        private void AddSeries(string seriesName, ViewType viewType, string[] dataAreaList, DataView dv)
        {
            int index = chartControl.Series.Add(seriesName, viewType);

            chartControl.Series[index].ArgumentDataMember = "TARGET_DATE";

            chartControl.Series[index].ValueDataMembers.AddRange(dataAreaList);
            //chartControl.Series[index].Label.Visible = false;            

            chartControl.Series[index].DataSource = dv;
            chartControl.Series[index].LegendText = string.IsNullOrEmpty(seriesName) ? "Total" : seriesName;
        }

        private List<int> GetSelectionList()
        {
            PivotGridCells cells = pivotGridControl1.Cells;
            DevExpress.XtraPivotGrid.Selection.IMultipleSelection selList = cells.MultiSelection;

            List<int> rows = new List<int>();
            foreach (Point pt in selList.SelectedCells)
            {
                if (rows.Contains(pt.Y) == false)
                    rows.Add(pt.Y);
            }
            return rows;
        }

        private static string Concat<TCh>(TCh separator, params object[] args)
        {
            if (args == null || args.Length == 0)
                return string.Empty;

            string first = (args[0] != null) ? args[0].ToString() : "";
            if (args.Length == 1)
                return first;
            if (args.Length == 2)
                return first + separator + args[1];
            if (args.Length == 3)
                return first + separator + args[1] + separator + args[2];
            if (args.Length == 4)
                return first + separator + args[1] + separator + args[2] + separator + args[3];

            StringBuilder sb = new StringBuilder(first);
            for (int i = 1; i < args.Length; i++)
            {
                sb.Append(separator);
                sb.Append(args[i]);
            }

            return sb.ToString();
        }

        private void pivotGridControl_CellSelectionChanged(object sender, EventArgs e)
        {
            if(firstChart == true)
            {
                this.chartControl.Show();
                firstChart = false;
            }

            if (SyncChart == false)
                return;

            List<int> rows = GetSelectionList();

            if (rows.Count != 0)
                this.dockPanel1.Show();
            else
                this.dockPanel1.HideSliding();

            FillChart((PivotGridControl)sender, rows);
        }

        private void pivotGridControl1_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
        {
            if (e.Field.FieldName == "STEP_ID")
            {
                double s1 = ConvertLayerIndex(e.Value1 as string);
                double s2 = ConvertLayerIndex(e.Value2 as string);

                e.Result = s1.CompareTo(s2);

                e.Handled = true;
            }

            if (e.Field.FieldName == "CATEGORY")
            {
                double c1 = ConvertCategoryIndex(e.Value1 as string);
                double c2 = ConvertCategoryIndex(e.Value2 as string);
                if (c1 == c2)
                    return;
                e.Result = c1.CompareTo(c2);

                e.Handled = true;
            }
        }

        private void pivotGridControl1_CellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            var fieldValue = e.GetRowFields();
            string key = string.Empty;

            if (e.ColumnField != null)
            {
                var colValue = e.GetFieldValue(e.ColumnField);
                foreach (var value in fieldValue)
                {
                    key += this.pivotGridControl1.GetFieldValue(value, e.RowIndex);
                }
                key += colValue;
 
                if (Stacked == false && _disPlayList.Contains(key))
                    e.DisplayText = string.Empty;
            }
        }

        private void pivotGridControl1_CustomDrawCell(object sender, PivotCustomDrawCellEventArgs e)
        {
            if (e.RowField == null)
                return;

            int moveQty;
            int targetQty;
            if (Stacked == true && e.GetFieldValue(e.RowField) as string == "MOVE")
            {
                moveQty = Convert.ToInt32(this.pivotGridControl1.GetCellValue(e.ColumnIndex, e.RowIndex));
                targetQty = Convert.ToInt32(this.pivotGridControl1.GetCellValue(e.ColumnIndex, e.RowIndex - 1));
                if (moveQty < targetQty)
                    e.Appearance.ForeColor = Color.Red;
            }
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void checkSyncChart_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            List<int> row_list = new List<int>();
            if (SyncChart == true)
            {
                row_list = GetSelectionList();
            }
            FillChart(this.pivotGridControl1, row_list);
        }

        private void buttonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.PIVOTEXPORT.ExportToExcel(this.pivotGridControl1);
        }

        private void radioPlanDate_EditValueChanged(object sender, EventArgs e)
        {
            if (this.radioPlanDate.EditValue.Equals("D"))
                DateTimeFormatChange(DefaultBoolean.False, "yyyy-MM-dd");
            else
                DateTimeFormatChange(DefaultBoolean.True, "yyyy-MM-dd HH:mm:ss");
        }

        private void GetChildNodes(TreeListNode targetNode, List<TreeListNode> selectedNodes, bool checkExpand)
        {
            selectedNodes.Add(targetNode);
            if (checkExpand && targetNode.Expanded == false)
                return;

            var en = targetNode.Nodes.GetEnumerator();
            TreeListNode child;
            while (en.MoveNext())
            {
                child = (TreeListNode)en.Current;
                selectedNodes.Add(child);

                if (child.HasChildren)
                    GetChildNodes(child, selectedNodes, checkExpand);
            }
        }

        private void treeList1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var hitInfo = this.treeList1.CalcHitInfo(e.Location);
            if (hitInfo == null || hitInfo.Node == null)
                this.treeList1.Selection.Clear();
        }

        private void treeList1_SelectionChanged(object sender, EventArgs e)
        {
            if (this.treeList1.Selection == null || this.treeList1.Selection.Count < 1)
                return;

            List<string> filterStringSet = new List<string>();

            foreach (var node in this.treeList1.Selection)
            {
                List<TreeListNode> childNodes = new List<TreeListNode>();
                GetChildNodes(node, childNodes, true);

                filterStringSet.Add(string.Join(",", childNodes.Select(item => $"'{item.GetDisplayText("PRODUCT")}'").ToArray()));
            }

            string filterString = string.Join(",", filterStringSet);

            this.pivotGridControl1.ActiveFilterString = $"[field_PRODUCT_ID] In ({filterString})";
        }

        private void editProdId_EditValueChanged(object sender, EventArgs e)
        {
            SetControl_DemandID(this.editDemandID);
        }

		private void treeList1_AfterCollapse(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            treeList1_SelectionChanged(sender, null);
        }

		private void treeList1_AfterExpand(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
		{
            treeList1_SelectionChanged(sender, null);
		}
	}
}
