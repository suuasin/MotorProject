using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using SmartAPS.UI.Properties;

namespace SmartAPS.UI.Analysis
{
    public partial class EqpUtilizationView : MyXtraPivotGridTemplate
    {
        private DataTable dtEqpAndArrange;
        private List<ResultItem> listResult;

        private BarEditItem _SelectedFirstCombo;
        private string _SelectedFirstColumn;

        private DateTime PlanStartTime
        {
            get
            {
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result).Date;
            }
        }

        private DateTime TargetStartTime
        {
            get
            {
                return Convert.ToDateTime(this.editDateTime.EditValue).Date;
            }
        }

        private DateTime TargetEndTime
        {
            get
            {
                return this.TargetStartTime.AddHours(Convert.ToInt32(this.TargetAddDays) * ShopCalendar.ShiftHours);
            }
        }



        private int TargetAddDays
        {
            get
            {
                return Convert.ToInt32(this.spinEditDays.EditValue);
            }
        }

        private class LoadStatData
        {
            public const string LINE_ID = "LINE_ID";
            public const string EQP_ID = "EQP_ID";
            public const string EQP_GROUP = "EQP_GROUP";
            public const string EQP_MODEL = "EQP_MODEL";
            public const string STEP_ID = "STEP_ID";
            public const string TARGET_DATE = "TARGET_DATE";
            public const string SETUP = "SETUP";
            public const string BUSY = "BUSY";
            public const string IDLERUN = "IDLERUN";
            public const string IDLE = "IDLE";
            public const string PM = "PM";
            public const string DOWN = "DOWN";
        }

        #region ResultItem
        private class ResultItem
        {
            public string LINE_ID;
            public string EQP_GROUP;
            //public string EQP_MODEL;
            public string EQP_ID;
            //public string STEP_ID;
            public string TARGET_DATE;
            public float SETUP;
            public float BUSY;
            public float IDLERUN;
            public float IDLE;
            public float PM;
            public float DOWN;
        }
        #endregion

        public EqpUtilizationView()
        {
            InitializeComponent();
        }

        public EqpUtilizationView(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetMainPivotGrid(this.pivotGridControl1);
            this.AddExtendPivotGridMenu(this.pivotGridControl1);

            ShowTotal(this.pivotGridControl1);
            SetControls();
            
            SetRadioQueryConditions();
        }

        protected override void Query()
        {
            var dt = GetData();
            BindData(dt);
            SetChart();
        }

        private void SetControls()
        {
            int baseMinute = ShopCalendar.StartTime.Minutes;

            //this.radioPlanDate.EditValue = AddDateType.DAY;
            repoDateEdit.EditMask = "yyyy-MM-dd";
            repoDateEdit.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            repoDateEdit.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
            this.editDateTime.EditValue = GetRptDate_1Hour(this.PlanStartTime, baseMinute);
            spinEditDays.EditValue = MyHelper.DATASVC.GetPlanPeriod(this.Result);

            SetQueryOptionControl();
        }

        private void SetQueryOptionControl()
        {
            //this.AddQueryOptionControl(QueryOptionKey.TargetStartTime, "START", MyCustEditType.PLANDATE_START_DATE, this.editDateTime, "yyyy-MM-dd HH:mm:ss");

            dtEqpAndArrange = GetEqpAndArrange();

            // set query conditions
            EngControlHelper.SetControl_StepID_Checked(cbStepID, this.Result);
            EngControlHelper.SetControl_EqpGroup_Checked(cbEqpGrp, this.Result);
            EngControlHelper.SetControl_EqpModel_Checked(cbEqpModel, this.Result);
        }

