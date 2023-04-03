using DevExpress.XtraCharts;
using DevExpress.XtraPivotGrid;
using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using SmartAPS.MozartStudio.Helper;
using SmartAPS.MozartStudio.Utils;
using Template.Lcd.Scheduling.Inputs;

namespace SmartAPS.MozartStudio.Analysis
{
	public partial class TargetPlanCompareView : MyXtraPivotGridTemplate
    {
        public enum QtyType
        {
            TARGET,
            PLAN,
            DIFF
        }

        public const string SHOP_ID = "SHOP_ID";
        public const string PRODUCT_ID = "PRODUCT_ID";
        public const string STEP_ID = "STEP_ID";
        public const string PRODUCTION_TYPE = "PRODUCTION_TYPE";
        public const string CATEGORY = "CATEGORY";
        public const string TOTAL = "TOTAL";
        public const string TARGET_DATE = "TARGET_DATE";
        public const string QTY = "QTY";

        #region Variables

        private Dictionary<string, int> _stepIndexs;

        private List<string> _dateList;

        /* dictionary */
        private Dictionary<string, CompareItem> _resultDict;

        private string TargetAreaID { get { return this.editAreaId.EditValue as string; } }

        private string TargetShopID { get { return this.editShopId.EditValue as string; } }

        private bool IsOnlyMainStep
        {
            get
            {
                if (this.checkOnlyMain.Checked == false)
                    return false;

                return true;
            }
        }

        private bool ShowAccumulativeQty
        {
            get
            {
                if (this.checkAccQty.Checked == true)
                    return true;
                return false;
            }
        }

        DateTime StartTime
        {
            get
            {
                DateTime t = Convert.ToDateTime(this.editStartDate.EditValue).Date;

                return ShopCalendar.StartTimeOfDay(t);
            }
        }
        
        DateTime EndTime
        {
            get
            {
                DateTime t = Convert.ToDateTime(this.editEndDate.EditValue).Date;

                return ShopCalendar.EndTimeOfDay(t);
            } 
        }

        private DateTime PlanStartTime
        {
            get
            {
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result); //planstart
            }
        }

        #endregion

        #region Input Data Transform

        public class Product
        {
            public string ShopID;
            public string ProductID;
            public string ProductGroup;
            public string ProductKind;
            
            public Product(Template.Lcd.Scheduling.Inputs.Product row)
            {
                this.ShopID = string.Empty;
                this.ProductID = string.Empty;
                this.ProductGroup = string.Empty;
                this.ProductKind = string.Empty;

                ParseRow(row);
            }

            private void ParseRow(Template.Lcd.Scheduling.Inputs.Product row)
            {
                // LINE_ID
                ShopID = row.SHOP_ID;
                ProductID = row.PRODUCT_ID;
                ProductGroup = row.PRODUCT_GROUP;
                ProductKind = row.PRODUCT_KIND;
            }

            internal class Schema
            {
                public static string SHOP_ID = "SHOP_ID";
                public static string PRODUCT_ID = "PRODUCT_ID";
                public static string PRODUCT_GROUP = "PRODUCT_GROUP";
                public static string PRODUCT_KIND = "PRODUCT_KIND";
            }
        }

        public class ProcStep
        {
            public string ShopID;
            public string ProductID;
            public string StepID;
            public string StepType;

            public ProcStep(DataRow row)
            {
                this.ShopID = string.Empty;
                this.ProductID = string.Empty;
                this.StepID = string.Empty;
                this.StepType = string.Empty;

                ParseRow(row);
            }

            private void ParseRow(DataRow row)
            {
                // LINE_ID
                ShopID = row.GetString(Schema.SHOP_ID);
                ProductID = row.GetString(Schema.PRODUCT_ID);
                StepID = row.GetString(Schema.STEP_ID);
                StepType = row.GetString(Schema.STEP_TYPE);
            }

            internal class Schema
            {
                public static string SHOP_ID = "SHOP_ID";
                public static string PRODUCT_ID = "PRODUCT_ID";
                public static string STEP_ID = "STEP_ID";
                public static string STEP_TYPE = "STEP_TYPE";
            }
        }


        public class StepTarget
        {
            public string AreaID;
            public string ShopID;
            public string ProductID;
            public string StepID;
            public string TargetDate;
            public DateTime CampareTargetDate;
            public float OutTargetQty;
            public string StepType;

