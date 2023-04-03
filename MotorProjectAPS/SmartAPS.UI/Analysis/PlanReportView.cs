using DevExpress.DataProcessing;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
//using SmartAPS.MozartStudio.Helper;
//using SmartAPS.MozartStudio.Utils;
//using Template.Lcd.Scheduling.Inputs;
//using Template.Lcd.Scheduling.Outputs;
using SmartAPS.UI.Utils;
using SmartAPS.UI.Helper;
using SmartAPS.Outputs;
using SmartAPS.Inputs;

namespace SmartAPS.UI.Analysis
{
    public partial class PlanReportView : MyXtraGridTemplate
    {
        public enum CategoryType
        {
            PLAN,    
            JOBCHANGE,
            WIPGAP,
            NONE



        }

        private DateTime PlanStartTime { get; set; }

        public DateTime PlanEndTime { get; set; }

        public PlanReportView()
        {
            InitializeComponent();
        }
        
        public PlanReportView(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetInitializeOption(this.gridControl1);

            SetPlanDate();

            RunQuery();

            UseSaveLayout();
        }

        private void SetPlanDate()
        {
            this.PlanStartTime = MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
            this.PlanEndTime = PlanStartTime.AddDays(30);
        }

        private void UseSaveLayout()
        {
            var gridView = this.gridView1;
            SetGridLayout(gridView);
        }

        protected override void Query()
        {
            var dt = GetData();

            BindData(dt);
            DrawChart(dt);
        }

        new private DataTable GetData()
        {
            DataTable dt = CreateSchema();

            var datas = CalData();
            var Setup = 0;
            if (datas != null && datas.Count > 0)
            {
                foreach (var info in datas.Values)
                {
                    DataRow row = dt.NewRow();

                    row[Schema.ITEMKEY] = info.ItemKey;
                    row[Schema.CATEGORY] = info.Category;
                    row[Schema.ITEM] = info.ItemName;
                    row[Schema.DESCRIPTION] = info.Description;
                    row[Schema.WEIGHT] = info.Weight;
                    row[Schema.TARGET] = info.Target;
                    row[Schema.PLAN] = info.Plan;
                    row[Schema.SCORE] = info.Score;

                    if(info.Category == "JOBCHANGE" )
                    {
                        Setup += info.Plan;
                    }

                    dt.Rows.Add(row);
                }

                //Total Row
                DataRow totalRow = dt.NewRow();
                totalRow[Schema.ITEMKEY] = Schema.TOTAL;
                totalRow[Schema.CATEGORY] = Schema.TOTALSCORE;
                totalRow[Schema.WEIGHT] = dt.AsEnumerable().Sum(t => t.Field<double>(Schema.WEIGHT));
                totalRow[Schema.PLAN] = Setup;
                totalRow[Schema.SCORE] = dt.AsEnumerable().Sum(t => t.Field<decimal>(Schema.SCORE));
                dt.Rows.Add(totalRow);
            }

            dt.DefaultView.Sort = "ITEMKEY ASC";
            dt = dt.DefaultView.ToTable();

            return dt;
        }

        private Dictionary<string, ResultData> CalData()
        {
            var items = ImportData_PlanReportTarget();
            if (items == null)
                return null;

            var eqpTable = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(this.Result);
            var stepTable = MyHelper.DATASVC.GetEntityData<STEP_MOVE>(this.Result);
            var configTable = MyHelper.DATASVC.GetEntityData<CONFIG>(this.Result);

            if (eqpTable == null || eqpTable.Count() == 0 ||
                stepTable == null || stepTable.Count() == 0 ||
                configTable == null || configTable.Count() == 0)
                return null;

            foreach (var item in items)
            {
                Cal_Item(item.Value, eqpTable, stepTable, configTable);
            }

            return items;
        }

