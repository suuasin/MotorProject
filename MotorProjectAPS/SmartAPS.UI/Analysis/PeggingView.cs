using DevExpress.Spreadsheet;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
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
	public partial class PeggingView : MyXtraGridTemplate
    {
        private Dictionary<string, ResultItem> _dict;
        
        // new
        private IEnumerable<WIP> _wipDt;
        private IEnumerable<PEG_HISTORY> _pegHistDt;
        private IEnumerable<UNPEG_HISTORY> _unpegHistDt;
        
        class PegHistoryData
        {
            public const string LINE_ID = "LINE_ID";
            public const string LOT_ID = "LOT_ID";
            public const string STEP_ID = "STEP_ID";
            public const string PRODUCT_ID = "PRODUCT_ID";
            public const string UNIT_QTY = "UNIT_QTY";
            public const string PEG_QTY = "PEG_QTY";
            public const string STATE = "STATE";
            public const string MO_DEMAND_ID = "MO_DEMAND_ID";
            public const string MO_PRODUCT_ID = "MO_PRODUCT_ID";
            public const string MO_DUE_DATE = "MO_DUE_DATE";
        }

        struct UnpegState
        {
            public const string WAIT = "WAIT";
            public const string RUN = "RUN";
        }

        #region ResultItem
        private class ResultItem
        {
            public string LINE_ID;
            public string PRODUCT_ID;
            public float TOTAL_QTY;
            public float PEG_WHOLE_QTY;
            public float PEG_PARTIAL_QTY;
            public float UNPEG_WHOLE_QTY;
            public float UNPEG_PARTIAL_QTY;
            public float PEG_RATIO;
        }
        #endregion
        
        public PeggingView()
        {
            InitializeComponent();
        }

        public PeggingView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        public PeggingView(IExperimentResultItem targetResult)
        {
            InitializeComponent();
            
            this.Result = targetResult;
            
            LoadDocument();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();
            
            SetControls();
        }

        protected override XtraGridTemplate GetCloneView()
        {
            return new PeggingView(this.Result);
        }

        protected override void Query()
        {
            DataTable dt = GetData();
            BindGrid(dt);
        }

        private bool IsCheckedAll()
        {
            var control = this.editProduct.Edit as RepositoryItemCheckedComboBoxEdit;
            int totalCnt = control.GetItems().Count();
            int checkedCnt = control.GetCheckedItems().ToString().Replace(" ", "").Split(',').Length;

            if (checkedCnt < totalCnt)
                return false;

            return true;
        }

        private void SetControls()
        {
            SetControl_Product(this.editProduct);
        }

        private void SetColumnCaption(GridView grid)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in grid.Columns)
            {
                col.Caption = col.FieldName.Replace('_', ' ');
            }
        }

        private void SetControl_Product(BarEditItem control)
        {
            control.BeginUpdate();

            EngControlHelper.SetControl_WIP_ProductID_Checked(editProduct, this.Result);

            control.EndUpdate();
            control.Refresh();
        }
        
        new private DataTable GetData()
        {
            DataTable table = new DataTable();
            AddColumns(table);
            SetColumnCaption(gridView1);

            FillData(table);
            
            return table;
        }

        private void AddColumns(DataTable dt)
        {
            dt.Columns.Add("LINE_ID", typeof(string));
            dt.Columns.Add("PRODUCT_ID", typeof(string));
            dt.Columns.Add("TOTAL_QTY", typeof(int));
            dt.Columns.Add("PEG_WHOLE_QTY", typeof(int));
            dt.Columns.Add("PEG_PARTIAL_QTY", typeof(int));
            dt.Columns.Add("UNPEG_WHOLE_QTY", typeof(int));
            dt.Columns.Add("UNPEG_PARTIAL_QTY", typeof(int));
            dt.Columns.Add("PEG_RATIO", typeof(float));
        }

        #region Filldata
        private void FillData(DataTable dt)
        {
            LoadData();

            if (!IsCheckedAll())
            {
                // in case of checked some product
                string[] selectedProds = editProduct.EditValue != null ? editProduct.EditValue.ToString().Replace(" ", "").Split(',') : null;

                foreach (var lineGrpWip in _wipDt.GroupBy(x => x.LINE_ID))
                {
                    string lineId = lineGrpWip.Key;

                    foreach (var grpWip in lineGrpWip.GroupBy(y => y.PRODUCT_ID))
                    {
                        if (!selectedProds.Contains(grpWip.Key))
                            continue;

                        DataRow row = dt.NewRow();
                        double totalQty = grpWip.Sum(x => x.UNIT_QTY);
                        row["LINE_ID"] = lineId;
                        row["PRODUCT_ID"] = grpWip.Key;
                        row["TOTAL_QTY"] = totalQty;

                        var grpPegHist = _pegHistDt.Where(x => x.LINE_ID == lineId && x.PRODUCT_ID == grpWip.Key);
                        var grpUnpegHist = _unpegHistDt.Where(x => x.LINE_ID == lineId && x.PRODUCT_ID == grpWip.Key);

                        double wholePeg = 0;
                        double partialPeg = 0;
                        foreach (var pegHist in grpPegHist)
                        {
                            if (pegHist.PEG_QTY == pegHist.UNIT_QTY)
                                wholePeg += pegHist.UNIT_QTY;
                            else
                                partialPeg += pegHist.PEG_QTY;
                        }

                        double wholeUnpeg = 0;
                        double partialUnpeg = 0;
                        foreach (var unpegHist in grpUnpegHist)
                        {
                            if (unpegHist.UNPEG_QTY == unpegHist.UNIT_QTY)
                                wholeUnpeg += unpegHist.UNIT_QTY;
                            else
                                partialUnpeg += unpegHist.UNPEG_QTY;
                        }

                        row["PEG_WHOLE_QTY"] = wholePeg;
                        row["PEG_PARTIAL_QTY"] = partialPeg;
                        row["UNPEG_WHOLE_QTY"] = wholeUnpeg;
                        row["UNPEG_PARTIAL_QTY"] = partialUnpeg;
                        row["PEG_RATIO"] = (wholePeg + partialPeg) / totalQty;

                        dt.Rows.Add(row);
                    }
                }

                dt.DefaultView.Sort = "LINE_ID, PRODUCT_ID";
            }
            else
            {
                // in case of checked all
                foreach (var lineGrpWip in _wipDt.GroupBy(x => x.LINE_ID))
                {
                    string lineId = lineGrpWip.Key;

                    DataRow row = dt.NewRow();
                    double totalQty = lineGrpWip.Sum(x => x.UNIT_QTY);
                    row["LINE_ID"] = lineId;
                    row["PRODUCT_ID"] = "";
                    row["TOTAL_QTY"] = totalQty;

                    var grpPegHist = _pegHistDt.Where(x => x.LINE_ID == lineId);
                    var grpUnpegHist = _unpegHistDt.Where(x => x.LINE_ID == lineId);

                    double wholePeg = 0;
                    double partialPeg = 0;
                    foreach (var pegHist in grpPegHist)
                    {
                        if (pegHist.PEG_QTY == pegHist.UNIT_QTY)
                            wholePeg += pegHist.UNIT_QTY;
                        else
                            partialPeg += pegHist.PEG_QTY;
                    }

                    double wholeUnpeg = 0;
                    double partialUnpeg = 0;
                    foreach (var unpegHist in grpUnpegHist)
                    {
                        if (unpegHist.UNPEG_QTY == unpegHist.UNIT_QTY)
                            wholeUnpeg += unpegHist.UNIT_QTY;
                        else
                            partialUnpeg += unpegHist.UNPEG_QTY;
                    }

                    row["PEG_WHOLE_QTY"] = wholePeg;
                    row["PEG_PARTIAL_QTY"] = partialPeg;
                    row["UNPEG_WHOLE_QTY"] = wholeUnpeg;
                    row["UNPEG_PARTIAL_QTY"] = partialUnpeg;
                    row["PEG_RATIO"] = (wholePeg + partialPeg) / totalQty;

                    dt.Rows.Add(row);
                }
                dt.DefaultView.Sort = "LINE_ID";
            }
        }

        #endregion

        #region LoadData
        private void LoadData()
        {
            _wipDt = MyHelper.DATASVC.GetEntityData<WIP>(this.Result);
            _pegHistDt = MyHelper.DATASVC.GetEntityData<PEG_HISTORY>(this.Result);
            _unpegHistDt = MyHelper.DATASVC.GetEntityData<UNPEG_HISTORY>(this.Result);
        }

        #endregion

        #region BindData
        private void BindGrid(DataTable dt)
        {
            gridControl1.DataSource = dt;
            
            gridView1.BestFitColumns();
            gridView1.Columns["PEG_RATIO"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["PEG_RATIO"].DisplayFormat.FormatString = "P";

            if (IsCheckedAll())
                gridView1.Columns["PRODUCT_ID"].Visible = false;
        }
        #endregion

        #region Detail Grid

        private DataTable GetDetailData()
        {
            DataTable table = new DataTable();
            DataRow selectedRow = gridView1.GetFocusedDataRow();

            AddDetailColumns(table);
            SetColumnCaption(gridViewDetail);
            FillDetailGrid(table, selectedRow);

            return table;
        }

        private void AddDetailColumns(DataTable table)
        {
            table.Columns.Add("UNPEG_CATEGORY", typeof(string));
            table.Columns.Add("UNPEG_REASON", typeof(string));
            table.Columns.Add("WAIT_LOT_QTY", typeof(int));
            table.Columns.Add("WAIT_UNIT_QTY", typeof(int));
            table.Columns.Add("RUN_LOT_QTY", typeof(int));
            table.Columns.Add("RUN_UNIT_QTY", typeof(int));
        }

        private void FillDetailGrid(DataTable table, DataRow selectedRow)
        {
            string selectedLineId = selectedRow["LINE_ID"].ToString();
            string selectedProdId = selectedRow["PRODUCT_ID"].ToString();

            if (!IsCheckedAll())
            {
                foreach (var grpLineUnpeg in _unpegHistDt.GroupBy(x => x.LINE_ID))
                {
                    if (grpLineUnpeg.Key != selectedLineId)
                        continue;

                    foreach (var grpProdUnpeg in grpLineUnpeg.GroupBy(x => x.PRODUCT_ID))
                    {
                        if (grpProdUnpeg.Key != selectedProdId)
                            continue;

                        foreach (var grpCatUnpeg in grpProdUnpeg.GroupBy(x => x.UNPEG_CATEGORY))
                        {
                            foreach (var grpReason in grpCatUnpeg.GroupBy(x => x.UNPEG_REASON))
                            {
                                DataRow row = table.NewRow();

                                row["UNPEG_CATEGORY"] = grpCatUnpeg.Key;
                                row["UNPEG_REASON"] = grpReason.Key;

                                int runLotQty = 0;
                                double runUnitQty = 0;
                                int waitLotQty = 0;
                                double waitUnitQty = 0;

                                foreach (var unpeg in grpReason)
                                {
                                    if (unpeg.STATE == UnpegState.RUN)
                                    {
                                        runLotQty++;
                                        runUnitQty += unpeg.UNIT_QTY;
                                    }
                                    else if (unpeg.STATE == UnpegState.WAIT)
                                    {
                                        waitLotQty++;
                                        waitUnitQty += unpeg.UNIT_QTY;
                                    }
                                }

                                row["RUN_LOT_QTY"] = runLotQty;
                                row["RUN_UNIT_QTY"] = runUnitQty;
                                row["WAIT_LOT_QTY"] = waitLotQty;
                                row["WAIT_UNIT_QTY"] = waitUnitQty;

                                table.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var grpLineUnpeg in _unpegHistDt.GroupBy(x => x.LINE_ID))
                {
                    if (grpLineUnpeg.Key != selectedLineId)
                        continue;

                    foreach (var grpCatUnpeg in grpLineUnpeg.GroupBy(x => x.UNPEG_CATEGORY))
                    {
                        foreach (var grpReason in grpCatUnpeg.GroupBy(x => x.UNPEG_REASON))
                        {
                            DataRow row = table.NewRow();

                            row["UNPEG_CATEGORY"] = grpCatUnpeg.Key;
                            row["UNPEG_REASON"] = grpReason.Key;

                            int runLotQty = 0;
                            double runUnitQty = 0;
                            int waitLotQty = 0;
                            double waitUnitQty = 0;

                            foreach (var unpeg in grpReason)
                            {
                                if (unpeg.STATE == UnpegState.RUN)
                                {
                                    runLotQty++;
                                    runUnitQty += unpeg.UNIT_QTY;
                                }
                                else if (unpeg.STATE == UnpegState.WAIT)
                                {
                                    waitLotQty++;
                                    waitUnitQty += unpeg.UNIT_QTY;
                                }
                            }

                            row["RUN_LOT_QTY"] = runLotQty;
                            row["RUN_UNIT_QTY"] = runUnitQty;
                            row["WAIT_LOT_QTY"] = waitLotQty;
                            row["WAIT_UNIT_QTY"] = waitUnitQty;

                            table.Rows.Add(row);
                        }
                    }
                }
            }
        }

        private void BindDetailGrid(DataTable table)
        {
            gridControlDetail.DataSource = table;
            gridViewDetail.BestFitColumns();
        }

        #endregion

        #region Event Handlers

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void buttonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.GRIDVIEWEXPORT.ExportToExcel(gridView1);
        }

        //private void checkProdVersion_CheckedChanged(object sender, ItemClickEventArgs e)
        //{
        //    RunQuery();
        //}

        #endregion

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            DataTable table = GetDetailData();
            BindDetailGrid(table);
        }
    }
}