            public StepTarget(Template.Lcd.Scheduling.Outputs.StepTarget row)
            {
                this.AreaID = string.Empty;
                this.ShopID = string.Empty;
                this.ProductID = string.Empty;
                this.StepID = string.Empty;
                this.TargetDate = string.Empty;
                this.CampareTargetDate = DateTime.MinValue;
                this.OutTargetQty = 0.0f;
                this.StepType = string.Empty;

                ParseRow(row);
            }

            ///
            /// 개발 포인트 
            ///
            private void ParseRow(Template.Lcd.Scheduling.Outputs.StepTarget row)
            {
                this.AreaID = row.AREA_ID;
                this.ShopID = row.SHOP_ID;
                this.ProductID = row.PRODUCT_ID;
                this.StepID = row.STEP_ID;

                this.TargetDate = ShopCalendar.SplitDate(row.TARGET_DATE).ToString("yyyyMMdd");
                this.CampareTargetDate = row.TARGET_DATE;

                this.OutTargetQty = Convert.ToSingle(row.TARGET_OUT_QTY);
                this.StepType = row.STEP_TYPE;
            }


            internal class Schema
            {
                public static string AREA_ID = "AREA_ID";
                public static string SHOP_ID = "SHOP_ID";
                public static string PRODUCT_ID = "PRODUCT_ID";
                public static string STEP_ID = "STEP_ID";
                public static string TARGET_DATE = "TARGET_DATE";
                public static string OUT_TARGET_QTY = "TARGET_OUT_QTY";
                public static string STEP_TYPE = "STEP_TYPE";
            }
        }

        public class StepMove
        {
            public string AreaID;
            public string ShopID;
            public string ProductID;
            public string StepID;
            public int StdStepSeq;
            public string ProductionType;
            public string TargetDate;
            public DateTime CompareTargetDate;
            public float InQty;
            public float OutQty;

            public StepMove(Template.Lcd.Scheduling.Outputs.StepMove row)
            {
                this.AreaID = string.Empty;
                this.ShopID = string.Empty;
                this.ProductID = string.Empty;
                this.StepID = string.Empty;
                this.StdStepSeq = 0;
                this.ProductionType = string.Empty;
                this.TargetDate = string.Empty;
                this.CompareTargetDate = DateTime.MinValue;
                this.InQty = 0.0f;
                this.OutQty = 0.0f;

                ParseRow(row);
            }

            ///
            /// 개발 포인트 
            ///
            private void ParseRow(Template.Lcd.Scheduling.Outputs.StepMove row)
            {
                this.AreaID = row.AREA_ID;
                this.ShopID = row.SHOP_ID;
                this.ProductID = row.PRODUCT_ID;
                this.StepID = row.STEP_ID;
                this.StdStepSeq = row.STD_STEP_SEQ;
                this.ProductionType = row.PRODUCTION_TYPE;

                //this.TargetDate = row.GetDateTime(Schema.TARGET_DATE).ToString("yyyyMMdd");
                this.TargetDate = MyHelper.DATE.DateToString(ShopCalendar.SplitDate(row.PLAN_DATE));
                this.CompareTargetDate = row.PLAN_DATE;
                this.InQty = row.IN_QTY;
                this.OutQty = row.OUT_QTY;
            }


            internal class Schema
            {
                public static string AREA_ID = "AREA_ID";
                public static string SHOP_ID = "SHOP_ID";
                public static string PRODUCT_ID = "PRODUCT_ID";
                public static string STEP_ID = "STEP_ID";
                public static string STD_STEP_SEQ = "STD_STEP_SEQ";
                public static string PRODUCTION_TYPE = "PRODUCTION_TYPE";
                public static string TARGET_DATE = "PLAN_DATE";
                public static string IN_QTY = "IN_QTY";
                public static string OUT_QTY = "OUT_QTY";
            }


        }


        #endregion

        #region Constructor
        /* constructor */
        public TargetPlanCompareView()
        {
            InitializeComponent();
        }

        public TargetPlanCompareView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        #endregion

        #region Load Document

        protected override void LoadDocument()
        {
            base.LoadDocument();

            _resultDict = new Dictionary<string, CompareItem>();

            this.SetMainPivotGrid(this.pivotGridControl1);

            this.AddExtendPivotGridMenu(this.pivotGridControl1);

            SetControls();

            RunQuery();
        }

