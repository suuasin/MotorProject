using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mozart.Studio.TaskModel.UserInterface;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.Projects;
using DevExpress.XtraPivotGrid;
using System.Globalization;
//using Mozart.SeePlan.BCP;
using DevExpress.XtraEditors.Controls;
using Mozart.Studio.Application;
using Mozart.Studio.TaskModel.Utility;
using Mozart.Studio.UIComponents;
using SmartAPS.UI.Utils;
using SmartAPS;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;

namespace SmartAPS.UI.Analysis
{
    public partial class PSI : MyXtraPivotGridTemplate
    {
        private Dictionary<string, PSIInformation> PSIInformationData = new Dictionary<string, PSIInformation>();

        private Dictionary<string, string> DemandEndStepId = new Dictionary<string, string>();

        public PSI()
        {
            InitializeComponent();
        }

        public PSI(IServiceProvider serviceProvider)
       : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {   
            base.LoadDocument();

            this.SetMainPivotGrid(this.ResultPivot);

            this.AddExtendPivotGridMenu(this.ResultPivot);

            SetControls();
            Initialize();
            InitializeData();
            GetData();
        }



        private void SetControls()
        {
            InitializeDateTimePicker();

            MyHelper.ENGCONTROL.SetControl_ProductID_Checked(this.editProdId, this.Result);

            int days = MyHelper.DATASVC.GetPlanPeriod(this.Result);
            this.numericUpDownWeek.EditValue = (int)Math.Ceiling((double)days / 7);
            this.checkEditAccuMoreOrLess.EditValue = true;
        }       

        private void InitializeDateTimePicker()
        {
            //this.dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            //this.dateTimePicker1.Format = DateTimePickerFormat.Custom;

            this.dateTimePicker1.EditValue = GetStartTime();            
        }

        //private void InitializeProductComboBox()
        //{
        //    List<string> productIDs = new List<string>();

        //    IEnumerable<DEMAND_HIS> demandList = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result); 

        //    foreach (DEMAND_HIS demand in demandList)
        //    {
        //        if(productIDs.Contains(demand.PRODUCT_ID) == false)
        //            productIDs.Add(demand.PRODUCT_ID);
        //    }

        //    productIDs.Sort((x, y) => x.CompareTo(y));

        //    foreach (string product in productIDs)
        //        repositoryItemCheckedComboBoxEdit1.Items.Add(new CheckedListBoxItem(product, true));
        //}

        private void InitializeData()
        {
            Dictionary<string, string> productProcessId = new Dictionary<string, string>();

            IEnumerable<PRODUCT> products = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result);
            foreach (var product in products)
                productProcessId[product.PRODUCT_ID] = product.PROCESS_ID;

            IEnumerable<STEP_ROUTE> steps = MyHelper.DATASVC.GetEntityData<STEP_ROUTE>(this.Result);
            Dictionary<string, List<STEP_ROUTE>> processSteps = new Dictionary<string, List<STEP_ROUTE>>();

            foreach (var step in steps)
            {
                if (processSteps.ContainsKey(step.PROCESS_ID) == false)
                    processSteps[step.PROCESS_ID] = new List<STEP_ROUTE>();

                processSteps[step.PROCESS_ID].Add(step);
            }

            IEnumerable<DEMAND_HIS> demands = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result);

