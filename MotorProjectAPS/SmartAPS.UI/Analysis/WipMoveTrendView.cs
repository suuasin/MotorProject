using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.Projects;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SmartAPS.UI.Analysis
{
    public partial class WipMoveTrendView : MyXtraPivotGridTemplate
    {
        private SortedSet<string> _dayHourList;
        private Dictionary<string, double> _stepIndexs;
        private List<int> _allRowsList;
        private List<string> _productList;
        private bool firstChart = true;

        private string TargetProductID
        {
            get
            {
                return this.editProdId.EditValue as string;
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
            public string ProductID { get; private set; }
            public string ProcessID { get; private set; }
            public string StepID { get; private set; }
            public double StepSeq { get; private set; }

            public string Category { get; private set; }
            public string TimeInfo { get; set; }

            public float TotalQty { get; private set; }

            public ResultItem(string lineID, string productID, string processID, string stepID, double stepSeq, string category, string timeInfo)
            {
                this.LineID = lineID;
                this.ProductID = productID;
                this.ProcessID = processID;
                this.StepID = stepID;
                this.StepSeq = stepSeq;
                this.Category = category;
                this.TimeInfo = timeInfo;
            }

            public ResultItem(string lineID, string productID, string processID, string stepID, double stepSeq, string category, string timeInfo, int totalQty)
            {
                this.LineID = lineID;
                this.ProductID = productID;
                this.ProcessID = processID;
                this.StepID = stepID;
                this.StepSeq = stepSeq;
                this.Category = category;
                this.TimeInfo = timeInfo;
                this.TotalQty = totalQty;

            }

            //public void UpdateQty(int inQty, int outQty)
            //{
            //    this.InQty += inQty;
            //    this.OutQty += outQty;
            //}

            public void UpdateQty(float qty)
            {
                this.TotalQty += qty;
            }
        }

        #endregion

        public WipMoveTrendView()
        {
            InitializeComponent();
        }

        public WipMoveTrendView(IServiceProvider serviceProvider)
           : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            this.SetMainPivotGrid(this.pivotGridControl1);

            this.AddExtendPivotGridMenu(this.pivotGridControl1);

            base.LoadDocument();

            SetControls();
        }

        protected override void Query()
        {
            SetTreeView();
            var dt = GetData();
            BindData(dt);
        }

        private void SetTreeView()
        {
            this.treeList1.BeginInit();
            this.treeList1.BeginUpdate();

            this.treeList1.ClearNodes();
            this.treeList1.Columns.Clear();
            if (this._productList != null && this._productList.Count > 0)
                this._productList.Clear();

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
        }

        private void SetControls()
        {
            SetControl_ProductID(this.editProdId);

            ShowTotal(this.pivotGridControl1);

            this.radioPlanDate.EditValue = "D";
            this.radioInOut.EditValue = "S";

            int baseMinute = ShopCalendar.StartTime.Minutes; //timespan
            
            this.editDateTime.EditValue = GetRptDate_1Hour(this.PlanStartTime, baseMinute);
            this.editDateSpin.EditValue = MyHelper.DATASVC.GetPlanPeriod(this.Result);

            this.treeList1.OptionsSelection.MultiSelect = true;
            this.treeList1.OptionsSelection.MultiSelectMode = DevExpress.XtraTreeList.TreeListMultiSelectMode.CellSelect;

            SetChart();
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
            var items = LoadData();

            var stepMove = items.Values;

            _stepIndexs = new Dictionary<string, double>();

            var table = dt.DataTable;
            //var sample = GetSampleItem(TargetAreaID, items);

            //var query = from ps in stdStepList
            //            join p in stepMove
            //            on ps.SHOP_ID + ps.STEP_ID equals p.ShopID + p.StepID into temp
            //            from tp in temp.DefaultIfEmpty()
            //            select new
            //            {
            //                SHOP_ID = tp != null ? tp.ShopID : ps.SHOP_ID,
            //                STD_STEP_ID = tp != null ? tp.StepID : ps.STEP_ID,
            //                PROD_ID = tp != null ? tp.ProductID : (sample != null ? sample.ProductID : null),
            //                PROD_VER = tp != null ? tp.ProductVersion : (sample != null ? sample.ProductVersion : null),
            //                PRODUCTION_TYPE = tp != null ? tp.ProductionType :(sample != null ? sample.ProductionType : null),
            //                TIME_INFO = tp != null ? tp.TimeInfo : (sample != null ? sample.TimeInfo : null),
            //                EQP_ID = tp != null ? tp.EqpID : null,
            //                EQP_GROUP_ID = tp != null ? tp.EqpGroupID : string.Empty,
            //                IN_QTY = tp != null ? tp.InQty : 0,
            //                OUT_QTY = tp != null ? tp.OutQty : 0,
            //                STD_STEP_SEQ = tp != null ? tp.StepSeq : ps.STEP_SEQ,
            //            };

            foreach (var item in stepMove)
            {
                if (item.ProductID == null)
                    continue;

                if (this._productList != null && this._productList.Count > 0 && this._productList.Contains(item.ProductID) == false)
                    continue;

                table.Rows.Add(
                    item.LineID ?? string.Empty,
                    item.ProductID ?? string.Empty,
                    item.ProcessID ?? string.Empty,
                    item.StepID ?? string.Empty,
                    item.Category,
                    item.TimeInfo,
                    item.TotalQty
                );

                string stepKey = item.StepID;
                _stepIndexs[stepKey] = item.StepSeq;
            }
        }

        private void BindData(XtraPivotGridHelper.DataViewTable table)
        {
            this.pivotGridControl1.BeginInit();
            this.pivotGridControl1.BeginUpdate();

            this.pivotGridControl1.ClearPivotGridFields();
            this.pivotGridControl1.CreatePivotGridFields(table);

            this.pivotGridControl1.DataSource = table.DataTable;

            this.pivotGridControl1.Fields["STD_STEP"].SortMode = PivotSortMode.Custom;

            //pivotGridControl1.CustomCellDisplayText += pivotGridControl1_CellDisplayText;    

            pivotGridControl1.Fields["QTY"].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields["QTY"].CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.Fields["STD_STEP"].SortMode = PivotSortMode.Custom;

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

        private Dictionary<string, ResultItem> LoadData()
        {
            Dictionary<string, ResultItem> items = new Dictionary<string, ResultItem>();

            bool isFirst = true;


            var stdStepDic = GetStepSeqDic();
            string[] category = new string[] { "MOVE", "WIP" };
            var stepMove = MyHelper.DATASVC.GetEntityData<STEP_MOVE>(this.Result).Where(t => t.PLAN_DATE >= this.QueryStartTime && t.PLAN_DATE < this.QueryEndTime);

            foreach (STEP_MOVE move in stepMove)
            {
                var std_info = stdStepDic.TryGetValue(move.STEP_ID, out double re);
                var std_seq = std_info == true ? re : 0;

                double stepSeq = std_seq;
          
                float qty = move.IN_QTY;
                if (this.radioPlanDate.EditValue.Equals("E"))
                    qty = move.OUT_QTY;

                if (isFirst)
                {
                    foreach (string dayHour in _dayHourList)
                    {
                        string dateString = GetDateString(dayHour);

                        foreach (string cate in category)
                        {
                            string key = move.LINE_ID + move.PRODUCT_ID + move.PROCESS_ID + move.STEP_ID + cate + dateString;
                            ResultItem padding;

                            if (items.TryGetValue(key, out padding) == false)
                            {
                                padding = new ResultItem(move.LINE_ID, move.PRODUCT_ID, move.PROCESS_ID, move.STEP_ID, stepSeq, cate, dateString);
                                items.Add(key, padding);
                            }
                        }
                        isFirst = false;
                    }
                }

                DateTime DateChange = GetRptDate_1Hour(move.PLAN_DATE, ShopCalendar.StartTime.Minutes);
                DateTime planDate = GetPlanDate(DateChange);
                string dateStr = GetDateString(planDate.ToString("yyyyMMddHHmm")); //GetDateString(planDate);
                foreach (string cate in category)
                {
                    string mkey = move.LINE_ID + move.PRODUCT_ID + move.PROCESS_ID + move.STEP_ID + cate + dateStr;

                    ResultItem ri = null;

                    if (items.TryGetValue(mkey, out ri) == false)
                    {
                        items.Add(mkey, ri = new ResultItem(move.LINE_ID, move.PRODUCT_ID, move.PROCESS_ID, move.STEP_ID, stepSeq, cate, dateStr));
                    }

                    if(cate == "MOVE")
                        ri.UpdateQty(qty);
                }
            }

            var stepWip = MyHelper.DATASVC.GetEntityData<STEP_WIP>(this.Result).Where(t=>t.TARGET_DATE >= this.QueryStartTime && t.TARGET_DATE < this.QueryEndTime);

            foreach (STEP_WIP wip in stepWip)
            {
                var std_info = stdStepDic.TryGetValue(wip.STEP_ID, out double re);
                var std_seq = std_info == true ? re : 0;

                double stepSeq = std_seq;

                DateTime DateChange = GetRptDate_1Hour(wip.TARGET_DATE, ShopCalendar.StartTime.Minutes);
                DateTime planDate = GetPlanDate(DateChange);
                string dateStr = GetDateString(planDate.ToString("yyyyMMddHHmm")); //GetDateString(planDate);

                foreach (string cate in category)
                {
                    string mkey = wip.LINE_ID + wip.PRODUCT_ID + wip.PROCESS_ID + wip.STEP_ID + cate + dateStr;

                    ResultItem wipri = null;

                    if (items.TryGetValue(mkey, out wipri) == false)
                        items.Add(mkey, wipri = new ResultItem(wip.LINE_ID, wip.PRODUCT_ID, wip.PROCESS_ID, wip.STEP_ID, stepSeq, cate, dateStr));

                    if (cate == "WIP")
                        wipri.UpdateQty((int)wip.WAIT_LOT_QTY + (int)wip.RUN_LOT_QTY);
                }
      
            }

            return items;
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

            dt.AddColumn("LINE_ID", "LINE_ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("PRODUCT_ID", "PRODUCT_ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("PROCESS_ID", "PROCESS_ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("STD_STEP", "STD_STEP", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("CATEGORY", "CATEGORY", typeof(string), PivotArea.RowArea, null, null);

            dt.AddColumn("TARGET_DATE", "TARGET_DATE", typeof(string), PivotArea.ColumnArea, null, null);

            dt.AddColumn("QTY", "QTY", typeof(int), PivotArea.DataArea, null, null);

            dt.Columns["QTY"].DefaultValue = 0.0f;

            dt.AddDataTablePrimaryKey(
                    new DataColumn[]
                    {
                        dt.Columns["LINE_ID"],
                        dt.Columns["PRODUCT_ID"],
                        dt.Columns["PROCESS_ID"],
                        dt.Columns["STD_STEP"],
                        dt.Columns["CATEGORY"],
                        dt.Columns["TARGET_DATE"],                   }
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

                ////ValueDataMembers에 복수개을 설정해도 첫번째 Field만 기록됨(그래서 한개만 추가 하도록 함)
                //break;
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

        private double ConvertLayerIndex(string stepID)
        {
            if (stepID == null)
                return 999;

            double seq = 0;
            if (_stepIndexs.TryGetValue(stepID, out seq))
                return seq;

            return 999;
        }

        private List<string> GetCategoryList(PivotGridControl pivotGrid)
        {
            List<string> list = new List<string>();
            List<PivotGridField> rowList = pivotGrid.GetFieldsByArea(PivotArea.ColumnArea);
            for (int i = 0; i < rowList.Count; i++)
            {
                string colName = pivotGrid.GetFieldByArea(PivotArea.ColumnArea, i).FieldName;
                list.Add(colName);
            }

            return list;
        }

        private void FillChart(PivotGridControl pivotGrid, List<int> rows)
        {
            XYDiagram xyDiagram = (XYDiagram)chartControl.Diagram;
            if (xyDiagram != null)
            {
                xyDiagram.AxisX.Label.Angle = 45;
                xyDiagram.AxisX.Label.ResolveOverlappingOptions.AllowHide = false;
                xyDiagram.AxisX.NumericScaleOptions.AutoGrid = false;
                xyDiagram.EnableAxisXScrolling = false;
                xyDiagram.EnableAxisYScrolling = true;
                xyDiagram.AxisX.Label.Font = new System.Drawing.Font("Tahoma", 7F);
            }

            chartControl.Series.Clear();

            DataTable dt = (DataTable)pivotGrid.DataSource;
            var colNameList = GetColNameList(pivotGrid);

            var dataAreaList = GetDataAreaList(pivotGrid);

            var categoryList = GetCategoryList(pivotGrid);

            //bool onlyOne = rows.Count == 1;

            foreach (int rowIdx in rows)
            {
                var valueList = GetRowValueList(pivotGrid, rowIdx);
                DataView dv = SummaryData(dt, colNameList, valueList);

                string seriesName = Concat('@', valueList.ToArray()); //MyHelper.STRING.ConcatKey
  
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

            this.dockPanel1.ShowSliding();

            if (SyncChart == false)
                return;

            List<int> rows = GetSelectionList();

            FillChart((PivotGridControl)sender, rows);
        }

        private void pivotGridControl1_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
        {
            if (e.Field.FieldName != "STD_STEP")
                return;

            double s1 = ConvertLayerIndex(e.Value1 as string);
            double s2 = ConvertLayerIndex(e.Value2 as string);

            e.Result = s1.CompareTo(s2);

            e.Handled = true;
        }

        private void pivotGridControl1_CellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            if (e.GetFieldValue(e.DataField) != null && e.GetFieldValue(e.DataField).ToString() == "0")
            {
                e.DisplayText = string.Empty;
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

            foreach (var node in treeList1.Selection)
            {
                List<TreeListNode> childNodes = new List<TreeListNode>();
                GetChildNodes(node, childNodes,true);

                filterStringSet.Add(string.Join(",", childNodes.Select(item => $"'{item.GetDisplayText("PRODUCT")}'").ToArray()));
            }

            string filterString = string.Join(",", filterStringSet);

            this.pivotGridControl1.ActiveFilterString = $"[field_PRODUCT_ID] In ({filterString})";
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

