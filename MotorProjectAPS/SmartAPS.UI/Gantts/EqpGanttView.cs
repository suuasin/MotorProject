using DevExpress.Spreadsheet;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserInterface;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using Mozart.Studio.UIComponents;
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
    public partial class EqpGanttView : MyXtraGridTemplate
    {
        public class QueryOptionKey
        {
            public const string TargetVersionNo = "VersionNo";
            public const string TargetLineID = "LineID";
            public const string TargetEqpId = "EqpId";
            public const string TargetEqpGroups = "EqpGroup";
            public const string TargetStartTime = "StartDate";
            public const string TargetAddDate = "AddDate";            
        }

        #region PROPERTY

        private EqpMaster EqpMgr { get; set; }

        private EqpGantt Gantt { get; set; }

        private List<EqpGantt.GanttInfo> CurrInfos { get; set; }

        private XtraSheetSizeHelper SizeManager { get; set; }

        private bool IsBeforeFirstQuery { get; set; }

        private DetailBarInfoView DispView { get; set; }

        //private ArrangeAnalysisView ArrangeView { get; set; }

        private bool IsNeedSetFocus { get; set; }

        private bool ShowLayerBar
        {
            get
            {
                return true;
            }
        }

        private bool IsFilterDownEqp
        {
            get 
            {
                return this.checkDownEqp.Checked == false;
            }
        }

        private bool IsShowProductColor
        {
            get
            {
                return checkProdColor.Checked;
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

        private EqpGantt.ViewMode SelectViewMode
        {
            get
            {
                var item = this.radioRowHeader.EditValue as string;
                string str = item as string;

                EqpGantt.ViewMode mode;
                if (Enum.TryParse(str, out mode))
                    return mode;

                return EqpGantt.ViewMode.EQPGROUP;
            }
        }
        private bool IsCheckShowBarTitle
        {
            get
            {
                return checkShowBarTitle.Checked;
            }
        }

        private bool IsEqpViewMode
        {
            get { return this.SelectViewMode == EqpGantt.ViewMode.EQP; }
        }

        private string TargetLineID
        {
            get
            {
                return this.editLineId.EditValue as string;
            }
        }

        private List<string> SelectedEqpIds
        {
            get
            {
                string targetEqpId = this.TargetEqpId;
                if (string.IsNullOrEmpty(targetEqpId))
                    return null;

                return targetEqpId.Split(',').Select(item => item.Trim()).ToList<string>();
            }
        }

        string TargetEqpId
        {
            get
            {
                return this.editEqpId.EditValue as string;
            }
        }

        private List<string> SelectedEqpGroups
        {
            get
            {
                string targetEqpGrp = this.TargetEqpGroup;
                if (string.IsNullOrEmpty(targetEqpGrp))
                    return null;

                return targetEqpGrp.Split(',').Select(item => item.Trim()).ToList<string>();
            }
        }

        private string TargetEqpGroup
        {
            get
            {
                return this.editEqpGroup.EditValue as string;
            }
		}

        private DateTime TargetStartTime
        {
            get
            {
                return Convert.ToDateTime(this.editDateTime.EditValue);
            }
        }

        private DateTime TargetEndTime
        {
            get
            {
                return this.TargetStartTime.AddHours(ShopCalendar.ShiftHours * this.TargetAddDays);
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
                return this.editProdId.EditValue as string;
            }
        }

        public MouseSelectType SelectedMouseSelectType
        {
            get
            {
                MouseSelectType type;
                string sele = this.radioRightBtn.EditValue as string;
                Enum.TryParse(sele, out type);

                return type;
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

        public EqpGanttView()
        {
            InitializeComponent();
        }

        public EqpGanttView(IServiceProvider serviceProvider)
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
            SetEqpGroup();
            SetEqpId();

            BindEvents();
        }

        private void SetGantt()
        {
            InitGantt();

            this.EqpMgr = new EqpMaster(this.Result);
            this.EqpMgr.LoadEqp();

            this.Gantt.Reset(this.TargetStartTime, this.EqpMgr);
        }

		private void SetEqpGroup()
		{
			var eqpGrpEdit = this.editEqpGroup.Edit as RepositoryItemCheckedComboBoxEdit;
            if (eqpGrpEdit == null)
                return;

            if (Gantt != null)
				this.Gantt.BindChkListEqpGroup(eqpGrpEdit, this.TargetLineID);

            eqpGrpEdit.TextEditStyle = TextEditStyles.Standard;
            eqpGrpEdit.Items.ToList<CheckedListBoxItem>().ForEach(item => item.CheckState = CheckState.Checked);
			this.editEqpGroup.EditValue = eqpGrpEdit.GetCheckedItems();
        }

        private void SetEqpId()
        {
            var eqpIdEdit = this.editEqpId.Edit as RepositoryItemCheckedComboBoxEdit;
            if (eqpIdEdit == null)
                return;

            if (Gantt != null)
                this.Gantt.BindChkListEqpId(eqpIdEdit, this.SelectedEqpGroups);

            eqpIdEdit.TextEditStyle = TextEditStyles.Standard;
            eqpIdEdit.Items.ToList<CheckedListBoxItem>().ForEach(item => item.CheckState = CheckState.Checked);
            this.editEqpId.EditValue = eqpIdEdit.GetCheckedItems();
        }

        private void InitGantt()
        {
            if (this.Gantt != null)
                return;

            this.Gantt = new EqpGantt(this.grid1, this.Result);

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

            this.editDateSpin.EditValue = MyHelper.DATASVC.GetPlanPeriod(this.Result);
            this.editDateTime.EditValue = MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);

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
            this.DispView = new DetailBarInfoView(this.Result, presetList, this.TargetStartTime, this.TargetAddDays);
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
      
            if (bar.State == EqpState.DOWN)
                foreColor = Color.White;
            else if (bar.IsGhostBar)
                foreColor = Color.Gray;

            args.ForeColor = foreColor;
            if (IsCheckShowBarTitle)
                args.Text = bar.GetTitle(this.Gantt.IsProductInBarTitle);
            args.DrawDefault = true;

            if (bar.State == EqpState.NONE || bar.State == EqpState.PM)
            {
                //args.Background = new BrushInfo(System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal, Color.Black, Color.White);
                args.Background = new BrushInfo(Color.Gray);
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
			int layColidx = SelectViewMode == EqpGantt.ViewMode.LAYER ? eqpColIdx - 1 : eqpColIdx + 1;
			int runQtyColIdx = e.Cell.CurrentRegion.ColumnCount - 2;
			int loadColIdx = e.Cell.CurrentRegion.ColumnCount - 3;

			string eqpId = Convert.ToString(gantt.Worksheet[cellRowIdx, eqpColIdx].Value);
			string layerId = Convert.ToString(gantt.Worksheet[cellRowIdx, layColidx].Value);

			if (SelectViewMode == EqpGantt.ViewMode.LAYER && string.IsNullOrEmpty(layerId) && gantt.LastRowIndex != cellRowIdx)
			{
				while (string.IsNullOrEmpty(layerId))
				{
					cellRowIdx -= 1;
					layerId = Convert.ToString(gantt.Worksheet[cellRowIdx, layColidx].Value);
				}
			}
			layerId = IsEqpViewMode == true || SelectViewMode == EqpGantt.ViewMode.LAYER ? layerId : null;


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
				DataTable db = Load_QtyView(eqpId, layerId, columnName);
				Load_QtyPopUp(db, eqpId, layerId);
			}

			if (cellColIdx == loadColIdx && cellText != ColName.LoadRate)
			{
				string columnName = ColName.LoadRate;
				DataTable db = Load_QtyView(eqpId, layerId, columnName);
				Load_QtyPopUp(db, eqpId, layerId);
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
                    Gantt.SelectedBar = new Bar(TargetStartTime, TargetStartTime.AddHours(1), 20, 20, EqpState.BUSY);
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

            List<EqpGantt.GanttInfo> list = GetGanttInfo(eqpID);
            if (list != null)
            {
                foreach (EqpGantt.GanttInfo info in list)
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
                                //drow[ColName.ProductVersion] = b.ProductVersion;
                                drow[ColName.StepId] = b.StepID;

                                //drow[ColName.LotPriority] = b.LotPriority;
                                //drow[ColName.EqpRecipe] = b.EqpRecipe;
                                //drow[ColName.WipInitRun] = b.WipInitRun;

                                if (b.State == EqpState.BUSY)
                                    drow[ColName.TIQty] = b.TIQty;
                            }

                            drow[ColName.DISPATCH_IN_TIME] = MyHelper.DATE.Format(b.DispatchInTime);
                            drow[ColName.StartTime] = MyHelper.DATE.Format(b.TkinTime);
                            drow[ColName.EndTime] = MyHelper.DATE.Format(b.TkoutTime);
                            drow[ColName.GapTime] = b.TkoutTime - b.TkinTime;

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

        private DataTable Load_QtyView(string targetId, string layerId, string columnName)
        {
            string[] idArr = SplitEqpID(targetId);
            string targetColumn = columnName;

            string eqpID = idArr[0];
            string subEqpID = idArr[1];
            string layerID = layerId;

            subEqpID = eqpID == subEqpID? null : subEqpID;
  
            DataTable qtyDt = CreateProductDataTable();
            List<EqpGantt.GanttInfo> list = GetGanttInfo(eqpID, layerID);

            Dictionary<string, int> qtyDic = new Dictionary<string, int>();
            Dictionary<string,double> timeDic = new Dictionary<string, double>();
            string[] stateArr = new string[] { STATE.Pm, STATE.Setup, STATE.Busy, STATE.Idle, STATE.idleRun , STATE.Down };
            
            foreach(string eqpState in stateArr)
            {
                timeDic.Add(eqpState, 0.0);
            }

            int allSum = 0;
            double C_RowSumTime = 0.0;
            double allTime = Math.Round(((this.TargetEndTime - this.TargetStartTime).TotalSeconds), 2);
            if (list != null)
            {
                foreach (EqpGantt.GanttInfo info in list)
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
            double rowLoad = Math.Round((C_RowSumTime / allTime * 100),1);

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
                row[ColName.LoadRate] = string.Format("{0}%", rowLoad) ;
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

            dt.Columns.Add(ColName.State,typeof(string));
            dt.Columns.Add(ColName.TimeSec,typeof(double));
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
            //dt.Columns.Add(ColName.SubEqpId, typeof(string));
            dt.Columns.Add(ColName.State, typeof(string));
            dt.Columns.Add(ColName.LotID, typeof(string));
            dt.Columns.Add(ColName.StepId, typeof(string));

            dt.Columns.Add(ColName.ProductID, typeof(string));
            dt.Columns[ColName.ProductID].Caption = "PRODUCT_ID";
            //dt.Columns.Add(ColName.ProductVersion, typeof(string));
            //dt.Columns[ColName.ProductVersion].Caption = "PRODUCT_VERSION";
            dt.Columns.Add(ColName.DISPATCH_IN_TIME, typeof(string));
            dt.Columns[ColName.DISPATCH_IN_TIME].Caption = "DISPATCH_IN_TIME";
            dt.Columns.Add(ColName.StartTime, typeof(string));
            dt.Columns[ColName.StartTime].Caption = "START_TIME";
            dt.Columns.Add(ColName.EndTime, typeof(string));
            dt.Columns[ColName.EndTime].Caption = "END_TIME";
            dt.Columns.Add(ColName.TIQty, typeof(int));
            dt.Columns[ColName.TIQty].Caption = "IN_QTY";
            dt.Columns.Add(ColName.GapTime, typeof(string));
            dt.Columns[ColName.GapTime].Caption = "PROCESSED_TIME";
            
            //dt.Columns.Add(ColName.LotPriority, typeof(int));
            //dt.Columns.Add(ColName.EqpRecipe, typeof(string));
            //dt.Columns.Add(ColName.WipInitRun, typeof(string));

            return dt;
        }

        private List<EqpGantt.GanttInfo> GetGanttInfo(string eqpID, string stepID = null)
        {
            List<EqpGantt.GanttInfo> result = new List<EqpGantt.GanttInfo>();

            if (string.IsNullOrEmpty(eqpID))
                return result;

            var list = this.CurrInfos;
            foreach (EqpGantt.GanttInfo info in list)
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

        //private void radioMainDataTable_SelectedIndexChanged_1(object sender, EventArgs e)
        //{
        //    this.barTitleGroup.Properties.Items[1].Enabled = true;

        //    if (dateEdit1.DateTime < PlanStartTime.AddDays(-3))
        //        dateEdit1.DateTime = PlanStartTime.AddDays(-3);
        //}

        //private void highLightOptionChkEdit_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.IsCheckedHighLightOption)
        //    {
        //        this.rdoMouseType.Enabled = true;
        //        this.tePattern.Enabled = true;
        //        this.prodPatternApplyBtn.Enabled = true;

        //        _gantt.TurnOnSelectMode();
        //        rdoMouseType.Enabled = true;
        //    }
        //    else
        //    {
        //        _gantt.TurnOffSelectMode();
        //        this.rdoMouseType.Enabled = false;
        //        this.tePattern.Enabled = false;
        //        this.prodPatternApplyBtn.Enabled = false;

        //        this.grid1.BeginUpdate();
        //        this.grid1.EndUpdate();
        //        this.grid1.Refresh();
        //        // 기능 안먹도록
        //    }
        //}

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

            SetBarTitleGroupProperties();
        }

        private void editLineId_EditValueChanged(object sender, EventArgs e)
        {
            if (this.radioBarTitle.Edit != null)
                (this.radioBarTitle.Edit as RepositoryItemRadioGroup).Items[1].Enabled = true;

            SetEqpGroup();
        }

        private void SetBarTitleGroupProperties()
        {
            if (this.radioBarTitle.Edit != null)
            {
                if (this.radioBarTitle.EditValue as string == MouseSelectType.Product.ToString())
                {
                    if (this.radioRightBtn.EditValue as string == MouseSelectType.PB.ToString())
                        this.radioRightBtn.EditValue = MouseSelectType.Product.ToString();

                    if (this.radioRightBtn.Edit != null)
                        (this.radioRightBtn.Edit as RepositoryItemRadioGroup).Items.GetItemByValue(MouseSelectType.PB.ToString()).Enabled = true;
                }
            }
            else
                if (this.radioRightBtn.Edit != null)
                    (this.radioRightBtn.Edit as RepositoryItemRadioGroup).Items.GetItemByValue(MouseSelectType.PB.ToString()).Enabled = true;
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
            if (e.KeyCode == Keys.Enter) {
                if (this.SelectedMouseSelectType != MouseSelectType.Pattern){
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

            Gantt.SelectedBar = new Bar(TargetStartTime, TargetStartTime.AddHours(1), 20, 20, EqpState.BUSY);

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
                double rate = _rowsumLoadTimeFrBar / (this.TargetEndTime - this.TargetStartTime).TotalSeconds * 100.0;
                sLoadRate = Math.Round(rate, 1).ToString() + "%";

                XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(_curRowIdx, ColName.LoadRate), sLoadRate);
                colHeader.GetCellInfo(_curRowIdx, ColName.LoadRate).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                string subJobChangeCnt = string.Format("{0:n0}", _subJobChg);
                XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(_curRowIdx, ColName.SetupCount), subJobChangeCnt);
                colHeader.GetCellInfo(_curRowIdx, ColName.SetupCount).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }

            //Gantt._splitCnt[

            string subTotalRun = string.Format("{0:n0}", _subTotal);
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(_curRowIdx, ColName.TotalRun), subTotalRun);
            colHeader.GetCellInfo(_curRowIdx, ColName.TotalRun).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            string jobChgColName = string.Format("{0} \n({1})", ColName.SetupCount, _totalJobChg.ToString("#.#"));
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(0, ColName.SetupCount), jobChgColName);
            colHeader.GetCellInfo(0, ColName.SetupCount).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;

            string loadRateColName = string.Empty;
            double avgLoadTIme = _totalLoadTImeFrBarDic.Count <= 0 ? 0 : _totalLoadTImeFrBarDic.Values.Average();
            double totalRateForSim = _totalLoadTImeFrBarDic.Count <= 0 ? 0
                    : avgLoadTIme / (this.TargetEndTime - this.TargetStartTime).TotalSeconds * 100.0;
            loadRateColName = string.Format("{0} \n({1}%)", ColName.LoadRate, totalRateForSim.ToString("#.#"));
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(0, ColName.LoadRate), loadRateColName);

            string totalString = string.Format("{0:n0}", _total);
            string totalColName = string.Format("{0} \n({1})", ColName.TotalRun, totalString);
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(0, ColName.TotalRun), totalColName);
            colHeader.GetCellInfo(0, ColName.TotalRun).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;

            int fromRowIdx = this.IsEqpViewMode ? _startSameEqpRowIdx : _startSameRowKeyIdx;
            MergeRows(fromRowIdx, Gantt.LastRowIndex);

            var eqpGroupCol = colHeader.GetCellInfo(0, ColName.EqpGroup);
            if (eqpGroupCol != null)
            {
                eqpGroupCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                eqpGroupCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }

            var layerCol = colHeader.GetCellInfo(0, ColName.StepId);
            if (layerCol != null)
            {
                layerCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                layerCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
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
                .Where(x => x.TkinTime < this.TargetEndTime)
                .Sum(x => {
                    DateTime startTime = x.TkinTime;
                    DateTime endTime = x.TkoutTime;

                    if (endTime > this.TargetEndTime)
                        endTime = this.TargetEndTime;

                    return (endTime - startTime).TotalSeconds;
                });

            _rowsumLoadTimeFrBar += loadTime;
        }

        void GanttView_BindRowAdding(object sender, GanttRowEventArgs args)
        {
            var worksheet = this.Gantt.Worksheet;

            var info = args.Item as EqpGantt.GanttInfo;
            var colHeader = Gantt.ColumnHeader;

            _curRowIdx = args.RowIndex;

            string eqpModel = info.EqpModel;
            string eqpGroup = info.EqpGroup;
            string eqpID = info.EqpID;
            string layer = info.StepID;

            SetRowHeaderValue(args.RowIndex, eqpModel, eqpGroup, eqpID, layer);

            this._rowsum = 0;
            this._rowsumJobChg = 0;
            this._rowsumLoadTimeFrBar = 0;

            if (args.Node == null)
                return;

            var rows = args.Node.LinkedBarList;

            if (rows.Count > 1 && args.Index > 0 && args.Index < rows.Count - 1)
            {
                var eqpGroupCol = colHeader.GetCellInfo(args.RowIndex, ColName.EqpGroup);
                if (eqpGroupCol != null)
                    XtraSheetHelper.SetCellText(eqpGroupCol, eqpGroup);

                var layerCol = colHeader.GetCellInfo(args.RowIndex, ColName.StepId);
                if (layerCol != null)
                    XtraSheetHelper.SetCellText(eqpGroupCol, layer);

                var eqpModelCol = colHeader.GetCellInfo(args.RowIndex, ColName.EqpModel);
                if (eqpModelCol != null)
                    XtraSheetHelper.SetCellText(eqpGroupCol, eqpModelCol);

                XtraSheetHelper.SetCellText(colHeader.GetCellInfo(args.RowIndex, ColName.EqpId), eqpID);

                PaintRowKeyedCell(args.RowIndex, _currColor);
            }
        }
  
        void GanttView_BindRowAdded(object sender, GanttRowEventArgs args)
        {
            var info = args.Item as EqpGantt.GanttInfo;
            var colHeader = Gantt.ColumnHeader;

            if (_totalLoadTImeFrBarDic.ContainsKey(info.EqpID) == false)
                _totalLoadTImeFrBarDic.Add(info.EqpID, _rowsumLoadTimeFrBar);

            if (this.IsEqpViewMode == false)
            {
                double rate = _rowsumLoadTimeFrBar / (this.TargetEndTime - this.TargetStartTime).TotalSeconds * 100.0;
                string loadRate = Math.Round(rate, 1).ToString() + "%";

                XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(args.RowIndex, ColName.LoadRate), loadRate);
                XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(args.RowIndex, ColName.SetupCount), _rowsumJobChg);


                colHeader.GetCellInfo(args.RowIndex, ColName.LoadRate).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }

            float dupQty = 0;
            if (Gantt._duplicatedQty.ContainsKey(info.EqpID))
                dupQty = Gantt._duplicatedQty[info.EqpID];

            XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(args.RowIndex, ColName.RunQtySum), _rowsum - dupQty);

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

            DateTime hstart = MyHelper.DATE.Trim(this.TargetStartTime, "HH");
            DateTime hend = MyHelper.DATE.Trim(this.TargetEndTime, "HH");
            if (hend == MyHelper.DATE.Trim(this.TargetEndTime, "mm"))
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
			Gantt.GanttType = GanttType.EqpGantt;

            SetGantt();

            //SetControls_DispDetail();

            this.CurrInfos = GenerateGantt_Sim();

            //this.ArrangeView = new ArrangeAnalysisView(this.TargetVersionNo);
            //this.ArrangeView.ImportData();

            BindGrid(this.CurrInfos);
        }

        private List<EqpGantt.GanttInfo> GenerateGantt_Sim()
        {
            var gantt = Gantt;

            gantt.PrepareData(this.IsProductInBarTitle, this.IsShowProductColor);

            gantt.BuildData_Sim(this.TargetLineID,
                                this.SelectedEqpGroups,
                                this.SelectedEqpIds,
                                this.TargetStartTime,
                                this.TargetEndTime,
                                this.SelectViewMode,
                                this.IsFilterDownEqp);

            return gantt.Expand(this.ShowLayerBar, this.SelectViewMode);
        }

        #endregion

        #region BIND GRID

        bool _isFirst = true;
        string _preLayer = string.Empty;
        string _preEqpID = string.Empty;
        string _preEqpGroup = string.Empty;
        string _preEqpModel = string.Empty;
        string _preRowKey = string.Empty;

        Color _preColor = XtraSheetHelper.AltColor;
        Color _currColor = XtraSheetHelper.AltColor2;

        int _startSameEqpRowIdx = 0;
        int _startSameRowKeyIdx = 0;
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
            _startSameRowKeyIdx = 0;

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

        private void BindGrid(List<EqpGantt.GanttInfo> currInfos)
        {
            Clear();

            Gantt.Workbook.BeginUpdate();
            Gantt.ResetWorksheet();

            this.Gantt.TurnOffSelectMode();

            SetColumnHeaders();

            this.Gantt.SchedBarComparer = new GanttMaster.CompareMBarList();

            if (currInfos != null && currInfos.Count > 1)
            {

                EqpGantt.SortOptions[] options = null;
                if (this.SelectViewMode == EqpGantt.ViewMode.EQPGROUP)
                {
                    options = new EqpGantt.SortOptions[]{EqpGantt.SortOptions.EQP_GROUP,
                                                         EqpGantt.SortOptions.EQP};
                }
                else if (this.SelectViewMode == EqpGantt.ViewMode.EQP)
                {
                    options = new EqpGantt.SortOptions[] { EqpGantt.SortOptions.EQP,
                                                           EqpGantt.SortOptions.STEP};
                }
                else if (this.SelectViewMode == EqpGantt.ViewMode.LAYER)
                {
                    options = new EqpGantt.SortOptions[] { EqpGantt.SortOptions.STEP,
                                                           EqpGantt.SortOptions.EQP };
                }
                else if (this.SelectViewMode == EqpGantt.ViewMode.EQPMODEL)
                {
                    options = new EqpGantt.SortOptions[] { EqpGantt.SortOptions.EQP_MODEL,
                                                           EqpGantt.SortOptions.EQP };
                }

                currInfos.Sort(new EqpGantt.CompareGanttInfo(Gantt.GanttType,
                                                             this.EqpMgr,
                                                             this.TargetLineID,
                                                             options));
            }

			this.Gantt.Bind(currInfos);

            this.Gantt.Workbook.EndUpdate();
        }

        private void SetRowHeaderValue(int rowIndex, string eqpModel, string eqpGroup, string eqpID, string layer)
        {
            if (this.IsEqpViewMode == false)
            {
                var colHeader = Gantt.ColumnHeader;

                if (_isFirst)
                {
                    _preLayer = layer;
                    _preEqpID = eqpID;
                    _preEqpModel = eqpModel;
                    _preEqpGroup = eqpGroup;
                    _startSameEqpRowIdx = rowIndex;
                    _startSameRowKeyIdx = rowIndex;

                    _isFirst = false;
                }

                if (eqpID.Equals(_preEqpID) == false)
                {
                    _preEqpID = eqpID;
                    _startSameEqpRowIdx = rowIndex;
                }

                if (eqpGroup.Equals(_preEqpGroup) == false && this.SelectViewMode == EqpGantt.ViewMode.EQPGROUP)
                {
                    MergeRows(_startSameRowKeyIdx, rowIndex - 1);

                    Color tmp = _preColor;
                    _preColor = _currColor;
                    _currColor = tmp;
                    _startSameRowKeyIdx = rowIndex;

                    _preEqpID = eqpID;
                    _preEqpGroup = eqpGroup;
                    _startSameEqpRowIdx = rowIndex;

                    if (_startSameEqpRowIdx > 1)
                    {
                        XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(rowIndex - 1, ColName.TotalRun), _subTotal);
                    }

                    _preEqpGroup = eqpGroup;
                    _preEqpID = eqpID;
                    _startSameEqpRowIdx = rowIndex;
                    _subTotal = 0;
                    _subJobChg = 0;
                }

                if (eqpModel.Equals(_preEqpModel) == false && this.SelectViewMode == EqpGantt.ViewMode.EQPMODEL)
                {
                    MergeRows(_startSameRowKeyIdx, rowIndex - 1);

                    Color tmp = _preColor;
                    _preColor = _currColor;
                    _currColor = tmp;
                    _startSameRowKeyIdx = rowIndex;

                    _preEqpID = eqpID;
                    _preEqpModel = eqpModel;
                    _startSameEqpRowIdx = rowIndex;

                    if (_startSameEqpRowIdx > 1)
                    {
                        XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(rowIndex - 1, ColName.TotalRun), _subTotal);
                    }

                    _preEqpModel = eqpModel;
                    _preEqpID = eqpID;
                    _startSameEqpRowIdx = rowIndex;
                    _subTotal = 0;
                    _subJobChg = 0;
                }

                if (layer.Equals(_preLayer) == false && this.SelectViewMode == EqpGantt.ViewMode.LAYER)
                {
                    MergeRows(_startSameRowKeyIdx, rowIndex - 1);

                    Color tmp = _preColor;
                    _preColor = _currColor;
                    _currColor = tmp;
                    _startSameRowKeyIdx = rowIndex;

                    if (_startSameEqpRowIdx > 1)
                    {
                        XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(rowIndex - 1, ColName.TotalRun), _subTotal);
                    }

                    _preLayer = layer;
                    _preEqpID = eqpID;
                    _startSameEqpRowIdx = rowIndex;
                    _subTotal = 0;
                    _subJobChg = 0;
                }

                PaintRowKeyedCell(rowIndex, _currColor);

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
                    Gantt.Worksheet[rowIndex, stepIdCol.ColumnIndex].SetCellText(layer);

                    XtraSheetHelper.SetCellText(stepIdCol, layer);
                    stepIdCol.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    stepIdCol.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                }

                XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.EqpId), eqpID);
                colHeader.GetCellInfo(rowIndex, ColName.EqpId).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                colHeader.GetCellInfo(rowIndex, ColName.EqpId).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }
            else
            {
                var colHeader = Gantt.ColumnHeader;

                if (_isFirst)
                {
                    _preLayer = layer;
                    _preEqpID = eqpID;
                    _startSameEqpRowIdx = rowIndex;
                    _startSameRowKeyIdx = rowIndex;

                    _isFirst = false;
                }

                if (layer.Equals(_preLayer) == false)
                {
                    _preLayer = layer;
                    _startSameRowKeyIdx = rowIndex;
                }

                if (eqpID.Equals(_preEqpID) == false)
                {
                    MergeRows(_startSameEqpRowIdx, rowIndex - 1);

                    if (_startSameEqpRowIdx > 1)
                    {
                        string sLoadRate = string.Empty;

                        double rate = _rowsumLoadTimeFrBar / (this.TargetEndTime - this.TargetStartTime).TotalSeconds * 100.0;
                        sLoadRate = Math.Round(rate, 1).ToString() + "%";

                        XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(rowIndex - 1, ColName.LoadRate), sLoadRate);
                        colHeader.GetCellInfo(rowIndex - 1, ColName.LoadRate).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                        XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(rowIndex - 1, ColName.TotalRun), _subTotal);
                        XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(rowIndex - 1, ColName.SetupCount), _subJobChg);
                    }

                    Color tmp = _preColor;
                    _preColor = _currColor;
                    _currColor = tmp;
                    _preEqpID = eqpID;

                    _preLayer = layer;
                    _startSameEqpRowIdx = rowIndex;
                    _subTotal = 0;
                    _subJobChg = 0;
                }

                PaintRowKeyedCell(rowIndex, _currColor);

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

                var stepId = colHeader.GetCellInfo(rowIndex, ColName.StepId);
                if (stepId != null)
                {
                    Gantt.Worksheet[rowIndex, stepId.ColumnIndex].SetCellText(layer);

                    XtraSheetHelper.SetCellText(stepId, layer);
                    stepId.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    stepId.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
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

            for (int colindex = 0; colindex < 2; colindex++)
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

            gantt.FixedColCount = 2;
            gantt.FixedRowCount = 2;

            int colCount = this.TargetAddDays * (int)ShopCalendar.ShiftHours + 5 + 2;

            var eqpModelCol = new XtraSheetHelper.SfColumn(ColName.EqpModel, ColName.EqpModel, 105);
            var eqpGroupCol = new XtraSheetHelper.SfColumn(ColName.EqpGroup, ColName.EqpGroup, 105);
            var eqpCol = new XtraSheetHelper.SfColumn(ColName.EqpId, ColName.EqpId, 110);
            var layerCol = new XtraSheetHelper.SfColumn(ColName.StepId, ColName.StepId, 105);

            if (this.SelectViewMode == EqpGantt.ViewMode.EQPGROUP)
                gantt.SetColumnHeaders(colCount, eqpGroupCol, eqpCol);
            else if (this.SelectViewMode == EqpGantt.ViewMode.EQP)
                gantt.SetColumnHeaders(colCount, eqpCol, layerCol);
            else if (this.SelectViewMode == EqpGantt.ViewMode.LAYER)
                gantt.SetColumnHeaders(colCount, layerCol, eqpCol);
            else if (this.SelectViewMode == EqpGantt.ViewMode.EQPMODEL)
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
            detailGridControl.DataSource = null;

            Search();

            SetBarTitleGroupProperties();
            ViewDispDetail();

            this.IsBeforeFirstQuery = false;
        }

        private void editVersionNo_EditValueChanged(object sender, EventArgs e)
        {
			this.SetEqpGroup();
			this.editDateSpin.EditValue = 1 * ShopCalendar.ShiftCount;
            this.editDateTime.EditValue = this.TargetStartTime;

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

        private void checkShowBarTitle_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.grid1.Refresh();
        }

        private void detailGridView_DoubleClick(object sender, EventArgs e)
        {
            DispView.OpenDispatchInfoPopup();
        }

        #endregion
    }
}