        private Dictionary<string, ResultData> ImportData_PlanReportTarget()
        {
            Dictionary<string, ResultData> datas = new Dictionary<string, ResultData>();

            var table = MyHelper.DATASVC.GetEntityData<PLAN_REPORT_TARGET>(this.Result);

            if (table == null && table.Count() == 0)
            {
                return null;
            }

            foreach (var it in table)
            {
                string itemKey = it.ITEM_KEY;
                string itemName = it.ITEM_NAME;
                string category = it.CATEGORY;
                string description = it.DESCRIPTION;
                double weight = Convert.ToDouble(it.WEIGHT);
                int target = Convert.ToInt32(it.TARGET);

                string key = itemKey;

                if (datas.ContainsKey(key))
                    continue;

                ResultData info = new ResultData(category,
                                                 itemKey,
                                                 itemName,
                                                 description,
                                                 weight,
                                                 target,
                                                 0);

                datas.Add(key, info);
            }

            return datas;
        }

        private void Cal_Item(ResultData item, IEnumerable<EQP_PLAN> eqpTable, IEnumerable<STEP_MOVE> stepTable, IEnumerable<CONFIG> configTable)
        {
            var ctype = item.CategoryType;
            if (ctype == CategoryType.NONE)
                return;

            var parseInfos1 = ParseParam_TargetItem(item, configTable);
            if (parseInfos1 == null || parseInfos1.Count == 0)
                return;

            var parseInfos2 = ParseParam_JC(item, configTable);

            if (ctype == CategoryType.PLAN)
            {
                item.Plan = Cal_Move(stepTable, parseInfos1);
            }
            else if (ctype == CategoryType.JOBCHANGE)
            {
                item.Plan = Cal_JC(eqpTable, parseInfos1, parseInfos2);
                item.Setup = Cal_JC(eqpTable, parseInfos1, parseInfos2);
            }
            else if (ctype == CategoryType.WIPGAP)
            {
                item.Plan = Cal_WIP_GAP(eqpTable, parseInfos1);
            }


            double weight2 = 1;
            if (parseInfos2 != null)
                weight2 = parseInfos2.Item2;

            item.SetScore(weight2);
        }

        private List<Tuple<string, string>> ParseParam_TargetItem(ResultData item, IEnumerable<CONFIG> configTable)
        {
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();

            if (configTable == null)
                return list;


            var find = configTable.Where(t => t.CODE_GROUP == "UI_PLANREPORT_TARGET_ITEM" && t.CODE_NAME == item.ItemKey).FirstOrDefault();

            if (find == null)
                return list;

            string codeValue = find.CODE_VALUE;
            if (string.IsNullOrEmpty(codeValue))
                return list;

            var mainArr = codeValue.Split(';');
            foreach (var str in mainArr)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                var subArr = str.Split('=');
                int count = subArr.Length;
                if (subArr == null || count == 0)
                    continue;

                var info = new Tuple<string, string>(subArr[0], (count < 2 ? null : subArr[1]));
                list.Add(info);
            }

            return list;
        }

        private Tuple<int, double, string, string> ParseParam_JC(ResultData item, IEnumerable<CONFIG> configTable)
        {
            if (configTable == null)
                return null;

            var find = configTable.Where(t => t.CODE_GROUP == "UI_PLANREPORT_JC_PARAMS" && t.CODE_NAME == item.ItemKey).FirstOrDefault();

            if (find == null)
                return null;

            string codeValue = find.CODE_VALUE;
            if (string.IsNullOrEmpty(codeValue))
                return null;

            var mainArr = codeValue.Split(';');

            int count = mainArr.Length;
            if (mainArr == null || count == 0)
                return null;

            int v0 = MyHelper.CONVERT.ToInt32(mainArr[0], 0);
            double v1 = count < 2 ? 0 : MyHelper.CONVERT.ToDouble(mainArr[1], 0);
            string v2 = count < 3 ? null : mainArr[2];
            string v3 = count < 4 ? null : mainArr[3];

            var info = new Tuple<int, double, string, string>(v0, v1, v2, v3);

            return info;
        }

