using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartAPS.MozartStudio.Utils;
using SmartAPS.MozartStudio.Properties;
using SmartAPS.MozartStudio.Helper;
using Template.Lcd.Scheduling.Inputs;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraCharts;
using Mozart.Data.Entity;
using Template.Lcd.Scheduling.Outputs;
using DevExpress.XtraGrid.Columns;

namespace SmartAPS.MozartStudio.Analysis
{
    public partial class WipVsMoveView : MyXtraGridTemplate
    {
        private List<string> TargetStepList { get; set; }

        private string TargetAreaID
        {
            get { return this.editAreaId.EditValue as string; }
        }

        private DateTime TargetPlanDate
        {
            get 
            {
                return MyHelper.DATE.StringToDateTime(this.editPlanDate.EditValue as string);
            }
        }

        private bool ShowSubStep
        {
            get
            {
                if (this.CheckSubStep.Checked == true)
                    return true;
                return false;
            }
        }

        public WipVsMoveView()
        {
            InitializeComponent();
        }

        public WipVsMoveView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetInitializeOption(this.gridControl1);

            SetControls();

            RunQuery();

            UseSaveLayout();
        }

        private void UseSaveLayout()
        {
            var gridView = this.gridView1;
            SetGridLayout(gridView); //layout test (mozart studio ver)
        }

        private void SetControls()
        {
            MyHelper.ENGCONTROL.SetControl_AreaID(this.editAreaId, this.Result);
            SetControl_DateCombo();
        }

        public void SetControl_DateCombo()
        {
            var control = this.editPlanDate;

            var cbEdit = this.repoDateEdit;
            if (cbEdit == null)
                return;

            control.EditValue = ""; //edit text clear
            cbEdit.Items.Clear();

            string targetAreaID = this.TargetAreaID;

            var table = MyHelper.DATASVC.GetEntityData<StepWip>(this.Result).OrderBy(x=>x.PLAN_DATE);
            if (table != null)
            {
                foreach (var item in table)
                {
                    if (item.AREA_ID != targetAreaID)
                        continue;

                    string value = MyHelper.DATE.DateTimeToString(item.PLAN_DATE);
                    if (cbEdit.Items.Contains(value) == false)
                        cbEdit.Items.Add(value);
                }
            }
            

            if (cbEdit.Items.Count > 0)
                control.EditValue = cbEdit.Items[0];
        }

        protected override void Query()
        {
            BindData();
            DrawGrid();

            FillChart(gridControl1);
        }

        private void DrawGrid()
        {
            gridView1.BestFitColumns();
            gridView1.OptionsCustomization.AllowFilter = false;
            gridView1.OptionsCustomization.AllowSort = false; //disable sorting
            gridView1.OptionsMenu.EnableColumnMenu = false;
        }

