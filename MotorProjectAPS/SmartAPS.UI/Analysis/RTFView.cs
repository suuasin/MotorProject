using DevExpress.XtraCharts;
using DevExpress.XtraPivotGrid;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using DevExpress.XtraBars;
using SmartAPS.UI.Properties;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.LookAndFeel;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserInterface;

namespace SmartAPS.UI.Analysis
{
    public partial class RTFView : MyXtraGridTemplate
    {
        private List<string> _productIds;
        private IEnumerable<ResultItem2> resultData;

        private Dictionary<string, string> DemandEndStepId = new Dictionary<string, string>();

        private List<string> TargetProductID
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

        //private DateTime TargetStartDate
        //{
        //    get
        //    {
        //        return Convert.ToDateTime(this.editStartDate.EditValue);
        //    }
        //}

        //private DateTime TargetEndDate
        //{
        //    get
        //    {
        //        return Convert.ToDateTime(this.editEndDate.EditValue);
        //    }
        //}

        #region ResultItem
        public class ResultItem
        {
            public string DEMAND_ID { get; set; }
            public string PRODUCT_ID { get; set; }
            public DateTime DUE_DATE { get; set; }
            public DateTime PLAN_DATE { get; set; }
            public double DEMAND_QTY { get; set; }
            public double ON_TIME_PLAN_QTY { get; set; }
            public double TOTAL_PLAN_QTY { get; set; }
            public double EARLINESS { get; set; }
            public double TARDINESS { get; set; }
            public double FULFILLMENT_RATE { get; set; }
            public double RTF_RATE { get; set; }
        }
        public class ResultItem2
        {
            public string DEMAND_ID { get; set; }
            public string PRODUCT_ID { get; set; }
            public string DUE_DATE { get; set; }
            public string LAST_OUT_DATE { get; set; }
            public double DEMAND_QTY { get; set; }
            //public double PEG_QTY { get; set; }
            public double ON_TIME_PLAN_QTY { get; set; }
            public double PLAN_QTY { get; set; }
            public double EARLINESS { get; set; }
            public double TARDINESS { get; set; }
            public double FULFILLMENT_RATE { get; set; }
            public double RTF_RATE { get; set; }
        }

        #endregion

        public RTFView()
        {
            InitializeComponent();
        }

        public RTFView(IServiceProvider serviceProvider)
       : base(serviceProvider)
        {
            InitializeComponent();
        }

        public RTFView(IExperimentResultItem result)
        {
            InitializeComponent();
            this.Result = result;
            this.ribbonControl1.Hide();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetInitializeOption(this.gridDetail);

            SetControls();
            SetDemandEndStep();

            //BindEvents();
        }

        //private void BindEvents()
        //{
        //    gridDetail.DoubleClick += GridDetail_DoubleClick;
        //}

     
        protected override void Query()
        {
            SetDemandEndStep();
            resultData = LoadData();
            BindMainGrid(resultData);

            //DrawGrid();
        }

        private void BindMainGrid(IEnumerable<ResultItem2> allData)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PRODUCT_ID", typeof(string));
            dt.Columns.Add("FULFILLMENT_RATE", typeof(double));
            dt.Columns.Add("RTF_RATE", typeof(double));
            dt.Columns.Add("EARLINESS(DAY)", typeof(double));
            dt.Columns.Add("TARDINESS(DAY)", typeof(double));

            foreach (var grpProd in allData.GroupBy(x => x.PRODUCT_ID))
            {
                double fulfill = 0f;
                double rtf = 0f;
                double earliness = int.MaxValue;
                double tardiness = int.MinValue;

                DataRow row = dt.NewRow();
                row["PRODUCT_ID"] = grpProd.Key;
                foreach (var data in grpProd)
                {
                    fulfill += data.FULFILLMENT_RATE;
                    rtf += data.RTF_RATE;

                    if (data.EARLINESS < earliness)
                        earliness = data.EARLINESS;
                    if (data.TARDINESS > tardiness)
                        tardiness = data.TARDINESS;
                }

                row["FULFILLMENT_RATE"] = fulfill / grpProd.Count();
                row["RTF_RATE"] = rtf / grpProd.Count();
                row["EARLINESS(DAY)"] = earliness;
                row["TARDINESS(DAY)"] = tardiness;
                dt.Rows.Add(row);
            }

            gridMain.DataSource = dt;
            SetGridDesign();
            gridMain.EndUpdate();
        }