        private int Cal_Move(IEnumerable<STEP_MOVE> table, List<Tuple<string, string>> parseInfos)
        {
            if (table == null || table.Count() == 0)
                return 0;

            if (parseInfos == null || parseInfos.Count == 0)
                return 0;

            DateTime planEndTime = this.PlanEndTime;

            int sum = 0;
            foreach (var it in table)
            {
                string processID = it.PROCESS_ID;
                string eqpGroupID = it.EQP_ID ?? "null";
                string shopID = it.LINE_ID;
                string stepID = it.STEP_ID;

                int qty = Convert.ToInt32(it.OUT_QTY);
                if (qty <= 0)
                    continue;

                DateTime startTime = it.PLAN_DATE;
                if (startTime >= planEndTime)
                    continue;

                string stepKey = string.Concat(shopID, stepID);
                
                var find = parseInfos.Find(t => t.Item1.Contains(eqpGroupID) || t.Item1.Contains(stepID));
                
                
                if (find == null)
                    continue;

                sum += qty;
            }

            return sum;
        }

        private int Cal_JC(IEnumerable<EQP_PLAN> table, List<Tuple<string, string>> parseInfos1,
            Tuple<int, double, string, string> parseInfos2)
        {
            if (table == null || table.Count() == 0)
                return 0;

            if (parseInfos1 == null || parseInfos1.Count == 0)
                return 0;

            DateTime planStartTime = this.PlanStartTime;
            DateTime planEndTime = this.PlanEndTime;

            int excludePriority = 0;
            string includeEqpStr = null;
            string excludeEqpStr = null;

            if (parseInfos2 != null)
            {
                excludePriority = parseInfos2.Item1;
                includeEqpStr = parseInfos2.Item3;
                excludeEqpStr = parseInfos2.Item4;
            }

            int sum = 0;
            foreach (var it in table)
            {
                var eqpStatus = MyHelper.ENUM.ToEnum(it.EQP_STATE_CODE, EqpState.NONE);
                if (eqpStatus != EqpState.SETUP)
                    continue;

                DateTime startTime = it.EQP_START_TIME;
                if (startTime < planStartTime)
                    continue;

                DateTime endTime = it.EQP_END_TIME;
                if (endTime >= planEndTime)
                    continue;

                string eqpGroupID = it.EQP_ID;
                string stepID = it.STEP_ID;

                var find = parseInfos1.Find(t => t.Item1.Contains(eqpGroupID) || t.Item1.Contains(stepID));
                if (find == null)
                    continue;

                if (string.IsNullOrEmpty(includeEqpStr) == false && includeEqpStr != "-")
                {
                    string eqpID = it.EQP_ID;
                    if (eqpID.Contains(includeEqpStr) == false)
                        continue;  
                }

                if (string.IsNullOrEmpty(excludeEqpStr) == false && excludeEqpStr != "-")
                {
                    string eqpID = it.EQP_ID;
                    if (eqpID.Contains(excludeEqpStr))
                        continue;
                } 

                var jcTime = TimeSpan.FromMinutes(MyHelper.CONVERT.ToDouble(find.Item2, 0));
                TimeSpan diff = endTime - startTime;

                if (diff < jcTime)
                    continue;

                sum += 1;
            }

            return sum;
        }

        private int Cal_WIP_GAP(IEnumerable<EQP_PLAN> table, List<Tuple<string, string>> parseInfos)
        {
            if (table == null || table.Count() == 0)
                return 0;

            if (parseInfos == null || parseInfos.Count == 0)
                return 0;

            DateTime planStartTime = this.PlanStartTime;
            DateTime planEndTime = this.PlanEndTime;

            var eqpPlan = table.Where(t => t.EQP_END_TIME < PlanEndTime &&
                                           string.IsNullOrEmpty(t.EQP_ID) == false);

            var groups = eqpPlan.GroupBy(t => string.Concat(t.LINE_ID, t.STEP_ID));

            Dictionary<string, List<int>> wipDic = new Dictionary<string, List<int>>();
            foreach (var it in groups)
            {
                var first = it.FirstOrDefault();
                if (first == null)
                    continue;

                string stepKey = it.Key;
                var find = parseInfos.Find(t => t.Item1.Contains(stepKey));
                if (find == null)
                    continue;

                int qty = Convert.ToInt32(it.Sum(t => t.PROCESS_QTY));

                string key = find.Item2;

                List<int> finds;
                if (wipDic.TryGetValue(key, out finds) == false)
                    wipDic.Add(key, finds = new List<int>());

                finds.Add(qty);
            }

            return Cal_WIP_GAP(wipDic);
        }