        #region Bind Data
        private void BindData()
        {
            this.gridControl1.DataSource = null;
            this.gridView1.Columns.Clear();

            this.TargetStepList = GetStepIDList(this.TargetAreaID);

            DataTable dt = CreateSchema();

            DateTime targetDate = this.TargetPlanDate;

            var table = MyHelper.DATASVC.GetEntityData<StepWip>(this.Result).OrderBy(x=>x.PRODUCT_ID);
            if (table != null)
            {
                foreach (var it in table)
                {
                    if (it.PLAN_DATE != targetDate)
                        continue;

                    string productID = it.PRODUCT_ID;
                    string productType = it.PRODUCTION_TYPE;
                    string stepID = it.STEP_ID;

                    if (dt.Columns.Contains(stepID) == false)
                        continue;

                    var row = dt.Rows.Find(new object[] { productID, productType }); //primary key로 find
                    if (row == null)
                    {
                        row = dt.NewRow();

                        row["PRODUCT_ID"] = productID;
                        row["PRODUCTION_TYPE"] = productType;

                        dt.Rows.Add(row);
                    }

                    row[stepID] = row.GetInt32(stepID) + (it.WAIT_QTY + it.RUN_QTY);
                }

            }
            this.gridControl1.DataSource = dt;

            //grid total show test
            foreach (GridColumn dc in gridView1.Columns)
            {
                if (dc.ColumnType == typeof(string))
                    continue;

                GridColumnSummaryItem sumitem = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, dc.FieldName, "SUM={0}");
                dc.Summary.Add(sumitem);
            }
        }

        #endregion 

        private DataTable CreateSchema()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("PRODUCT_ID");
            dt.Columns.Add("PRODUCTION_TYPE");

            var stepList = this.TargetStepList;
            if (stepList != null)
            {
                foreach (var stepID in stepList)
                {
                    if (dt.Columns.Contains(stepID))
                        continue;

                    var col = dt.Columns.Add(stepID, typeof(int));
                }
            }

            dt.PrimaryKey = new DataColumn[] { dt.Columns["PRODUCT_ID"], dt.Columns["PRODUCTION_TYPE"] };

            return dt;
        }

        private List<string> GetStepIDList(string areaID)
        {
            var table = MyHelper.DATASVC.GetEntityData<StdStep>(this.Result); //step <StdWip>

            // * SUB STEP 확인
            bool isOnlyMainStep = this.ShowSubStep == false;
            if (isOnlyMainStep)
                table = table.Where(t => t.STEP_TYPE == "MAIN"); //

            List<string> list = new List<string>();

            var finds = table.Where(t => t.AREA_ID == areaID).OrderBy(t => t.STEP_SEQ).ToList(); //STD_STEP_SEQ
            foreach (var it in finds)
            {
                string stepID = it.STEP_ID;
                if (list.Contains(stepID) == false)
                    list.Add(stepID);
            }

            return list;

        }

        #region Draw Chart
        private void FillChart(GridControl gridControl1)
        {
            XYDiagram xyDiagram = (XYDiagram)this.chartControl.Diagram;
            if (xyDiagram != null)
            {
                xyDiagram.AxisX.Label.Angle = 90; //45
                xyDiagram.AxisX.Label.ResolveOverlappingOptions.AllowHide = false;
                xyDiagram.AxisX.NumericScaleOptions.AutoGrid = false;
                xyDiagram.EnableAxisXScrolling = false;
                xyDiagram.EnableAxisYScrolling = true;
                xyDiagram.AxisX.Label.Font = new System.Drawing.Font("Tahoma", 7F);
                xyDiagram.AxisX.QualitativeScaleOptions.AutoGrid = false; //show all step ids

                if(xyDiagram.SecondaryAxesY.Count == 0)
                {
                    SecondaryAxisY swtqaAxis = new SecondaryAxisY("StepWipAccumulate");
                    xyDiagram.SecondaryAxesY.Add(swtqaAxis);
                }
            }
            this.chartControl.Series.Clear();

            FillChart_ProductBars();

            FillChart_StepMoveOutQty();

            FillChart_Accumulate();

        }

        private void FillChart_ProductBars()
        {
            //fill from dt
            DataTable dt = (DataTable)gridControl1.DataSource;

            if (dt == null)
                return;

            foreach (DataRow row in dt.Rows)
            {
                string seriesName = row["PRODUCT_ID"].ToString(); //prod_id
                Series swtw = new Series(seriesName, ViewType.StackedBar);
                //swtw.ArgumentDataMember = "STEP_ID";

                foreach (string stepid in this.TargetStepList)
                {
                    if (!dt.Columns.Contains(stepid))
                        continue;

                    var qty = row[stepid].GetType().Name == "DBNull" ? 0 : row[stepid];
                    swtw.Points.Add(new SeriesPoint(stepid, qty));
                }
                this.chartControl.Series.Add(swtw);
            }

            //show stackedbar total labels //sum
            //if(this.chartControl.Series.Count != 0)
            //{
            //    StackedBarTotalLabel totalLabel = ((XYDiagram)this.chartControl.Diagram).DefaultPane.StackedBarTotalLabel;
            //    totalLabel.Visible = true;
            //    totalLabel.ShowConnector = true;
            //    totalLabel.TextPattern = "{TV}"; //total text
            //}

        }


        private void FillChart_Accumulate()
        {
            //누적 stepwip sum line chart 추가
            DataTable dt = (DataTable)gridControl1.DataSource;

            if (dt == null)
                return;
    
            //PRODUCT 들의 step별 합
            Series swtqa = new Series("StepWipTotalQtyAccumulate", ViewType.Line);
            ((LineSeriesView)swtqa.View).AxisY = ((XYDiagram)this.chartControl.Diagram).SecondaryAxesY[0]; //assign secondary axis
            swtqa.View.Color = Color.Orange;

            //Label 표시
            PointSeriesLabel label = (PointSeriesLabel)swtqa.Label;
            label.Position = PointLabelPosition.Outside;
            label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAroundPoint;
            label.TextColor = Color.Orange;
            label.TextPattern = "{V}";

            var stepList = this.TargetStepList;
            if (stepList != null)
            {
                int accumulate = 0;

                int count = stepList.Count;
                for (int i = count - 1; i >= 0; i--) //reverse
                {
                    string stepID = stepList[i];

                    int sumqty = dt.Select().Sum(x => x.GetInt32(stepID));
                    accumulate += sumqty;

                    SeriesPoint sp = new SeriesPoint(stepID, accumulate);
                    swtqa.Points.Add(sp);
                }
            }
            this.chartControl.Series.Add(swtqa);
            this.chartControl.Series["StepWipTotalQtyAccumulate"].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
        }

        private void FillChart_StepMoveOutQty()
        {
            //stepmove wip sum line chart 추가

            //StepMoveOutQtyTable(dt) 생성
            DataTable dt = new DataTable();
            dt.Columns.Add("STEP_ID");
            dt.Columns.Add("SUM_OUT_QTY");

            var stepMove = MyHelper.DATASVC.GetEntityData<StepMove>(this.Result);
            var result = stepMove.Where(t => t.AREA_ID == this.TargetAreaID).OrderBy(t => t.STD_STEP_SEQ).GroupBy(t => t.STEP_ID); //group by step id
                                                                                                                                   //&& t.PLAN_DATE == this.TargetPlanDate            
            foreach (var r in result)
            {
                string stepID = r.Key;

                DataRow row = dt.NewRow();
                row["STEP_ID"] = stepID;

                float smoq = r.Sum(x => x.OUT_QTY); //step별로 grouped 된 out_qty 총합...
                row["SUM_OUT_QTY"] = smoq;

                dt.Rows.Add(row);
            }

            //fill chart by dt
            if (dt == null)
                return;

            Series smoqs = new Series("StepMoveOutQty", ViewType.Line);
            ((LineSeriesView)smoqs.View).AxisY = ((XYDiagram)this.chartControl.Diagram).SecondaryAxesY[0]; //
            smoqs.View.Color = Color.Red;
            foreach (DataRow row in dt.Rows)
            {
                smoqs.Points.Add(new SeriesPoint(row["STEP_ID"], row["SUM_OUT_QTY"]));
            }

            this.chartControl.Series.Add(smoqs);

        }
        #endregion

        private void btnQuery_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
            UseSaveLayout();
        }

        private void btnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //save to excel
            ExportToExcel(this.gridView1);
        }

        private void editVersionNo_EditValueChanged(object sender, EventArgs e)
        {
            MyHelper.ENGCONTROL.SetControl_AreaID(this.editAreaId, this.Result);
            SetControl_DateCombo();
        }

        private void editAreaId_EditValueChanged(object sender, EventArgs e)
        {
            SetControl_DateCombo();
        }

        private void CheckSubStep_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            RunQuery();
        }
    }
}