            foreach (var demand in demands)
            {
                string processId;
                if (productProcessId.TryGetValue(demand.PRODUCT_ID, out processId) == false)
                    continue;

                if (processSteps.ContainsKey(processId) == false)
                    continue;

                this.DemandEndStepId[demand.DEMAND_ID] = processSteps[processId].OrderByDescending(x => x.STEP_SEQ).FirstOrDefault().STEP_ID;
            }
        }

        private void Initialize()
        {
            this.ResultPivot.Fields.Clear();
            this.PSIInformationData = new Dictionary<string, PSIInformation>();
        }

        protected override void Query()
        {   
            HashSet<ResultItem> resultItem = CreateResultItem();

            BindData(resultItem);
        }

        new private XtraPivotGridHelper.DataViewTable GetData()
        {            
            //CollectOutActData();//기초재고설정 -- OutAct를 임의로 기초재고로 설정
            CollectDemandData();
            CollectOperPlanData();

            return null;
        }

        #region CollectData
        //private void CollectOutActData()
        //{
        //    IEnumerable<DEMAND> demandList = MyHelper.DATASVC.GetEntityData<ACT>(this.Result);
        //    List<OUT_ACT> actList = this.Result.LoadInput<ACT>("OUT_ACT").ToList();

        //    List<string> validProducts = checkedComboBoxProduct.Properties.Items.GetCheckedValues().ConvertAll(x => x.ToString());

        //    Dictionary<string, double> actQtyInfos = new Dictionary<string, double>();

        //    foreach (OUT_ACT actInfo in actList)
        //    {
        //        if (actQtyInfos.ContainsKey(actInfo.PRODUCT_ID) == false)
        //            actQtyInfos.Add(actInfo.PRODUCT_ID, 0);

        //        actQtyInfos[actInfo.PRODUCT_ID] += actInfo.ACT_QTY;
        //    }

        //    foreach (KeyValuePair<string, double> actInfo in actQtyInfos)
        //    {
        //        string productID = actInfo.Key;

        //        if (validProducts.Contains(productID) == false)
        //            continue;

        //        if (this.PSIInformationData.ContainsKey(productID) == false)
        //        {
        //            CreatePSIInformation(productID);
        //        }

        //        PSIInformation psiInfo;
        //        if (this.PSIInformationData.TryGetValue(productID, out psiInfo))
        //        {
        //            psiInfo.BasicProductionQty += actInfo.Value;
        //        }
        //    }
        //}
        private void CollectDemandData()
        {
            List<DEMAND_HIS> demandList = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result).ToList();

            demandList.Sort((x, y) => x.DUE_DATE.CompareTo(y.DUE_DATE));
            
            List<string> validProducts = repositoryItemCheckedComboBoxEdit1.Items.GetCheckedValues().ConvertAll(x => x.ToString());

            foreach (DEMAND_HIS demand in demandList)
            {
                string productID = demand.PRODUCT_ID;

                if (validProducts.Contains(productID) == false)
                    continue;

                string weekNo = MyHelper.DATE.WeekOfYear(demand.DUE_DATE).ToString();
                string yearMonth = MyHelper.DATE.GetMonthNo(demand.DUE_DATE);

                if (this.PSIInformationData.ContainsKey(productID) == false)
                {
                    CreatePSIInformation(productID);
                }

                string key = CreateKey(productID, weekNo, yearMonth);

                PSIInformation psiInfo;
                if (this.PSIInformationData.TryGetValue(key, out psiInfo))
                {
                    psiInfo.DemandQty += demand.DEMAND_QTY;
                }
            }
        }
        private void CollectOperPlanData()
        {
            IEnumerable<EQP_PLAN> eqpPlans = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(this.Result);

            List<string> validProducts = repositoryItemCheckedComboBoxEdit1.Items.GetCheckedValues().ConvertAll(x => x.ToString());

            foreach (EQP_PLAN plan in eqpPlans)
            {
                if (plan.EQP_STATE_CODE != "BUSY")
                    continue;

                string lastOperation = GetLastOperation(plan);
                if (plan.STEP_ID != lastOperation)
                    continue;

                string productID = plan.PRODUCT_ID;

                if (validProducts.Contains(productID) == false)
                    continue;

                string weekNo = MyHelper.DATE.WeekOfYear(plan.EQP_END_TIME).ToString();
                string yearMonth = MyHelper.DATE.GetMonthNo(plan.EQP_END_TIME);

                if (this.PSIInformationData.ContainsKey(productID) == false)
                {
                    CreatePSIInformation(productID);
                }

                string key = CreateKey(productID, weekNo, yearMonth);

                PSIInformation psiInfo;
                if (this.PSIInformationData.TryGetValue(key, out psiInfo))
                {
                    psiInfo.PlanQty += plan.PROCESS_QTY;
                }
            }
        }
        #endregion CollectData

        public void BindData(HashSet<ResultItem> result)
        {
            SetPivotGridFields(this.ResultPivot);
            SetPivotGridData(this.ResultPivot, result);
        }

        private HashSet<ResultItem> CreateResultItem()
        {
            DateTime filterStartTime = (DateTime)this.dateTimePicker1.EditValue;
            DateTime filterEndTime = filterStartTime.AddDays((Convert.ToInt32(this.numericUpDownWeek.EditValue) * 7));

            string filterStartWeek = MyHelper.DATE.WeekOfYear(filterStartTime).ToString();
            string filterEndWeek = MyHelper.DATE.WeekOfYear(filterEndTime).ToString();

            HashSet<ResultItem> resultItem = new HashSet<ResultItem>();

            HashSet<string> products = new HashSet<string>();

            foreach(PSIInformation info in this.PSIInformationData.Values)
            {
                products.Add(info.ProductID);
            }

            foreach(string product in products)
            {
                PSIInformation info;
                if (this.PSIInformationData.TryGetValue(product, out info))
                {
                    info.BOH = info.BasicProductionQty;
                    
                    while (info.NextPSIInfo != null)
                    {
                        if (info.PrevPSIInfo != null)
                        {
                            info.BOH = info.PrevPSIInfo.EOH;
                            info.AccumulateMoreOrLess = info.PrevPSIInfo.MoreOrLess;
                        }

                        if (filterStartWeek.CompareTo(info.WeekNo) <= 0 && filterEndWeek.CompareTo(info.WeekNo) >= 0)
                        {
                            foreach (Category category in Enum.GetValues(typeof(Category)))
                            {
                                //if (category == Category.NONE || category == Category.BOH || category == Category.EOH)
                                //    continue;

                                if (category != Category.Demand && category != Category.PlanQty && category != Category.Inventory)
                                    continue;

                                double value = GetResultItemValue(info, category);

                                ResultItem item = new ResultItem(info.ProductID, category, info.WeekNo, info.YearMonth, value);

                                resultItem.Add(item);
                            }
                        }
                        else
                        {

                        }

                        info = info.NextPSIInfo;
                    }
                }
            }

            return resultItem;
        }
        private void CreatePSIInformation(string productID)
        {
            DateTime startTime = GetStartTime();

            int gap = Convert.ToInt32(Math.Ceiling(((DateTime)this.dateTimePicker1.EditValue).Subtract(startTime).TotalDays));

            if (gap < 0)
                gap = 0;

            PSIInformation prevInfo = null;
            for (int i = 0; i <= gap + Convert.ToInt32(this.numericUpDownWeek.EditValue) * 7; i++)
            {
                startTime = startTime.AddDays(1);

                string weekNo = MyHelper.DATE.WeekOfYear(startTime).ToString();
                string yearMonth = MyHelper.DATE.GetMonthNo(startTime);

                string key = CreateKey(productID, weekNo, yearMonth);

                PSIInformation psiInfo;
                if (this.PSIInformationData.TryGetValue(key, out psiInfo) == false)
                {
                    psiInfo = new PSIInformation(productID, weekNo, yearMonth, (bool)this.checkEditAccuMoreOrLess.EditValue);

                    this.PSIInformationData.Add(key, psiInfo);

                    if (this.PSIInformationData.ContainsKey(productID) == false)
                        this.PSIInformationData.Add(productID, psiInfo);

                    if (prevInfo != null)
                    {
                        psiInfo.PrevPSIInfo = prevInfo;
                        prevInfo.NextPSIInfo = psiInfo;
                    }

                    prevInfo = psiInfo;
                }
            }
        }
        private double GetResultItemValue(PSIInformation info, Category category)
        {
            double value = 0;

            switch (category)
            {
                case Category.BOH:
                    {
                        value = info.BOH;
                        break;
                    }
                case Category.EOH:
                    {
                        value = info.EOH;
                        break;
                    }
                case Category.Demand:
                    {
                        value = info.DemandQty;
                        break;
                    }
                case Category.PlanQty:
                    {
                        value = info.PlanQty;
                        break;
                    }
                case Category.RTF:
                    {
                        value = info.RTF;
                        break;
                    }
                case Category.RTFRate:
                    {
                        value = info.RTFRate;
                        break;
                    }
                case Category.Inventory:
                    {
                        value = info.MoreOrLess;
                        break;
                    }
                default:
                    {
                        value = 0;
                        break;
                    }
            }

            return value;
        }
       
        private void SetPivotGridData(PivotGridControl pivotGrid, IEnumerable<ResultItem> dt)
        {
            pivotGrid.BeginUpdate();
            pivotGrid.DataSource = dt.ToBindingList();
            pivotGrid.EndUpdate();
            pivotGrid.BestFit();
            pivotGrid.Fields[1].SortMode = PivotSortMode.Custom;
        }
        private void SetPivotGridFields(PivotGridControl pivotGrid)
        {
            pivotGrid.AddFieldRowArea<ResultItem>((c) => c.ProductID, "PRODUCT ID");
            pivotGrid.AddFieldRowArea<ResultItem>((c) => c.Category, "CATEGORY");
            pivotGrid.AddFieldColumnArea<ResultItem>((c) => c.YearMonth, null, "YEAR MONTH");
            pivotGrid.AddFieldColumnArea<ResultItem>((c) => c.WeekNo, null, "WEEK NO");

            var v = pivotGrid.AddFieldDataArea<ResultItem>((c) => c.Value, null, "VALUE");
            v.CellFormat.FormatString = "#,###";
        }
        private string GetLastOperation(EQP_PLAN plan)
        {
            string lastOperation = string.Empty;

            this.DemandEndStepId.TryGetValue(plan.DEMAND_ID, out lastOperation);

            return lastOperation;
        }
        internal DateTime GetStartTime()
        {
            DateTime startTime = MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
            
            return new DateTime(startTime.Year, startTime.Month, startTime.Day);
        }

        internal DateTime GetEndTime()
        {
            return MyHelper.DATASVC.GetPlanEndTime(this.Result);
        }
        public static string CreateKey(params string[] strArr)
        {
            string sValue = null;
            foreach (string str in strArr)
            {
                if (sValue == null)
                    sValue = str;
                else
                    sValue += '@' + str;
            }

            return sValue ?? string.Empty;
        }
        public string GetCategoryString(Category category)
        {
            string str = category.ToString();

            if (category == PSI.Category.Demand)
                str += "(S)";
            else if (category == PSI.Category.PlanQty)
                str += "(P)";
            else if (category == PSI.Category.BOH || category == PSI.Category.EOH || category == PSI.Category.Inventory)
                str += "(I)";
            else if (category == PSI.Category.RTFRate)
                str += "(%)";

            return str;
        }

        public T StringToEnum<T>(string src, T defValue)
        {
            if (string.IsNullOrEmpty(src))
                return defValue;

            foreach (string en in Enum.GetNames(typeof(T)))
            {
                if (en.Equals(src, StringComparison.CurrentCultureIgnoreCase))
                {
                    defValue = (T)Enum.Parse(typeof(T), src, true);
                    return defValue;
                }
            }

            return defValue;
        }

        #region Event
        //private void QuaryButton_Click(object sender, EventArgs e)
        //{
        //    if (this.Result == null)
        //        return;

        //    Initialize();

        //    CollectData();

        //    HashSet<ResultItem> resultItem = CreateResultItem();

        //    SetPivotGridFields(this.ResultPivot);
        //    SetPivotGridData(this.ResultPivot, resultItem);
        //}

        private void ResultPivot_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
        {
            try
            {                
                if (e.Value == null)
                    return;

                if (e.GetRowFields().Count() <= 1)
                    return;

                if (e.DataField.FieldName == "Value" && e.GetFieldValue(e.GetRowFields()[1]).ToString() == Category.Inventory.ToString())
                {
                    double value = Convert.ToDouble(e.Value);

                    if (value < 0)
                        e.Appearance.ForeColor = Color.Red;
                    else if (value > 0)
                        e.Appearance.ForeColor = Color.Blue;
                }

                if (e.RowField == null || e.ColumnField == null) return;

                string rowFieldValue = e.GetFieldValue(e.RowField).ToString();

                if (rowFieldValue == "Demand")
                    e.Appearance.BackColor = Color.LightBlue;
            }
            catch(Exception ex)
            {

            }
        }

        private void ResultPivot_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null)
                    return;

                if (e.GetRowFields().Count() <= 1)
                    return;

                if (e.DataField.FieldName == "Value" && e.GetFieldValue(e.GetRowFields()[1]).ToString() == Category.RTFRate.ToString())
                {
                    e.DisplayText = Convert.ToDouble(e.Value).ToString("P");
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void ResultPivot_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
        {
            Category category1 = StringToEnum(e.Value1.ToString(), Category.NONE);
            Category category2 = StringToEnum(e.Value2.ToString(), Category.NONE);

            int x = (int)category1;
            int y = (int)category2;

            e.Result = x.CompareTo(y);

            e.Handled = true;
        }

        private void ResultPivot_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e)
        {
            if (e.ValueType == PivotGridValueType.GrandTotal)
            {
                if (e.IsColumn)
                    e.DisplayText = "GRAND TOTAL";
            }

            if (e.Field == null)
                return;

            if (e.Field.Area == PivotArea.RowArea && e.Field.FieldName == "Category")
            {
                Category category = StringToEnum(e.Value.ToString(), Category.NONE);

                e.DisplayText = GetCategoryString(category);
            }
        }

        private void ResultPivot_CustomCellValue(object sender, PivotCellValueEventArgs e)
        {
            if (e.RowField == null)
                return;

            e.RowField.Appearance.Header.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.RowField.Appearance.Header.Options.UseTextOptions = true;
            e.RowField.Appearance.Cell.Options.UseTextOptions = true;

            if (e.ColumnValueType != PivotGridValueType.GrandTotal || (e.ColumnField == null && e.RowField == null))
                return;

            List<double> summaryValues = GetSummaryValues(e);
            e.Value = GetCustomTotalValue(e, summaryValues);

            if (e.Value == null || e.Value.ToString() == "")
                e.Value = "0";
        }

        private double GetCustomTotalValue(PivotCellValueEventArgs e, List<double> summaryValues)
        {
            if (e.GetFieldValue(e.GetRowFields()[1]).ToString() == Category.RTFRate.ToString())
            {
                return summaryValues.Average();
            }
            else if (e.GetFieldValue(e.GetRowFields()[1]).ToString() == Category.Inventory.ToString())
            {
                return summaryValues[summaryValues.Count - 1];
            }
            else
            {
                return summaryValues.Sum();
            }
        }

        private List<double> GetSummaryValues(PivotCellValueEventArgs e)
        {
            PivotSummaryDataSource sds = e.CreateSummaryDataSource();
            List<double> values = new List<double>();

            for (var i = 0; i <= sds.RowCount; i++)
            {
                var value = sds.GetValue(i, e.DataField);
                if (value == null) 
                    continue;

                values.Add(double.Parse(value.ToString()));
            }

            return values;
        }
        #endregion Event

        #region InnerClass
        public class PSIInformation
        {
            public string ProductID { get; private set; }
            public string WeekNo { get; private set; }
            public string YearMonth { get; private set; }
            public double DemandQty { get; set; }
            public double PlanQty { get; set; }
            public double BasicProductionQty { get; set; }  
            public double AccumulateMoreOrLess { get; set; }
            public double BOH { get; set; }
            public double EOH
            {
                get
                {
                    double eoi = 0;

                    if (this.ApplyMoreOrLessCarryForward)
                    {
                        if(this.PrevPSIInfo != null && this.PrevPSIInfo.MoreOrLess < 0)
                            eoi = this.BOH + this.PlanQty - this.DemandQty + this.PrevPSIInfo.MoreOrLess;
                        else
                            eoi = this.BOH + this.PlanQty - this.DemandQty;
                    }
                    else
                    {
                        eoi = this.BOH + this.PlanQty - this.DemandQty;
                    }

                    if (eoi <= 0)
                        eoi = 0;

                    return eoi;
                }
            }
            public double RTF
            {
                get
                {
                    return Math.Min(this.DemandQty, this.PlanQty + this.BOH);
                }
            }

            public double RTFRate
            {
                get
                {
                    if (this.DemandQty <= 0 && this.MoreOrLess > -0.000001)
                        return 1;

                    if (this.DemandQty <= 0)
                        return 0;

                    return this.RTF / this.DemandQty;
                }
            }

            public double MoreOrLess
            {
                get
                {
                    if (this.ApplyMoreOrLessCarryForward)
                    {
                        double prevMoreOrLess = 0;

                        if(this.PrevPSIInfo != null && this.PrevPSIInfo.MoreOrLess < 0)
                        {
                            prevMoreOrLess = this.PrevPSIInfo.MoreOrLess;
                        }

                        return this.PlanQty + this.BOH - this.DemandQty + prevMoreOrLess;
                    }
                    else
                    {
                        return this.PlanQty + this.BOH - this.DemandQty;
                    }

                }
            }

            public PSIInformation PrevPSIInfo { get; set; }
            public PSIInformation NextPSIInfo { get; set; }

            public bool ApplyMoreOrLessCarryForward { get; set; }

            public PSIInformation(string prodID, string weekNo, string yearMonth, bool applyMoreOrLessCarryForward)
            {
                this.ProductID = prodID;
                this.WeekNo = weekNo;
                this.YearMonth = yearMonth;
                this.ApplyMoreOrLessCarryForward = applyMoreOrLessCarryForward;
            }
        }

        public class ResultItem
        {
            public string ProductID { get; private set; }
            public string Category { get; private set; }
            public string WeekNo { get; private set; }
            public string YearMonth { get; private set; }
            public double Value { get; private set; }

            public ResultItem(string productID, Enum category, string weekNo, string yearMonth, double value)
            {
                this.ProductID = productID;
                this.Category = category.ToString();

                //if(weekNo.Length >= 6)
                //    this.WeekNo = string.Format("W{0}", weekNo.Substring(4, 2));

                this.WeekNo = string.Format("W{0}", weekNo);

                this.YearMonth = yearMonth;
                this.Value = value;
            }
        }

        public enum Category
        {
            Demand = 1,
            RTF = 2,
            Inventory = 9,
            RTFRate = 4,
            PlanQty = 5,
            BOH = 6,
            EOH = 7,
            NONE = 999
        }
        #endregion InnerClass

        private void ResultPivot_CellDoubleClick(object sender, PivotCellEventArgs e)
        {
            var columnFields = e.GetColumnFields();
            var columnField = columnFields.FirstOrDefault(x => x.FieldName == "YearMonth");

            if (columnField == null)
                return;

            var rowFields = e.GetRowFields();
            var rowField = rowFields.FirstOrDefault(x => x.FieldName == "ProductID");

            if (rowField == null)
                return;

            string yearStr   = e.GetFieldValue(columnField).ToString().Substring(0,4);
            string weekStr   = e.GetFieldValue(e.ColumnField).ToString();
            string productId = e.GetFieldValue(rowField).ToString();

            int year = 0;
            int week = 0;
            Int32.TryParse(yearStr, out year);
            Int32.TryParse(weekStr.Replace("W", ""), out week);

            if (year == 0 || week == 0)
                return;

            DateTime startdate = MyHelper.DATE.GetFirstDayOfWeek(year, week);
            DateTime endDate  = startdate.EndDayOfWeek(DayOfWeek.Saturday);

            DetailView detailView = new DetailView(this.Result, new List<string>() { productId }, startdate, endDate, GetStartTime(), GetEndTime());
            detailView.Show();
            detailView.Query();
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.Result == null)
                return;

            Initialize();
            GetData();
            RunQuery();
        }

        private void buttonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.PIVOTEXPORT.ExportToExcel(this.ResultPivot);
        }
    }
}