        private void SetGridDesign()
        {
            this.gvMain.Columns["FULFILLMENT_RATE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gvMain.Columns["RTF_RATE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gvMain.Columns["EARLINESS(DAY)"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gvMain.Columns["TARDINESS(DAY)"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;

            this.gvMain.Columns["PRODUCT_ID"].Width = 150;
            this.gvMain.Columns["FULFILLMENT_RATE"].DisplayFormat.FormatString = "P0";
            this.gvMain.Columns["FULFILLMENT_RATE"].Width = 140;
            this.gvMain.Columns["RTF_RATE"].DisplayFormat.FormatString = "P0";
            this.gvMain.Columns["RTF_RATE"].Width = 130;
            this.gvMain.Columns["EARLINESS(DAY)"].DisplayFormat.FormatString = "#,##0.##";
            this.gvMain.Columns["EARLINESS(DAY)"].Width = 130;
            this.gvMain.Columns["TARDINESS(DAY)"].DisplayFormat.FormatString = "#,##0.##";
            this.gvMain.Columns["TARDINESS(DAY)"].Width = 130;

            SetColumnCaption(gvMain);

        }

        public void Query(List<string> productID, DateTime startTime, DateTime endTime)
        {
            this.editProdId.EditValue = String.Join(",", productID);
            this.editStartDate.EditValue = startTime;
            this.editEndDate.EditValue = endTime;

            Query();
        }

        public GridView GetGridControl()
        {
            return this.gvDetail;
        }

        private void SetControls()
        {
            SetDateTimePicker();
            MyHelper.ENGCONTROL.SetControl_ProductID_Checked(this.editProdId, this.Result);
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

            //this.editStartDate.EditValue = GetStartTime();
            //this.editEndDate.EditValue = GetEndTime();
        }

        internal DateTime GetStartTime()
        {
            return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
        }

        internal DateTime GetEndTime()
        {
            return MyHelper.DATASVC.GetPlanEndTime(this.Result);
        }

        #region LoadData

        private IEnumerable<ResultItem2> LoadData()
        {
            int maxTardiness = 999;
            int filterTardiness = 999;
            DateTime filterDueDate = GetEndTime();

            if (checkShowAllDmd.Checked)
            {
                filterTardiness = int.MaxValue;
                filterDueDate = DateTime.MaxValue;
            }

            Dictionary<string, ResultItem> dic = new Dictionary<string, ResultItem>();

            _productIds = this.TargetProductID;

            var plan = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(this.Result)
                           .Where(x => _productIds.Contains(x.PRODUCT_ID)/* && this.BetweenDate(x.EQP_END_TIME, TargetStartDate, TargetEndDate)*/
                           && GetLastOperation(x) == x.STEP_ID)
                           //.GroupBy(x => new { x.PRODUCT_ID, x.DEMAND_ID })
                           .Select(x => new { 
                                                x.DEMAND_ID, 
                                                x.PRODUCT_ID, 
                                                PLAN_DATE = x.EQP_END_TIME, 
                                                x.DUE_DATE, 
                                                QUT_QTY = x.PROCESS_QTY, 
                                                EARLINESS = x.EQP_END_TIME < x.DUE_DATE ? (x.EQP_END_TIME - x.DUE_DATE).TotalDays : 0,
                                                TARDINESS = x.EQP_END_TIME > x.DUE_DATE ? (x.EQP_END_TIME - x.DUE_DATE).TotalDays : 0,
                                                STATE = x.EQP_STATE_CODE
                                            });

            var demand = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result)
                .Where(x => _productIds.Contains(x.PRODUCT_ID) /*&& this.BetweenDate(x.DUE_DATE, TargetStartDate, TargetEndDate)*/)
                .Select(t => new { t.DEMAND_ID, t.DUE_DATE, t.PRODUCT_ID, t.DEMAND_QTY });

            var product = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result).Where(x => _productIds.Contains(x.PRODUCT_ID)).Select(x => new { x.PRODUCT_ID });

            foreach (var dm in demand)
            {
                ResultItem item;
                if (dic.TryGetValue(dm.DEMAND_ID, out item) == false)
                {
                    item = new ResultItem();
                    dic[dm.DEMAND_ID] = item;
                }
                item.DEMAND_ID = dm.DEMAND_ID;
                item.PRODUCT_ID = dm.PRODUCT_ID;
                item.DUE_DATE = dm.DUE_DATE;
                item.DEMAND_QTY = dm.DEMAND_QTY;
                item.PLAN_DATE = DateTime.MinValue;
                item.ON_TIME_PLAN_QTY = 0;
                item.TARDINESS = 0; //maxTardiness;
                item.EARLINESS = 0; //int.MinValue;
            }
            
            foreach (var p in plan)
            {
                ResultItem item;

                if (p.DEMAND_ID == null || p.STATE != "BUSY")
                    continue;

                if (dic.TryGetValue(p.DEMAND_ID, out item) == false)
                {
                    item = new ResultItem();
                    dic[p.DEMAND_ID] = item;
                }

                item.DEMAND_ID = p.DEMAND_ID;
                item.PRODUCT_ID = p.PRODUCT_ID;

                if (item.PLAN_DATE < p.PLAN_DATE)
                    item.PLAN_DATE = p.PLAN_DATE;

                if (p.PLAN_DATE <= p.DUE_DATE)
                    item.ON_TIME_PLAN_QTY += p.QUT_QTY;

                item.TOTAL_PLAN_QTY += p.QUT_QTY;

                if (item.TARDINESS < p.TARDINESS)
                    item.TARDINESS = p.TARDINESS;

                if (item.EARLINESS > p.EARLINESS)
                    item.EARLINESS = p.EARLINESS;
            }

            var rtf = dic.Values.Select(x => new
            {
                x.DEMAND_ID,
                x.PRODUCT_ID,
                x.DUE_DATE,
                x.PLAN_DATE,
                x.DEMAND_QTY,
                
                x.ON_TIME_PLAN_QTY,
                x.TOTAL_PLAN_QTY,
                x.TARDINESS,
                x.EARLINESS,
                RTF_RATE = x.DEMAND_QTY == 0 ? 0 : Math.Min(100, Math.Round((x.ON_TIME_PLAN_QTY / x.DEMAND_QTY), 2)),
                FULFILLMENT_RATE = x.DEMAND_QTY == 0 ? 0 : Math.Round((x.TOTAL_PLAN_QTY / x.DEMAND_QTY), 2)
            }).OrderBy(x => x.PRODUCT_ID).Where(x => x.TARDINESS < filterTardiness &&  x.DUE_DATE <= filterDueDate);

            var final = from a in rtf
                        join b in product on a.PRODUCT_ID equals b.PRODUCT_ID into grp
                        from g in grp.DefaultIfEmpty()
                        select new ResultItem2
                        {
                            DEMAND_ID = a.DEMAND_ID,
                            PRODUCT_ID = a.PRODUCT_ID,
                            DUE_DATE = a.DUE_DATE == DateTime.MinValue ? "" : a.DUE_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                            LAST_OUT_DATE = a.PLAN_DATE == DateTime.MinValue ? "" : a.PLAN_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                            DEMAND_QTY = a.DEMAND_QTY,
                            ON_TIME_PLAN_QTY = a.ON_TIME_PLAN_QTY,
                            PLAN_QTY = a.TOTAL_PLAN_QTY,
                            TARDINESS = a.TARDINESS == 999 ? 0 : a.TARDINESS,
                            EARLINESS = a.EARLINESS == int.MinValue ? 0 : a.EARLINESS,
                            FULFILLMENT_RATE = a.FULFILLMENT_RATE,
                            RTF_RATE = a.RTF_RATE
                        };

            return final;
        }


        private string GetLastOperation(EQP_PLAN plan)
        {
            string lastOperation = string.Empty;

            if (plan.DEMAND_ID == null)
                return lastOperation;

            this.DemandEndStepId.TryGetValue(plan.DEMAND_ID, out lastOperation);

            return lastOperation;
        }

        private void SetDemandEndStep()
        {
            Dictionary<string, string> productProcessId = new Dictionary<string, string>();

            var products = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result);

            foreach (var product in products)
                productProcessId[product.PRODUCT_ID] = product.PROCESS_ID;

            var steps = MyHelper.DATASVC.GetEntityData<STEP_ROUTE>(this.Result);
            Dictionary<string, List<STEP_ROUTE>> processSteps = new Dictionary<string, List<STEP_ROUTE>>();

            foreach (var step in steps)
            {
                if (processSteps.ContainsKey(step.PROCESS_ID) == false)
                    processSteps[step.PROCESS_ID] = new List<STEP_ROUTE>();

                processSteps[step.PROCESS_ID].Add(step);
            }

            var demands = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result);
            
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


