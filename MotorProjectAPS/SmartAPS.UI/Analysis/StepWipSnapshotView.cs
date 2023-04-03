using DevExpress.Spreadsheet;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using Mozart.Common;
using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserInterface;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using System.Windows.Forms;

namespace SmartAPS.UI.Analysis
{
	public partial class StepWipSnapshotView : MyXtraPivotGridTemplate
    {

        private Dictionary<string, ResultItem> _dict;
        private Dictionary<string, double> _stepIndexs;
        private IEnumerable<STD_STEP_INFO> _stdStepDt;
        private bool isChild = false;
        private bool isStepRow = false;
        private bool firstChart = true;
        private List<int> _allRowsList;



        private string TargetDate
        {
            get { return this.editPlanDate.EditValue as string; }
            set { this.editPlanDate.EditValue = value; }
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

        class StepWipData
        {
            public const string LINE_ID = "LINE_ID";
            public const string PRODUCT_ID = "PRODUCT_ID";
            public const string PROCESS_ID = "PROCESS_ID";
            public const string STD_STEP = "STEP";
            public const string TARGET_DATE = "TARGET_DATE";
            public const string WAIT_LOT_QTY = "WAIT_LOT_QTY";
            public const string RUN_LOT_QTY = "RUN_LOT_QTY";
            public const string TOTAL_QTY = "TOTAL_QTY";
        }

        class StepMoveData
        {
            public const string STD_STEP = "STD_STEP";
        }

        #region ResultItem
        private class ResultItem
        {
            public string LINE_ID;
            public string PROD_ID;
            public string PROC_ID;
            public string STEP_ID;
            public string DATE_INFO;
            public float WAIT_LOT_QTY;
            public float RUN_LOT_QTY;
            public double STEP_SEQ;
        }
        #endregion

        public StepWipSnapshotView()
        {
            InitializeComponent();
        }

        public StepWipSnapshotView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        public StepWipSnapshotView(IExperimentResultItem targetResult, string targetTime)
        {
            InitializeComponent();
            
            this.Result = targetResult;
            this.TargetDate = targetTime;

            this.isChild = true;

            LoadDocument();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetMainPivotGrid(this.pivotGridControl1);

            this.AddExtendPivotGridMenu(this.pivotGridControl1);

            ShowTotal(this.pivotGridControl1);

            SetControls();
        }

        protected override XtraPivotGridTemplate GetCloneView()
        {
            return new StepWipSnapshotView(this.Result, this.TargetDate);
        }

        protected override void Query()
        {
            var dt = GetData();
            BindData(dt);
        }

        private void SetControls()
        {
            SetControl_DateCombo(this.editPlanDate);
            SetChart();
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

        public void SetControl_DateCombo(BarEditItem control)
        {
            control.BeginUpdate();

            var cbEdit = control.Edit as RepositoryItemComboBox;
            if (cbEdit == null)
                return;

            cbEdit.Items.Clear();           

            var table = MyHelper.DATASVC.GetEntityData<STEP_WIP>(this.Result);
            List<string> list = new List<string>();
            if (table != null)
            {
                foreach (var item in table)
                {
                    string value = GetRptDate_1Hour(item.TARGET_DATE, ShopCalendar.StartTime.Minutes).ToString("yyyyMMddHHmm");
                    if (!list.Contains(value)) list.Add(value);
                }
            }
            list.Sort();
            cbEdit.Items.AddRange(list);
            if (!isChild)
                if (cbEdit.Items.Count > 0)
                    control.EditValue = cbEdit.Items[0];

            control.EndUpdate();
            control.Refresh();
        }

        public DateTime GetRptDate_1Hour(DateTime t, int baseMinute)
        {
            //1시간 단위
            int baseHours = 1;

            //ex) HH:00:00
            DateTime rptDate = new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0).AddMinutes(baseMinute); //DateTime.Trim(t,"HH")

            ////baseMinute(ex.30분) 이상인 경우 이후 시간대 baseMinute의 실적
            ////07:30 = 06:30(초과) ~ 07:30(이하)인경우, 06:40 --> 07:30, 07:30 --> 07:30, 07:40 --> 08:30
            //if (t.Minute > baseMinute)
            //{
            //    rptDate = rptDate.AddHours(baseHours);
            //}

            return rptDate;
        }

        new private XtraPivotGridHelper.DataViewTable GetData()
        {
            XtraPivotGridHelper.DataViewTable table = CreateDataViewSchema();
           
            FillData(table);
            
            return table;
        }

        #region Filldata
        private void FillData(XtraPivotGridHelper.DataViewTable dt)
        {
            LoadData();
     
            var stepWip = _dict.Values;

            //var sample = GetSampleItem(targetAreaID);
    
            //var query = from ps in stdStepList
            //            join p in stepWip
            //            on ps.SHOP_ID + ps.STEP_ID equals p.SHOP_ID + p.STD_STEP_ID into temp
            //            from tp in temp.DefaultIfEmpty()
            //            select new
            //            {
            //                AREA_ID = tp != null ? tp.AREA_ID : ps.AREA_ID,
            //                SHOP_ID = tp != null ? tp.SHOP_ID : ps.SHOP_ID,
            //                PROD_ID = tp != null ? tp.PROD_ID : (sample != null ? sample.PROD_ID : null),
            //                PROD_VER = tp != null ? tp.PROD_VER : (sample != null ? sample.PROD_VER : null),
            //                PRODUCTION_TYPE = tp != null ? tp.PRODUCTION_TYPE : (sample != null ? sample.PRODUCTION_TYPE : null),
            //                STD_STEP_ID = tp != null ? tp.STD_STEP_ID : ps.STEP_ID,
            //                WAIT_LOT_QTY = tp != null ? tp.WAIT_LOT_QTY : 0,
            //                RUN_LOT_QTY = tp != null ? tp.RUN_LOT_QTY : 0,
            //                STD_STEP_SEQ = tp != null ? tp.STEP_SEQ : ps.STEP_SEQ,
            //            };

            _stepIndexs = new Dictionary<string, double>();

            var table = dt.DataTable;

            foreach (var item in stepWip)
            {
                if (item.PROD_ID == null)
                    continue;

                table.Rows.Add(item.LINE_ID ?? string.Empty,
                               item.STEP_ID ?? string.Empty,
                               item.PROD_ID ?? string.Empty,
                               item.PROC_ID ?? string.Empty,
                               item.WAIT_LOT_QTY,
                               item.RUN_LOT_QTY,
                               item.WAIT_LOT_QTY + item.RUN_LOT_QTY);

                string stepKey = item.STEP_ID;
                _stepIndexs[stepKey] = item.STEP_SEQ;
            }
        }

        //private ResultItem GetSampleItem(string areaID)
        //{
        //    var sw = _dict.Values;
        //    if (sw == null)
        //        return null;

        //    var sample = sw.Where(t => t.AREA_ID == areaID).FirstOrDefault();
        //    if (sample != null)
        //        return sample;

        //    return null;
        //}
        #endregion

        #region BindData
        private void BindData(XtraPivotGridHelper.DataViewTable dt)
        {
            this.pivotGridControl1.BeginUpdate();

            this.pivotGridControl1.ClearPivotGridFields();
            this.pivotGridControl1.CreatePivotGridFields(dt);

            this.pivotGridControl1.DataSource = dt.DataTable;

            this.pivotGridControl1.OptionsView.ShowFilterHeaders = true;

            for (int i = 0; i < this.pivotGridControl1.Fields.Count; i++)
            {
                if (this.pivotGridControl1.Fields[i].FieldName == "WAIT_LOT_QTY" || this.pivotGridControl1.Fields[i].FieldName == "RUN_LOT_QTY")
                    this.pivotGridControl1.Fields[i].Area = PivotArea.FilterArea;
            }

            pivotGridControl1.CustomCellDisplayText += pivotGridControl1_CellDisplayText;

            pivotGridControl1.Fields[StepWipData.TOTAL_QTY].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields[StepWipData.TOTAL_QTY].CellFormat.FormatString = "#,##0";

            pivotGridControl1.Fields[StepWipData.RUN_LOT_QTY].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields[StepWipData.RUN_LOT_QTY].CellFormat.FormatString = "#,##0";

            pivotGridControl1.Fields[StepWipData.WAIT_LOT_QTY].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields[StepWipData.WAIT_LOT_QTY].CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.EndUpdate();
            pivotGridControl1.BestFitColumnArea();
            pivotGridControl1.BestFitRowArea();

            this.pivotGridControl1.Fields[StepWipData.STD_STEP].SortMode = PivotSortMode.Custom;

            this.SetPivotGridLayout(this.MainPivotGrid, false);
        }

        #endregion

        #region LoadData
        private void LoadData()
        {
            var stepWips = MyHelper.DATASVC.GetEntityData<STEP_WIP>(this.Result);
            _dict = new Dictionary<string, ResultItem>();

            string targetDate = this.TargetDate;

            var stepSeqDic = GetStepSeqDic();

            foreach (STEP_WIP item in stepWips)
            {
                string dateStr = GetRptDate_1Hour(item.TARGET_DATE, ShopCalendar.StartTime.Minutes).ToString("yyyyMMddHHmm");
                if (dateStr != targetDate && targetDate != "ALL")
                    continue;

                string stepID = item.STEP_ID;
                var std_info = stepSeqDic.TryGetValue(item.STEP_ID, out double re);
                var std_seq = re;

                ResultItem ri;
                string key = MyHelper.STRING.CreateKey(item.LINE_ID, item.PRODUCT_ID, item.PROCESS_ID);
                if (_dict.TryGetValue(key, out ri) == false)
                {

                    ri = new ResultItem();

                    ri.LINE_ID = item.LINE_ID;
                    ri.PROD_ID = item.PRODUCT_ID;
                    ri.PROC_ID = item.PROCESS_ID;
                    ri.STEP_ID = stepID;
                    ri.STEP_SEQ = std_seq;
                    ri.DATE_INFO = dateStr;

                    _dict.Add(key, ri);
                }

                ri.WAIT_LOT_QTY += Convert.ToSingle(item.WAIT_LOT_QTY);
                ri.RUN_LOT_QTY += Convert.ToSingle(item.RUN_LOT_QTY);
            }
        }

        private Dictionary<string, double> GetStepSeqDic()
        {
            _stdStepDt = MyHelper.DATASVC.GetEntityData<STD_STEP_INFO>(this.Result);

            Dictionary<string, double> list = new Dictionary<string, double>();

            var finds = _stdStepDt.OrderBy(t => t.STEP_SEQ);
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
        #endregion

        #region CreateSchema

        private XtraPivotGridHelper.DataViewTable CreateDataViewSchema()
        {
            XtraPivotGridHelper.DataViewTable dt = new XtraPivotGridHelper.DataViewTable();

            dt.AddColumn(StepWipData.LINE_ID, "LINE ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn(StepWipData.STD_STEP, "STD STEP", typeof(string), PivotArea.ColumnArea, null, null);

            dt.AddColumn(StepWipData.PRODUCT_ID, "PRODUCT ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn(StepWipData.PROCESS_ID, "PROCESS ID", typeof(string), PivotArea.RowArea, null, null);

            dt.AddColumn(StepWipData.WAIT_LOT_QTY, "WAIT LOT QTY", typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn(StepWipData.RUN_LOT_QTY, "RUN LOT QTY", typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn(StepWipData.TOTAL_QTY, "TOTAL QTY", typeof(float), PivotArea.DataArea, null, null);

            dt.Columns[StepWipData.TOTAL_QTY].DefaultValue = 0.0f;

            dt.AddDataTablePrimaryKey(
                    new DataColumn[]
                    {
                        dt.Columns[StepWipData.LINE_ID],
                        dt.Columns[StepWipData.STD_STEP],
                        dt.Columns[StepWipData.PRODUCT_ID],
                        dt.Columns[StepWipData.PROCESS_ID]
                    }
                );

            return dt;
        }
        #endregion

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

        private void ShowTotal(PivotGridControl pivot)
        {
            pivot.OptionsView.ShowRowTotals = false;
            pivot.OptionsView.ShowRowGrandTotals = false;
            pivot.OptionsView.ShowColumnTotals = true;
            pivot.OptionsView.ShowColumnGrandTotals = true;
        }


        private DataView SummaryData(DataTable dt, List<string> colNameList, List<string> valueList)
        {
            DataTable result = CreateSummaryTable(colNameList);
            FillRowZero(result, valueList);
            int keyColCount = colNameList.Count;    
             
            foreach (DataRow row in dt.Rows)
            {
                object[] pks = colNameList.Contains(StepWipData.STD_STEP) ? new object[keyColCount] : new object[keyColCount + 1];

                for (int i = 0; i < keyColCount; i++)
                {
                    string colName = colNameList[i];
                    pks[i] = row.GetString(colName);
                }
                if (!colNameList.Contains(StepWipData.STD_STEP))
                {
                    string stdStep = row.GetString(StepWipData.STD_STEP);
                    pks[keyColCount] = stdStep;
                }
                DataRow findRow = result.Rows.Find(pks);
                if (findRow == null)
                    continue;

                int waitQty = row.GetInt32(StepWipData.WAIT_LOT_QTY);
                if (waitQty > 0 && !colNameList.Contains(StepWipData.WAIT_LOT_QTY))
                    findRow[StepWipData.WAIT_LOT_QTY] = findRow.GetInt32(StepWipData.WAIT_LOT_QTY) + waitQty;

                int runQty = row.GetInt32(StepWipData.RUN_LOT_QTY);
                if (runQty > 0 && !colNameList.Contains(StepWipData.RUN_LOT_QTY))
                    findRow[StepWipData.RUN_LOT_QTY] = findRow.GetInt32(StepWipData.RUN_LOT_QTY) + runQty;

                int totalQty = row.GetInt32(StepWipData.TOTAL_QTY);
                if (totalQty > 0 && !colNameList.Contains(StepWipData.TOTAL_QTY))
                    findRow[StepWipData.TOTAL_QTY] = findRow.GetInt32(StepWipData.TOTAL_QTY) + totalQty;
            }

            return new DataView(result, string.Empty, string.Empty, DataViewRowState.CurrentRows);
        }


        private DataTable CreateSummaryTable(List<string> colNameList)
        {
            DataTable result = new DataTable();
            string[] columArr = {StepWipData.WAIT_LOT_QTY, StepWipData.RUN_LOT_QTY, StepWipData.TOTAL_QTY };
            List<DataColumn> pkList = new List<DataColumn>();
            foreach (var colName in colNameList)
            {
                var col = result.Columns.Add(colName, typeof(string));
                pkList.Add(col);
            }
            if (!result.Columns.Contains(StepWipData.STD_STEP))
            {
                var scol = result.Columns.Add(StepWipData.STD_STEP, typeof(string));
                pkList.Add(scol);
                isStepRow = false;
            }
            else isStepRow = true; 

            foreach (string nameCol in columArr) 
            {
                if (!result.Columns.Contains(nameCol))
                    result.Columns.Add(nameCol, typeof(int));
            }
            result.PrimaryKey = pkList.ToArray();

            return result;
        }

        private void FillRowZero(DataTable result, List<string> valueList)
        {
            var stdStepList = _stdStepDt;
            if (stdStepList == null)
                return;

            if (isStepRow)
                stdStepList = stdStepList.Where(t => t.STD_STEP_ID == valueList[valueList.Count() - 1]);
            int count = valueList.Count;


            foreach (var stdStep in stdStepList.OrderBy(t => t.STEP_SEQ))
            {
                DataRow row = result.NewRow();

                for (int i = 0; i < count; i++)
                    row[i] = valueList[i];

                row[StepWipData.STD_STEP] = stdStep.STD_STEP_ID;
                row[StepWipData.WAIT_LOT_QTY] = 0;
                row[StepWipData.RUN_LOT_QTY] = 0;
                row[StepWipData.TOTAL_QTY] = 0;

                result.Rows.Add(row);
            }
        }
        

        #region //Chart

        private void FillChart(PivotGridControl pivotGrid, List<int> rows)
        {          
            XYDiagram xyDiagram = (XYDiagram)chartControl.Diagram;
            if (xyDiagram != null)
            {
                xyDiagram.AxisX.Label.Angle = 90;

                xyDiagram.AxisX.Label.Staggered = false;
                xyDiagram.AxisX.Label.ResolveOverlappingOptions.AllowHide = false;
                xyDiagram.AxisX.NumericScaleOptions.AutoGrid = false;
                xyDiagram.AxisX.QualitativeScaleOptions.AutoGrid = false;
                xyDiagram.EnableAxisXScrolling = true;
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

                string seriesName = MyHelper.STRING.ConcatKey(valueList.ToArray());
               
                AddSeries(seriesName, ViewType.StackedBar, dataAreaList, dv);
                //if (onlyOne)
                //    AddSeries(seriesName, ViewType.Bar, dataAreaList, dv);
            }
        }

        private void AddSeries(string seriesName, ViewType viewType, string[] dataAreaList, DataView dv)
        {
            int index = chartControl.Series.Add(seriesName, viewType);
            chartControl.Series[index].ArgumentDataMember = StepWipData.STD_STEP;

            chartControl.Series[index].ValueDataMembers.AddRange(dataAreaList);

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


        #endregion //Chart


        #region Event Handlers
        private void pivotGridControl1_CellSelectionChanged(object sender, EventArgs e)
        {
            if (firstChart == true)
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

        private void pivotGridControl1_CellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            if (e.DataField == null)
                return;

            if (e.GetFieldValue(e.DataField) != null && e.GetFieldValue(e.DataField).ToString() == "0")
            {
                e.DisplayText = string.Empty;
            }
        }

        private void pivotGridControl1_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
        {
            if (e.Field.FieldName != StepWipData.STD_STEP)
                return;

            double s1 = ConvertLayerIndex(e.Value1 as string);
            double s2 = ConvertLayerIndex(e.Value2 as string);

            e.Result = s1.CompareTo(s2);

            e.Handled = true;
        }

        private double ConvertLayerIndex(string stepID)
        {
            if (stepID == null)
                return 999;

            double seq;
            if (_stepIndexs.TryGetValue(stepID, out seq))
                return seq;

            return 999;
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isChild = false;
            RunQuery();
        }

        private void buttonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.PIVOTEXPORT.ExportToExcel(pivotGridControl1);
        }

        private void checkShowSub_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void checkProdVersion_CheckedChanged(object sender, ItemClickEventArgs e)
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
        #endregion
    }
}