        private DataTable GetEqpAndArrange()
        {
            DataTable res = new DataTable();
            var dtEquipment = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(this.Result);
            var dtArrange = MyHelper.DATASVC.GetEntityData<EQP_ARRANGE>(this.Result);
            
            res.Columns.Add(LoadStatData.LINE_ID, typeof(string));
            res.Columns.Add(LoadStatData.EQP_GROUP, typeof(string));
            res.Columns.Add(LoadStatData.EQP_MODEL, typeof(string));
            res.Columns.Add(LoadStatData.STEP_ID, typeof(string));
            res.Columns.Add("PROCESS_ID", typeof(string));
            res.Columns.Add("PRODUCT_ID", typeof(string));
            res.Columns.Add(LoadStatData.EQP_ID, typeof(string));

            foreach (var row in dtArrange)
            {
                EQUIPMENT equipment = dtEquipment.Where(x => x.EQP_ID == row.EQP_ID).FirstOrDefault();

                if (equipment != null)
                {
                    DataRow newRow = res.NewRow();

                    newRow[LoadStatData.LINE_ID] = equipment.LINE_ID;
                    newRow[LoadStatData.EQP_GROUP] = equipment.EQP_GROUP;
                    newRow[LoadStatData.EQP_MODEL] = equipment.EQP_MODEL;
                    newRow[LoadStatData.STEP_ID] = row.STEP_ID;
                    newRow["PROCESS_ID"] = row.PROCESS_ID;
                    newRow["PRODUCT_ID"] = row.PRODUCT_ID;
                    newRow[LoadStatData.EQP_ID] = row.EQP_ID;
                    res.Rows.Add(newRow);
                }
            }

            res.DefaultView.Sort = LoadStatData.EQP_ID;

            return res;
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

        #region CreateSchema
        private XtraPivotGridHelper.DataViewTable CreateDataViewSchema()
        {
            XtraPivotGridHelper.DataViewTable dt = new XtraPivotGridHelper.DataViewTable();

            dt.AddColumn(LoadStatData.TARGET_DATE, "TARGET DATE", typeof(string), PivotArea.ColumnArea, null, null);

            dt.AddColumn(LoadStatData.LINE_ID, "LINE ID", typeof(string), PivotArea.RowArea, null, null);
            //dt.AddColumn(LoadStatData.STEP_ID, "STEP ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn(LoadStatData.EQP_GROUP, "EQP GROUP", typeof(string), PivotArea.RowArea, null, null);
            //dt.AddColumn(LoadStatData.EQP_MODEL, "EQP MODEL", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn(LoadStatData.EQP_ID, "EQP ID", typeof(string), PivotArea.RowArea, null, null);

            dt.AddColumn(LoadStatData.BUSY, LoadStatData.BUSY, typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn(LoadStatData.IDLERUN, "IDLE RUN", typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn(LoadStatData.IDLE, LoadStatData.IDLE, typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn(LoadStatData.SETUP, LoadStatData.SETUP, typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn(LoadStatData.PM, LoadStatData.PM, typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn(LoadStatData.DOWN, LoadStatData.DOWN, typeof(float), PivotArea.DataArea, null, null);

            dt.AddDataTablePrimaryKey(
                    new DataColumn[]
                    {
                        dt.Columns[LoadStatData.LINE_ID],
                        //dt.Columns[LoadStatData.STEP_ID],
                        dt.Columns[LoadStatData.EQP_GROUP],
                        //dt.Columns[LoadStatData.EQP_MODEL],
                        dt.Columns[LoadStatData.EQP_ID],
                        dt.Columns[LoadStatData.TARGET_DATE]
                    }
                );

            return dt;
        }
        #endregion

        #region Filldata
        private void FillData(XtraPivotGridHelper.DataViewTable dt)
        {
            LoadData();

            DataTable dtResult = dt.DataTable;

            foreach (var item in listResult)  // 코드 줄이기
            {
                DataRow row = dtResult.NewRow();
                row[LoadStatData.LINE_ID] = item.LINE_ID;
                //row[LoadStatData.STEP_ID] = item.STEP_ID;
                row[LoadStatData.EQP_GROUP] = item.EQP_GROUP;
                //row[LoadStatData.EQP_MODEL] = item.EQP_MODEL;
                row[LoadStatData.EQP_ID] = item.EQP_ID;
                row[LoadStatData.TARGET_DATE] = item.TARGET_DATE;
                row[LoadStatData.SETUP] = item.SETUP;
                row[LoadStatData.BUSY] = item.BUSY;
                row[LoadStatData.IDLERUN] = item.IDLERUN;
                row[LoadStatData.IDLE] = item.IDLE;
                row[LoadStatData.PM] = item.PM;
                row[LoadStatData.DOWN] = item.DOWN;
                dtResult.Rows.Add(row);
            }
        }

        #endregion

        private DateTime StrToDateTime(string str)
        {
            string strDate = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2) + " 00:00:00";
            return Convert.ToDateTime(strDate);
        }

        #region LoadData
        private void LoadData()
        {
            var loadStats = MyHelper.DATASVC.GetEntityData<LOAD_STAT>(this.Result).OrderBy(x => x.TARGET_DATE);
            DataTable stepInfo = GetStepByEqp();
            List<EQUIPMENT> eqps = GetEquipmentList();
            listResult = new List<ResultItem>();
            string[] selectedEqpIDs = cbEqpID.EditValue != null ? cbEqpID.EditValue.ToString().Replace(" ", "").Split(',') : null;

            if (selectedEqpIDs == null || selectedEqpIDs.Length == 0)
                return;

            foreach (LOAD_STAT item in loadStats)
            {
                if (!selectedEqpIDs.Contains(item.EQP_ID))
                    continue;

                DateTime targetDate = StrToDateTime(item.TARGET_DATE.Substring(0, 8));
                if (targetDate < TargetStartTime || targetDate >= TargetEndTime)
                    continue;

                EQUIPMENT eqp = eqps.FirstOrDefault(x => x.EQP_ID == item.EQP_ID);
                string eqpGroup = eqp.EQP_GROUP;
                //string eqpModel = eqp.EQP_MODEL;

                //DataRow stepRow = stepInfo.Select("EQP_ID = '" + item.EQP_ID + "'").FirstOrDefault();
                //string stepId = string.Empty;
                //if (stepRow != null)
                //    stepId = stepRow["STEP_ID"].ToString();

                ResultItem ri = new ResultItem();

                ri.LINE_ID = item.LINE_ID;
                ri.EQP_GROUP = eqpGroup;
                //ri.EQP_MODEL = eqpModel;
                ri.EQP_ID = item.EQP_ID;
                //ri.STEP_ID = stepId;
                ri.TARGET_DATE = item.TARGET_DATE.Substring(0, 8);
                ri.SETUP = item.SETUP;
                ri.BUSY = item.BUSY;
                ri.IDLERUN = item.IDLERUN;
                ri.IDLE = item.IDLE;
                ri.PM = item.PM;
                ri.DOWN = item.DOWN;

                listResult.Add(ri);
            }
        }

        private DataTable GetStepByEqp()
        {
            var eqpArr = MyHelper.DATASVC.GetEntityData<EQP_ARRANGE>(this.Result);

            DataTable list = new DataTable();
            list.Columns.Add("STEP_ID", typeof(string));
            list.Columns.Add("EQP_ID", typeof(string));

            foreach (var item in eqpArr.OrderBy(x => x.STEP_ID))
            {
                if (list.Select("STEP_ID = '" + item.STEP_ID + "' AND EQP_ID = '" + item.EQP_ID + "'").Length == 0)
                {
                    DataRow row = list.NewRow();
                    row["STEP_ID"] = item.STEP_ID;
                    row["EQP_ID"] = item.EQP_ID;
                    list.Rows.Add(row);
                }
            }
            return list;
        }

        private List<EQUIPMENT> GetEquipmentList()
        {
            var equipments = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(this.Result);
            List<EQUIPMENT> list = new List<EQUIPMENT>();

            List<string> keyList = new List<string>();
            foreach (var item in equipments)
            {
                string key = item.EQP_ID;
                if (keyList.Contains(key))
                    continue;

                keyList.Add(key);

                list.Add(item);
            }
            return list;
        }

        #endregion

        #region BindData
        private void BindData(XtraPivotGridHelper.DataViewTable dt)
        {
            this.pivotGridControl1.BeginUpdate();

            this.pivotGridControl1.ClearPivotGridFields();
            this.pivotGridControl1.CreatePivotGridFields(dt);
            this.pivotGridControl1.DataSource = dt.DataTable;
            this.pivotGridControl1.OptionsView.ShowFilterHeaders = true;

            pivotGridControl1.CustomCellDisplayText += pivotGridControl1_CellDisplayText;

            //pivotGridControl1.Fields[LoadStatData.BUSY].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //pivotGridControl1.Fields[LoadStatData.BUSY].CellFormat.FormatString = "#,##0";
            //pivotGridControl1.Fields[LoadStatData.IDLERUN].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //pivotGridControl1.Fields[LoadStatData.IDLERUN].CellFormat.FormatString = "#,##0";
            //pivotGridControl1.Fields[LoadStatData.IDLE].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //pivotGridControl1.Fields[LoadStatData.IDLE].CellFormat.FormatString = "#,##0";
            //pivotGridControl1.Fields[LoadStatData.SETUP].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //pivotGridControl1.Fields[LoadStatData.SETUP].CellFormat.FormatString = "#,##0";
            //pivotGridControl1.Fields[LoadStatData.PM].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //pivotGridControl1.Fields[LoadStatData.PM].CellFormat.FormatString = "#,##0";
            //pivotGridControl1.Fields[LoadStatData.DOWN].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //pivotGridControl1.Fields[LoadStatData.DOWN].CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.EndUpdate();
            pivotGridControl1.BestFitColumnArea();
            pivotGridControl1.BestFitRowArea();

            //this.pivotGridControl1.Fields[LoadStatData.STD_STEP].SortMode = PivotSortMode.Custom;

            this.SetPivotGridLayout(this.pivotGridControl1, false);
        }

        #endregion

        private void ShowTotal(PivotGridControl pivot)
        {
            pivot.OptionsView.ShowRowTotals = false;
            pivot.OptionsView.ShowRowGrandTotals = false;
            pivot.OptionsView.ShowColumnTotals = true;
            pivot.OptionsView.ShowColumnGrandTotals = true;
        }

        #region Chart   

        private void SetChart()
        {
            List<int> rows = new List<int>();
            var count = pivotGridControl1.Cells.RowCount;
            for (int i = 0; i < count; i++)
            {
                rows.Add(i);
            }

            FillChart(pivotGridControl1, rows);
        }

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

                xyDiagram.AxisX.Label.Font = new Font("Tahoma", 7F);
            }

            chartControl.Series.BeginUpdate();
            chartControl.Series.Clear();

            DataTable dt = (DataTable)pivotGrid.DataSource;

            if (dt.Rows.Count > 0)
            {
                DataView dv = null;
                var colNameList = GetColNameList(pivotGrid);
                var dataAreaList = GetDataAreaList(pivotGrid);
                List<List<string>> valueList = new List<List<string>>(); // rows의 평균을 구하기 위한 List

                foreach (int rowIdx in rows)
                {
                    valueList.Add(GetRowValueList(pivotGrid, rowIdx));
                }
                dv = SummaryData(dt, colNameList, valueList, dataAreaList); // 선택된 pivot행에서의 rowColumn에 해당하는 값들을 dv로 묶음
                AddSeries(ViewType.Bar, dataAreaList, dv);
            }
            chartControl.Series.EndUpdate();
        }

        private void AddSeries(ViewType viewType, string[] dataAreaList, DataView dv)
        {
            // get TARGET_DATE in distinct
            List<string> listDate = new List<string>();
            DataTable dtDates = dv.ToTable(true, new string[] { LoadStatData.TARGET_DATE });
            foreach (DataRow row in dtDates.Rows)
                listDate.Add(row[0].ToString());

            AttachAvgData(dv, listDate, dataAreaList);

            foreach (string area in dataAreaList)
            {
                Series series = new Series(area, viewType);

                foreach (string date in listDate)
                {
                    DataRow valueRow = dv.Table.Select(LoadStatData.TARGET_DATE + " = '" + date + "' AND AREA = '" + area + "'").FirstOrDefault();
                    double value = double.Parse(valueRow["VALUE"].ToString());
                    // 숫자로 된 8자리 string date를 사용하면 SeriesPoint에서는 숫자로 인식함. Convert 필요
                    string convertedDate = date == "AVG" ? date : date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
                    series.Points.Add(new SeriesPoint(convertedDate, value));
                }
                chartControl.Series.Add(series);
            }
        }

        private void AttachAvgData(DataView dv, List<string> listDate, string[] dataAreaList)
        {
            // 차트에 평균값 기록을 위해 dv에 강제로 입력
            DataTable dt = dv.Table;

            foreach (string area in dataAreaList)
            {
                DataRow newRow = dt.NewRow();
                newRow[LoadStatData.TARGET_DATE] = "AVG";
                newRow["AREA"] = area;

                double value = 0f;

                foreach (string date in listDate)
                {
                    DataRow valueRow = dv.Table.Select(LoadStatData.TARGET_DATE + " = '" + date + "' AND AREA = '" + area + "'").FirstOrDefault();
                    value += double.Parse(valueRow["VALUE"].ToString());
                }

                newRow["VALUE"] = value / listDate.Count;
                dt.Rows.Add(newRow);
            }

            listDate.Add("AVG");
        }

        private DataView SummaryData(DataTable dt, List<string> colNameList, List<List<string>> valueList, string[] arrDataArea)
        {
            DataTable resultDT = new DataTable();
            resultDT.Columns.Add(LoadStatData.TARGET_DATE, typeof(string));
            resultDT.Columns.Add("AREA", typeof(string));
            resultDT.Columns.Add("VALUE", typeof(float));

            DataTable colAvgDT = new DataTable();
            string strSelect = string.Empty;

            // avrDT에 선택된 모든 행 데이터 추가
            for (int i = 0; i < valueList.Count; i++)
            {
                for (int j = 0; j < colNameList.Count; j++)
                {
                    strSelect += colNameList[j] + " = '" + valueList[i][j] + "'";
                    if (j != colNameList.Count - 1)
                        strSelect += " AND ";
                }

                if (colAvgDT.Rows.Count == 0)
                {
                    DataRow[] selectRows = dt.Select(strSelect);
                    if (selectRows.Length != 0)
                        colAvgDT = selectRows.CopyToDataTable();
                }
                else
                {
                    foreach (DataRow sRow in dt.Select(strSelect))
                    {
                        colAvgDT.ImportRow(sRow);
                    }
                }
                strSelect = string.Empty;
            }

            // TARGET_DATE별 각 항목마다 평균값 구하기 (Column Avg)
            foreach (DataRow row in colAvgDT.Rows)
            {
                if (resultDT.Select(LoadStatData.TARGET_DATE + " = '" + row[LoadStatData.TARGET_DATE] + "'").Length == 0)
                {
                    foreach (string area in arrDataArea)
                    {
                        DataRow resRow = resultDT.NewRow();
                        resRow[LoadStatData.TARGET_DATE] = row[LoadStatData.TARGET_DATE];
                        resRow["AREA"] = area;
                        resRow["VALUE"] = colAvgDT.Compute("Avg(" + area + ")", LoadStatData.TARGET_DATE + " = " + row[LoadStatData.TARGET_DATE]);

                        resultDT.Rows.Add(resRow);
                    }
                }
            }

            return new DataView(resultDT, string.Empty, string.Empty, DataViewRowState.CurrentRows);
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

        private string[] GetDataAreaList(PivotGridControl pivotGrid)
        {
            List<string> list = new List<string>();

            var dataAreaList = pivotGrid.GetFieldsByArea(PivotArea.DataArea);
            foreach (var field in dataAreaList)
            {
                if (field.DataType == typeof(string))
                    continue;

                list.Add(field.FieldName);
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
            List<int> rows = GetSelectionList();
            FillChart((PivotGridControl)sender, rows);
        }


        private void pivotGridControl1_CellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            if (e.DataField == null)
                return;

            //if (e.GetFieldValue(e.DataField) != null && e.GetFieldValue(e.DataField).ToString() == "0")
            //{
            //    e.DisplayText = string.Empty;
            //}
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void buttonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.PIVOTEXPORT.ExportToExcel(this.pivotGridControl1);
        }

        #endregion

        private void pivotGridControl1_CustomCellValue(object sender, PivotCellValueEventArgs e)
        {
            if (e.ColumnValueType == PivotGridValueType.GrandTotal)
            {
                if (e.SummaryValue != null)
                    e.Value = e.SummaryValue.Average;
            }
        }

        private void radioQueryCondition_EditValueChanged(object sender, EventArgs e)
        {
            SetRadioQueryConditions();
        }

        private void SetRadioQueryConditions()
        {
            switch (radioQueryCondition.EditValue)
            {
                case LoadStatData.STEP_ID:
                    _SelectedFirstCombo = cbStepID;
                    _SelectedFirstColumn = LoadStatData.STEP_ID;
                    cbStepID.Enabled = true;
                    cbEqpGrp.Enabled = false;
                    cbEqpModel.Enabled = false;
                    break;
                case LoadStatData.EQP_GROUP:
                    _SelectedFirstCombo = cbEqpGrp;
                    _SelectedFirstColumn = LoadStatData.EQP_GROUP;
                    cbStepID.Enabled = false;
                    cbEqpGrp.Enabled = true;
                    cbEqpModel.Enabled = false;
                    break;
                case LoadStatData.EQP_MODEL:
                    _SelectedFirstCombo = cbEqpModel;
                    _SelectedFirstColumn = LoadStatData.EQP_MODEL;
                    cbStepID.Enabled = false;
                    cbEqpGrp.Enabled = false;
                    cbEqpModel.Enabled = true;
                    break;
                default:
                    break;
            }
            SetCbProdIDList(_SelectedFirstCombo);
        }

        private void cbStepID_EditValueChanged(object sender, EventArgs e)
        {
            SetCbProdIDList(cbStepID);
        }

        private void cbEqpGrp_EditValueChanged(object sender, EventArgs e)
        {
            SetCbProdIDList(cbEqpGrp);
        }

        private void cbEqpModel_EditValueChanged(object sender, EventArgs e)
        {
            SetCbProdIDList(cbEqpModel);
        }

        private void cbProdID_EditValueChanged(object sender, EventArgs e)
        {
            string firstQuery = GetQueryString(_SelectedFirstCombo);
            SetCbEqpIDList(cbProdID, firstQuery);
        }

        private string GetQueryString(BarEditItem control)
        {
            string selectQuery = string.Empty;
            string[] selectedConditions = control.EditValue.ToString().Replace(" ", "").Split(',');
            for (int i = 0; i < selectedConditions.Length; i++)
            {
                selectQuery += _SelectedFirstColumn + " = '" + selectedConditions[i] + "'";
                if (i != selectedConditions.Length - 1)
                    selectQuery += " OR ";
            }

            return selectQuery;
        }

        private void SetCbProdIDList(BarEditItem control)
        {
            List<object> listProdID = new List<object>();
            string selectQuery = GetQueryString(control);

            foreach (DataRow row in dtEqpAndArrange.Select(selectQuery))
            {
                if (!listProdID.Contains(row["PRODUCT_ID"]))
                    listProdID.Add(row["PRODUCT_ID"]);
            }

            EngControlHelper.SetControl_CheckedComboBase(cbProdID, listProdID);
            SetCbEqpIDList(cbProdID, selectQuery);
        }

        private void SetCbEqpIDList(BarEditItem control, string andCondition)
        {
            List<object> listEqpID = new List<object>();
            string selectQuery = string.Empty;
            string[] selectedConditions = control.EditValue.ToString().Replace(" ", "").Split(',');
            for (int i = 0; i < selectedConditions.Length; i++)
            {
                if (i == 0)
                    selectQuery += "(";

                selectQuery += "PRODUCT_ID" + " = '" + selectedConditions[i] + "'";
                
                if (i != selectedConditions.Length - 1)
                    selectQuery += " OR ";
            }

            selectQuery += ") AND (" + andCondition + ")";

            foreach (DataRow row in dtEqpAndArrange.Select(selectQuery))
            {
                if (!listEqpID.Contains(row[LoadStatData.EQP_ID]))
                    listEqpID.Add(row[LoadStatData.EQP_ID]);
            }

            EngControlHelper.SetControl_CheckedComboBase(cbEqpID, listEqpID);
        }
    }
}