        public bool BetweenDate(DateTime date, DateTime startTime, DateTime endTime)
        {
            if (startTime <= date && date <= endTime)
                return true;

            return false;
        }

        #endregion

        #region Bind & Draw Grid

        private void BindDetailGrid(IEnumerable<ResultItem2> dt)
        {
            this.gvDetail.BeginInit();
            this.gvDetail.BeginUpdate();
            this.gridDetail.DataSource = dt.ToBindingList();

            AddFooter();

            this.gvDetail.Columns["DEMAND_QTY"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gvDetail.Columns["PLAN_QTY"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gvDetail.Columns["EARLINESS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gvDetail.Columns["TARDINESS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gvDetail.Columns["RTF_RATE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gvDetail.Columns["FULFILLMENT_RATE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;

            this.gvDetail.Columns["DEMAND_QTY"].DisplayFormat.FormatString = "#,##0";
            this.gvDetail.Columns["PLAN_QTY"].DisplayFormat.FormatString = "#,##0";
            this.gvDetail.Columns["EARLINESS"].DisplayFormat.FormatString = "#,##0.##";
            this.gvDetail.Columns["TARDINESS"].DisplayFormat.FormatString = "#,##0.##";
            this.gvDetail.Columns["RTF_RATE"].DisplayFormat.FormatString = "P0";
            this.gvDetail.Columns["FULFILLMENT_RATE"].DisplayFormat.FormatString = "P0";

            SetColumnCaption(gvDetail);

            this.gvDetail.EndUpdate();
            this.gvDetail.EndInit();

            gvDetail.BestFitColumns();
        }

        private void SetColumnCaption(GridView gridview)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridview.Columns)
            {
                col.Caption = col.FieldName.Replace('_', ' ');
            }
        }

        private void AddFooter()
        {
            if (gvDetail.Columns["DEMAND_QTY"].Summary.Count > 0)
                gvDetail.Columns["DEMAND_QTY"].Summary.RemoveAt(0);

            if (gvDetail.Columns["PLAN_QTY"].Summary.Count > 0)
                gvDetail.Columns["PLAN_QTY"].Summary.RemoveAt(0);

            if (gvDetail.Columns["ON_TIME_PLAN_QTY"].Summary.Count > 0)
                gvDetail.Columns["ON_TIME_PLAN_QTY"].Summary.RemoveAt(0);

            if (gvDetail.Columns["FULFILLMENT_RATE"].Summary.Count > 0)
                gvDetail.Columns["FULFILLMENT_RATE"].Summary.RemoveAt(0);

            if (gvDetail.Columns["RTF_RATE"].Summary.Count > 0)
                gvDetail.Columns["RTF_RATE"].Summary.RemoveAt(0);

            gvDetail.Columns["DEMAND_QTY"].Summary.Add(DevExpress.Data.SummaryItemType.Sum);
            gvDetail.Columns["PLAN_QTY"].Summary.Add(DevExpress.Data.SummaryItemType.Sum);
            gvDetail.Columns["ON_TIME_PLAN_QTY"].Summary.Add(DevExpress.Data.SummaryItemType.Sum);
            gvDetail.Columns["FULFILLMENT_RATE"].Summary.Add(DevExpress.Data.SummaryItemType.Average, "FULFILLMENT_RATE", "AVG={0:p}");
            gvDetail.Columns["RTF_RATE"].Summary.Add(DevExpress.Data.SummaryItemType.Average, "RTF_RATE", "AVG={0:p}");
        }

        private void DrawGrid()
        {
            this.gvDetail.OptionsCustomization.AllowFilter = false;
            this.gvDetail.OptionsCustomization.AllowSort = false; //disable sorting
            this.gvDetail.OptionsMenu.EnableColumnMenu = false;
        }
        #endregion

        #region Event

        private void buttonLoad_ItemClick(object sender, ItemClickEventArgs e)
        {
            Query();
        }

        private void buttonExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            MyHelper.GRIDVIEWEXPORT.ExportToExcel(this.gvDetail);
        }
        

        private void gvDetail_DoubleClick(object sender, EventArgs e)
        {
            GridControl gc = this.gridDetail;

            GridView gv = sender as GridView;

            Point pt = gc.PointToClient(Control.MousePosition);
            GridHitInfo hitInfo = gv.CalcHitInfo(pt);

            if (hitInfo.InRow == false && hitInfo.InRowCell == false)
                return;

            ResultItem2 item = gv.GetRow(hitInfo.RowHandle) as ResultItem2;
            if (item == null)
                return;

            // OperPlanPopUp view = new OperPlanPopUp(this._result, item.DEMAND_ID);

            //view.Show();
        }

        private void gvDetail_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView gridView = sender as GridView; ;

            ResultItem2 resultItem = gridView.GetRow(e.RowHandle) as ResultItem2;

            if (resultItem == null)
                return;

            string colName = e.Column.FieldName;

            if (colName == "DEMAND_QTY" || colName == "PLAN_QTY" || colName == "ON_TIME_PLAN_QTY")
            {
                double demandQty = resultItem.DEMAND_QTY;
                double planQty = resultItem.ON_TIME_PLAN_QTY;

                if (colName == "DEMAND_QTY")
                {
                    if (demandQty > 0)
                    {
                        e.Appearance.BackColor = Color.LightBlue;
                    }
                }
            }

            if (colName == "FULFILLMENT_RATE" || colName == "RTF_RATE")
            {
                object value = gvDetail.GetRowCellValue(e.RowHandle, e.Column);
                if (Convert.ToDouble(value) != 1f)
                    e.Appearance.ForeColor = Color.Red;
            }
        }
        