        protected override void Query()
        {
            SetDateRange();

            LoadData();

            XtraPivotGridHelper.DataViewTable dataViewTable = BindData();

            FillData(dataViewTable);

            DrawGrid(dataViewTable);
        }

        private void SetDateRange()
        {
            DateTime st = this.StartTime;
            DateTime et = this.EndTime;

            _dateList = new List<string>();
            for (DateTime dt = st; dt < et; dt = dt.AddDays(1))
            {
                string day = MyHelper.DATE.DateToString(dt);
                _dateList.Add(day);
            }
        }

        #endregion

        #region Set Controls
        private void SetControls()
        {
            MyHelper.ENGCONTROL.SetControl_AreaID(this.editAreaId, this.Result); //this.TargetVersionNo
            MyHelper.ENGCONTROL.SetControl_ShopID(this.editShopId, this.TargetAreaID, this.Result); //this.TargetVersionNo, this.TargetAreaID

            //List<string> shopList = null;
            //if (this.TargetShopID != Consts.ALL)
            //{
            //    shopList = new List<string>();
            //    shopList.Add(this.TargetShopID);
            //}

            //MyHelper.ENGCONTROL.SetControl_StepID(this.editStepId, this.TargetVersionNo, shopList);
            //MyHelper.ENGCONTROL.SetControl_ProductionType(this.editProdType, this.TargetVersionNo);
            //MyHelper.ENGCONTROL.SetControl_ProductID_Checked(this.editProdId, this.TargetVersionNo, shopList);

            DateTime st = ShopCalendar.SplitDate(this.PlanStartTime);
            
            this.editStartDate.EditValue = st;
            this.editEndDate.EditValue = st.AddDays(7);
        }

        private List<string> GetMainStdStepList(string areaID)
        {
            List<string> list = new List<string>();

            var stdStep = MyHelper.DATASVC.GetEntityData<StdStep>(this.Result);
            if (stdStep == null)
                return list;
        
            var mainStdStep = stdStep.Where(t => (t.AREA_ID == "ALL" || t.AREA_ID == areaID) && t.STEP_TYPE == "MAIN").OrderBy(t => t.STEP_SEQ);       
            foreach (var item in mainStdStep)
            {
                list.Add(item.STEP_ID);
            }

            return list;
        }

        #endregion

        #region Load Data

        private void LoadData()
        {
            _resultDict.Clear();

            LoadData_Target(_resultDict);
            LoadData_Plan(_resultDict);
            LoadData_Diff(_resultDict);
        }

        private void LoadData_Target(Dictionary<string, CompareItem> results)
        {
            //string filter = null;

            string targetShopID = this.TargetShopID;
            //if (targetShopID != Consts.ALL)
            //    filter = string.Format("{0}", targetShopID);

            var table = MyHelper.DATASVC.GetEntityData<Template.Lcd.Scheduling.Outputs.StepTarget>(this.Result); // (this.TargetVersionNo, filter);
            if (table == null)
                return;

            var mainStdStepList = GetMainStdStepList(this.TargetAreaID);

            DateTime st = this.StartTime;
            DateTime et = this.EndTime;

            foreach (var row in table)
            {
                StepTarget item = new StepTarget(row);

#if DEBUG
                if (item.ProductID == "TH645A1AB100" && item.StepID == "9900")
                    Console.WriteLine();

                if(item.ShopID == "CF")
                    Console.WriteLine();
#endif

                if (item.StepType != "MAIN")
                    continue;

                if (this.TargetAreaID != item.AreaID)
                    continue;


                if (item.CampareTargetDate < st)
                    continue;

                if (item.CampareTargetDate >= et)
                    continue;

#if DEBUG
                if (item.ProductID == "TH645A1AB100" && item.StepID == "9900")
                    Console.WriteLine();

                if (item.ShopID == "CF")
                    Console.WriteLine();
#endif

                if (this.TargetShopID != "ALL")
                {
                    if (MyHelper.STRING.Equals(item.ShopID, targetShopID) == false)
                        continue;
                }

                if (IsOnlyMainStep)
                {
                    if (mainStdStepList.Contains(item.StepID) == false)
                        continue;
                }

                string key = item.ShopID + item.ProductID + item.StepID;

                CompareItem compItem;
                if (_resultDict.TryGetValue(key, out compItem) == false)
                {

                    compItem = new CompareItem(item.ShopID, item.ProductID, item.StepID);
                    results.Add(key, compItem);

                    compItem.SHOP_ID = item.ShopID;
                    compItem.PRODUCT_ID = item.ProductID;
                    compItem.STEP_ID = item.StepID;
                    compItem.PRODUCTION_TYPE = "OwnerP";
                }

                compItem.AddQty(item.TargetDate, item.OutTargetQty, QtyType.TARGET);
            }

        }

