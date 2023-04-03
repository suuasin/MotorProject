using DevExpress.XtraCharts;
using DevExpress.XtraBars;
using DevExpress.XtraPivotGrid;
using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using DevExpress.Utils;

namespace SmartAPS.UI.Analysis
{
	public partial class StepWipTrendView : MyXtraPivotGridTemplate
    {
        private Dictionary<string, ResultItem> _dict;
        private SortedSet<string> _dayHourList;
        private Dictionary<string, double> _stepIndexs;
        private List<int> _allRowsList;
        private bool firstChart = true;


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
                return Convert.ToDateTime(this.editDateTime.EditValue).AddHours(Convert.ToInt32(this.editDateSpin.EditValue) * ShopCalendar.ShiftHours); 
            }
        }

        private DateTime GetPlanDate(DateTime t)
        {
            DateTime planDate = t;
            if (MyHelper.ENUM.Equals(this.radioPlanDate.EditValue, "D"))
                planDate = ShopCalendar.StartTimeOfDayT(t);
            return planDate;
        }

        public DateTime GetRptDate_1Hour(DateTime t, int baseMinute)
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
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result); //planstart
            }
        }

        #region Internal Class : ResultItem
        internal class ResultItem
        {
            public string LINE_ID;
            public string PROD_ID;
            public string PROC_ID;
            public string STD_STEP;
            public double STEP_SEQ;
            public string DATE_INFO;
            public float WAIT_LOT_QTY;
            public float RUN_LOT_QTY;
        }
        #endregion

        public StepWipTrendView()
        {
            InitializeComponent();
        }

        public StepWipTrendView(IServiceProvider serviceProvider)
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
            var dt = GetData();
            BindData(dt);
        }

        #region SetControl
        private void SetControls()
        {
            ShowTotal(this.pivotGridControl1);

            //int baseMinute = 60; //timespan
            this.radioPlanDate.EditValue = "D";

            int baseMinute = ShopCalendar.StartTime.Minutes;

            this.repoDateTime.EditMask = "yyyy-MM-dd HH:mm:ss";
            this.repoDateTime.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            this.repoDateTime.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
            this.editDateTime.EditValue = GetRptDate_1Hour(this.PlanStartTime, baseMinute);
            this.editDateSpin.EditValue = MyHelper.DATASVC.GetPlanPeriod(this.Result);

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

        new private XtraPivotGridHelper.DataViewTable GetData()
        {
            SetDateRanges();

            XtraPivotGridHelper.DataViewTable table = CreateDataViewSchema();
            FillData(table);

            return table;
        }

        #region FillData
        private void FillData(XtraPivotGridHelper.DataViewTable dt)
        {
            LoadData();

            var stepWip = _dict.Values;

            _stepIndexs = new Dictionary<string, double>();

            var table = dt.DataTable;
            //var query = from ps in stdStepList
            //            join p in stepWip
            //            on ps.SHOP_ID + ps.STEP_ID equals p.LINE_ID + p.STD_STEP into temp
            //            from tp in temp.DefaultIfEmpty()
            //            select new
            //            {
            //                SHOP_ID = tp != null ? tp.LINE_ID : ps.SHOP_ID,
            //                STD_STEP_ID = tp != null ? tp.STD_STEP : ps.STEP_ID,
            //                PROD_ID = tp != null ? tp.PROD_ID : (sample != null ? sample.PROD_ID : null),
            //                PROD_VER = tp != null ? tp.PROD_VER : (sample != null ? sample.PROD_VER : null),
            //                PRODUCTION_TYPE = tp != null ? tp.PRODUCTION_TYPE : (sample != null ? sample.PRODUCTION_TYPE : null),
            //                DATE_INFO = tp != null ? tp.DATE_INFO : (sample != null ? sample.DATE_INFO : null),
            //                WAIT_LOT_QTY = tp != null ? tp.WAIT_LOT_QTY : 0,
            //                RUN_LOT_QTY = tp != null ? tp.RUN_LOT_QTY : 0,
            //                STD_STEP_SEQ = tp != null ? tp.STEP_SEQ : ps.STEP_SEQ,
            //            };

            foreach (var item in stepWip)
            {
                if (item.PROD_ID == null)
                    continue;

                table.Rows.Add(item.LINE_ID ?? string.Empty,
                            item.PROD_ID ?? string.Empty,
                            item.PROC_ID ?? string.Empty,
                            item.STD_STEP ?? string.Empty,
                            item.DATE_INFO ?? string.Empty,
                            item.WAIT_LOT_QTY,
                            item.RUN_LOT_QTY,
                            item.WAIT_LOT_QTY + item.RUN_LOT_QTY);

                string stepKey = item.STD_STEP;
                _stepIndexs[stepKey] = item.STEP_SEQ;
            }
        }
        #endregion

        private void BindData(XtraPivotGridHelper.DataViewTable table)
        {
            this.pivotGridControl1.BeginUpdate();

            this.pivotGridControl1.ClearPivotGridFields();
            this.pivotGridControl1.CreatePivotGridFields(table);

            this.pivotGridControl1.DataSource = table.DataTable;

            this.pivotGridControl1.OptionsView.ShowFilterHeaders = true;

            for (int i = 0; i < this.pivotGridControl1.Fields.Count; i++)
            {
                if (this.pivotGridControl1.Fields[i].FieldName == "WAIT_LOT_QTY" || this.pivotGridControl1.Fields[i].FieldName == "RUN_LOT_QTY")
                    this.pivotGridControl1.Fields[i].Area = PivotArea.FilterArea;
            }
            pivotGridControl1.CustomCellDisplayText += pivotGridControl1_CellDisplayText;

            pivotGridControl1.Fields["TOTAL_QTY"].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields["TOTAL_QTY"].CellFormat.FormatString = "#,##0";

            pivotGridControl1.Fields["RUN_LOT_QTY"].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields["RUN_LOT_QTY"].CellFormat.FormatString = "#,##0";

            pivotGridControl1.Fields["WAIT_LOT_QTY"].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields["WAIT_LOT_QTY"].CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.Fields["STD_STEP"].SortMode = PivotSortMode.Custom;

            this.pivotGridControl1.EndUpdate();
            pivotGridControl1.BestFitColumnArea();

            this.SetPivotGridLayout(this.pivotGridControl1, false);

        }

        #region GetDateString
        private string GetDateString(string value, bool withTime = true)
        {
            DateTime primary;

            if (value == null)
            {
                primary = DateTime.MinValue;
                return primary.ToString("yyyyMMddHHmm");
            }

            //value = MyHelper.STRING.Trim(value);
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
            value = tmp;

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
        #endregion

        private void SetDateRanges()
        {
            _dayHourList = new SortedSet<string>();

            //차트 간격 변경
            float interval = 1f;
            if (radioPlanDate.EditValue as string == "D")
                interval = 24f;
            DateTime st = GetPlanDate(this.QueryStartTime);
            DateTime et = this.QueryEndTime;
            //DateTime st = this.QueryStartTime;
            //DateTime et = this.QueryEndTime;
            
            //add PlanStartTime
            _dayHourList.Add(st.ToString("yyyyMMddHHmm"));

            int baseMinute = ShopCalendar.StartTime.Minutes;
            DateTime baseT = GetRptDate_1Hour(st, baseMinute);

            for (DateTime t = baseT; t < et; t = t.AddHours(interval))
            {
                string str = t.ToString("yyyyMMddHHmm");
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

        #region LoadData
        private void LoadData()
        {
            _dict = new Dictionary<string, ResultItem>();

            bool isFirst = true;

            var stepSeqDic = GetStepSeqDic();

            var stepWip = MyHelper.DATASVC.GetEntityData<STEP_WIP>(this.Result);
            foreach (var item in stepWip)
            {
                if (item.TARGET_DATE < this.QueryStartTime)
                    continue;
                
                if (item.TARGET_DATE >= this.QueryEndTime)
                    continue;

                string stepID = item.STEP_ID;
                var std_info = stepSeqDic.TryGetValue(item.STEP_ID, out double re);
                var std_seq = std_info == true ? re : 0;

                if (isFirst)
                {
                    foreach (string dayHour in _dayHourList)
                    {
                        string dateString = GetDateString(dayHour); //GetDateString(date);
                        string k = item.LINE_ID + item.PRODUCT_ID + item.PROCESS_ID + stepID + dateString;

                        ResultItem padding;

                        if (_dict.TryGetValue(k, out padding) == false)
                        {
                            padding = new ResultItem();

                            padding.LINE_ID = item.LINE_ID;
                            padding.STEP_SEQ = std_seq;
                            padding.PROD_ID = item.PRODUCT_ID;
                            padding.PROC_ID = item.PROCESS_ID;
                            padding.STD_STEP = item.STEP_ID;

                            padding.DATE_INFO = dateString;

                            padding.WAIT_LOT_QTY = 0;
                            padding.RUN_LOT_QTY = 0;

                            _dict.Add(k, padding);
                        }
                    }

                    isFirst = false;
                }


                ResultItem ri;

                DateTime dateChange = GetRptDate_1Hour(item.TARGET_DATE, ShopCalendar.StartTime.Minutes);
                DateTime planDate = GetPlanDate(dateChange);
                string dateStr2 = GetDateString(planDate.ToString("yyyyMMddHHmm"));

                string key = item.LINE_ID + item.PRODUCT_ID + item.PROCESS_ID + stepID + dateStr2;

                if (_dict.TryGetValue(key, out ri) == false)
                {
                    ri = new ResultItem();
                    ri.LINE_ID = item.LINE_ID;
                    ri.PROD_ID = item.PRODUCT_ID;
                    ri.PROC_ID = item.PROCESS_ID;
                    ri.STD_STEP = item.STEP_ID;
                    ri.STEP_SEQ = std_seq;
                    ri.DATE_INFO = dateStr2;

                    _dict.Add(key, ri);
                }

                ri.WAIT_LOT_QTY += Convert.ToSingle(item.WAIT_LOT_QTY);
                ri.RUN_LOT_QTY += Convert.ToSingle(item.RUN_LOT_QTY);
            }
        }
        #endregion

        #region CreateDataViewTable
        private XtraPivotGridHelper.DataViewTable CreateDataViewSchema()
        {
            XtraPivotGridHelper.DataViewTable dt = new XtraPivotGridHelper.DataViewTable();

            dt.AddColumn("LINE_ID", "LINE_ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("PRODUCT_ID", "PRODUCT_ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("PROCESS_ID", "PROCESS_ID", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("STD_STEP", "STD_STEP", typeof(string), PivotArea.RowArea, null, null);

            dt.AddColumn("TARGET_DATE", "TARGET_DATE", typeof(string), PivotArea.ColumnArea, null, null);

            dt.AddColumn("WAIT_LOT_QTY", "WAIT_LOT_QTY", typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn("RUN_LOT_QTY", "RUN_LOT_QTY", typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn("TOTAL_QTY", "TOTAL_QTY", typeof(float), PivotArea.DataArea, null, null);

            dt.Columns["TOTAL_QTY"].DefaultValue = 0.0f;

            dt.AddDataTablePrimaryKey(
                    new DataColumn[]
                    {
                        dt.Columns["LINE_ID"],
                        dt.Columns["PRODUCT_ID"],
                        dt.Columns["PROCESS_ID"],
                        dt.Columns["STD_STEP"],
                        dt.Columns["TARGET_DATE"]
                    }
                );

            return dt;
        }
        #endregion

        //private List<StdStep> GetStdStepList(string areaID)
        //{
        //    var stdStep = MyHelper.DATASVC.GetEntityData<StdStep>(this.Result);

        //    List<StdStep> list = new List<StdStep>();

        //    var finds = stdStep.Where(t => t.AREA_ID == areaID).OrderBy(t => t.STEP_SEQ);
        //    if (finds == null || finds.Count() == 0)
        //        return list;

        //    List<string> keyList = new List<string>();
        //    foreach (var item in finds)
        //    {
        //        string key = item.SHOP_ID + item.STEP_ID;
        //        if (keyList.Contains(key))
        //            continue;

        //        keyList.Add(key);

        //        list.Add(item);
        //    }

        //    return list;
        //}

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

                int waitQty = row.GetInt32("WAIT_LOT_QTY");
                if (waitQty > 0)
                    findRow["WAIT_LOT_QTY"] = findRow.GetInt32("WAIT_LOT_QTY") + waitQty;

                int runQty = row.GetInt32("RUN_LOT_QTY");
                if (runQty > 0)
                    findRow["RUN_LOT_QTY"] = findRow.GetInt32("RUN_LOT_QTY") + runQty;

                int totalQty = row.GetInt32("TOTAL_QTY");
                if (totalQty > 0)
                    findRow["TOTAL_QTY"] = findRow.GetInt32("TOTAL_QTY") + totalQty;
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

            List<string> qtyColList = new List<string> { "WAIT_LOT_QTY", "RUN_LOT_QTY", "TOTAL_QTY" };
            foreach (var qtyCol in qtyColList)
            {
                if (!result.Columns.Contains(qtyCol))
                    result.Columns.Add(qtyCol, typeof(int));
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
                row["WAIT_LOT_QTY"] = 0;
                row["RUN_LOT_QTY"] = 0;
                row["TOTAL_QTY"] = 0;

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
      
        #region DrawChart

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

            DataTable dt = (DataTable)pivotGrid.DataSource;
            var colNameList = GetColNameList(pivotGrid);

            int i = 0;
            foreach (int rowIdx in rows)
            {
                var valueList = GetRowValueList(pivotGrid, rowIdx);
                DataView dv = SummaryData(dt, colNameList, valueList);

                string seriesName = Concat('@',valueList.ToArray()); //@ : KeySeparator

                this.chartControl.Series.Add(seriesName, ViewType.Line);
                this.chartControl.Series[i].ArgumentDataMember = "TARGET_DATE";
                this.chartControl.Series[i].ValueDataMembers.AddRange(dataAreaList);

                this.chartControl.Series[i].DataSource = dv;
                this.chartControl.Series[i].LegendText = string.IsNullOrEmpty(seriesName) ? "Total" : seriesName;
                i++;
            }
        }
        #endregion

        //private StdStep FindMainStep(string shopID, string stdStepID)
        //{
        //    var list = _stdStepList;

        //    int index = list.FindIndex(t => t.SHOP_ID == shopID && t.STEP_ID == stdStepID);
        //    if (index < 0)
        //        return null;

        //    int count = list.Count;
        //    for (int i = index; i < count; i++)
        //    {
        //        var stdStep = list[i];
        //        if (stdStep.STEP_TYPE == "MAIN")
        //            return stdStep;
        //    }

        //    return null;
        //}

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

        #region ConcatKey
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

        #endregion

        #region Event 
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
       
        private void pivotGridControl1_CellDoubleClick(object sender, PivotCellEventArgs e)
        { 
            if (e.ColumnField == null) // TOTAL Double Click 시 방지
                return;

            string targetTime = pivotGridControl1.GetFieldValue(e.ColumnField, e.ColumnIndex) as string;

            IExperimentResultItem targetResult = this.Result;

            DateTime target;
            if (this.radioPlanDate.EditValue.Equals("D"))
            {
                target = DateTime.ParseExact(targetTime, "yyyyMMdd", null);
                DateTime result = new DateTime(target.Year, target.Month, target.Day, ShopCalendar.StartTime.Hours, ShopCalendar.StartTime.Minutes, 0);
                targetTime = result.ToString("yyyyMMddHHmm");
            }

            ShowStepWipSnapshot(targetResult, targetTime);
        }

        private void ShowStepWipSnapshot(IExperimentResultItem targetResult, string targetTime)
        {
            var view = new StepWipSnapshotView(targetResult, targetTime);
            var dialog = new Form();
            
            view.Dock = DockStyle.Fill;
            dialog.Size = new Size(900, 700);
            dialog.Controls.Clear();
            dialog.Controls.Add(view);
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.Show();
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

        private void pivotGridControl1_CellSelectionChanged(object sender, EventArgs e)
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

        private void buttonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.PIVOTEXPORT.ExportToExcel(this.pivotGridControl1);
        }

        private void CheckSubStep_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void CheckProdVersion_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void radioPlanDate_EditValueChanged(object sender, EventArgs e)
        {
            if (this.radioPlanDate.EditValue.Equals("D"))
                DateTimeFormatChange(DefaultBoolean.False, "yyyy-MM-dd");
            else
                DateTimeFormatChange(DefaultBoolean.True, "yyyy-MM-dd HH:mm:ss");
        }
        #endregion
    }
    #endregion
} 