        private void gvMain_RowClick(object sender, RowClickEventArgs e)
        {
            

            string prodID = gvMain.GetRowCellValue(e.RowHandle, "PRODUCT_ID").ToString();
            if (!string.IsNullOrEmpty(prodID))
            {
                var table = resultData.Where(x => x.PRODUCT_ID == prodID);
                BindDetailGrid(table);
            }
        }

        private void gvMain_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {

            string colName = e.Column.FieldName;

            if (colName == "FULFILLMENT_RATE")
            {
                string FULFILLMENT_RATE = gvMain.GetRowCellValue(e.RowHandle, "FULFILLMENT_RATE").ToString();

                if (colName == "FULFILLMENT_RATE")
                {
                    if (FULFILLMENT_RATE != "1")
                    {
                        e.Appearance.BackColor = Color.LightPink;
                    }
                }
            }
            if(colName == "RTF_RATE")
            {
                e.Appearance.BackColor = Color.LightCyan;
            }
            if (colName == "EARLINESS(DAY)" || colName== "TARDINESS(DAY)")
            {
                e.Appearance.BackColor = Color.PaleGreen;
            }
        }
        #endregion

        private void gridMain_DoubleClick(object sender, EventArgs e)
        {

            GridControl control = (GridControl)sender;
            GridView view = (GridView)control.MainView;
            //DataRow selectRow = gridView1.GetFocusedDataRow();
            DataRow selectRow = view.GetFocusedDataRow();
            if (selectRow == null)
                return;

            string productID = selectRow.GetString("PRODUCT_ID");      
            OpenPopUp(this.Result, productID);
        }