        private void LoadData_Plan(Dictionary<string, CompareItem> results)
        {
            //string filter = null;

            string targetShopID = this.TargetShopID;
            //if (targetShopID != Consts.ALL)
            //    filter = string.Format("{0}", targetShopID);

            var table = MyHelper.DATASVC.GetEntityData<Template.Lcd.Scheduling.Outputs.StepMove>(this.Result); // (this.TargetVersionNo, filter);
            if (table == null)
                return;

            var mainStdStepList = GetMainStdStepList(this.TargetAreaID);

            _stepIndexs = new Dictionary<string, int>();

            DateTime st = this.StartTime;
            DateTime et = this.EndTime;

            foreach (var row in table)
            {
                StepMove item = new StepMove(row);

                if (this.TargetAreaID != item.AreaID)
                    continue;

                if (item.CompareTargetDate < st)
                    continue;

                if (item.CompareTargetDate >= et)
                    continue;

                if (this.TargetShopID != "ALL")
                {
                    if (MyHelper.STRING.Equals(item.ShopID, targetShopID) == false)
                        continue;
                }

                if (IsOnlyMainStep)
                {
                    if (mainStdStepList.Contains(item.StepID) == false)
                        continue;
                }

                string key = item.ShopID + item.ProductID + item.StepID;

                string stepKey = item.StepID;
                _stepIndexs[stepKey] = item.StdStepSeq;

                CompareItem compItem;
                if (!results.TryGetValue(key, out compItem))
                {
                    compItem = new CompareItem(item.ShopID, item.ProductID, item.StepID);
                    _resultDict.Add(key, compItem);

                    compItem.PRODUCT_ID = item.ProductID;
                    compItem.STEP_ID = item.StepID;
                    compItem.SHOP_ID = item.ShopID;
                    compItem.PRODUCTION_TYPE = item.ProductionType;
                }

                compItem.AddQty(item.TargetDate, item.OutQty, QtyType.PLAN);
            }
        }

        private void LoadData_Diff(Dictionary<string, CompareItem> results)
        {
            if (results == null || results.Count == 0)
                return;

            foreach (var item in results.Values)
            {
                item.SetDiff();
            }
        }

        #endregion

        #region Bind Data
        private XtraPivotGridHelper.DataViewTable BindData()
        {
            XtraPivotGridHelper.DataViewTable dataViewTable = new XtraPivotGridHelper.DataViewTable();
            dataViewTable.AddColumn(SHOP_ID, SHOP_ID, typeof(string), PivotArea.RowArea, null, null);

            dataViewTable.AddColumn(STEP_ID, STEP_ID, typeof(string), PivotArea.RowArea, null, null);
            dataViewTable.AddColumn(PRODUCT_ID, PRODUCT_ID, typeof(string), PivotArea.RowArea, null, null);
            dataViewTable.AddColumn(PRODUCTION_TYPE, PRODUCTION_TYPE, typeof(string), PivotArea.RowArea, null, null);
            dataViewTable.AddColumn(CATEGORY, CATEGORY, typeof(string), PivotArea.RowArea, null, null);
            dataViewTable.AddColumn(TOTAL, TOTAL, typeof(string), PivotArea.RowArea, null, null);
            dataViewTable.AddColumn(TARGET_DATE, TARGET_DATE, typeof(string), PivotArea.ColumnArea, null, null);

            dataViewTable.AddColumn(QTY, QTY, typeof(float), PivotArea.DataArea, null, null);

            dataViewTable.AddDataTablePrimaryKey(
                new DataColumn[]
                    {
                        dataViewTable.Columns[SHOP_ID],
                        dataViewTable.Columns[STEP_ID],
                        dataViewTable.Columns[PRODUCT_ID],
                        dataViewTable.Columns[CATEGORY],
                        dataViewTable.Columns[TARGET_DATE]
                    });

            return dataViewTable;
        }
        #endregion

