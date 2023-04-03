using DevExpress.XtraCharts;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraBars;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using DevExpress.Utils;

namespace SmartAPS.UI.Analysis
{
	public partial class StepMoveTrendView : MyXtraPivotGridTemplate
    {
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
            if (this.radioPlanDate.EditValue as string == "D")
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
            public string EqpID { get; private set; }
            public string TimeInfo { get; set; }

            public int InQty { get; private set; }
            public int OutQty { get; private set; }

            public ResultItem(string lineID, string productID, string processID, string stepID, double stepSeq, string eqpID, string timeInfo)
            {
                this.LineID = lineID;
                this.ProductID = productID;
                this.ProcessID = processID;
                this.StepID = stepID;
                this.StepSeq = stepSeq;
                this.EqpID = eqpID;
                this.TimeInfo = timeInfo;
            }

            public ResultItem(string lineID, string productID, string processID, string stepID, double stepSeq, string eqpID, string timeInfo, int inQty, int outQty)
            {
                this.LineID = lineID;
                this.ProductID = productID;
                this.ProcessID = processID;
                this.StepID = stepID;
                this.StepSeq = stepSeq;
                this.EqpID = eqpID;

                this.TimeInfo = timeInfo;

                this.InQty = inQty;
                this.OutQty = outQty;
            }

            public void UpdateQty(int inQty, int outQty)
            {
                this.InQty += inQty;
                this.OutQty += outQty;
            }
        }

        #endregion

        public StepMoveTrendView()
        {
            InitializeComponent();
        }

        public StepMoveTrendView(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            InitializeComponent();
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
            var dt = GetData();
            BindData(dt);
        }

        private void SetControls()
        {
            ShowTotal(this.pivotGridControl1);

            this.radioPlanDate.EditValue = "D";

			int baseMinute = ShopCalendar.StartTime.Minutes; //timespan

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
                
                table.Rows.Add(
                    item.LineID ?? string.Empty,
                    item.ProductID ?? string.Empty,
                    item.ProcessID ?? string.Empty,
                    item.StepID ?? string.Empty,
                    item.EqpID ?? string.Empty,
                    item.TimeInfo,
                    item.InQty,
                    item.OutQty
                );

                string stepKey = item.StepID;
                _stepIndexs[stepKey] = item.StepSeq;
            }
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
            this.pivotGridControl1.BeginUpdate();

            this.pivotGridControl1.ClearPivotGridFields();
            this.pivotGridControl1.CreatePivotGridFields(table);

            this.pivotGridControl1.DataSource = table.DataTable;

            this.pivotGridControl1.Fields["STD_STEP"].SortMode = PivotSortMode.Custom;

            //pivotGridControl1.CustomCellDisplayText += pivotGridControl1_CellDisplayText;
            this.pivotGridControl1.EndUpdate();

            this.pivotGridControl1.BestFitColumnArea();

            pivotGridControl1.Fields["OUT_QTY"].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields["OUT_QTY"].CellFormat.FormatString = "#,##0";

            pivotGridControl1.Fields["IN_QTY"].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            pivotGridControl1.Fields["IN_QTY"].CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.Fields["STD_STEP"].SortMode = PivotSortMode.Custom;

            this.SetPivotGridLayout(this.pivotGridControl1, false);
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

            //bool isOnlyMainStep = this.ShowSubStep == false;

            var stepMove = MyHelper.DATASVC.GetEntityData<STEP_MOVE>(this.Result);            
            foreach (STEP_MOVE item in stepMove)
            {

                if (item.PLAN_DATE < this.QueryStartTime)
                    continue;

                if (item.PLAN_DATE >= this.QueryEndTime)
                    continue;

                var std_info = stdStepDic.TryGetValue(item.STEP_ID, out double re);
                var std_seq = std_info == true ? re : 0;

                string lineID = item.LINE_ID;
                string productID = item.PRODUCT_ID;
                string processID = item.PROCESS_ID;
                string stdStep = item.STEP_ID;
                double stepSeq = std_seq;
                string eqpID = item.EQP_ID;

                if (isFirst)
                {
                    foreach (string dayHour in _dayHourList)
                    {
                        string dateString = GetDateString(dayHour);
                        string k = lineID + productID + processID + stdStep + eqpID + dateString;

                        ResultItem padding;
                        if (items.TryGetValue(k, out padding) == false)
                        {
                            padding = new ResultItem(item.LINE_ID, item.PRODUCT_ID, item.PROCESS_ID, item.STEP_ID, stepSeq, item.EQP_ID, dateString, 0, 0);

                            items.Add(k, padding);
                        }
                    }
                    isFirst = false;
                }

                DateTime DateChange = GetRptDate_1Hour(item.PLAN_DATE, ShopCalendar.StartTime.Minutes);
                DateTime planDate = GetPlanDate(DateChange);
                string dateStr = GetDateString(planDate.ToString("yyyyMMddHHmm")); //GetDateString(planDate);

                string key = lineID + productID + processID + stdStep + eqpID + dateStr;

                ResultItem ri = null;
                if (items.TryGetValue(key, out ri) == false)
                    items.Add(key, ri = new ResultItem(lineID, productID, processID, stdStep, stepSeq, eqpID, dateStr));

                ri.UpdateQty((int)item.IN_QTY, (int)item.OUT_QTY);
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

            dt.AddColumn("EQP_ID", "EQP_ID", typeof(string), PivotArea.FilterArea, null, null);
            dt.AddColumn("TARGET_DATE", "TARGET_DATE", typeof(string), PivotArea.ColumnArea, null, null);
            dt.AddColumn("IN_QTY", "IN_QTY", typeof(float), PivotArea.DataArea, null, null);
            dt.AddColumn("OUT_QTY", "OUT_QTY", typeof(float), PivotArea.DataArea, null, null);

            dt.AddDataTablePrimaryKey(
                    new DataColumn[]
                    {
                        dt.Columns["LINE_ID"],
                        dt.Columns["PRODUCT_ID"],
                        dt.Columns["PROCESS_ID"],
                        dt.Columns["STD_STEP"],
                        dt.Columns["EQP_ID"],
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

                int inQty = row.GetInt32("IN_QTY");
                if (inQty > 0)
                    findRow["IN_QTY"] = findRow.GetInt32("IN_QTY") + inQty;

                int outQty = row.GetInt32("OUT_QTY");
                if (outQty > 0)
                    findRow["OUT_QTY"] = findRow.GetInt32("OUT_QTY") + outQty;
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

            List<string> qtyColList = new List<string> { "IN_QTY", "OUT_QTY" };
            foreach(var qtyCol in qtyColList)
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
                row["IN_QTY"] = 0;
                row["OUT_QTY"] = 0;

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

            //bool onlyOne = rows.Count == 1;

            foreach (int rowIdx in rows)
            {
                var valueList = GetRowValueList(pivotGrid, rowIdx);
                DataView dv = SummaryData(dt, colNameList, valueList);

                string seriesName = Concat('@', valueList.ToArray()); //MyHelper.STRING.ConcatKey

                if (dataAreaList.Length > 1)
                {
                    foreach (string area in dataAreaList)
                    {
                        AddSeries(seriesName + "@" + area, ViewType.Line, new string[] { area }, dv);
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

		private void buttonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void radioPlanDate_EditValueChanged(object sender, EventArgs e)
        {
            if (this.radioPlanDate.EditValue.Equals("D"))
                DateTimeFormatChange(DefaultBoolean.False, "yyyy-MM-dd");
            else
                DateTimeFormatChange(DefaultBoolean.True, "yyyy-MM-dd HH:mm:ss");
        }
    }
}