        private void gridDetail_DoubleClick(object sender, EventArgs e)
        {
            GridControl control = (GridControl)sender;
            GridView view = (GridView)control.MainView;
            ResultItem2 selectRow = (ResultItem2)view.GetFocusedRow();
            //if (selectRow == null)
            //    return;



            string deamndID = selectRow.DEMAND_ID;
            OpenDetailPopUp(this.Result, deamndID);

        }
        private void OpenPopUp(IExperimentResultItem result, string productID) //gridMain 팝업
        {
            var view = new RTFViewPopUp(result, productID);
            var dialog = new PopUpForm(view);
            view.Dock = DockStyle.Fill;
            dialog.StartPosition = FormStartPosition.CenterScreen;
            dialog.Size = new Size(1200, 600);
            dialog.Controls.Clear();
            dialog.Controls.Add(view);
            dialog.Show();
        }
        private void OpenDetailPopUp(IExperimentResultItem result, string demandID) //gridDetail  팝업
        {
            var view = new RTFViewDetailPopUp(result, demandID);
            var dialog = new PopUpForm(view);
            view.Dock = DockStyle.Fill;
            dialog.StartPosition = FormStartPosition.CenterScreen;
            dialog.Size = new Size(1200, 600);
            dialog.Controls.Clear();
            dialog.Controls.Add(view);
            dialog.Show();
        }
    }
}