        #region Fill Data
        private void FillData(XtraPivotGridHelper.DataViewTable dataViewTable)
        {
            CompareItem total = new CompareItem("TOTAL", "", "");

            foreach (CompareItem item in _resultDict.Values)
            {
                item.SetAccQty();

                foreach (var targetDate in item.Dates)
                {
                    FillData(dataViewTable, item, targetDate, total, QtyType.TARGET);

                    FillData(dataViewTable, item, targetDate, total, QtyType.PLAN);

                    FillData(dataViewTable, item, targetDate, total, QtyType.DIFF);
                }               
            }

            //Total
            _resultDict.Add("TOTAL", total);

            List<float> totalTargetList = new List<float>();
            List<float> totalPlanList = new List<float>();
            foreach (var item in _resultDict.Values)
            {
                totalTargetList.Add(item.TARGET_TOTAL);
                totalPlanList.Add(item.PLAN_TOTAL);
            }

            float totalTarge = totalTargetList.Sum();
            float totalPlan = totalPlanList.Sum();

            foreach (KeyValuePair<string, float> pair in total.Targets)
            {
                dataViewTable.DataTable.Rows.Add(
                            total.SHOP_ID,
                            total.STEP_ID,
                            total.PRODUCT_ID,
                            total.PRODUCTION_TYPE,
                            QtyType.TARGET.ToString(),
                            string.Format("{0:#,##0}", totalTarge),
                            pair.Key,
                            pair.Value
                            );
            }

            foreach (KeyValuePair<string, float> pair in total.Plans)
            {
                dataViewTable.DataTable.Rows.Add(
                            total.SHOP_ID,
                            total.STEP_ID,
                            total.PRODUCT_ID,
                            total.PRODUCTION_TYPE,
                            QtyType.PLAN.ToString(),
                            string.Format("{0:#,##0}", totalPlan),
                            pair.Key,
                            pair.Value
                            );
            }
        }

        private void FillData(XtraPivotGridHelper.DataViewTable dataViewTable, CompareItem item, string targetDate, CompareItem total, QtyType qtyType)
        {
            string category = qtyType.ToString();
            float qty = 0.0f;
            if (ShowAccumulativeQty)
                qty = item.GetAccQty(targetDate, qtyType);
            else
                qty = item.GetQty(targetDate, qtyType);

            dataViewTable.DataTable.Rows.Add(
            item.SHOP_ID,
            item.STEP_ID,
            item.PRODUCT_ID,
            item.PRODUCTION_TYPE,
            category,
            string.Format("{0:#,##0}", item.GetTotalQty(qtyType)),
            targetDate,
            qty);

            //Total
            var infos = qtyType == QtyType.TARGET ? total.Targets : total.Plans;
            if(qtyType == QtyType.DIFF)
                infos = total.Diffs;

            float prevQty = 0;
            if (infos.TryGetValue(targetDate, out prevQty))
                infos[targetDate] += qty;
            else
                infos.Add(targetDate, 0);
        }

        #endregion

        private void pivotGridControl_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
        {
            if (e.Field.FieldName != STEP_ID)
                return;

            int s1 = ConvertLayerIndex(e.Value1 as string);
            int s2 = ConvertLayerIndex(e.Value2 as string);

            e.Result = s1.CompareTo(s2);

            e.Handled = true;
        }

        private int ConvertLayerIndex(string stepID)
        {
            if (stepID == null)
                return 999;

            int seq = 0;
            if (_stepIndexs.TryGetValue(stepID, out seq))
                return seq;

            return 999;
        }

        #region Draw Grid
        private void DrawGrid(XtraPivotGridHelper.DataViewTable dt)
        {
            pivotGridControl1.BeginUpdate();

            XtraPivotGridHelper.ClearPivotGridFields(pivotGridControl1);
            XtraPivotGridHelper.CreatePivotGridFields(pivotGridControl1, dt);
            pivotGridControl1.DataSource = dt.DataTable;

            formatCells(pivotGridControl1);

            pivotGridControl1.EndUpdate();

            pivotGridControl1.OptionsView.HideAllTotals();

            foreach (PivotGridField field in pivotGridControl1.Fields)
            {
                if (field.FieldName == TOTAL)
                {
                    field.Appearance.Value.Options.UseTextOptions = true;
                    field.Appearance.Value.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                }
            }

            pivotGridControl1.BestFit();
        }

