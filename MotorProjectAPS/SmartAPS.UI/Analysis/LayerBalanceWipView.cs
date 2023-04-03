using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartAPS.MozartStudio.Helper;
using SmartAPS.MozartStudio.Utils;
using Template.Lcd.Scheduling.Inputs;
using Template.Lcd.Scheduling.Outputs;

namespace SmartAPS.MozartStudio.Analysis
{
	public partial class LayerBalanceWipView : MyXtraPivotGridTemplate
    {
        private Dictionary<string, ResultItem> _dict;
        private SortedSet<string> _dayHourList;

        private List<BalanceStep> _balanceStepList;

        private Dictionary<string, int> _stepIndexs;
        private Dictionary<string, string> _eqpGroupDic;
        private List<int> _allRowsList;


        private string TargetAreaID
        {
            get
            { return this.editAreaId.EditValue as string; }
        }

        private List<string> TargetEqpGroupList
        {
            get
            {

                List<string> list = new List<string>();

                foreach (CheckedListBoxItem item in this.repoEqpGroup.Items)
                {
                    if (item.CheckState == CheckState.Checked)
                        list.Add(item.ToString());
                }

                return list;
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
            get { return Convert.ToDateTime(this.editDateTime.EditValue); }
        }

        private DateTime QueryEndTime
        {
            get { return Convert.ToDateTime(this.editDateTime.EditValue).AddHours(Convert.ToInt32(this.editDateSpin.EditValue) * ShopCalendar.ShiftHours); }
        }

        private DateTime PlanStartTime
        {
            get
            {
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
            }
        }

        public LayerBalanceWipView()
        {
            InitializeComponent();
        }

        public LayerBalanceWipView(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetMainPivotGrid(this.pivotGridControl1);
            this.AddExtendPivotGridMenu(this.pivotGridControl1);
            SetControls();
            RunQuery();
        }

        #region Internal Class : ResultItem

        internal class ResultItem
        {
            public string SHOP_ID;            
            public string PRODUCT_ID;
            public string PROCESS_ID;
            public string STEP_ID;
            public string STD_STEP;
            public int STEP_SEQ;
            public string DATE_INFO;
            public float WAIT_QTY;
            public float RUN_QTY;

            public ResultItem()
            {

            }
        }

        private class BalanceStep
        {
            private List<string> Steps { get; set; }

            public string AreaID { get; private set; }
            public string ShopID { get; private set; }

            public string FromStep { get; private set; }
            public string ToStep { get; private set; }

            public int Seq { get; private set; }

            public int BalanceWipQty { get; private set; }
            public float BalanceGap { get; private set; }

            public string Key
            {
                get { return string.Format("{0}~{1}", this.FromStep, this.ToStep); }
            }

            public BalanceStep()
            {
                this.Steps = new List<string>();
            }

            public void Import(StdStep stdStep, List<StdStep> stdStepList)
            {
                this.AreaID = stdStep.AREA_ID;
                this.ShopID = stdStep.SHOP_ID;
                this.FromStep = stdStep.STEP_ID;
                this.Seq = stdStep.STEP_SEQ;

                this.ToStep = stdStep.BALANCE_TO_STEP;
                this.BalanceWipQty = stdStep.BALANCE_WIP_QTY;
                this.BalanceGap = stdStep.BALANCE_GAP;

                AddStep(stdStepList);
            }

            private void AddStep(List<StdStep> stdStepList)
            {
                string areaID = this.AreaID;
                string fromStep = this.FromStep;
                string toStep = this.ToStep;

                int sindex = stdStepList.FindIndex(t => t.AREA_ID == areaID && t.STEP_ID == fromStep);
                if (sindex < 0)
                    return;

                int eindex = stdStepList.FindIndex(t => t.AREA_ID == areaID && t.STEP_ID == toStep);
                if (eindex < 0)
                    return;

                for (int i = sindex; i <= eindex; i++)
                {
                    var stdStep = stdStepList[i];
                    this.Steps.Add(stdStep.STEP_ID);
                }
            }

            public bool Contains(string stepID)
            {
                if (string.IsNullOrEmpty(stepID))
                    return false;

                if (this.Steps == null || this.Steps.Count == 0)
                    return false;

                if (this.Steps.Contains(stepID))
                    return true;

                return false;
            }
        }

        class DataConst
        {
            public const string SHOP_ID = "SHOP_ID";
            public const string EQP_GROUP = "EQP_GROUP";
            public const string STD_STEP = "STEP";

            public const string TARGET_DATE = "TARGET_DATE";
            public const string WIP_QTY = "WIP";

            public const string BALANCE_STEP = "BALANCE_STEP";
            public const string BALANCE_WIP_QTY = "BASE_QTY";
            public const string GAP_QTY = "GAP";
        }

        #endregion

        #region SetControl

        private void SetControls()
        {
            MyHelper.ENGCONTROL.SetControl_AreaID(this.editAreaId, this.Result);
            this.SetControl_EqpGroup(this.editEqpGroup, this.TargetAreaID);
            ShowTotal(this.pivotGridControl1);

            this.editDateTime.EditValue = this.PlanStartTime;
            this.editDateSpin.EditValue = 2;
        }

        private void SetChart()
        {
            List<int> rows = new List<int>();
            var count = pivotGridControl1.Cells.RowCount;
            for (int i = 0; i < count; i++)
            {
                rows.Add(i);
            }
            _allRowsList = rows;
            FillChart(pivotGridControl1, _allRowsList);
        }

        private void SetControl_EqpGroup(BarEditItem control, string areaId)
        {
            var cbEdit = control.Edit as RepositoryItemCheckedComboBoxEdit;
            if (cbEdit == null)
                return;

            cbEdit.Items.Clear();
            var table = MyHelper.DATASVC.GetEntityData<StdStep>(this.Result);

            List<string> list = new List<string>();
            foreach (var item in table.Distinct())
            {
                string bal = item.BALANCE_TO_STEP;
                if (string.IsNullOrEmpty(bal) || Mozart.SeePlan.StringUtility.IsEmptyID(bal))
                    continue;

                if (areaId != null && item.AREA_ID != areaId)
                    continue;

                string value = item.DSP_EQP_GROUP_ID;

                if (list.Contains(value) == false)
                    list.Add(value);
            }

            list.Sort();
            list.ForEach(item => cbEdit.Items.Add(item));
            cbEdit.Items.ToList<CheckedListBoxItem>().ForEach(item => item.CheckState = CheckState.Checked);
            control.EditValue = cbEdit.GetCheckedItems();
        }

        #endregion

        #region BindData

        protected override void Query()
        {
            var dt = GetData();
            BindData(dt);
            SetChart();
        }

        private void GetEqpGroupDic(List<StdStep> stdStepList)
        {
            _eqpGroupDic = new Dictionary<string, string>();

            foreach (var item in stdStepList)
            {
                string key = MyHelper.STRING.CreateKey(item.SHOP_ID, item.STEP_ID);
                if (_eqpGroupDic.ContainsKey(key) == false)
                    _eqpGroupDic.Add(key, item.DSP_EQP_GROUP_ID);
            }
        }

        new private XtraPivotGridHelper.DataViewTable GetData()
        {
            SetDateRanges();
            LoadData();

            XtraPivotGridHelper.DataViewTable table = CreateDataViewTable();

            FillData(table);

            return table;
        }

        private void LoadData()
        {
            _dict = new Dictionary<string, ResultItem>();

            string targetAreaID = this.TargetAreaID;
            bool isFirst = true;

            var stdStepList = MyHelper.ENGCONTROL.GetList_StdStep(targetAreaID, this.Result);
            _balanceStepList = GetBalanceStep(stdStepList);

            GetEqpGroupDic(stdStepList);

            var table = MyHelper.DATASVC.GetEntityData<StepWip>(this.Result);
            foreach (var item in table)
            {
                if (item.AREA_ID != targetAreaID)
                    continue;

                if (item.PLAN_DATE < this.QueryStartTime)
                    continue;

                if (item.PLAN_DATE >= this.QueryEndTime)
                    continue;

                string stepID = item.STEP_ID;
                string stdStepID = item.STD_STEP_ID;
                int stepSeq = item.STD_STEP_SEQ;

                var stdStep = FindMainStep(item.SHOP_ID, stdStepID, stdStepList);
                if (stdStep == null)
                    continue;

                stepID = stdStep.STEP_ID;
                stdStepID = stdStep.STEP_ID;
                stepSeq = stdStep.STEP_SEQ;

                if (isFirst)
                {
                    foreach (string date in _dayHourList)
                    {
                        ResultItem padding;

                        string dateString = GetDateString(date);

                        string k = item.SHOP_ID + item.PRODUCT_ID + item.PROCESS_ID + stdStepID + dateString;

                        if (_dict.TryGetValue(k, out padding) == false)
                        {
                            padding = new ResultItem();

                            padding.SHOP_ID = item.SHOP_ID;
                            padding.PRODUCT_ID = item.PRODUCT_ID;
                            padding.PROCESS_ID = item.PROCESS_ID;
                            padding.STEP_ID = item.STEP_ID;
                            padding.STD_STEP = item.STD_STEP_ID;
                            padding.STEP_SEQ = item.STD_STEP_SEQ;
                            padding.DATE_INFO = dateString;

                            padding.WAIT_QTY = 0;
                            padding.RUN_QTY = 0;

                            _dict.Add(k, padding);
                        }
                    }

                    isFirst = false;
                }

                ResultItem ri;

                DateTime planDate = item.PLAN_DATE;
                string dateStr2 = GetDateString(planDate);

                string key = item.SHOP_ID + item.PRODUCT_ID + item.PROCESS_ID + stdStepID + dateStr2;

                if (_dict.TryGetValue(key, out ri) == false)
                {

                    ri = new ResultItem();

                    ri.SHOP_ID = item.SHOP_ID;
                    ri.PRODUCT_ID = item.PRODUCT_ID;
                    ri.PROCESS_ID = item.PROCESS_ID;
                    ri.STEP_ID = item.STEP_ID;
                    ri.STD_STEP = item.STD_STEP_ID;
                    ri.STEP_SEQ = item.STD_STEP_SEQ;
                    ri.DATE_INFO = dateStr2;

                    _dict.Add(key, ri);
                }

                ri.WAIT_QTY += Convert.ToSingle(item.WAIT_QTY);
                ri.RUN_QTY += Convert.ToSingle(item.RUN_QTY);
            }
        }

        private XtraPivotGridHelper.DataViewTable CreateDataViewTable()
        {
            XtraPivotGridHelper.DataViewTable dt = new XtraPivotGridHelper.DataViewTable();

            dt.AddColumn(DataConst.SHOP_ID, DataConst.SHOP_ID, typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn(DataConst.EQP_GROUP, DataConst.EQP_GROUP, typeof(string), PivotArea.RowArea, null, null);

            dt.AddColumn(DataConst.BALANCE_STEP, DataConst.BALANCE_STEP, typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn(DataConst.BALANCE_WIP_QTY, DataConst.BALANCE_WIP_QTY, typeof(float), PivotArea.RowArea, null, null);

            dt.AddColumn(DataConst.TARGET_DATE, DataConst.TARGET_DATE, typeof(string), PivotArea.ColumnArea, null, null);

            dt.AddColumn(DataConst.WIP_QTY, DataConst.WIP_QTY, typeof(float), PivotArea.DataArea, null, null);
            dt.Columns[DataConst.WIP_QTY].DefaultValue = 0.0f;

            dt.AddColumn(DataConst.GAP_QTY, DataConst.GAP_QTY, typeof(float), PivotArea.DataArea, null, null);
            dt.Columns[DataConst.GAP_QTY].DefaultValue = 0.0f;

            dt.AddDataTablePrimaryKey(
                    new DataColumn[]
                    {
                        dt.Columns[DataConst.SHOP_ID],
                        dt.Columns[DataConst.BALANCE_STEP],
                        dt.Columns[DataConst.TARGET_DATE]
                    }
                );

            return dt;
        }

        private void FillData(XtraPivotGridHelper.DataViewTable dt)
        {
            _stepIndexs = new Dictionary<string, int>();

            var blist = _balanceStepList;
            var table = dt.DataTable;

            var targetEqpGroupList = this.TargetEqpGroupList;

            foreach (var item in _dict.Values)
            {
                if (item.PRODUCT_ID == null)
                    continue;

                string stdStepID = item.STD_STEP;
                var finds = blist.FindAll(t => t.Contains(stdStepID));
                if (finds == null || finds.Count == 0)
                    continue;

                foreach (var bstep in finds)
                {
                    string balanceStep = bstep.Key;
                    string shopID = bstep.ShopID;
                    int seq = bstep.Seq;
                    int balanceWipQty = bstep.BalanceWipQty;

                    string key = MyHelper.STRING.CreateKey(bstep.ShopID, bstep.FromStep);
                    string eqpGroupID = _eqpGroupDic[key];

                    if (targetEqpGroupList.Contains(eqpGroupID) == false)
                        continue;

                    var row = table.Rows.Find(new object[] { shopID, balanceStep, item.DATE_INFO });

                    if (row == null)
                    {
                        row = table.NewRow();

                        row[DataConst.SHOP_ID] = shopID;
                        row[DataConst.EQP_GROUP] = eqpGroupID;
                        row[DataConst.BALANCE_STEP] = balanceStep;
                        row[DataConst.BALANCE_WIP_QTY] = balanceWipQty;
                        row[DataConst.TARGET_DATE] = item.DATE_INFO;

                        table.Rows.Add(row);

                        _stepIndexs[balanceStep] = seq;
                    }

                    float qty = item.WAIT_QTY + item.RUN_QTY;

                    //FromStep : RUN ~ ToStep : WAIT
                    if (stdStepID == bstep.FromStep)
                        qty = item.RUN_QTY;
                    else if (stdStepID == bstep.ToStep)
                        qty = item.WAIT_QTY;

                    row[DataConst.WIP_QTY] = row.GetInt32(DataConst.WIP_QTY) + qty;
                    row[DataConst.GAP_QTY] = row.GetInt32(DataConst.WIP_QTY) - row.GetInt32(DataConst.BALANCE_WIP_QTY);
                }
            }
        }

        private List<BalanceStep> GetBalanceStep(List<StdStep> stdStepList)
        {
            List<BalanceStep> list = new List<BalanceStep>();

            foreach (var stdStep in stdStepList)
            {
                if (string.IsNullOrEmpty(stdStep.BALANCE_TO_STEP))
                    continue;

                BalanceStep bstep = new BalanceStep();
                bstep.Import(stdStep, stdStepList);

                list.Add(bstep);
            }

            return list;
        }

        private StdStep FindMainStep(string shopID, string stdStepID, List<StdStep> list)
        {
            int index = list.FindIndex(t => t.SHOP_ID == shopID && t.STEP_ID == stdStepID);
            if (index < 0)
                return null;

            int count = list.Count;
            for (int i = index; i < count; i++)
            {
                var stdStep = list[i];
                if (MyHelper.STRING.Equals(stdStep.STEP_TYPE, "MAIN"))
                    return stdStep;
            }

            return null;
        }

        #endregion

        #region DrawGrid

        private void BindData(XtraPivotGridHelper.DataViewTable dt)
        {
            this.pivotGridControl1.BeginUpdate();

            this.pivotGridControl1.ClearPivotGridFields();
            this.pivotGridControl1.CreatePivotGridFields(dt);

            this.pivotGridControl1.DataSource = dt.DataTable;

            this.pivotGridControl1.OptionsView.ShowFilterHeaders = true;

            for (int i = 0; i < this.pivotGridControl1.Fields.Count; i++)
            {
                if (this.pivotGridControl1.Fields[i].FieldName == DataConst.WIP_QTY)
                    this.pivotGridControl1.Fields[i].Area = PivotArea.FilterArea;
            }

            pivotGridControl1.CustomCellDisplayText += pivotGridControl1_CellDisplayText;

            pivotGridControl1.Fields[DataConst.WIP_QTY].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields[DataConst.WIP_QTY].CellFormat.FormatString = "#,##0";

            pivotGridControl1.Fields[DataConst.GAP_QTY].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields[DataConst.GAP_QTY].CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.Fields[DataConst.BALANCE_STEP].SortMode = PivotSortMode.Custom;

            this.pivotGridControl1.Fields[DataConst.BALANCE_WIP_QTY].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.pivotGridControl1.Fields[DataConst.BALANCE_WIP_QTY].CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.EndUpdate();
            pivotGridControl1.BestFitColumnArea();

            this.SetPivotGridLayout(this.MainPivotGrid, false);
        }

        #endregion

        #region DrawChart

        private void ShowTotal(PivotGridControl pivot, bool isCheck = false)
        {
            pivot.OptionsView.ShowRowTotals = false;
            pivot.OptionsView.ShowRowGrandTotals = false;
            pivot.OptionsView.ShowColumnTotals = isCheck;
            pivot.OptionsView.ShowColumnGrandTotals = isCheck;
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
                break;
            }

            return list.ToArray();
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

        private void FillChart(PivotGridControl pivotGrid, List<int> rows)
        {
            XYDiagram xyDiagram = (XYDiagram)this.chartControl.Diagram;
            if (xyDiagram != null)
            {
                xyDiagram.AxisX.Label.Angle = 45;
                xyDiagram.AxisX.Label.ResolveOverlappingOptions.AllowHide = false;
                xyDiagram.AxisX.NumericScaleOptions.AutoGrid = false;
                xyDiagram.EnableAxisXScrolling = false;
                xyDiagram.EnableAxisYScrolling = true;
                xyDiagram.AxisX.Label.Font = new System.Drawing.Font("Tahoma", 7F);
            }

            this.chartControl.Series.Clear();

            var dataAreaList = GetDataAreaList(pivotGrid);

            DataTable dataViewTable = (DataTable)pivotGrid.DataSource;
            var colNameList = GetColNameList(pivotGrid);

            int i = 0;
            foreach (int rowIdx in rows)
            {
                var valueList = GetRowValueList(pivotGrid, rowIdx);
                DataView dv = SummaryData(dataViewTable, colNameList, valueList);

                string seriesName = MyHelper.STRING.ConcatKey(valueList.ToArray());

                this.chartControl.Series.Add(seriesName, ViewType.Line);
                this.chartControl.Series[i].ArgumentDataMember = DataConst.TARGET_DATE;
                this.chartControl.Series[i].ValueDataMembers.AddRange(dataAreaList);

                this.chartControl.Series[i].DataSource = dv;
                this.chartControl.Series[i].LegendText = string.IsNullOrEmpty(seriesName) ? "Total" : seriesName;
                i++;
            }
        }

        private List<string> GetColNameList(PivotGridControl pivotGrid)
        {
            List<string> list = new List<string>();

            List<PivotGridField> rowList = pivotGrid.GetFieldsByArea(PivotArea.RowArea);
            for (int i = 0; i < rowList.Count; i++)
            {
                var field = pivotGrid.GetFieldByArea(PivotArea.RowArea, i);
                if (field.DataType != typeof(string))
                    continue;

                string colName = field.ToString();
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

                string dayHour = row.GetString(DataConst.TARGET_DATE);
                pks[keyColCount] = dayHour;

                DataRow findRow = result.Rows.Find(pks);
                if (findRow == null)
                    continue;

                int totalQty = row.GetInt32(DataConst.WIP_QTY);
                if (totalQty > 0)
                    findRow[DataConst.WIP_QTY] = findRow.GetInt32(DataConst.WIP_QTY) + totalQty;

                int gapQty = row.GetInt32(DataConst.GAP_QTY);
                findRow[DataConst.GAP_QTY] = findRow.GetInt32(DataConst.GAP_QTY) + gapQty;

            }

            return new DataView(result, string.Empty, DataConst.TARGET_DATE, DataViewRowState.CurrentRows);
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

            var tcol = result.Columns.Add(DataConst.TARGET_DATE, typeof(string));
            pkList.Add(tcol);

            result.Columns.Add(DataConst.WIP_QTY, typeof(int));
            result.Columns.Add(DataConst.GAP_QTY, typeof(int));

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

                row[DataConst.TARGET_DATE] = dayHour;
                row[DataConst.WIP_QTY] = 0;
                row[DataConst.GAP_QTY] = 0;

                result.Rows.Add(row);
            }
        }

        public struct ChartData
        {
            public string ColName;
            public HashSet<string> Datas;

            public ChartData(string colName)
            {
                this.ColName = colName;
                this.Datas = new HashSet<string>();
            }

            public void AddData(string str)
            {
                this.Datas.Add(str);
            }
        }

        #endregion

        #region Event Handlers

        private void SetDateRanges()
        {
            _dayHourList = new SortedSet<string>();

            //차트 간격 변경
            float interval = 1f;

            DateTime st = this.QueryStartTime;
            DateTime et = this.QueryEndTime;

            //add PlanStartTime
            _dayHourList.Add(GetDateString(st));

            int baseMinute = ShopCalendar.StartTime.Minutes;
            DateTime baseT = MyHelper.DATE.GetRptDate_1Hour(st, baseMinute);

            for (DateTime t = baseT; t < et; t = t.AddHours(interval))
            {
                string str = GetDateString(t);
                _dayHourList.Add(str);
            }
        }

        private string GetDateString(string dayHour)
        {
            DateTime t = MyHelper.DATE.StringToDateTime(dayHour);

            return GetDateString(t);
        }

        private string GetDateString(DateTime t)
        {
            string dateString = t.ToString("yyyyMMddHHmm");

            return dateString;
        }

        private void pivotGridControl1_CellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            if (e.GetFieldValue(e.DataField) != null && e.GetFieldValue(e.DataField).ToString() == "0")
            {
                e.DisplayText = string.Empty;
            }
        }

        private void pivotGridControl1_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
        {
            if (e.Field.FieldName != DataConst.BALANCE_STEP)
                return;

            int s1 = ConvertLayerIndex(e.Value1 as string);
            int s2 = ConvertLayerIndex(e.Value2 as string);

            e.Result = s1.CompareTo(s2);

            e.Handled = true;
        }


        private void pivotGridControl1_CellSelectionChanged(object sender, EventArgs e)
        {
            if (SyncChart == false)
                return;

            List<int> rows = GetSelectionList();

            FillChart((PivotGridControl)sender, rows);
        }

        private void AreaComboEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetControl_EqpGroup(this.editEqpGroup, this.TargetAreaID);
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void editVersionNo_EditValueChanged(object sender, EventArgs e)
        {
            MyHelper.ENGCONTROL.SetControl_AreaID(this.editAreaId, this.Result);
            this.SetControl_EqpGroup(this.editEqpGroup, this.TargetAreaID);

            this.editDateTime.EditValue = this.PlanStartTime;
            this.editDateSpin.EditValue = 2;
        }

        private void buttonExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.PIVOTEXPORT.ExportToExcel(this.pivotGridControl1);
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
        #endregion

        private int ConvertLayerIndex(string stepID)
        {
            if (stepID == null)
                return 999;

            int seq = 0;
            if (_stepIndexs.TryGetValue(stepID, out seq))
                return seq;

            return 999;
        }
	}
}

