using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserLibrary;
using SmartAPS;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartAPS.UI.Analysis
{
	public partial class DemandvsPlan : MyXtraPivotGridTemplate
    {
        private const string _pageID = "DemandvsPlan";

        private Dictionary<string, double> slackDic { get; set; }
        private new Dictionary<string, ResultItem> demandDic { get; set; }
        private Dictionary<string, DEMAND> _demandDict { get; set; }
        private Dictionary<string, PRODUCT> _productDict { get; set; }
        private Dictionary<string, STEP_ROUTE> _stepRouteDict { get; set; }

        private bool isCumulative { get; set; }

        private PivotGridField dataField { get; set; }
        private PivotGridField columnField { get; set; }

        private List<string> TargeteProductID
        {
            get
            {
                var targetProdId = this.editProdId.EditValue as string;
                if (string.IsNullOrEmpty(targetProdId))
                    return null;
                else
                    return targetProdId.Split(',').Select(item => item.Trim()).ToList<string>();
            }
        }

        private DateTime TargetStartDate
        {
            get
            {
                return Convert.ToDateTime(this.editStartDate.EditValue);
            }
        }

        private DateTime TargetEndDate
        {
            get
            {
                return Convert.ToDateTime(this.editEndDate.EditValue);
            }
        }

        public DemandvsPlan()
        {
            InitializeComponent();
        }

        public DemandvsPlan(IServiceProvider serviceProvider)
       : base(serviceProvider)
        {
            InitializeComponent();
        }

        public DemandvsPlan(IExperimentResultItem result)
        {
            InitializeComponent();
            this.Result = result;
            //this.ribbonControl1.Hide();
            this.panelControl2.Hide();
            InitSettings();
            SetPivotControl();
        }

        public DemandvsPlan(IExperimentResultItem result, List<string> productId, DateTime startTime, DateTime endTime)
        {
            InitializeComponent();
            this.Result = result;

            InitSettings();
            SetDateTimePicker(startTime, endTime);

            LoadDocument();
        }

        private void InitSettings()
        {
            SetOperationInfo();
            SetControls();
            //this.expandablePanel1.BackColor = Color.FromArgb(235, 236, 239);
        }


        private void SetControls()
        {
            SetDateTimePicker();
            SetProductCombo();
            SetPivotControl();
        }

        private void SetOperationInfo()
        {
            _demandDict = new Dictionary<string, DEMAND>();
            _productDict = new Dictionary<string, PRODUCT>();
            _stepRouteDict = new Dictionary<string, STEP_ROUTE>();

            var demands = MyHelper.DATASVC.GetEntityData<DEMAND>(this.Result);
            var products = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result);
            var stepRoutes = MyHelper.DATASVC.GetEntityData<STEP_ROUTE>(this.Result);

            foreach (var demand in demands)
                if (_demandDict.ContainsKey(demand.DEMAND_ID) == false)
                    _demandDict.Add(demand.DEMAND_ID, demand);
            foreach (var product in products)
                if (_productDict.ContainsKey(product.PRODUCT_ID) == false)
                    _productDict.Add(product.PRODUCT_ID, product);
            foreach (var route in stepRoutes)
                if (_stepRouteDict.ContainsKey(route.PROCESS_ID) == false)
                    _stepRouteDict.Add(route.PROCESS_ID, route);
                else
                    if (route.STEP_SEQ > _stepRouteDict[route.PROCESS_ID].STEP_SEQ)
                        _stepRouteDict[route.PROCESS_ID] = route;
        }


        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetMainPivotGrid(this.pivotGridControl1);
            this.AddExtendPivotGridMenu(this.pivotGridControl1);

            InitSettings();
        }

        private void SetDateTimePicker(DateTime startTime, DateTime endTime)
        {
            this.editStartDate.EditValue = startTime;
            this.editEndDate.EditValue = endTime;
        }

        private void SetDateTimePicker()
        {
            this.editStartDate.Edit.DisplayFormat.FormatString = "yyyy-MM-dd";
            this.editStartDate.Edit.DisplayFormat.FormatType = FormatType.Custom;
            this.editStartDate.Edit.EditFormat.FormatString = "yyyy-MM-dd";
            this.editStartDate.Edit.EditFormat.FormatType = FormatType.Custom;

            this.editEndDate.Edit.DisplayFormat.FormatString = "yyyy-MM-dd";
            this.editEndDate.Edit.DisplayFormat.FormatType = FormatType.Custom;
            this.editEndDate.Edit.EditFormat.FormatString = "yyyy-MM-dd";
            this.editEndDate.Edit.EditFormat.FormatType = FormatType.Custom;

            this.editStartDate.EditValue = GetStartTime();
            this.editEndDate.EditValue = GetEndTime();
        }
        private void SetProductCombo()
        {
            if (this.Result == null)
                return;

            MyHelper.ENGCONTROL.SetControl_ProductID_Checked(this.editProdId, this.Result);
            //var productIds = MyHelper.DATASVC.GetEntityData<DEMAND>(this.Result).Select(x => x.PRODUCT_ID).Distinct().OrderBy(x => x);
            ////this.Result.LoadInput<DEMAND>("DEMAND").Select(x => x.PRODUCT_ID).Distinct().OrderBy(x => x);

            //foreach (var productId in productIds)
            //    this.checkedComboBoxEdit1.Properties.Items.Add(new CheckedListBoxItem(productId, true));
        }

        private void SetPivotControl()
        {
            MakeDataTableFields();
        }

        public PivotGridField GetColumnField()
        {
            return this.columnField;
        }

        private void MakeDataTableFields()
        {
            this.pivotGridControl1.DataSource = null;
            this.pivotGridControl1.Fields.Clear();

            this.pivotGridControl1.AddFieldRowArea<ResultItem>((row) => row.PRODUCT_ID);
            //this.pivotGridControl1.AddFieldRowArea<ResultItem>((row) => row.PRODUCT_SERIES);
            this.pivotGridControl1.AddFieldRowArea<ResultItem>((row) => row.CATEGORY);

            this.columnField = this.pivotGridControl1.AddFieldColumnArea<ResultItem>(data => data.DAY);
            this.dataField = this.pivotGridControl1.AddFieldDataArea<ResultItem>(data => data.QTY);
            dataField.CellFormat.FormatString = "#,##0";

            this.pivotGridControl1.OptionsBehavior.HorizontalScrolling = DevExpress.XtraPivotGrid.PivotGridScrolling.Control;
            this.pivotGridControl1.OptionsView.ShowRowGrandTotals = false;
            this.pivotGridControl1.OptionsView.ShowColumnGrandTotalHeader = false;
            this.pivotGridControl1.OptionsView.ShowRowTotals = false;
            this.pivotGridControl1.OptionsView.ShowFilterHeaders = false;
            this.pivotGridControl1.OptionsView.ColumnTotalsLocation = PivotTotalsLocation.Near;
        }

        internal DateTime GetStartTime()
        {
            return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
        }

        internal DateTime GetEndTime()
        {
            return MyHelper.DATASVC.GetPlanEndTime(this.Result);
        }

        public void Query(bool cumulative, List<string> productIds = null)
        {
            DateTime startTime = this.TargetStartDate;
            DateTime endTime = this.TargetEndDate;

            if (productIds == null)
                productIds = this.TargeteProductID;
                //checkedComboBoxEdit1.Properties.Items.GetCheckedValues().ConvertAll(x => x.ToString());

            this.Query(productIds, startTime, endTime, cumulative);

            //LoadDefaultLayOutFromXml();
        }

        public void Query(List<string> productID, DateTime startTime, DateTime endTime, bool cumulative)
        {
            this.isCumulative = cumulative;
            this.pivotGridControl1.BeginUpdate();
            this.pivotGridControl1.DataSource = Calculate(productID, startTime, endTime, cumulative).ToBindingList();
            this.pivotGridControl1.EndUpdate();
            this.pivotGridControl1.BestFitColumnArea();
            this.pivotGridControl1.BestFitRowArea();
        }

        //private void LoadDefaultLayOutFromXml()
        //{
        //    string dirPath = string.Format("{0}\\DefaultLayOut", _application.ApplicationPath);
        //    string fileName = string.Format("{0}.xml", _pageID);

        //    PivotGridLayoutHelper.LoadXml(this.control.GetPivotControl(), dirPath, fileName);
        //}

        #region Excel Export

        //상위메뉴 Data>ExportToExcel --> Enable
        //protected override bool UpdateCommand(Command command)
        //{
        //    bool handled = false;

        //    switch (command.CommandID)
        //    {
        //        case DataCommands.DataExportToExcel:
        //            command.Enabled = true;
        //            handled = true;
        //            break;
        //    }

        //    if (handled) return true;
        //    return base.UpdateCommand(command);
        //}

        //protected override bool HandleCommand(Command command)
        //{
        //    bool handled = false;

        //    switch (command.CommandID)
        //    {
        //        case DataCommands.DataExportToExcel:
        //            {
        //                PivotGridExporter.ExportToExcel(this.control.GetPivotControl());
        //                handled = true;
        //                break;
        //            }
        //    }

        //    if (handled) return true;
        //    return base.HandleCommand(command);
        //}

        #endregion

        #region Event

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Query(this.checkShowCumulative.Checked);
        }

        private void buttonExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.PIVOTEXPORT.ExportToExcel(this.pivotGridControl1);
        }

        private void checkShowCumulative_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Query(this.checkShowCumulative.Checked);
        }

        #endregion

        private IEnumerable<ResultItem> Calculate(List<string> productId, DateTime startTime, DateTime endTime, bool cumulative)
        {
			this.slackDic = new Dictionary<string, double>();
			this.demandDic = new Dictionary<string, ResultItem>();

            //var product = this._result.LoadInput<PRODUCT>("PRODUCT").Where(x => productId.Contains(x.PRODUCT_ID)).Select(x => new { x.PRODUCT_ID, x.MODEL2 });
            var product = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result).Where(x => productId.Contains(x.PRODUCT_ID)).Select(x => new { x.PRODUCT_ID, x.PRODUCT_NAME });

            var rtf = CreateCompareItems(productId, startTime, endTime);

            Dictionary<string, HashSet<DateTime>> usedDates = new Dictionary<string, HashSet<DateTime>>();

            var rtf2 = rtf.Select(x => new CompareItem
            {
                DEMAND_ID = x.DEMAND_ID,
                PRODUCT_ID = x.PRODUCT_ID,
                DUE_DATE = x.DUE_DATE,
                PLAN_DATE = x.PLAN_DATE,
                DEMAND_QTY = x.DEMAND_QTY,
                PLAN_QTY = x.PLAN_QTY,
				SLACK_DAYS = x.SLACK_DAYS, //x.PLAN_DATE == DateTime.MinValue ? (x.DUE_DATE - x.PLAN_DATE).TotalDays : (x.DUE_DATE - x.PLAN_DATE).TotalDays,
				RTF_RATIO = x.DEMAND_QTY == 0 ? 0 : Math.Round((x.PLAN_QTY / x.DEMAND_QTY) * 100, 2)
            }).OrderBy(x => x.PRODUCT_ID);

            List<ResultItem> results = new List<ResultItem>();

            Dictionary<string, string> productDic = new Dictionary<string, string>();

			foreach (var p in MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result))
				productDic[p.PRODUCT_ID] = p.PRODUCT_NAME;

			Dictionary<string, ResultItem> demandResultDic = new Dictionary<string, ResultItem>();
            Dictionary<string, ResultItem> planResultDic = new Dictionary<string, ResultItem>();

            foreach (var r in rtf2)
            {
                ResultItem result = CreateResultItem(r, demandResultDic);
                if (result != null)
                {
                    result.PRODUCT_SERIES = productDic.ContainsKey(result.PRODUCT_ID) ? productDic[result.PRODUCT_ID] : "NO PRODUCT INFO";
                    results.Add(result);
                }

                result = CreateResultItem(r, planResultDic, false);
                if (result != null)
                {
                    result.PRODUCT_SERIES = productDic.ContainsKey(result.PRODUCT_ID) ? productDic[result.PRODUCT_ID] : "NO PRODUCT INFO";
                    results.Add(result);
                }
            }

            List<ResultItem> dummyItem = FillEmptyResult(productDic, startTime, endTime, GetUsedDates(rtf), true);
            dummyItem.AddRange(FillEmptyResult(productDic, startTime, endTime, GetUsedDates(rtf, false), false));
            results.AddRange(dummyItem);

            if (cumulative == true)
                results = GetCumulativeResults(results).ToList();

            return results;
        }

        private IEnumerable<ResultItem> GetCumulativeResults(List<ResultItem> results)
        {
            Dictionary<string, double> demandCum = new Dictionary<string, double>();
            Dictionary<string, double> planCum = new Dictionary<string, double>();

            var sortedResults = results.OrderBy(x => x.PRODUCT_ID).ThenBy(x => x.DAY);

            Dictionary<string, double> dic;
            foreach (var result in sortedResults)
            {
                if (result.CATEGORY == "Demand")
                    dic = demandCum;
                else
                    dic = planCum;

                double value = 0;

                if (dic.TryGetValue(result.PRODUCT_ID, out value) == false)
                {
                    dic[result.PRODUCT_ID] = result.QTY;
                    continue;
                }

                result.QTY = result.QTY + value;
                dic[result.PRODUCT_ID] = result.QTY;
            }

            return sortedResults;
        }

        private IEnumerable<CompareItem> CreateCompareItems(List<string> productId, DateTime startTime, DateTime endTime)
        {
            var plan = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(this.Result)
            //var plan = this._result.LoadOutput<OPER_PLAN>("OPER_PLAN")
                .Where(x => productId.Contains(x.PRODUCT_ID) && this.BetweenDate(x.EQP_START_TIME, startTime, endTime) && x.STEP_ID == GetLastOperation(x.DEMAND_ID)) //x.OPER_ID == "POL" && 
                .GroupBy(x => new { x.DEMAND_ID, x.PRODUCT_ID })
                .Select(x => new { DEMAND_ID = x.Key.DEMAND_ID, PRODUCT_ID = x.Key.PRODUCT_ID, PLAN_DATE = x.Max(y => y.EQP_START_TIME), OUTQty = x.Sum(y => y.PROCESS_QTY), SLACK_DAY = SlackDay(x.Min(y => y.EQP_START_TIME), x.Max(y => y.DUE_DATE)) })
                ;

            var demand = MyHelper.DATASVC.GetEntityData<DEMAND>(this.Result)
            //var demand = this._result.LoadInput<DEMAND>("DEMAND")
                .Where(x => productId.Contains(x.PRODUCT_ID) && this.BetweenDate(x.DUE_DATE, startTime, endTime))
                .Select(x => new { x.DEMAND_ID, x.DUE_DATE, x.PRODUCT_ID, x.DEMAND_QTY });

            Dictionary<string, CompareItem> dic = new Dictionary<string, CompareItem>();

            foreach (var d in demand)
            {
                CompareItem item;
                if (dic.TryGetValue(d.DEMAND_ID, out item) == false)
                {
                    item = new CompareItem();
                    dic[d.DEMAND_ID] = item;
                }

                item.DEMAND_ID = d.DEMAND_ID;
                item.PRODUCT_ID = d.PRODUCT_ID;
                item.DUE_DATE = d.DUE_DATE;
                item.DEMAND_QTY = d.DEMAND_QTY;
                item.PLAN_DATE = DateTime.MinValue;
                item.PLAN_QTY = 0;
				item.SLACK_DAYS = double.MinValue;
			}

            foreach (var p in plan)
            {
                CompareItem item;
                if (dic.TryGetValue(p.DEMAND_ID, out item) == false)
                {
                    item = new CompareItem();
                    dic[p.DEMAND_ID] = item;
                }

                item.DEMAND_ID = p.DEMAND_ID;
                item.PRODUCT_ID = p.PRODUCT_ID;
                item.PLAN_DATE = p.PLAN_DATE;
                item.PLAN_QTY = p.OUTQty;
				item.SLACK_DAYS = p.SLACK_DAY;
			}

            return dic.Values;
        }

        public DateTime ConvertDateTime(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            return dt;
        }

        public bool BetweenDate(DateTime date, DateTime startTime, DateTime endTime)
        {
            if (startTime <= date && date <= endTime)
                return true;

            return false;
        }

        public double SlackDay(DateTime startTime, DateTime endTime)
        {
            TimeSpan ts = endTime - startTime;
            return ts.TotalDays;
		}

        private Dictionary<string, HashSet<string>> GetUsedDates(IEnumerable<CompareItem> list, bool isDemand = true)
        {
            Dictionary<string, HashSet<string>> usedDates = new Dictionary<string, HashSet<string>>();

            foreach (CompareItem item in list)
            {
                HashSet<string> dates;
                if (usedDates.TryGetValue(item.PRODUCT_ID, out dates) == false)
                {
                    dates = new HashSet<string>();
                    usedDates[item.PRODUCT_ID] = dates;
                }

                string day = GetDayString(item.DUE_DATE);
                if (isDemand && item.DUE_DATE != DateTime.MinValue && dates.Contains(day) == false)
                    dates.Add(day);

                day = GetDayString(item.PLAN_DATE);
                if (isDemand == false && item.PLAN_DATE != DateTime.MinValue && dates.Contains(day) == false)
                    dates.Add(day);
            }

            return usedDates;
        }
        private List<ResultItem> FillEmptyResult(Dictionary<string, string> productDic, DateTime startTime, DateTime endTime, Dictionary<string, HashSet<string>> usedDates, bool isDemand)
        {
            List<ResultItem> results = new List<ResultItem>();
            for (DateTime date = startTime; date <= endTime; date = date.AddDays(1))
            {
                string day = GetDayString(date);

                foreach (var productId in usedDates.Keys)
                {
                    HashSet<string> set = usedDates[productId];

                    if (set.Contains(day) == true)
                        continue;

                    string model = productDic.ContainsKey(productId) ? productDic[productId] : "NO PRODUCT INFO";

                    if (isDemand)
                    {
                        ResultItem item = CreateDummyReusltItem(productId, model, day, "Demand");
                        string key = CreateKey(item.PRODUCT_ID, item.DAY);
                        results.Add(item);

                        if (this.isCumulative)
                            this.demandDic[key] = item;
                    }
                    else
                        results.Add(CreateDummyReusltItem(productId, model, day, "Plan"));
                }
            }

            return results;
        }

        private ResultItem CreateDummyReusltItem(string productId, string model, string day, string type)
        {
            ResultItem result = new ResultItem();
            result.PRODUCT_ID = productId;
            result.PRODUCT_SERIES = model;
            result.DAY = day;
            result.CATEGORY = type;
            result.QTY = 0;

            return result;


        }

        private string GetDayString(DateTime date)
        {
            //return string.Format("{0}/{1:00}", date.Month, date.Day);
            return date.ToString("yyyyMMdd");
        }

        private string GetLastOperation(string demandId)
        {
            DEMAND demand;
            if (_demandDict.TryGetValue(demandId, out demand))
            {
                PRODUCT product;
                if (_productDict.TryGetValue(demand.PRODUCT_ID, out product))
                {
                    STEP_ROUTE route;
                    if (_stepRouteDict.TryGetValue(product.PROCESS_ID, out route))
                        return route.STEP_ID;
				}
			}
            //string lastOperation = string.Empty;

            //List<OPERATION> opertionInfos = this._result.LoadInput<OPERATION>("OPERATION").ToList(); ;

            //opertionInfos.Sort((x, y) => y.SEQUENCE.CompareTo(x.SEQUENCE));

            //lastOperation = opertionInfos.ElementAt(0).OPER_ID;

            return string.Empty;
        }

        private ResultItem CreateResultItem(CompareItem comp, Dictionary<string, ResultItem> results, bool isDemand = true)
        {
            DateTime date = isDemand == true ? comp.DUE_DATE : comp.PLAN_DATE;

            if (isDemand && comp.DUE_DATE == DateTime.MinValue)
                date = comp.PLAN_DATE;
            else if (isDemand == false && comp.PLAN_DATE == DateTime.MinValue)
                date = comp.DUE_DATE;

            string day = GetDayString(date);

            bool hasItem = false;
            ResultItem result;
            string key = CreateKey(comp.PRODUCT_ID, day);

            if (results.TryGetValue(key, out result) == false)
            {
                result = new ResultItem();
                results[key] = result;
            }
            else
                hasItem = true;


            result.DEMAND_ID = comp.DEMAND_ID;
            result.DAY = GetDayString(date);
            result.PRODUCT_ID = comp.PRODUCT_ID;
            result.QTY = result.QTY + (isDemand ? comp.DEMAND_QTY : comp.PLAN_QTY);
            result.CATEGORY = isDemand ? "Demand" : "Plan";

            if (isDemand)
            {
                if (this.demandDic.ContainsKey(key) == false)
                    this.demandDic[key] = result;

                return hasItem == false ? result : null;
            }

			double slackDay = 0;
			if (this.slackDic.TryGetValue(key, out slackDay) == false)
				this.slackDic[key] = comp.SLACK_DAYS;
			else
				this.slackDic[key] = Math.Min(slackDay, comp.SLACK_DAYS);

			return hasItem == false ? result : null;
        }

        public string CreateKey(string productId, string day)
        {
            return string.Format("{0}/{1}", productId, day);

        }

        #region Internal Class : ResultItem
        internal class ResultItem : Expandable
        {
            public string PRODUCT_ID { get; set; }
            public string PRODUCT_SERIES { get; set; }
            public string DEMAND_ID { get; set; }
            public string CATEGORY { get; set; }
            public string DAY { get; set; }
            public double QTY { get; set; }
        }

        public class CompareItem
        {
            public string DEMAND_ID { get; set; }
            public string PRODUCT_ID { get; set; }
            public string MODEL { get; set; }
            public DateTime DUE_DATE { get; set; }
            public DateTime PLAN_DATE { get; set; }
            public double DEMAND_QTY { get; set; }
            public double PLAN_QTY { get; set; }
			public double SLACK_DAYS { get; set; }
			public double RTF_RATIO { get; set; }
        }
        #endregion


        #region Event
        private void PivotGridControl1_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
        {
            if (e.RowField == null || e.ColumnField == null) return;

            string key = GetKeyFromEventArgs(e);

            string rowFieldValue = e.GetFieldValue(e.RowField).ToString();

            if (rowFieldValue == "Demand")
            {
                if (e.DisplayText != "")
                    e.Appearance.BackColor = Color.LightBlue;
                else if (string.IsNullOrEmpty(e.DisplayText))
                    e.Appearance.BackColor = Color.LightCyan;
            }
            else
            {

                if (this.isCumulative == false)
                {
                    if (string.IsNullOrEmpty(e.DisplayText) || e.DisplayText == "0")
                    {
                        if (this.demandDic.ContainsKey(key) == true)
                            e.Appearance.ForeColor = Color.Red;
                    }

                    double slack;
                    if (slackDic.TryGetValue(key, out slack) == false)
                        return;

                    if (slack == double.MinValue)
                        return;

                    if (slack < 0)
                        e.Appearance.ForeColor = Color.Red;
                    //else
                    //e.Appearance.BackColor = Color.Green;
                }
                else
                {
                    ResultItem item;
                    if (this.demandDic.TryGetValue(key, out item) == false)
                        return;

                    double d;
                    double.TryParse(e.Value.ToString(), out d);

                    if (item.QTY > d)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Blue;


                }
            }

        }

        private void PivotGridControl1_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            if (e.Value == null)
                return;

            if (e.ColumnValueType == DevExpress.XtraPivotGrid.PivotGridValueType.GrandTotal && this.isCumulative)
            {
                PivotGridControl pgc = sender as PivotGridControl;
                int column = pgc.Cells.ColumnCount;

                //Demand Accum 또는 Plan Accum 마지막 값으로 변경.
                //DevExpress.XtraPivotGrid.PivotCellEventArgs cellInfo = pgc.Cells.GetCellInfo(column - 1, e.RowIndex);
                
                e.DisplayText = pgc.GetCellValue(column - 1, e.RowIndex).ToString();
                
                return;
            }

            double d;
            Double.TryParse(e.Value.ToString(), out d);

            if (d <= 0)
            {
                string key = GetKeyFromEventArgs(e);
                if (this.demandDic.ContainsKey(key) == true && isCumulative == false)
                    e.DisplayText = "0";
                else
                    e.DisplayText = string.Empty;
            }
        }

        private string GetKeyFromEventArgs(PivotCellBaseEventArgs e)
        {
            PivotGridField field = e.GetRowFields().FirstOrDefault(x => x.FieldName == "PRODUCT_ID");

            if (field == null || e.ColumnField == null)
                return string.Empty;

            string productFieldValue = e.GetFieldValue(field).ToString();
            string columnFiledVlaue = e.GetFieldValue(e.ColumnField).ToString();

            string key = CreateKey(productFieldValue, columnFiledVlaue);

            return key;

        }

        #endregion

        private void PivotGridControl1_CellClick(object sender, PivotCellEventArgs e)
        {
            PivotGridField field = e.GetRowFields().FirstOrDefault(x => x.FieldName == "PRODUCT_ID");

            if (field == null || e.ColumnField == null)
                return;

            string productFieldValue = e.GetFieldValue(field).ToString();
            string columnFiledVlaue = e.GetFieldValue(e.ColumnField).ToString();

            string key = CreateKey(productFieldValue, columnFiledVlaue);

            double slack = 0;
            if (this.slackDic.TryGetValue(key, out slack) == false)
                return;

            e.RowField.ToolTips.ValueText = slack.ToString();
        }

        private void ToolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            if (e.SelectedControl != pivotGridControl1)
                return;

            PivotGridHitInfo hitInfo = pivotGridControl1.CalcHitInfo(e.ControlMousePosition);
            if (hitInfo.HitTest != PivotGridHitTest.Cell)
                return;

            PivotCellEventArgs cellInfo = hitInfo.CellInfo;
            PivotGridField field = cellInfo.GetRowFields().FirstOrDefault(x => x.FieldName == "PRODUCT_ID");

            if (field == null)
                return;

            if (cellInfo.ColumnField == null)
                return;

            if (cellInfo.GetFieldValue(cellInfo.RowField).ToString() == "Demand")
                return;

            string productFieldValue = hitInfo.CellInfo.GetFieldValue(field).ToString();
            string columnFiledVlaue = hitInfo.CellInfo.GetFieldValue(cellInfo.ColumnField).ToString();

            string key = CreateKey(productFieldValue, columnFiledVlaue);

			double slack = 0;
			if (this.slackDic.TryGetValue(key, out slack) == false)
				return;

			e.Info = new ToolTipControlInfo("SlackDay", slack.ToString(), "SlackDay");
		}
	}
}