        private void formatCells(PivotGridControl pivotGridControl1)
        {
            
            pivotGridControl1.Fields[STEP_ID].SortMode = PivotSortMode.Custom;

            pivotGridControl1.Fields[QTY].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;

            pivotGridControl1.Fields[QTY].CellFormat.FormatString = "#,##0";
            
            pivotGridControl1.CustomDrawCell += this.pivotGridControl1_CustomDrawCell;
            pivotGridControl1.CustomCellDisplayText += this.pivotGridControl1_CellDisplayText;

            pivotGridControl1.FocusedCellChanged += this.pivotGridControl1_FocusedCellChanged;
            pivotGridControl1.CustomFieldSort += this.pivotGridControl_CustomFieldSort;

        }

        private void pivotGridControl1_CustomDrawCell(object sender, PivotCustomDrawCellEventArgs e)
        {
            int index = 0;
            PivotGridField[] fields = e.GetRowFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Caption == CATEGORY)
                {
                    index = i; break;
                }
            }

            if (e.Value != null && (decimal)e.GetFieldValue(e.DataField) < 0)
            {
                e.Appearance.Options.UseBackColor = true;
                //e.Appearance.BackColor = Color.Yellow;
                //e.Appearance.BackColor2 = Color.Yellow;
                e.Appearance.ForeColor = Color.Red;
            }

            index = 0;

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Caption == "FACTORY")
                {
                    index = i; break;
                }
            }

            if (index >= 0 && fields.Length > index && e.GetFieldValue(fields[index]).ToString().StartsWith(TOTAL))
            {
                e.Appearance.BackColor = Color.LightGreen;
                e.Appearance.BackColor2 = Color.LightGreen;
                e.Appearance.Options.UseBackColor = true;
            }

            //if (index >= 0 && fields.Length > index && e.GetFieldValue(fields[index]).ToString().StartsWith("Plan"))
            //{
            //    e.Appearance.BackColor = Color.Gainsboro;
            //    e.Appearance.BackColor2 = Color.Gainsboro;
            //    e.Appearance.Options.UseBackColor = true;
            //}
        }

        #endregion

        #region Chart

        private void DrawChart(string shopID, string stepID, string prodID)
        {
            string key = shopID + prodID + stepID;

            CompareItem item;
            if (_resultDict.TryGetValue(key, out item) == false)
                return;

            this.chartControl1.Series.Clear();

            DataTable dt = CreateChartTable();

            FillChartTable(item, dt);

            GenerateLineSeries(dt, QtyType.TARGET.ToString());
            GenerateLineSeries(dt, QtyType.PLAN.ToString());
            GenerateLineSeries(dt, QtyType.DIFF.ToString());

        }

        private void FillChartTable(CompareItem item, DataTable dt)
        {
            List<string> dates = item.Targets.Keys.ToList<string>();

            foreach (string d in dates)
            {
                if (_dateList.Contains(d) == false)
                    _dateList.Add(d);
            }
            _dateList.Sort();

            float tarQty = 0.0f;
            float planQty = 0.0f;
            float diffQty = 0.0f;
                  
            foreach (string date in _dateList)
            {
                // TARGET
                float qty1 = 0.0f;
                item.Targets.TryGetValue(date, out qty1);
                tarQty += qty1;

                DataRow chartRow = dt.NewRow();

                chartRow[CATEGORY] = QtyType.TARGET.ToString();
                chartRow[TARGET_DATE] = date;
                chartRow[QTY] = tarQty;

                dt.Rows.Add(chartRow);


                // Plan
                float qty2 = 0.0f;
                item.Plans.TryGetValue(date, out qty2);
                planQty += qty2;

                DataRow chartRow2 = dt.NewRow();

                chartRow2[CATEGORY] = QtyType.PLAN.ToString();
                chartRow2[TARGET_DATE] = date;
                chartRow2[QTY] = planQty;

                dt.Rows.Add(chartRow2);

                // DIFF
                float qty3 = 0.0f;
                item.Diffs.TryGetValue(date, out qty3);
                diffQty += qty3;

                DataRow chartRow3 = dt.NewRow();

                chartRow3[CATEGORY] = QtyType.DIFF.ToString();
                chartRow3[TARGET_DATE] = date;
                chartRow3[QTY] = diffQty;

                dt.Rows.Add(chartRow3);
            }
        }

        private DataTable CreateChartTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(CATEGORY);
            dt.Columns.Add(TARGET_DATE);
            dt.Columns.Add(QTY, typeof(float));

            return dt;
        }

        private void GenerateLineSeries(DataTable dt, string category)
        {
            Series series = new Series(category, DevExpress.XtraCharts.ViewType.Line);
            this.chartControl1.Series.Add(series);

            series.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Auto;
            series.ArgumentDataMember = TARGET_DATE;
            series.ValueScaleType = DevExpress.XtraCharts.ScaleType.Numerical;
            series.ValueDataMembers.AddRange(new string[] { QTY });
            series.CrosshairLabelPattern = "{S}({A}) : {V:##0.0}";
            (series.View as LineSeriesView).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
            (series.View as LineSeriesView).LineMarkerOptions.Size = 9;

            string filter = string.Format("{0} = '{1}'", CATEGORY, category);
            DataView view = new DataView(dt, filter, null, DataViewRowState.CurrentRows);
            series.DataSource = view;

            Color color;
            if (category == QtyType.TARGET.ToString())
                color = Color.CornflowerBlue;
            else if (category == QtyType.PLAN.ToString())
                color = Color.MediumVioletRed;
            else
                color = Color.ForestGreen;

            series.View.Color = color;
        }


        #endregion 


        #region Helper Functions

        private string NextDateToString(string str)
        {
            DateTime dt = DateUtility.DbToDate(str);

            dt = dt.AddDays(1);

            return dt.ToString("yyyyMMdd");
        }

        #endregion

        #region Inner Class : CompareItem

        internal class CompareItem
        {
            public string SHOP_ID { get; set; }
            
            public string PRODUCT_ID { get; set; }
            public string STEP_ID { get; set; }
            public string PRODUCTION_TYPE { get; set; }

            public float TARGET_TOTAL { get; set; }
            public float PLAN_TOTAL { get; set; }

            public float DIFF_TOTAL
            {
                get { return this.PLAN_TOTAL - this.TARGET_TOTAL; }
            }

            public List<string> Dates { get; set; }

            // <TARGET_DATE, CHIP_QTY>
            public Dictionary<string, float> Targets;
            public Dictionary<string, float> AccTargets;
            public Dictionary<string, float> Plans;
            public Dictionary<string, float> AccPlans;
            public Dictionary<string, float> Diffs;
            public Dictionary<string, float> AccDiffs;

            public CompareItem(string factory, string productID, string stepID)
            {
                this.SHOP_ID = factory;
                this.PRODUCT_ID = productID;
                this.STEP_ID = stepID;

                this.Dates = new List<string>();
                this.Targets = new Dictionary<string, float>();
                this.AccTargets = new Dictionary<string, float>();
                this.Plans = new Dictionary<string, float>();
                this.AccPlans = new Dictionary<string, float>();
                this.Diffs = new Dictionary<string, float>();
                this.AccDiffs = new Dictionary<string, float>();

                this.TARGET_TOTAL = 0;
                this.PLAN_TOTAL = 0;
            }

            public void AddQty(string targetDate, float qty, QtyType qtyType)
            {
                if (qtyType == QtyType.DIFF)
                    return;

                if (this.Dates.Contains(targetDate) == false)
                    this.Dates.Add(targetDate);

                bool isPlan = qtyType == QtyType.PLAN;
                var infos = qtyType == QtyType.PLAN ? this.Plans : this.Targets;

                float prevQty;
                if (infos.TryGetValue(targetDate, out prevQty))
                    infos[targetDate] += qty;
                else
                    infos.Add(targetDate, qty);

                if (isPlan)
                    this.PLAN_TOTAL += qty;
                else
                    this.TARGET_TOTAL += qty;
            }

            public float GetQty(string targetDate, QtyType qtyType)
            {
                var infos = qtyType == QtyType.TARGET ? this.Targets : this.Plans;

                if (qtyType == QtyType.DIFF)
                    infos = this.Diffs;

                float qty;
                if (infos.TryGetValue(targetDate, out qty))
                    return qty;

                return 0;
            }

            public float GetTotalQty(QtyType qtyType)
            {
                if (qtyType == QtyType.TARGET)
                    return this.TARGET_TOTAL;

                if (qtyType == QtyType.PLAN)
                    return this.PLAN_TOTAL;

                if (qtyType == QtyType.DIFF)
                    return this.DIFF_TOTAL;

                return 0;
            }

            public void SetDiff()
            {
                foreach (string targetDate in this.Dates)
                {
                    float targetQty = GetQty(targetDate, QtyType.TARGET);
                    float planQty = GetQty(targetDate, QtyType.PLAN);
                    float diffQty = planQty - targetQty;

                    float prevQty;
                    if (this.Diffs.TryGetValue(targetDate, out prevQty))
                        this.Diffs[targetDate] += diffQty;
                    else
                        this.Diffs.Add(targetDate, diffQty);
                }
            }

            public void SetAccQty()
            {
                float accTargetQty = 0.0f;
                this.Dates.Sort();
                foreach (string targetDate in this.Dates)
                {
                    float targetQty = GetQty(targetDate, QtyType.TARGET);
                    accTargetQty = accTargetQty + targetQty;

                    float prevAccQty;
                    if (this.AccTargets.TryGetValue(targetDate, out prevAccQty))
                        this.AccTargets[targetDate] += accTargetQty;
                    else
                        this.AccTargets.Add(targetDate, accTargetQty);
                }

                float accPlanQty = 0.0f;
                this.Dates.Sort();
                foreach (string targetDate in this.Dates)
                {
                    float planQty = GetQty(targetDate, QtyType.PLAN);
                    accPlanQty = accPlanQty + planQty;

                    float prevAccQty;
                    if (this.AccPlans.TryGetValue(targetDate, out prevAccQty))
                        this.AccPlans[targetDate] += accPlanQty;
                    else
                        this.AccPlans.Add(targetDate, accPlanQty);
                }

                float accDiffQty = 0.0f;
                this.Dates.Sort();
                foreach (string targetDate in this.Dates)
                {
                    float diffQty = GetQty(targetDate, QtyType.DIFF);
                    accDiffQty = accDiffQty + diffQty;

                    float prevAccQty;
                    if (this.AccDiffs.TryGetValue(targetDate, out prevAccQty))
                        this.AccDiffs[targetDate] += accDiffQty;
                    else
                        this.AccDiffs.Add(targetDate, accDiffQty);
                }
            }

            public float GetAccQty(string targetDate, QtyType qtyType)
            {
                var infos = qtyType == QtyType.TARGET ? this.AccTargets : this.AccPlans;

                if (qtyType == QtyType.DIFF)
                    infos = this.AccDiffs;

                float qty;
                if (infos.TryGetValue(targetDate, out qty))
                    return qty;

                return 0;
            }
        }

        #endregion

        #region Event Handlers

        private void pivotGridControl1_FocusedCellChanged(object sender, EventArgs e)
        {
            PivotCellEventArgs c = this.pivotGridControl1.Cells.GetFocusedCellInfo();

            string shopID = string.Empty;
            string stepID = string.Empty;
            string productionType = string.Empty;
            string prodID = string.Empty;

            foreach (PivotGridField r in c.GetRowFields())
            {
                string strFieldName = r.FieldName;
                string strFieldValue = r.GetValueText(c.GetFieldValue(r));

                if (strFieldName == SHOP_ID)
                    shopID = strFieldValue;
                else if (strFieldName == STEP_ID)
                    stepID = strFieldValue;
                else if (strFieldName == PRODUCT_ID)
                    prodID = strFieldValue;
                else if (strFieldName == PRODUCTION_TYPE)
                    productionType = strFieldName;                   
            }

            DrawChart(shopID, stepID, prodID);
        }

        private void pivotGridControl1_CellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            if (e.GetFieldValue(e.DataField) != null && e.GetFieldValue(e.DataField).ToString() == "0")
            {
                e.DisplayText = string.Empty;
            }
        }

        private void shopIdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (_isFirstLoad == false)
            //    return;

            //RunQuery();
        }


        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void buttonExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.PIVOTEXPORT.ExportToExcel(this.pivotGridControl1);
        }

        private void checkAccQty_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void checkOnlyMain_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void editAreaId_EditValueChanged(object sender, EventArgs e)
        {
            MyHelper.ENGCONTROL.SetControl_ShopID(this.editShopId, this.TargetAreaID, this.Result); //, this.TargetVersionNo, this.TargetAreaID
        }

        #endregion
    }
}