        private int Cal_WIP_GAP(Dictionary<string, List<int>> wipDic)
        {
            int wipGAPCount = 0;

            foreach (var item in wipDic)
            {
                int minQty = GetMinMaxValue(item.Value).Item1;
                int maxQty = GetMinMaxValue(item.Value).Item2;

                if ((minQty * 2 - maxQty) < 0)
                    wipGAPCount += 1;
            }

            return wipGAPCount;
        }

        private DataTable CreateSchema()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(Schema.ITEMKEY, typeof(string));
            dt.Columns.Add(Schema.CATEGORY, typeof(string));
            dt.Columns.Add(Schema.ITEM, typeof(string));
            dt.Columns.Add(Schema.DESCRIPTION, typeof(string));
            dt.Columns.Add(Schema.WEIGHT, typeof(double));
            dt.Columns.Add(Schema.TARGET, typeof(int));
            dt.Columns.Add(Schema.PLAN, typeof(int));
            dt.Columns.Add(Schema.SCORE, typeof(decimal));
            return dt;
        }

        new private void BindData(DataTable dt)
        {
            gridView1.Columns.Clear();
            this.gridControl1.DataSource = dt;

            SetColumnsWidth();
        }

        private void SetColumnsWidth()
        {
            gridView1.Columns[Schema.ITEMKEY].Width = 120;
            gridView1.Columns[Schema.CATEGORY].Width = 220;
            gridView1.Columns[Schema.ITEM].Width = 270;
            gridView1.Columns[Schema.DESCRIPTION].Width = 180;
            gridView1.Columns[Schema.WEIGHT].Width = 90;
            gridView1.Columns[Schema.TARGET].Width = 90;
            gridView1.Columns[Schema.PLAN].Width = 90;
            gridView1.Columns[Schema.SCORE].Width = 90;

            gridView1.Columns[Schema.ITEMKEY].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

            foreach (GridColumn column in gridView1.Columns)
                column.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;

            gridView1.CustomColumnSort += GridView1_CustomColumnSort;
        }

        private void GridView1_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            GridView view = sender as GridView;

            e.Handled = true;

            if (Schema.TOTALSCORE == Convert.ToString(view.GetRowCellValue(e.ListSourceRowIndex1, Schema.CATEGORY)))
            {
                e.Result = (e.SortOrder == DevExpress.Data.ColumnSortOrder.Ascending ? 1 : -1);
            }
            else if (Schema.TOTALSCORE == Convert.ToString(view.GetRowCellValue(e.ListSourceRowIndex2, Schema.CATEGORY)))
            {
                e.Result = (e.SortOrder == DevExpress.Data.ColumnSortOrder.Ascending ? -1 : 1);
            }
            else
            {
                e.Handled = false;
            }
        }

        #region Chart
        private void DrawChart(DataTable dt)
        {
            DrawDoughnutChart(dt);
            DrawBarChart(dt);
        }

        private void DrawDoughnutChart(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;
            DataTable chartTable = SetChartTable();

            try
            {
                foreach (DataRow info in dt.Rows)
                {
                    string item = info[Schema.ITEM].ToString();
                    decimal weight = Convert.ToDecimal(info[Schema.WEIGHT]);
                    decimal score = Convert.ToDecimal(info[Schema.SCORE]);
                    decimal lackScore = weight - score;
                    DataRow dRow = chartTable.NewRow();
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }

                    dRow[Schema.ITEM] = item;
                    dRow[Schema.WEIGHT] = lackScore;
                    dRow[Schema.SCORE] = score;
                    chartTable.Rows.Add(dRow);
                }

                doughnutChart.Series.Clear();
                Series ratioSeries = new Series("Ratio", ViewType.Doughnut);
                doughnutChart.Series.Add(ratioSeries);

                doughnutChart.PaletteName = "Office";
                doughnutChart.Legend.Visibility = DefaultBoolean.False;

                decimal sumLackScore = 0;
                foreach (DataRow row in chartTable.Rows)
                {
                    sumLackScore += (decimal)row[Schema.WEIGHT];
                    SeriesPoint scorePoint = new SeriesPoint(row[Schema.ITEM].ToString(), (decimal)row[Schema.SCORE]);

                    ratioSeries.Points.Add(scorePoint);
                }
                SeriesPoint lackPoint = new SeriesPoint(Schema.LACKSCORE, sumLackScore);
                ratioSeries.Points.Add(lackPoint);

                decimal totalScore = 0;
                decimal totalsetup = 0;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (Convert.ToString(gridView1.GetRowCellValue(i, Schema.CATEGORY)) == Schema.TOTALSCORE)
                    {
                        totalScore = Convert.ToDecimal(gridView1.GetRowCellValue(i, Schema.SCORE));
                    }

                    if (Convert.ToString(gridView1.GetRowCellValue(i, Schema.CATEGORY)) == Schema.TOTALSCORE)
                    {
                        totalsetup = Convert.ToDecimal(gridView1.GetRowCellValue(i, Schema.PLAN));
                    }

                }

                DoughnutChartSetting(ratioSeries, totalScore, totalsetup);
            }
            catch
            {

            }
        }

        private void DrawBarChart(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;
            DataTable chartTable = SetChartTable();

            try
            {
                foreach (DataRow info in dt.Rows)
                {
                    string item = info[Schema.ITEM].ToString();
                    decimal weight = Convert.ToDecimal(info[Schema.WEIGHT]);
                    decimal score = Convert.ToDecimal(info[Schema.SCORE]);
                    decimal ratio = weight == 0 ? 0 : (score / weight) * 100;

                    DataRow dRow = chartTable.NewRow();
                    if (string.IsNullOrEmpty(item))
                    {
                        dRow[Schema.ITEM] = Convert.ToString(info[Schema.CATEGORY]);
                        dRow[Schema.WEIGHT] = weight;
                        dRow[Schema.SCORE] = score;
                        chartTable.Rows.InsertAt(dRow, 0);
                        continue;
                    }

                    dRow[Schema.ITEM] = item;
                    dRow[Schema.WEIGHT] = weight;
                    dRow[Schema.SCORE] = ratio;

                    chartTable.Rows.Add(dRow);
                }

                barChart.Series.Clear();
                Series scoreSeries = new Series(Schema.SCORE, ViewType.StackedBar);
                barChart.Series.Add(scoreSeries);

                barChart.PaletteName = "Office";
                barChart.Legend.Visibility = DefaultBoolean.True;

                foreach (DataRow row in chartTable.Rows)
                {
                    SeriesPoint scorePoint = new SeriesPoint(row[Schema.ITEM].ToString(), (decimal)row[Schema.SCORE]);

                    if (scorePoint.Argument == Schema.TOTALSCORE)
                    {
                        scorePoint = new SeriesPoint(Schema.TOTAL, (decimal)row[Schema.SCORE]);
                        scoreSeries.Points.Insert(0, scorePoint);
                        continue;
                    }
                    scoreSeries.Points.Add(scorePoint);
                }

                BarChartSetting(scoreSeries);
            }
            catch
            {

            }
        }

        private void DoughnutChartSetting(Series ratioSeries, decimal totalScore, decimal totalSetup)
        {
            ratioSeries.SeriesPointsSorting = SortingMode.Descending;
            ratioSeries.SeriesPointsSortingKey = SeriesPointKey.Value_1;
            ratioSeries.LabelsVisibility = DefaultBoolean.True;

            DoughnutSeriesLabel label = (DoughnutSeriesLabel)ratioSeries.Label;
            label.Position = PieSeriesLabelPosition.TwoColumns;
            label.ResolveOverlappingMode = ResolveOverlappingMode.Default;
            label.BackColor = Color.Transparent;
            label.Font = new Font("Tahoma", 10);

            Series landAreaSeries = doughnutChart.Series["Ratio"];
            DoughnutSeriesView pieView = landAreaSeries.View as DoughnutSeriesView;
            if (pieView == null) return;
            pieView.TotalLabel.Visible = true;
            pieView.TotalLabel.TextPattern = $"{Schema.TOTALSCORE}\r\n{totalScore}\r\n{Schema.TOTALSETUP}\r\n{totalSetup}";
            pieView.TotalLabel.Font = new Font("Tahoma", 14);
        }

        private void BarChartSetting(Series scoreSeries)
        {
            scoreSeries.LabelsVisibility = DefaultBoolean.True;

            XYDiagram diagram = barChart.Diagram as XYDiagram;
            diagram.AxisY.WholeRange.SetMinMaxValues(0, 100);
            diagram.AxisY.WholeRange.SideMarginsValue = 0;
            diagram.AxisY.NumericScaleOptions.AutoGrid = false;
            diagram.AxisY.NumericScaleOptions.GridSpacing = 25;
            diagram.AxisY.GridLines.Visible = false;
            diagram.AxisX.Label.Angle = 0;

            StackedBarSeriesLabel label = (StackedBarSeriesLabel)scoreSeries.Label;
            label.TextPattern = "{V:#.#}";
            label.Position = BarSeriesLabelPosition.Center;
            label.ResolveOverlappingMode = ResolveOverlappingMode.Default;
            label.Border.Visibility = DefaultBoolean.False;
            label.BackColor = Color.Transparent;
            label.TextColor = Color.White;
            label.Font = new Font("Tahoma", 10);

        }

        private DataTable SetChartTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Schema.ITEM, typeof(string));
            dt.Columns.Add(Schema.WEIGHT, typeof(decimal));
            dt.Columns.Add(Schema.SCORE, typeof(decimal));
            return dt;
        }

        #endregion

        private Tuple<int, int> GetMinMaxValue(List<int> list)
        {
            if (list.Count > 0)
            {
                int minValue = list.Min();
                int maxValue = list.Max();

                return Tuple.Create<int, int>(minValue, maxValue);
            }
            return Tuple.Create<int, int>(0, 0);
        }

        public class Schema
        {
            public const string ITEMKEY = "ITEMKEY";
            public const string CATEGORY = "CATEGORY";
            public const string ITEM = "ITEM";
            public const string DESCRIPTION = "DESCRIPTION";
            public const string WEIGHT = "WEIGHT";
            public const string TARGET = "TARGET";
            public const string PLAN = "PLAN";
            public const string SCORE = "SCORE";
            public const string LACKSCORE = "LACK SCORE";
            public const string SETUP = "SETUP";
            public const string TOTAL = "TOTAL";
            public const string TOTALSCORE = "TOTAL SCORE";
            public const string TOTALSETUP = "TOTAL SETUP";
            public const string ITEMTEXT = "ITEM_TEXT";
            public const string ITEMVALUE = "ITEM_VALUE";
        }

        public class ResultData
        {
            public string Category { get; set; }
            public string ItemKey { get; set; }
            public string ItemName { get; set; }
            public string Description { get; set; }
            public double Weight { get; set; }
            public int Target { get; set; }
            public int Plan { get; set; }
            public double Score { get; set; }
            public double Setup { get; set; }
            public CategoryType CategoryType { get; set; }

            public ResultData(string category, string itemKey, string itemName, string description, double weight,
                                int target, int plan)
            {
                this.Category = MyHelper.STRING.ToSafeUpper(category);
                this.ItemKey = itemKey;
                this.ItemName = itemName;
                this.Description = description;
                this.Weight = weight;
                this.Target = target;
                this.Plan = plan;
                this.Setup = 0;
                this.Score = 0;

                this.CategoryType = MyHelper.ENUM.ToEnum(this.Category, CategoryType.NONE);
            }

            public void SetScore(double weight2)
            {
                double val = GetVal(this.CategoryType, this.Target, this.Plan, weight2);

                this.Score = Math.Round(val * this.Weight, 1);
            }

            private double GetVal(CategoryType ctype, double target, double plan, double weight2)
            {
                double val = 1;

                if (ctype == CategoryType.JOBCHANGE)
                {
                    if (plan <= target || plan == 0 )
                        return val;
                }

                if (ctype == CategoryType.PLAN)
                {
                    if (plan <= target)
                        return val;
                }

                if (ctype == CategoryType.PLAN)
                {
                    val = 1 - (plan - target) / target;
                }
                else if (ctype == CategoryType.JOBCHANGE)
                {
                    val = 1 - (plan - target) / target ;
                }
                else if (ctype == CategoryType.WIPGAP)
                {
                    double diff = plan - target;
                    if (diff > 0)
                        val = 1 - (diff * 0.2);
                }

                return Math.Max(val, 0);
            }
        }

        private void GridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e == null || e.Column == null)
                return;

            string category = Convert.ToString(this.gridView1.GetRowCellValue(e.RowHandle, Schema.CATEGORY));
            if (category == Schema.TOTALSCORE)
            {
                e.Appearance.BackColor = Color.AliceBlue;
            }

            if (string.IsNullOrEmpty(e.DisplayText) == false)
            {
                decimal weight = Convert.ToDecimal(this.gridView1.GetRowCellValue(e.RowHandle, Schema.WEIGHT));
                decimal score = Convert.ToDecimal(this.gridView1.GetRowCellValue(e.RowHandle, Schema.SCORE));

                if (score < weight && category != Schema.TOTALSCORE)
                {
                    if (e.Column.FieldName == Schema.ITEM || e.Column.FieldName == Schema.SCORE)
                        e.Appearance.BackColor = Color.Yellow;
                }

                if (category == Schema.TOTALSCORE && e.Column.FieldName == Schema.SCORE)
                {
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;

                    if (score <= 90)
                        e.Appearance.BackColor = Color.Red;
                }
            }
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();

            UseSaveLayout();
        }

        private void doughnutChart_CustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e)
        {
            if (e.SeriesPoint.Values.Count() > 0)
            {
                e.LabelText = $"{e.SeriesPoint.Argument} \n{Schema.SCORE} : {e.SeriesPoint.Values[0]}";
            }

            if (e.SeriesPoint.Argument == Schema.LACKSCORE)
            {
                PieDrawOptions opts = (PieDrawOptions)e.SeriesDrawOptions;
                opts.FillStyle.FillMode = FillMode.Solid;
                e.SeriesDrawOptions.Color = Color.White;
                e.LabelText = "";
            }
        }

        private void barChart_CustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e)
        {
            if (e.SeriesPoint.Argument == Schema.TOTAL)
            {
                e.SeriesDrawOptions.Color = Color.DarkBlue;
                e.LegendDrawOptions.Color = Color.Red;
            }

            if (e.Series.Name == Schema.LACKSCORE)
            {
                e.SeriesDrawOptions.Color = Color.White;
                e.LegendDrawOptions.Color = Color.White;
            }
        }

        private void gridView1_EndSorting(object sender, EventArgs e)
        {
            if (gridView1 != null && gridView1.RowCount > 0)
                gridView1.FocusedRowHandle = 0;
        }
    }
}
