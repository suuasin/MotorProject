using DevExpress.Spreadsheet;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserInterface;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using Mozart.Studio.UIComponents;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartAPS.UI.Gantts
{
    public partial class ProductGanttView : MyXtraGridTemplate
    {
        public class QueryOptionKey
        {
            public const string TargetVersionNo = "VersionNo";
            public const string TargetLineID = "LineID";
            public const string TargetEqpId = "EqpId";
            public const string TargetEqpGroups = "EqpGroup";
            public const string TargetStartDate = "StartDate";
            public const string TargetAddDate = "AddDate";
        }

        #region PROPERTY

        private bool isQuerying { get; set; }
        private EqpMaster EqpMgr { get; set; }

        private ProductGantt Gantt { get; set; }

        private List<ProductGantt.GanttInfo> CurrInfos { get; set; }

        private XtraSheetSizeHelper SizeManager { get; set; }

        private bool IsBeforeFirstQuery { get; set; }

        private DetailBarInfoView DispView { get; set; }

        private List<string> ProductList { get; set; }

        private bool IsNeedSetFocus { get; set; }

        private string TargetProductID
        {
            get
            {
                return this.editProdId.EditValue as string;
            }
        }

        private bool ShowLayerBar
        {
            get
            {
                return true;
            }
        }
        
        private bool IsCheckShowBarTitle
        {
            get
            {
                return checkShowBarTitle.Checked;
            }
        }

        private bool IsProductInBarTitle
        {
            get
            {
                var item = this.radioBarTitle.EditValue as string;

                return MyHelper.STRING.Equals(item, "Product");
            }
        }

        private ProductGantt.ViewMode SelectViewMode
        {
            get
            {
                //var item = this.radioRowHeader.EditValue as string;
                //string str = item as string;

                //ProductGantt.ViewMode mode;
                //if (Enum.TryParse(str, out mode))
                //    return mode;

                //return ProductGantt.ViewMode.EQPGROUP;
                return ProductGantt.ViewMode.PRODUCT;
            }
        }

        private bool IsEqpViewMode
        {
            get { return this.SelectViewMode == ProductGantt.ViewMode.EQP; }
        }

        private string TargetLineID
        {
            get
            {
                return this.editLineId.EditValue as string;
            }
        }

        private string TargetDemandID
        {
            get
            {
                return this.editDemandId.EditValue as string;
            }
        }

        private List<string> SelectedProductIds
        {
            //get
            //{
            //    string targetProductId = this.TargetProductID;
            //    if (string.IsNullOrEmpty(targetProductId))
            //        return null;

            //    return targetProductId.Split(',').Select(item => item.Trim()).ToList();
            //}
            get; set;
        }

        private DateTime TargetStartDate
        {
            get
            {
                return Convert.ToDateTime(this.editDateTime.EditValue);
            }
        }

        private DateTime TargetEndDate
        {
            get
            {
                return this.TargetStartDate.AddHours(ShopCalendar.ShiftHours * this.TargetAddDays);
            }
        }

        private int TargetAddDays
        {
            get
            {
                return Convert.ToInt32(this.editDateSpin.EditValue);
            }
        }

        public string PatternOfProductID
        {
            get
            {
                return this.searchProdId.EditValue as string;
            }
        }

        public MouseSelectType SelectedMouseSelectType
        {
            get
            {
                //MouseSelectType type;
                //string sele = this.radioRightBtn.EditValue as string;
                //Enum.TryParse(sele, out type);

                //return type;
                return MouseSelectType.Product;
            }
        }

        public int CellWidth
        {
            get
            {
                return Convert.ToInt32(this.editCellWidth.EditValue);
            }
        }

        public int CellHeight
        {
            get
            {
                return Convert.ToInt32(this.editCellHeight.EditValue);
            }
        }

        #endregion

        public ProductGanttView()
        {
            InitializeComponent();
        }

        public ProductGanttView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();

            this.SizeManager = new XtraSheetSizeHelper(this.CellWidth, this.CellHeight);
            this.SizeManager.Spreadsheet = this.grid1;
            this.SizeManager.OnChanged_CellSize = () =>
            {
                this.editCellWidth.EditValue = this.SizeManager.CellWidth;
                this.editCellHeight.EditValue = this.SizeManager.CellHeight;
            };
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetInitializeOption(detailGridControl);
            this.IsBeforeFirstQuery = true;

            SetControls();
            SetGantt();
            SetEditProductId();
            SetEditDemandId();

            BindEvents();
        }

        private void SetTreeView()
        {
            this.treeProduct.BeginInit();
            this.treeProduct.BeginUpdate();

            this.treeProduct.ClearNodes();
            this.treeProduct.Columns.Clear();

            if (this.ProductList != null && this.ProductList.Count > 0)
                this.ProductList.Clear();

            if (this.dockPanelTree.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Hidden)
                this.dockPanelTree.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;

            // create columns
            TreeListColumn colProduct = this.treeProduct.Columns.Add();
            colProduct.FieldName = "PRODUCT";
            colProduct.Caption = "PRODUCT";
            colProduct.VisibleIndex = 0;
            colProduct.Visible = true;
            TreeListColumn colFulfill = this.treeProduct.Columns.Add();
            colFulfill.FieldName = "FULFILLMENT";
            colFulfill.Caption = "FULFILLMENT";
            colFulfill.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            colFulfill.Format.FormatString = "P";
            colFulfill.VisibleIndex = 1;
            colFulfill.Visible = true;
            TreeListColumn colRTF = this.treeProduct.Columns.Add();
            colRTF.FieldName = "RTF_RATE";
            colRTF.Caption = "RTF RATE";
            colRTF.VisibleIndex = 2;
            colRTF.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            colRTF.Format.FormatString = "P";
            colRTF.Visible = true;

            var productRoutes = MyHelper.DATASVC.GetEntityData<PRODUCT_ROUTE>(this.Result);
            var products = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result);
            var eqpPlans = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(this.Result).ToList();
            var stepTargets = MyHelper.DATASVC.GetEntityData<STEP_TARGET>(this.Result).ToList();
            var stepRoutes = MyHelper.DATASVC.GetEntityData<STEP_ROUTE>(this.Result).ToList();

            foreach (var prod in products)
            {
                string lastStep = stepRoutes.Where(x => x.PROCESS_ID == prod.PROCESS_ID).OrderBy(x => x.STEP_SEQ).Last().STEP_ID;
                
                double sumFulfillProcQty = eqpPlans.Where(x => x.EQP_STATE_CODE == "BUSY" && x.PRODUCT_ID == prod.PRODUCT_ID && x.STEP_ID == lastStep).Sum(x => x.PROCESS_QTY);
                double sumRtfProcQty = eqpPlans.Where(x => x.EQP_STATE_CODE == "BUSY" && x.PRODUCT_ID == prod.PRODUCT_ID && x.STEP_ID == lastStep && x.EQP_END_TIME < x.TARGET_DATE)
                                       .Sum(x => x.PROCESS_QTY);
                double sumOutQty = stepTargets.Where(x => x.PRODUCT_ID == prod.PRODUCT_ID && x.STEP_ID == lastStep).Sum(x => x.OUT_QTY);

                double fulfillment = sumOutQty == 0 ? 0 : sumFulfillProcQty / sumOutQty;
                double rtfRate = sumOutQty == 0 ? 0 : sumRtfProcQty / sumOutQty;

                this.treeProduct.AppendNode(new object[] { prod.PRODUCT_ID, fulfillment, rtfRate }, null);
            }

            foreach (var route in productRoutes.OrderBy(r => r.FROM_PRODUCT_ID))
            {
                TreeListNode parentNodes = null;
                TreeListNode childNodes = null;
                if (this.treeProduct.Nodes.Count > 0)
                {
                    parentNodes = this.treeProduct.GetNodeList().FirstOrDefault(n => n.GetDisplayText("PRODUCT").Equals(route.TO_PRODUCT_ID));
                    childNodes = this.treeProduct.GetNodeList().FirstOrDefault(n => n.GetDisplayText("PRODUCT").Equals(route.FROM_PRODUCT_ID));
                }

                if (parentNodes == null)
                    parentNodes = this.treeProduct.AppendNode(new object[] { route.TO_PRODUCT_ID }, null);

                if (childNodes == null)
                    childNodes = this.treeProduct.AppendNode(new object[] { route.FROM_PRODUCT_ID }, parentNodes);
                else
                    this.treeProduct.MoveNode(childNodes, parentNodes);
            }

            if (this.TargetProductID.ToUpper().Trim() != "ALL")
            {
                this.dockPanelTree.ShowSliding();

                TreeListNode searchNode = this.treeProduct.GetNodeList().FirstOrDefault(n => n.GetDisplayText("PRODUCT").Equals(this.TargetProductID));

                if (searchNode != null)
                {
                    this.treeProduct.MoveNode(searchNode, null);
                    List<TreeListNode> selectedNodes = new List<TreeListNode>();
                    GetChildNodes(searchNode, selectedNodes, false);

                    this.treeProduct.GetNodeList().ForEach(node =>
                    {
                        if (selectedNodes.Contains(node) == false)
                            node.Remove();
                        else
                            node.ExpandAll();
                    });

                    this.ProductList = selectedNodes.Select(item => item.GetDisplayText("PRODUCT")).ToList();
                }
                else
                    this.ProductList.Add(this.TargetProductID);
            }
            else
            {
                var prodList = (editProdId.Edit as RepositoryItemComboBox).Items.Cast<string>().ToList();
                prodList.Remove("ALL");
                this.ProductList = prodList;
            }

            this.treeProduct.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeView;
            this.treeProduct.OptionsBehavior.Editable = false;
            this.treeProduct.OptionsView.ShowColumns = true;
            this.treeProduct.OptionsView.ShowFilterPanelMode = DevExpress.XtraTreeList.ShowFilterPanelMode.Never;
            
            this.treeProduct.EndUpdate();
            this.treeProduct.EndInit();
            
            this.treeProduct.BestFitColumns();
        }

        private void GetChildNodes(TreeListNode targetNode, List<TreeListNode> selectedNodes, bool checkExpand)
        {
            selectedNodes.Add(targetNode);
            if (checkExpand && targetNode.Expanded == false)
                return;

            var en = targetNode.Nodes.GetEnumerator();
            TreeListNode child;
            while (en.MoveNext())
            {
                child = (TreeListNode)en.Current;
                selectedNodes.Add(child);

                if (child.HasChildren)
                    GetChildNodes(child, selectedNodes, checkExpand);
            }
        }

        private void SetGantt()
        {
            InitGantt();

            this.EqpMgr = new EqpMaster(this.Result);
            this.EqpMgr.LoadEqp();

            this.Gantt.Reset(this.TargetStartDate, this.EqpMgr);
        }

        //private void SetEqpGroup()
        //{
        //    var eqpGrpEdit = this.editEqpGroup.Edit as RepositoryItemCheckedComboBoxEdit;
        //    if (eqpGrpEdit == null)
        //        return;

        //    if (Gantt != null)
        //        this.Gantt.BindChkListEqpGroup(eqpGrpEdit, this.TargetLineID);

        //    eqpGrpEdit.TextEditStyle = TextEditStyles.Standard;
        //    eqpGrpEdit.Items.ToList<CheckedListBoxItem>().ForEach(item => item.CheckState = CheckState.Checked);
        //    this.editEqpGroup.EditValue = eqpGrpEdit.GetCheckedItems();
        //}

        //private void SetEqpId()
        //{
        //    var eqpIdEdit = this.editEqpId.Edit as RepositoryItemCheckedComboBoxEdit;
        //    if (eqpIdEdit == null)
        //        return;

        //    if (Gantt != null)
        //        this.Gantt.BindChkListEqpId(eqpIdEdit, this.SelectedEqpGroups);

        //    eqpIdEdit.TextEditStyle = TextEditStyles.Standard;
        //    eqpIdEdit.Items.ToList<CheckedListBoxItem>().ForEach(item => item.CheckState = CheckState.Checked);
        //    this.editEqpId.EditValue = eqpIdEdit.GetCheckedItems();
        //}

        private void SetEditProductId()
        {
            List<object> list = new List<object>();

            var table = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result);
            if (table != null)
            {
                foreach (var item in table.Distinct())
                {
                    string value = item.PRODUCT_ID;

                    if (this.TargetLineID != "ALL" && item.LINE_ID != this.TargetLineID)
                        continue;

                    if (!String.IsNullOrEmpty(value) && !list.Contains(value))
                        list.Add(value);
                }
            }

            EngControlHelper.SetControl_ComboBase(editProdId, true, list);
        }

        private void SetEditDemandId()
        {
            List<object> list = new List<object>();

            var table = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result);
            if (table != null)
            {
                foreach (var item in table.Distinct())
                {
                    string value = item.DEMAND_ID;

                    if (this.TargetProductID != "ALL" && item.PRODUCT_ID != this.TargetProductID)
                        continue;

                    if (!String.IsNullOrEmpty(value) && !list.Contains(value))
                        list.Add(value);
                }
            }

            EngControlHelper.SetControl_ComboBase(editDemandId, true, list);
        }

        private void InitGantt()
        {
            if (this.Gantt != null)
                return;


            this.Gantt = new ProductGantt(this.grid1, this.Result);

            var gantt = this.Gantt;

            gantt.DefaultColumnWidth = this.CellWidth;
            gantt.DefaultRowHeight = this.CellHeight;

            gantt.MouseSelType = MouseSelectType.Product;

            this.SetColumnHeaderView();
        }

        private void Search()
        {
            Gantt.DefaultColumnWidth = this.CellWidth;
            Gantt.DefaultRowHeight = this.CellHeight;
            
            this.buttonLoad.Enabled = false;
            this.buttonLoad2.Enabled = false;
            
            RunQuery();

            this.buttonLoad.Enabled = true;
            this.buttonLoad2.Enabled = true;
        }

        #region SET CONTROLS

        private void SetControls()
        {
            MyHelper.ENGCONTROL.SetControl_LineID(this.editLineId, this.Result, true);

            editDateSpin.EditValue = MyHelper.DATASVC.GetPlanPeriod(this.Result);
            editDateTime.EditValue = MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);

            checkShowBarTitle.Checked = true;
            
            detailGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            detailGridView.SetFocusedRowModified();
            detailGridView.OptionsMenu.ShowGroupSummaryEditorItem = true;
            detailGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;

            SetControls_DispDetail();
            SetCellSize();
        }

        private void SetCellSize()
        {
            try
            {
                string cellWidth = this.GetLocalSetting(this.ServiceProvider, this.ViewName + "ganttCellWidth");
                string cellHeight = this.GetLocalSetting(this.ServiceProvider, this.ViewName + "ganttCellHeight");

                if (!string.IsNullOrWhiteSpace(cellWidth))
                    this.editCellWidth.EditValue = Convert.ToInt32(cellWidth);
                if (!string.IsNullOrWhiteSpace(cellHeight))
                    this.editCellHeight.EditValue = Convert.ToInt32(cellHeight);
            }
            catch
            { }
        }

        private void SetControls_DispDetail()
        {
            var presetList = GetPresetList();
            this.DispView = new DetailBarInfoView(this.Result, presetList, this.TargetStartDate, this.TargetAddDays);
        }

        private List<PRESET_INFO_HIS> GetPresetList()
        {
            List<PRESET_INFO_HIS> list = new List<PRESET_INFO_HIS>();

            var table = MyHelper.DATASVC.GetEntityData<PRESET_INFO_HIS>(this.Result);
            if (table == null)
                return list;

            return table.ToList();
        }

        private string[] SplitEqpID(string eqp_id)
        {
            string chamberId;
            string eqpId;
            List<string> idList = new List<string>();

            string[] id = eqp_id.Split('-');
            eqpId = id[0];
            if (id.Length > 1)
            {
                string[] arr = id[1].Split('(');
                chamberId = arr[0];
            }
            else chamberId = eqpId;

            idList.Add(eqpId);
            idList.Add(chamberId);

            return idList.ToArray();
        }

        private void SetColumnHeaderView()
        {
            var styleBook = this.Gantt.Workbook.Styles;

            var customHeader = styleBook.Add("CustomHeader");
            var customHeaderCenter = styleBook.Add("CustomHeaderCenter");
            var customHeaderRight = styleBook.Add("CustomHeaderRight");
            var customDefaultCenter = styleBook.Add("CustomDefaultCenter");

            customHeader.Fill.BackgroundColor = Color.MintCream;
            customHeader.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            customHeader.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            customHeaderCenter.Fill.BackgroundColor = Color.MintCream;
            customHeaderCenter.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            customHeaderCenter.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            customHeaderRight.Fill.BackgroundColor = Color.MintCream;
            customHeaderRight.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            customHeaderRight.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Right;

            customDefaultCenter.Fill.BackgroundColor = Color.MintCream;
            customDefaultCenter.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            customDefaultCenter.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        }

        private void BindEvents()
        {
            Gantt.HeaderHourChanged += new GanttColumnHeaderEventHandler(GanttView_HeaderHourChanged);
            Gantt.HeaderShiftChanged += new GanttColumnHeaderEventHandler(GanttView_HeaderShiftChanged);
            Gantt.HeaderDone += new GanttColumnHeaderEventHandler(GanttView_HeaderDone);

            Gantt.BindItemAdded += new GanttItemEventHandler(GanttView_BindItemAdded);
            Gantt.BindRowAdding += new GanttRowEventHandler(GanttView_BindRowAdding);
            Gantt.BindRowAdded += new GanttRowEventHandler(GanttView_BindRowAdded);
            Gantt.BindBarAdded += new GanttCellEventHandler(GanttView_BindBarAdded);
            Gantt.BindDone += new EventHandler(GanttView_BindDone);

            Gantt.BarClick += new BarEventHandler(GanttView_BarClick);
            Gantt.BarDoubleClick += new BarEventHandler(GanttView_BarDoubleClick);
            Gantt.BarDraw += new BarDrawEventHandler(GanttView_BarDraw);

            Gantt.CellClick += new CellEventHandler(GanttView_CellClick);
            Gantt.CellDoubleClick += new CellEventHandler(GanttView_CellDoubleClick);
        }

        #region BAR HANDLING

        void GanttView_BarDraw(object sender, BarDrawEventArgs args)
        {
            var bar = args.Bar as GanttBar;
            var brushinfo = Gantt.GetBrushInfo(bar, this.PatternOfProductID);

            args.Background = brushinfo;
            args.DrawFrame = Gantt.EnableSelect && Gantt.SelectedBar != null && !Gantt.CompareToSelectedBar(bar, this.PatternOfProductID);
            args.FrameColor = Color.White;
            args.DrawFrame = true;

            Color foreColor = Color.Black;

            if (bar.State == EqpState.DOWN || bar.State == EqpState.PM)
            {
                foreColor = Color.White;
            }
            else if (bar.IsGhostBar)
            {
                args.Background = new BrushInfo(System.Drawing.Drawing2D.HatchStyle.Percent20, Color.Gray, Color.White);
            }

            args.ForeColor = foreColor;
            if(IsCheckShowBarTitle)
                args.Text = bar.GetTitle(this.Gantt.IsProductInBarTitle);
            args.DrawDefault = true;

            if (!bar.IsOnTime)
            {
                args.Background = new BrushInfo(System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal, Color.Gray, brushinfo.BackColor);
            }

            //if (bar != null && bar.OwnerType == "OwnerE")
            //{
            //    var p = new Pen(Color.Black, 2);
            //    args.Graphics.DrawRectangle(p, bar.Bounds.X, bar.Bounds.Y, bar.Bounds.Width + 0.5f, bar.Bounds.Height);
            //}

            //if (bar != null && bar.EqpRecipe == "Y")
            //{
            //    var p = new Pen(Color.Blue, 3);
            //    args.Graphics.DrawRectangle(p, bar.Bounds.X, bar.Bounds.Y, bar.Bounds.Width + 0.5f, bar.Bounds.Height);
            //}
        }

        private void GanttView_BarDoubleClick(object sender, BarEventArgs e)
        {

        }

        private void GanttView_CellClick(object sender, CellEventArgs e)
        {

        }

        private void GanttView_CellDoubleClick(object sender, CellEventArgs e)
        {
            var gantt = this.Gantt;
            string cellText = e.Cell.DisplayText;

            int cellColIdx = e.Cell.ColumnIndex;
            int cellRowIdx = e.RowIndex;
            int eqpColIdx = IsEqpViewMode ? 0 : 1;
            int stepColidx = SelectViewMode == ProductGantt.ViewMode.STEP ? eqpColIdx - 1 : eqpColIdx + 1;
            int runQtyColIdx = e.Cell.CurrentRegion.ColumnCount - 2;
            int loadColIdx = e.Cell.CurrentRegion.ColumnCount - 3;

            string eqpId = Convert.ToString(gantt.Worksheet[cellRowIdx, eqpColIdx].Value);
            string stepId = Convert.ToString(gantt.Worksheet[cellRowIdx, stepColidx].Value);

            if (SelectViewMode == ProductGantt.ViewMode.STEP && string.IsNullOrEmpty(stepId) && gantt.LastRowIndex != cellRowIdx)
            {
                while (string.IsNullOrEmpty(stepId))
                {
                    cellRowIdx -= 1;
                    stepId = Convert.ToString(gantt.Worksheet[cellRowIdx, stepColidx].Value);
                }
            }
            stepId = IsEqpViewMode == true || SelectViewMode == ProductGantt.ViewMode.STEP ? stepId : null;


            if (string.IsNullOrEmpty(eqpId) && gantt.LastRowIndex != cellRowIdx)
            {
                while (string.IsNullOrEmpty(eqpId))
                {
                    cellRowIdx -= 1;
                    eqpId = Convert.ToString(gantt.Worksheet[cellRowIdx, eqpColIdx].Value);
                }
            }

            if (string.IsNullOrEmpty(cellText))
                return;

            if (cellColIdx == eqpColIdx && cellText != ColName.EqpId)
            {
                EqpPopUp(this.Result, eqpId);
            }

            if (cellColIdx == runQtyColIdx && cellText != ColName.RunQtySum)
            {
                string columnName = ColName.RunQtySum;
                DataTable db = Load_QtyView(eqpId, stepId, columnName);
                Load_QtyPopUp(db, eqpId, stepId);
            }

            if (cellColIdx == loadColIdx && cellText != ColName.LoadRate)
            {
                string columnName = ColName.LoadRate;
                DataTable db = Load_QtyView(eqpId, stepId, columnName);
                Load_QtyPopUp(db, eqpId, stepId);
            }
        }

        void GanttView_BarClick(object sender, BarEventArgs e)
        {
            if (this.Gantt.ColumnHeader == null)
                return;

            this.grid1.BeginUpdate();

            if (e.Mouse.Button == MouseButtons.Right && e.Bar != null)
            {
                Gantt.TurnOnSelectMode();

                if (this.SelectedMouseSelectType == MouseSelectType.Pattern)
                {
                    Gantt.SelectedBar = new Bar(TargetStartDate, TargetStartDate.AddHours(1), 20, 20, EqpState.BUSY);
                }
                else
                {
                    Gantt.SelectedBar = e.Bar as GanttBar;
                }
            }
            else
            {
                Gantt.TurnOffSelectMode();
            }

            this.grid1.EndUpdate();
            this.grid1.Refresh();

            if (e.Mouse.Button == MouseButtons.Right)
                return;

            var bar = e.Bar as GanttBar;

            ViewDispDetail(bar);
            ViewEqpProcessDetail(bar);
            ViewArrangeDetail(bar);
            HighLightSelectRow(e.RowIndex);
        }

        private void HighLightSelectRow(int rowIndex)
        {
            var worksheet = this.Gantt.Worksheet;

            worksheet.SelectRowUsed(rowIndex, false, 0);
        }

        private void ViewDispDetail(GanttBar bar = null)
        {
            if (this.DispView == null)
                return;

            this.DispView.SetBarInfo(bar);
        }

        private void ViewArrangeDetail(GanttBar bar)
        {
            //if (bar == null)
            //    return;

            //if (ArrangeView == null)
            //    this.ArrangeView = new ArrangeAnalysisView(this.TargetVersionNo);

            //var control = this.ArrangeView;

            //arrangePanel.Controls.Clear();
            //arrangePanel.Controls.Add(control);
            //control.Dock = DockStyle.Fill;
            //control.Show();

            //string eqpID = bar.EqpID;
            //string subEqpID = bar.SubEqpID;
            //string wipInitRun = bar.WipInitRun;

            //string matchKey = ArrangeAnalysisView.CreateKey_Match(eqpID, bar.StepID, bar.ProductID,
            //    string.IsNullOrEmpty(bar.ProductVersion) ? Consts.NULL_ID : bar.ProductVersion,
            //    string.IsNullOrEmpty(bar.ToolID) ? Consts.NULL_ID : bar.ToolID);

            //control.Query(eqpID, subEqpID, wipInitRun, matchKey);
        }

        private void ViewEqpProcessDetail(GanttBar bar = null)
        {
            this.IsNeedSetFocus = true;

            DataTable dt = CreateProcessDataTable();

            string eqpID = bar == null ? null : bar.EqpID;

            List<ProductGantt.GanttInfo> list = GetGanttInfo(eqpID);
            if (list != null)
            {
                foreach (ProductGantt.GanttInfo info in list)
                {
                    foreach (var item in info.Items)
                    {
                        foreach (GanttBar b in item.Value)
                        {
                            if (b.State == EqpState.IDLE || b.State == EqpState.IDLERUN)
                                continue;

                            if (b.State != EqpState.DOWN)
                            {
                                if (b.BarKey != item.Key)
                                    continue;
                            }

                            DataRow drow = dt.NewRow();

                            drow[ColName.EqpId] = b.EqpID;
                            drow[ColName.State] = b.State.ToString();

                            if (b.State == EqpState.BUSY || b.State == EqpState.SETUP)
                            {
                                drow[ColName.LotID] = b.OrigLotID;
                                drow[ColName.ProductID] = b.ProductID;
                                drow[ColName.StepId] = b.StepID;

                                if (b.State == EqpState.BUSY)
                                    drow[ColName.TIQty] = b.TIQty;
                            }

                            drow[ColName.DISPATCH_IN_TIME] = MyHelper.DATE.Format(b.DispatchInTime);
                            drow[ColName.StartTime] = MyHelper.DATE.Format(b.TkinTime);
                            drow[ColName.EndTime] = MyHelper.DATE.Format(b.TkoutTime);
                            drow[ColName.GapTime] = b.TkoutTime - b.TkinTime;
                            drow[ColName.LPST] = MyHelper.DATE.Format(b.TargetDate);

                            TimeSpan tard = b.TkinTime - b.TargetDate;
                            string strTard = (tard.Days * 24 + tard.Hours).ToString() + ":"
                                             + Math.Abs(tard.Minutes).ToString() 
                                             + ":" + Math.Abs(tard.Seconds).ToString();
                            drow[ColName.Tardiness] = strTard;

                            dt.Rows.Add(drow);
                        }
                    }
                }
            }

            detailGridControl.BeginUpdate();
            detailGridControl.DataSource = new DataView(dt, "", ColName.StartTime, DataViewRowState.CurrentRows);
            detailGridControl.EndUpdate();

            detailGridView.BestFitColumns();

            MyHelper.ENGCONTROL.SetGridViewColumnWidth(detailGridView);
        }

        private DataTable Load_QtyView(string targetId, string stepId, string columnName)
        {
            string[] idArr = SplitEqpID(targetId);
            string targetColumn = columnName;

            string eqpID = idArr[0];
            string subEqpID = idArr[1];
            string stepID = stepId;

            subEqpID = eqpID == subEqpID ? null : subEqpID;

            DataTable qtyDt = CreateProductDataTable();
            List<ProductGantt.GanttInfo> list = GetGanttInfo(eqpID, stepID);

            Dictionary<string, int> qtyDic = new Dictionary<string, int>();
            Dictionary<string, double> timeDic = new Dictionary<string, double>();
            string[] stateArr = new string[] { STATE.Pm, STATE.Setup, STATE.Busy, STATE.Idle, STATE.idleRun, STATE.Down };

            foreach (string eqpState in stateArr)
            {
                timeDic.Add(eqpState, 0.0);
            }

            int allSum = 0;
            double C_RowSumTime = 0.0;
            double allTime = Math.Round(((this.TargetEndDate - this.TargetStartDate).TotalSeconds), 2);
            if (list != null)
            {
                foreach (ProductGantt.GanttInfo info in list)
                {
                    foreach (var item in info.Items)
                    {
                        foreach (GanttBar b in item.Value)
                        {
                            double timevalue = 0;
                            double time = (b.EndTime - b.StartTime).TotalSeconds;
                            string state = Convert.ToString(b.State);

                            if (!timeDic.ContainsKey(state))
                            {
                                timeDic.Add(state, time);
                            }
                            else
                            {
                                timeDic.TryGetValue(state, out timevalue);
                                double sum = timevalue + time;
                                timeDic[state] = sum;
                            }

                            if (b.State == EqpState.BUSY || b.State == EqpState.IDLERUN || b.State == EqpState.SETUP)
                                C_RowSumTime += (b.EndTime - b.StartTime).TotalSeconds;

                            if (b.State == EqpState.IDLE || b.State == EqpState.IDLERUN)
                                continue;

                            if (b.State != EqpState.DOWN)
                            {
                                if (b.BarKey != item.Key)
                                    continue;
                            }

                            if (b.State == EqpState.BUSY || b.State == EqpState.SETUP && b.TIQty != 0)
                            {
                                DataRow drow = qtyDt.NewRow();
                                drow[ColName.ProductID] = b.ProductID;
                                if (b.State == EqpState.BUSY)
                                    drow[ColName.TIQty] = b.TIQty;
                                qtyDt.Rows.Add(drow);
                            }

                        }
                    }
                }
            }
            timeDic[STATE.Idle] = allTime - C_RowSumTime;
            double rowLoad = Math.Round((C_RowSumTime / allTime * 100), 1);

            if (targetColumn == ColName.LoadRate)
            {
                DataTable reDt = CreateLoadDateTable();
                foreach (var item in timeDic)
                {
                    DataRow drow = reDt.NewRow();
                    drow[ColName.State] = item.Key;
                    drow[ColName.TimeSec] = item.Value;
                    drow[ColName.LoadRate] = string.Format("{0}%", Math.Round(item.Value / allTime * 100, 1));
                    reDt.Rows.Add(drow);
                }
                DataRow row = reDt.NewRow();
                row[ColName.State] = "TOTAL_LOAD";
                row[ColName.TimeSec] = C_RowSumTime;
                row[ColName.LoadRate] = string.Format("{0}%", rowLoad);
                reDt.Rows.Add(row);
                return reDt;
            }

            else
            {
                foreach (DataRow dr in qtyDt.Rows)
                {
                    int value = 0;
                    if (!qtyDic.ContainsKey(dr[ColName.ProductID] as string))
                        qtyDic.Add(dr[ColName.ProductID] as string, Convert.ToInt32(dr[ColName.TIQty]));
                    else
                    {
                        qtyDic.TryGetValue(dr[ColName.ProductID] as string, out value);
                        int sum = value + Convert.ToInt32(dr[ColName.TIQty]);
                        qtyDic[dr[ColName.ProductID] as string] = sum;
                    }
                }
                DataTable resultDt = CreateProductDataTable();
                foreach (var item in qtyDic)
                {
                    DataRow drow = resultDt.NewRow();
                    drow[ColName.ProductID] = item.Key;
                    drow[ColName.TIQty] = item.Value;
                    allSum += item.Value;
                    resultDt.Rows.Add(drow);
                }

                DataRow row = resultDt.NewRow();
                row[ColName.ProductID] = "TOTAL_QTY";
                row[ColName.TIQty] = allSum;
                resultDt.Rows.Add(row);

                return resultDt;
            }
        }

        private DataTable CreateProductDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(ColName.ProductID, typeof(string));
            dt.Columns.Add(ColName.TIQty, typeof(int));
            dt.Columns[ColName.TIQty].Caption = ColName.RunQtySum;

            return dt;
        }

        private DataTable CreateLoadDateTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(ColName.State, typeof(string));
            dt.Columns.Add(ColName.TimeSec, typeof(double));
            dt.Columns.Add(ColName.LoadRate, typeof(string));

            return dt;
        }

        private void Load_QtyPopUp(DataTable dt, string targetEqp, string targetLayer)
        {
            var view = new EqpGanttLoadQtyView(dt, targetEqp, targetLayer);
            var dialog = new PopUpForm(view);
            view.Dock = DockStyle.Fill;
            dialog.Size = new Size(600, 400);
            dialog.Controls.Clear();
            dialog.Controls.Add(view);
            dialog.Text = String.Format("INFORMATION");
            dialog.StartPosition = FormStartPosition.CenterScreen;

            dialog.Show();
        }

        private void EqpPopUp(IExperimentResultItem result, string targetId)
        {
            var view = new EqpGanttPopUpView(result, targetId);

            var dialog = new PopUpForm(view);
            view.Dock = DockStyle.Fill;
            dialog.Size = new Size(600, 800);
            dialog.Controls.Clear();
            dialog.Controls.Add(view);
            dialog.Text = String.Format("EQP ID Information");
            dialog.StartPosition = FormStartPosition.CenterScreen;

            dialog.Show();
        }


        private DataTable CreateProcessDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(ColName.EqpId, typeof(string));
            dt.Columns.Add(ColName.State, typeof(string));
            dt.Columns.Add(ColName.LotID, typeof(string));
            dt.Columns.Add(ColName.StepId, typeof(string));
            dt.Columns.Add(ColName.ProductID, typeof(string));
            dt.Columns[ColName.ProductID].Caption = "PRODUCT ID";
            dt.Columns.Add(ColName.DISPATCH_IN_TIME, typeof(string));
            dt.Columns[ColName.DISPATCH_IN_TIME].Caption = "DISPATCH IN TIME";
            dt.Columns.Add(ColName.StartTime, typeof(string));
            dt.Columns[ColName.StartTime].Caption = "START TIME";
            dt.Columns.Add(ColName.EndTime, typeof(string));
            dt.Columns[ColName.EndTime].Caption = "END TIME";
            dt.Columns.Add(ColName.LPST, typeof(string));
            dt.Columns.Add(ColName.Tardiness, typeof(string));
            dt.Columns.Add(ColName.TIQty, typeof(int));
            dt.Columns[ColName.TIQty].Caption = "IN QTY";
            dt.Columns.Add(ColName.GapTime, typeof(string));
            dt.Columns[ColName.GapTime].Caption = "PROCESSED TIME";
            
            return dt;
        }

        private List<ProductGantt.GanttInfo> GetGanttInfo(string eqpID, string stepID = null)
        {
            List<ProductGantt.GanttInfo> result = new List<ProductGantt.GanttInfo>();

            if (string.IsNullOrEmpty(eqpID))
                return result;

            var list = this.CurrInfos;
            foreach (ProductGantt.GanttInfo info in list)
            {
                if (info.EqpID != eqpID)
                    continue;

                if (stepID != null)
                {
                    if (info.StepID != stepID)
                        continue;
                }

                result.Add(info);
            }
            return result;
        }

        private void rdoMouseType_EditValueChanged(object sender, EventArgs e)
        {
            if (Gantt == null)
                return;

            Gantt.MouseSelType = this.SelectedMouseSelectType;
        }

        private void grid1_PopupMenuShowing(object sender, DevExpress.XtraSpreadsheet.PopupMenuShowingEventArgs e)
        {
            e.Menu = null;
        }

        private void barTitleGroup_EditValueChanged(object sender, EventArgs e)
        {
            if (this.IsBeforeFirstQuery == false)
                return;
        }

        private void editLineId_EditValueChanged(object sender, EventArgs e)
        {
            if (this.radioBarTitle.Edit != null)
                (this.radioBarTitle.Edit as RepositoryItemRadioGroup).Items[1].Enabled = true;

            SetEditProductId();
            SetEditDemandId();
        }

        private void tePattern_EditValueChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.PatternOfProductID))
                return;

            if (this.SelectedMouseSelectType == MouseSelectType.Pattern)
            {
                prodPatternApply();
            }
        }

        private void repoProdId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.SelectedMouseSelectType != MouseSelectType.Pattern)
                {
                    this.radioRightBtn.EditValue = MouseSelectType.Pattern.ToString();
                }
                this.ribbonControl1.Manager.ActiveEditItemLink.PostEditor();

                prodPatternApply();
            }
        }

        private void prodPatternApply()
        {
            if (this.SelectedMouseSelectType != MouseSelectType.Pattern)
                return;

            this.grid1.BeginUpdate();

            Gantt.TurnOnSelectMode();

            Gantt.SelectedBar = new Bar(TargetStartDate, TargetStartDate.AddHours(1), 20, 20, EqpState.BUSY);

            this.grid1.EndUpdate();
            this.grid1.Refresh();
        }

        #endregion

        #region BAR BUILD

        void GanttView_BindDone(object sender, EventArgs e)
        {
            var colHeader = Gantt.ColumnHeader;

            // 마지막 Row 값 세팅
            if (this.IsEqpViewMode)
            {
                string sLoadRate = string.Empty;
                double rate = _rowsumLoadTimeFrBar / (this.TargetEndDate - this.TargetStartDate).TotalSeconds * 100.0;
                sLoadRate = Math.Round(rate, 1).ToString() + "%";

                XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(_curRowIdx, ColName.LoadRate), sLoadRate);
                colHeader.GetCellInfo(_curRowIdx, ColName.LoadRate).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                string subJobChangeCnt = string.Format("{0:n0}", _subJobChg);
                XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(_curRowIdx, ColName.SetupCount), subJobChangeCnt);
                colHeader.GetCellInfo(_curRowIdx, ColName.SetupCount).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }

            string subTotalRun = string.Format("{0:n0}", _subTotal);
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(_curRowIdx, ColName.TotalRun), subTotalRun);
            colHeader.GetCellInfo(_curRowIdx, ColName.TotalRun).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            string jobChgColName = string.Format("{0} \n({1})", ColName.SetupCount, _totalJobChg.ToString("#.#"));
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(0, ColName.SetupCount), jobChgColName);
            colHeader.GetCellInfo(0, ColName.SetupCount).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;

            string loadRateColName = string.Empty;
            double avgLoadTIme = _totalLoadTImeFrBarDic.Count <= 0 ? 0 : _totalLoadTImeFrBarDic.Values.Average();
            double totalRateForSim = _totalLoadTImeFrBarDic.Count <= 0 ? 0
                    : avgLoadTIme / (this.TargetEndDate - this.TargetStartDate).TotalSeconds * 100.0;
            loadRateColName = string.Format("{0} \n({1}%)", ColName.LoadRate, totalRateForSim.ToString("#.#"));
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(0, ColName.LoadRate), loadRateColName);

            string totalString = string.Format("{0:n0}", _total);
            string totalColName = string.Format("{0} \n({1})", ColName.TotalRun, totalString);
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(0, ColName.TotalRun), totalColName);
            colHeader.GetCellInfo(0, ColName.TotalRun).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;

            int fromRowIdx = this.IsEqpViewMode ? _startSameEqpRowIdx : _startSameRowKeyIdxFirst;
            MergeRows(fromRowIdx, Gantt.LastRowIndex, 0);
            int fromRowIdxSecondCol = _startSameRowKeyIdxSecond;
            MergeRows(fromRowIdxSecondCol, Gantt.LastRowIndex, 1);

            var eqpGroupCol = colHeader.GetCellInfo(0, ColName.EqpGroup);
            if (eqpGroupCol != null)
            {
                eqpGroupCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                eqpGroupCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }

            var stepCol = colHeader.GetCellInfo(0, ColName.StepId);
            if (stepCol != null)
            {
                stepCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                stepCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }

            colHeader.GetCellInfo(0, ColName.EqpId).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            colHeader.GetCellInfo(0, ColName.EqpId).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;

            colHeader.GetCellInfo(0, ColName.LoadRate).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            colHeader.GetCellInfo(0, ColName.LoadRate).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            colHeader.GetCellInfo(0, ColName.RunQtySum).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            colHeader.GetCellInfo(0, ColName.RunQtySum).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            colHeader.GetCellInfo(0, ColName.TotalRun).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            colHeader.GetCellInfo(0, ColName.TotalRun).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            PaintTotColumnCell();
        }

        void GanttView_BindBarAdded(object sender, GanttCellEventArgs args)
        {
            args.Bar.CumulateQty(ref _rowsum, ref _rowsum);

            _rowsumJobChg += args.Bar.BarList.Where(x => x.State == EqpState.SETUP).Count();

            double loadTime = args.Bar.BarList.Where(x => x.State == EqpState.BUSY || x.State == EqpState.IDLERUN /*|| x.State == EqpState.SETUP*/)
                .Where(x => x.TkinTime < this.TargetEndDate)
                .Sum(x =>
                {
                    DateTime startTime = x.TkinTime;
                    DateTime endTime = x.TkoutTime;

                    if (endTime > this.TargetEndDate)
                        endTime = this.TargetEndDate;

                    return (endTime - startTime).TotalSeconds;
                });

            _rowsumLoadTimeFrBar += loadTime;
        }

        void GanttView_BindRowAdding(object sender, GanttRowEventArgs args)
        {
            var worksheet = this.Gantt.Worksheet;

            var info = args.Item as ProductGantt.GanttInfo;
            var colHeader = Gantt.ColumnHeader;

            _curRowIdx = args.RowIndex;

            string productId = info.ProductID;
            string eqpModel = info.EqpModel;
            string eqpGroup = info.EqpGroup;
            string eqpID = info.EqpID;
            string step = info.StepID;

            SetRowHeaderValue(args.RowIndex, productId, eqpModel, eqpGroup, eqpID, step);

            this._rowsum = 0;
            this._rowsumJobChg = 0;
            this._rowsumLoadTimeFrBar = 0;

            if (args.Node == null)
                return;

            var rows = args.Node.LinkedBarList;

            if (rows.Count > 1 && args.Index > 0 && args.Index < rows.Count - 1)
            {
                var prodIdCol = colHeader.GetCellInfo(args.RowIndex, ColName.ProductID);
                if (prodIdCol != null)
                    XtraSheetHelper.SetCellText(prodIdCol, productId);

                var eqpGroupCol = colHeader.GetCellInfo(args.RowIndex, ColName.EqpGroup);
                if (eqpGroupCol != null)
                    XtraSheetHelper.SetCellText(eqpGroupCol, eqpGroup);

                var stepCol = colHeader.GetCellInfo(args.RowIndex, ColName.StepId);
                if (stepCol != null)
                    XtraSheetHelper.SetCellText(eqpGroupCol, step);

                var eqpModelCol = colHeader.GetCellInfo(args.RowIndex, ColName.EqpModel);
                if (eqpModelCol != null)
                    XtraSheetHelper.SetCellText(eqpGroupCol, eqpModelCol);

                XtraSheetHelper.SetCellText(colHeader.GetCellInfo(args.RowIndex, ColName.EqpId), eqpID);

                PaintRowKeyedCell(args.RowIndex, _currColor);
            }
        }

        void GanttView_BindRowAdded(object sender, GanttRowEventArgs args)
        {
            var info = args.Item as ProductGantt.GanttInfo;
            var colHeader = Gantt.ColumnHeader;

            if (_totalLoadTImeFrBarDic.ContainsKey(info.EqpID) == false)
                _totalLoadTImeFrBarDic.Add(info.EqpID, _rowsumLoadTimeFrBar);

            if (this.IsEqpViewMode == false)
            {
                double rate = _rowsumLoadTimeFrBar / (this.TargetEndDate - this.TargetStartDate).TotalSeconds * 100.0;
                string loadRate = Math.Round(rate, 1).ToString() + "%";

                XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(args.RowIndex, ColName.LoadRate), loadRate);
                XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(args.RowIndex, ColName.SetupCount), _rowsumJobChg);


                colHeader.GetCellInfo(args.RowIndex, ColName.LoadRate).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }

            XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(args.RowIndex, ColName.RunQtySum), _rowsum);

            _subTotal += _rowsum;
            _total += _rowsum;
            _subJobChg += _rowsumJobChg;
            _totalJobChg += _rowsumJobChg;
        }

        void GanttView_BindItemAdded(object sender, GanttItemEventArgs args)
        {
        }

        #endregion

        #region HEADER BUILD

        void GanttView_HeaderDone(object sender, GanttColumnHeaderEventArgs e)
        {
            var colHeader = e.ColumnHeader;

            colHeader.AddColumn(new XtraSheetHelper.SfColumn(ColName.SetupCount, 40));
            colHeader.AddColumn(new XtraSheetHelper.SfColumn(ColName.LoadRate, 50));
            colHeader.AddColumn(new XtraSheetHelper.SfColumn(ColName.RunQtySum, 50));
            colHeader.AddColumn(new XtraSheetHelper.SfColumn(ColName.TotalRun, 50));

            this.SizeManager.LeftExceptCount = this.Gantt.FixedColCount;
            this.SizeManager.TopExceptCount = this.Gantt.FixedRowCount;

            this.SizeManager.RightExceptCount = 4;
        }

        void GanttView_HeaderShiftChanged(object sender, GanttColumnHeaderEventArgs args)
        {
            try
            {
                MergeHeader_Shift(args);
            }
            catch { }
        }

        private void MergeHeader_Shift(GanttColumnHeaderEventArgs args)
        {
            string parttern = this.Gantt.DateKeyPattern;

            DateTime shiftStart = MyHelper.DATE.Trim(args.Time, "HH");

            DateTime hstart = MyHelper.DATE.Trim(this.TargetStartDate, "HH");
            DateTime hend = MyHelper.DATE.Trim(this.TargetEndDate, "HH");
            if (hend == MyHelper.DATE.Trim(this.TargetEndDate, "mm"))
                hend = hend.AddHours(-1);

            DateTime ss = shiftStart;
            DateTime ee = ss.AddHours(ShopCalendar.ShiftHours - 1);

            if (ss < hstart)
                ss = hstart;

            if (ee > hend)
                ee = hend;

            var sColName = ss.ToString(parttern);
            var eColName = ee.ToString(parttern);
            string headText = Gantt.GetJobChgShiftCntFormat(ss);

            var gcol = new XtraSheetHelper.SfGroupColumn(headText, sColName, eColName);
            args.ColumnHeader.AddGroupColumn(gcol);
        }

        void GanttView_HeaderHourChanged(object sender, GanttColumnHeaderEventArgs args)
        {
            string key = args.Time.ToString(Gantt.DateKeyPattern);
            string caption = Gantt.GetJobChgHourCntFormat(args.Time);

            args.ColumnHeader.AddColumn(new XtraSheetHelper.SfColumn(key, caption, Gantt.DefaultColumnWidth, true, false));
        }

        #endregion

        #endregion

        #region BIND DATA

        protected override void Query()
        {
            Gantt.GanttType = GanttType.ProdGantt;

            SetGantt();

            //SetControls_DispDetail();

            this.CurrInfos = GenerateGantt_Sim();

            //this.ArrangeView = new ArrangeAnalysisView(this.TargetVersionNo);
            //this.ArrangeView.ImportData();

            BindGrid(this.CurrInfos);
        }

        private List<ProductGantt.GanttInfo> GenerateGantt_Sim()
        {
            var gantt = Gantt;

            gantt.PrepareData(this.IsProductInBarTitle, false);

            gantt.BuildData_Sim(this.TargetLineID,
                                this.ProductList,
                                this.TargetDemandID,
                                this.TargetStartDate,
                                this.TargetEndDate,
                                this.SelectViewMode);

            return gantt.Expand(this.ShowLayerBar, this.SelectViewMode);
        }

        #endregion

        #region BIND GRID

        bool _isFirst = true;
        string _preProdID = string.Empty;
        string _preStepID = string.Empty;
        string _preEqpID = string.Empty;
        string _preEqpGroup = string.Empty;
        string _preEqpModel = string.Empty;
        string _preRowKey = string.Empty;

        Color _preColor = XtraSheetHelper.AltColor;
        Color _currColor = XtraSheetHelper.AltColor2;

        int _startSameEqpRowIdx = 0;
        int _startSameRowKeyIdxFirst = 0;
        int _startSameRowKeyIdxSecond = 0;
        int _curRowIdx = 0;

        double _subTotal = 0;
        double _total = 0;
        double _subJobChg = 0;
        double _totalJobChg = 0;

        Dictionary<string, double> _totalLoadTimeFrStatDic;
        Dictionary<string, double> _totalLoadTImeFrBarDic;

        double _rowsum = 0;
        double _rowsumJobChg = 0;
        double _rowsumLoadTimeFrBar = 0;

        public Dictionary<string, int> _moveByLayer;

        private void Clear()
        {
            _isFirst = true;
            _preRowKey = string.Empty;

            _preColor = XtraSheetHelper.AltColor;
            _currColor = XtraSheetHelper.AltColor2;

            _startSameEqpRowIdx = 0;
            _startSameRowKeyIdxFirst = 0;
            _startSameRowKeyIdxSecond = 0;

            _subTotal = 0;
            _total = 0;
            _subJobChg = 0;
            _totalJobChg = 0;

            _totalLoadTimeFrStatDic = new Dictionary<string, double>();
            _totalLoadTImeFrBarDic = new Dictionary<string, double>();

            _rowsum = 0;
            _rowsumJobChg = 0;
            _rowsumLoadTimeFrBar = 0;
        }

        private void BindGrid(List<ProductGantt.GanttInfo> currInfos)
        {
            Clear();

            Gantt.Workbook.BeginUpdate();
            Gantt.ResetWorksheet();

            this.Gantt.TurnOffSelectMode();

            SetColumnHeaders();

            this.Gantt.SchedBarComparer = new GanttMaster.CompareMBarList();

            if (currInfos != null && currInfos.Count > 1)
            {

                ProductGantt.SortOptions[] options = null;
                if (this.SelectViewMode == ProductGantt.ViewMode.PRODUCT)
                {
                    options = new ProductGantt.SortOptions[]{ ProductGantt.SortOptions.PRODUCT,
                                                              ProductGantt.SortOptions.STEP,
                                                              ProductGantt.SortOptions.EQP };
                }
                if (this.SelectViewMode == ProductGantt.ViewMode.EQPGROUP)
                {
                    options = new ProductGantt.SortOptions[]{ ProductGantt.SortOptions.EQP_GROUP,
                                                         ProductGantt.SortOptions.EQP };
                }
                else if (this.SelectViewMode == ProductGantt.ViewMode.EQP)
                {
                    options = new ProductGantt.SortOptions[] { ProductGantt.SortOptions.EQP,
                                                           ProductGantt.SortOptions.STEP };
                }
                else if (this.SelectViewMode == ProductGantt.ViewMode.STEP)
                {
                    options = new ProductGantt.SortOptions[] { ProductGantt.SortOptions.STEP,
                                                           ProductGantt.SortOptions.EQP };
                }
                else if (this.SelectViewMode == ProductGantt.ViewMode.EQPMODEL)
                {
                    options = new ProductGantt.SortOptions[] { ProductGantt.SortOptions.EQP_MODEL,
                                                           ProductGantt.SortOptions.EQP };
                }

                //currInfos.Sort(new ProductGantt.CompareGanttInfo(Gantt.GanttType,
                //                                             this.EqpMgr,
                //                                             this.TargetLineID,
                //                                             options));
            }

            this.Gantt.Bind(currInfos);

            this.Gantt.Workbook.EndUpdate();
        }

        private void SetRowHeaderValue(int rowIndex, string prodID, string eqpModel, string eqpGroup, string eqpID, string stepID)
        {
            if (this.IsEqpViewMode == false)
            {
                var colHeader = Gantt.ColumnHeader;

                if (_isFirst)
                {
                    _preProdID = prodID;
                    _preStepID = stepID;
                    _preEqpID = eqpID;
                    _preEqpModel = eqpModel;
                    _preEqpGroup = eqpGroup;
                    _startSameEqpRowIdx = rowIndex;
                    _startSameRowKeyIdxFirst = rowIndex;
                    _startSameRowKeyIdxSecond = rowIndex;

                    _isFirst = false;
                }

                if (eqpID.Equals(_preEqpID) == false)
                {
                    _preEqpID = eqpID;
                    _startSameEqpRowIdx = rowIndex;
                }
                
                // merge step col
                if (stepID.Equals(_preStepID) == false && this.SelectViewMode == ProductGantt.ViewMode.PRODUCT)
                {
                    MergeRows(_startSameRowKeyIdxSecond, rowIndex - 1, 1);

                    //Color tmp = _preColor;
                    //_preColor = _currColor;
                    //_currColor = tmp;
                    _startSameRowKeyIdxSecond = rowIndex;

                    _preEqpID = eqpID;
                    _preStepID = stepID;
                    //_startSameEqpRowIdx = rowIndex;

                    //if (_startSameEqpRowIdx > 1)
                    //{
                    //    XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(rowIndex - 1, ColName.TotalRun), _subTotal);
                    //}

                    _preEqpID = eqpID;
                    _preStepID = stepID;
                    //_startSameEqpRowIdx = rowIndex;
                    _subTotal = 0;
                    _subJobChg = 0;
                }


                // merge prod col
                if (prodID.Equals(_preProdID) == false && this.SelectViewMode == ProductGantt.ViewMode.PRODUCT)
                {
                    MergeRows(_startSameRowKeyIdxFirst, rowIndex - 1, 0);

                    Color tmp = _preColor;
                    _preColor = _currColor;
                    _currColor = tmp;
                    _startSameRowKeyIdxFirst = rowIndex;

                    _preEqpID = eqpID;
                    _preStepID = stepID;
                    _preProdID = prodID;
                    _startSameEqpRowIdx = rowIndex;

                    if (_startSameEqpRowIdx > 1)
                    {
                        XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(rowIndex - 1, ColName.TotalRun), _subTotal);
                    }

                    _preEqpID = eqpID;
                    _preStepID = stepID;
                    _preProdID = prodID;
                    _startSameEqpRowIdx = rowIndex;
                    _subTotal = 0;
                    _subJobChg = 0;
                }

                PaintRowKeyedCell(rowIndex, _currColor);



                var prodIdCol = colHeader.GetCellInfo(rowIndex, ColName.ProductID);
                if (prodIdCol != null)
                {
                    Gantt.Worksheet[rowIndex, prodIdCol.ColumnIndex].SetCellText(prodID);

                    XtraSheetHelper.SetCellText(prodIdCol, prodID);
                    prodIdCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    prodIdCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                }

                var eqpGroupCol = colHeader.GetCellInfo(rowIndex, ColName.EqpGroup);
                if (eqpGroupCol != null)
                {
                    Gantt.Worksheet[rowIndex, eqpGroupCol.ColumnIndex].SetCellText(eqpGroup);

                    XtraSheetHelper.SetCellText(eqpGroupCol, eqpGroup);
                    eqpGroupCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    eqpGroupCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                }

                var eqpModelCol = colHeader.GetCellInfo(rowIndex, ColName.EqpModel);
                if (eqpModelCol != null)
                {
                    Gantt.Worksheet[rowIndex, eqpModelCol.ColumnIndex].SetCellText(eqpModel);

                    XtraSheetHelper.SetCellText(eqpModelCol, eqpModel);
                    eqpModelCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    eqpModelCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                }

                var stepIdCol = colHeader.GetCellInfo(rowIndex, ColName.StepId);
                if (stepIdCol != null)
                {
                    Gantt.Worksheet[rowIndex, stepIdCol.ColumnIndex].SetCellText(stepID);

                    XtraSheetHelper.SetCellText(stepIdCol, stepID);
                    stepIdCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    stepIdCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                }

                XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.EqpId), eqpID);
                colHeader.GetCellInfo(rowIndex, ColName.EqpId).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                colHeader.GetCellInfo(rowIndex, ColName.EqpId).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }
        }

        private void MergeRows(int fromRowIdx, int toRowIdx, int colIdx = 0)
        {
            var worksheet = this.Gantt.Worksheet;

            worksheet.MergeRowsOneColumn(fromRowIdx, toRowIdx, colIdx);

            if (colIdx > 1)
                SetBorder(fromRowIdx, toRowIdx);
        }

        private void SetBorder(int fromRowIdx, int toRowIdx)
        {
            var worksheet = this.Gantt.Worksheet;
            var color = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            
            for (int i = fromRowIdx; i <= toRowIdx; i++)
            {
                if (i == fromRowIdx)
                {
                    XtraSheetHelper.SetRowBorderTopLine(worksheet, i, color, BorderLineStyle.Thin);
                }
                else
                {
                    XtraSheetHelper.SetRowBorderTopLine(worksheet, i, Color.Transparent, BorderLineStyle.Thin);
                }

                XtraSheetHelper.SetRowBorderBottomLine(worksheet, i, Color.Transparent);
            }
        }

        private void PaintRowKeyedCell(int rowIndex, Color color)
        {
            var worksheet = this.Gantt.Worksheet;

            for (int colindex = 0; colindex < 3; colindex++)
            {
                worksheet[rowIndex, colindex].FillColor = color;
            }
        }

        private void PaintTotColumnCell()
        {
            var worksheet = Gantt.Worksheet;
            var colHeader = Gantt.ColumnHeader;
            worksheet.SetUsedColumnFillColor(colHeader.TryGetColumnIndex(ColName.SetupCount), Color.FromArgb(248, 223, 224));
            worksheet.SetUsedColumnFillColor(colHeader.TryGetColumnIndex(ColName.LoadRate), Color.FromArgb(254, 240, 222));
            worksheet.SetUsedColumnFillColor(colHeader.TryGetColumnIndex(ColName.RunQtySum), Color.FromArgb(219, 236, 216));
            worksheet.SetUsedColumnFillColor(colHeader.TryGetColumnIndex(ColName.TotalRun), Color.FromArgb(204, 255, 195));

            var usedRange = worksheet.GetUsedRange();
            Formatting rangeFormatting = usedRange.BeginUpdateFormatting();
            rangeFormatting.Font.Color = Color.Black;
            usedRange.EndUpdateFormatting(rangeFormatting);
        }

        #endregion

        #region SET COLUMN

        struct ColName
        {
            public static string EqpModel = "EQP_MODEL";
            public static string EqpGroup = "EQP_GROUP";
            public static string EqpId = "EQP_ID";
            public static string SubEqpId = "SUB_EQP_ID";
            public static string State = "STATE";
            public static string LotID = "LOT_ID";
            public static string ProductID = "PRODUCT_ID";
            public static string ProductVersion = "PRODUCT_VERSION";
            public static string DISPATCH_IN_TIME = "DISPATCH_IN_TIME";
            public static string StartTime = "START_TIME";
            public static string EndTime = "END_TIME";
            public static string GapTime = "GAP_TIME";
            public static string NextTkinTime = "NEXT_TKIN_TIME";
            public static string TimeSec = "TIME(sec)";
            public static string EqpRecipe = "EQP_RECIPE";
            public static string LotPriority = "LOT_PRIORITY";
            public static string WipInitRun = "WIP_INIT_RUN";
            public static string LPST = "LPST";
            public static string Tardiness = "TARDINESS";

            public static string StepId = "STEP_ID";

            public static string TIQty = "IN_QTY";
            public static string TOQty = "OUT_QTY";

            public static string LoadRate = "LOAD";
            public static string ChangRage = "CHANGE";
            public static string SetupCount = "SETUP";

            public static string TIQtySum = "T/I\nQTY";
            public static string RunQtySum = "RUN QTY";
            public static string TITotal = "T/I\nTOTAL";
            public static string TotalRun = "TOTAL";
        }

        struct STATE
        {
            public static string Pm = "PM";
            public static string Setup = "SETUP";
            public static string Busy = "BUSY";
            public static string Idle = "IDLE";
            public static string idleRun = "IDLERUN";
            public static string Down = "DOWN";
        }

        protected void SetColumnHeaders()
        {
            var gantt = this.Gantt;

            gantt.FixedColCount = 3;
            gantt.FixedRowCount = 2;

            int colCount = this.TargetAddDays * (int)ShopCalendar.ShiftHours + 5 + 2;

            var productCol = new XtraSheetHelper.SfColumn(ColName.ProductID, ColName.ProductID, 105);
            var eqpModelCol = new XtraSheetHelper.SfColumn(ColName.EqpModel, ColName.EqpModel, 105);
            var eqpGroupCol = new XtraSheetHelper.SfColumn(ColName.EqpGroup, ColName.EqpGroup, 105);
            var eqpCol = new XtraSheetHelper.SfColumn(ColName.EqpId, ColName.EqpId, 110);
            var stepCol = new XtraSheetHelper.SfColumn(ColName.StepId, ColName.StepId, 105);

            if (this.SelectViewMode == ProductGantt.ViewMode.PRODUCT)
                gantt.SetColumnHeaders(colCount, productCol, stepCol, eqpCol);
            else if (this.SelectViewMode == ProductGantt.ViewMode.EQPGROUP)
                gantt.SetColumnHeaders(colCount, eqpGroupCol, eqpCol);
            else if (this.SelectViewMode == ProductGantt.ViewMode.EQP)
                gantt.SetColumnHeaders(colCount, eqpCol, stepCol);
            else if (this.SelectViewMode == ProductGantt.ViewMode.STEP)
                gantt.SetColumnHeaders(colCount, stepCol, eqpCol);
            else if (this.SelectViewMode == ProductGantt.ViewMode.EQPMODEL)
                gantt.SetColumnHeaders(colCount, eqpModelCol, eqpCol);

            gantt.Worksheet.Rows[0].Style = Gantt.Workbook.Styles["CustomHeader"];
            gantt.Worksheet.Rows[1].Style = Gantt.Workbook.Styles["CustomHeader"];

            gantt.Worksheet.SelectedCell[0, 0].Style = Gantt.Workbook.Styles["CustomHeaderCenter"];
            gantt.Worksheet.SelectedCell[0, 1].Style = Gantt.Workbook.Styles["CustomHeaderCenter"];
        }

        #endregion 

        #region EVENT

        private void detailGridView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0)
                return;

            if (this.Gantt == null)
                return;

            GanttBar bar = this.Gantt.SelectedBar as GanttBar;
            if (bar == null)
                return;

            if (bar.IsGhostBar)
                return;

            var text = detailGridView.GetRowCellDisplayText(e.RowHandle, ColName.StartTime);
            if (string.IsNullOrEmpty(text))
                return;

            if (text == MyHelper.DATE.Format(bar.StartTime))
            {
                e.Appearance.BackColor = Color.LightCoral;

                if (this.IsNeedSetFocus)
                {
                    detailGridView.FocusedRowHandle = e.RowHandle;
                    this.IsNeedSetFocus = false;
                }
            }
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.isQuerying = true;
            detailGridControl.DataSource = null;

            SetTreeView();
            Search();
            ViewDispDetail();

            this.IsBeforeFirstQuery = false;
            this.isQuerying = false;
        }

        private void editVersionNo_EditValueChanged(object sender, EventArgs e)
        {
            //this.SetEqpGroup();
            this.editDateSpin.EditValue = 1 * ShopCalendar.ShiftCount;
            this.editDateTime.EditValue = this.TargetStartDate;

            SetControls_DispDetail();
        }

        private void buttonExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Gantt.ExportToExcel(this.Tag + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        private void editCellWidth_EditValueChanged(object sender, EventArgs e)
        {
            this.SizeManager.CellWidth = this.CellWidth;
        }

        private void editCellHeight_EditValueChanged(object sender, EventArgs e)
        {
            this.SizeManager.CellHeight = this.CellHeight;
        }

        private void buttonSaveSize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.SaveLocalSetting(this.ServiceProvider, this.ViewName + "ganttCellWidth", this.CellWidth.ToString());
            this.SaveLocalSetting(this.ServiceProvider, this.ViewName + "ganttCellHeight", this.CellHeight.ToString());
        }


        #endregion

        private void treeProduct_SelectionChanged(object sender, EventArgs e)
        {
            if (isQuerying || this.treeProduct.Selection == null || this.treeProduct.Selection.Count < 1)
                return;

            List<string> filterStringSet = new List<string>();

            foreach (var node in this.treeProduct.Selection)
            {
                List<TreeListNode> childNodes = new List<TreeListNode>();
                GetChildNodes(node, childNodes, true);

                filterStringSet.Add(string.Join(",", childNodes.Select(item => $"{item.GetDisplayText("PRODUCT")}").ToArray()));
            }

            string filterString = string.Join(",", filterStringSet);

            //this.ProductList.Clear();
            this.ProductList = filterString.Split(',').Distinct().ToList();
            Query();

            //this.grid1.ActiveFilterString = $"[field_PRODUCT_ID] In ({filterString})";

        }

        private void checkShowBarTitle_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.grid1.Refresh();
        }

        private void checkTargetComp_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Gantt.IsCheckTargetComp = checkTargetComp.Checked;
        }

        private void detailGridView_DoubleClick(object sender, EventArgs e)
        {
            DispView.OpenDispatchInfoPopup();
        }

        private void editProdId_EditValueChanged(object sender, EventArgs e)
        {
            SetEditDemandId();
        }
    }
}
