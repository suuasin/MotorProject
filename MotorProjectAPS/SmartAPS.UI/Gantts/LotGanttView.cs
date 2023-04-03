using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.TaskModel.UserLibrary.GanttChart;
using DevExpress.Spreadsheet;
using SmartAPS.UI.Helper;
using DevExpress.DataProcessing;
using DevExpress.XtraEditors.Repository;
using SmartAPS.UI.Gantts;
using SmartAPS.UI.Utils;
using SmartAPS.UserLibrary.Extensions;

namespace SmartAPS.UI.LotGantts
{    public partial class LotGanttView  : MyXtraGridTemplate
    {
        private EqpMaster _eqpMgr;

        private LotGantt Gantt;

        HashSet<string> _prodIDList;
        HashSet<string> _stepIDList;      

        private LotBarDetailView DispView { get; set; }

        private XtraSheetSizeHelper SizeManager { get; set; }

        private DateTime PlanStartTime
        {
            get
            {
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
            }
        }

        private bool IsOnlyToolMode
        {
            get { return this.radioViewGroup.EditValue as string == "LINE"; }
        }

        private LotGantt.ViewMode SelectViewMode
        {
            get
            {
                var viewGroup = this.radioViewGroup.EditValue as string;

                LotGantt.ViewMode mode;
                if (Enum.TryParse(viewGroup, out mode))
                    return mode;

                return LotGantt.ViewMode.PROD;
            }
        }

        private string TargetLineID
        {
            get
            {
                return this.editLineId.EditValue as string;
            }
        }

        private string EqpIdPattern
        {
            get
            {
                //var eqpId = this.editEqpId.EditValue as string;
                //return string.IsNullOrEmpty(eqpId) ? "" : eqpId.ToUpper();
                return "";
            }
        }

        private HashSet<string> SelectedProdIDs
        {
            get
            {
				if (_prodIDList == null)
					_prodIDList = new HashSet<string>();
				else
					_prodIDList.Clear();

				_prodIDList = this.TargetCheckedProductIdList.ToHashSet();

				return _prodIDList;
            }
        }

        private List<string> TargetCheckedProductIdList
        {
            get
            {
                return (this.editProdId.Edit as RepositoryItemCheckedComboBoxEdit).GetCheckedItemsToList();
            }
        }

        private HashSet<string> SelectedStepIDs
        {
            get
            {
                if (_stepIDList == null)
                    _stepIDList = new HashSet<string>();
                else
                    _stepIDList.Clear();

                _stepIDList = this.TargetCheckedStepIdList.ToHashSet();

                return _stepIDList;
            }
        }

        private List<string> TargetCheckedStepIdList
        {
            get
            {
                return (this.editStepId.Edit as RepositoryItemCheckedComboBoxEdit).GetCheckedItemsToList();
            }
        }

        //private List<string> SelectedMaskIDs
        //{
        //    get
        //    {
        //        // 선택된 장비 그룹 등록
        //        if (_maskIDList == null)
        //            _maskIDList = new List<string>();
        //        else
        //            _maskIDList.Clear();

        //        _maskIDList = this.TargetCheckedMaskList.ToList();

        //        return _maskIDList;
        //    }
        //}

        //private List<string> TargetCheckedMaskList
        //{
        //    get
        //    {
        //        string key = QueryOptionKey.TargetMask;
        //        var infos = this.QueryOptions;

        //        if (infos != null && infos.ContainsKey(key))
        //            return this.QOControl.GetCheckedItemsToList(key);

        //        return (this.editMask.Edit as RepositoryItemCheckedComboBoxEdit).GetCheckedItemsToList();
        //    }
        //}

        private bool IsStepView
        {
            get { return false; }
        }

        private DateTime StartDate
        {
            get
            {
                return Convert.ToDateTime(this.editDateTime.EditValue);
            }
        }

        private DateTime EndDate
        {
            get
            {
                return StartDate.AddHours(ShopCalendar.ShiftHours * this.TargetAddDays);
            }
        }

        private int TargetAddDays
        {
            get
            {
                return Convert.ToInt32(this.editDateSpin.EditValue);
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

        public int CellWidthSize
        {
            get
            {
                return Convert.ToInt32(editCellWidth.EditValue);
			}
        }

        public int CellHeightSize
        {
            get
            {
                return Convert.ToInt32(editCellHeight.EditValue);
			}
        }

        private bool IsNeedSetFocus { get; set; }

        public LotGanttView()
        {
            InitializeComponent();
        }

        public LotGanttView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();

            this.SizeManager = new XtraSheetSizeHelper(this.CellWidthSize, this.CellHeightSize);
            this.SizeManager.Spreadsheet = this.grid1;
            this.SizeManager.OnChanged_CellSize = () =>
            {
                this.editCellWidth.EditValue = this.SizeManager.CellWidth;
                this.editCellHeight.EditValue = this.SizeManager.CellHeight;
            };
        }

        protected bool ExportExcel()
        {
            try
            {
                this.Gantt.ExportToExcel(this.Tag + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            SetInitializeOption(this.gridControl1);

            SetControls();

            InitializeData();

            BindEvents();

            RunQuery();                        
        }

        private void SetControls()
        {
            this.editDateSpin.EditValue = MyHelper.DATASVC.GetPlanPeriod(this.Result);
            this.editDateTime.EditValue = this.PlanStartTime;

            this.radioRightBtn.EditValue = "Product";
            this.radioViewGroup.EditValue = "PROD";
            
            MyHelper.ENGCONTROL.SetControl_LineID(this.editLineId, this.Result, true);
            MyHelper.ENGCONTROL.SetControl_ProductID_Checked(this.editProdId, this.Result);
            MyHelper.ENGCONTROL.SetControl_StepID_Checked(this.editStepId, this.Result);

			SetCellSize();

            InitControl_DispDetail();
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

        private void InitControl_DispDetail()
        {
            var view = new LotBarDetailView();

            dispatchingDetailpanel.Controls.Add(view);
            view.Dock = DockStyle.Fill;

            this.DispView = view;
        }

        private void InitializeData()
        {
            try {
                _eqpMgr = new EqpMaster(this.Result);
                _eqpMgr.LoadEqp();

                Gantt = new LotGantt(this.grid1, this.TargetLineID, this.PlanStartTime, _eqpMgr, this.Result);

                Gantt.DefaultColumnWidth = CellWidthSize;
                Gantt.DefaultRowHeight = CellHeightSize;

                Gantt.MouseSelType = MouseSelectType.Product;

                this.SetColumnHeaderView();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
			}
        }

        private void SetColumnHeaderView()
        {
            var styleBook = this.Gantt.Workbook.Styles;

            var customHeader = styleBook.Add("CustomHeader");
            var customHeaderCenter = styleBook.Add("CustomHeaderCenter");
            var customHeaderRight = styleBook.Add("CustomHeaderRight");
            var customDefaultCenter = styleBook.Add("CustomDefaultCenter");

            customHeader.Fill.BackgroundColor = Color.MintCream;

            customHeaderCenter.Fill.BackgroundColor = Color.MintCream;
            customHeaderCenter.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            customHeaderCenter.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            customHeaderRight.Fill.BackgroundColor = Color.MintCream;
            customHeaderRight.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            customHeaderRight.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Right;

            customDefaultCenter.Fill.BackgroundColor = Color.WhiteSmoke;
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
            //_gantt.BarDoubleClick += new BarEventHandler(GanttView_BarDoubleClick);
            Gantt.BarDraw += new BarDrawEventHandler(GanttView_BarDraw);
        }

        protected override void Query()
        {
            Gantt.DefaultColumnWidth = this.CellWidthSize;
            Gantt.DefaultRowHeight = this.CellHeightSize;

            Cursor.Current = Cursors.WaitCursor;
            buttonLoad.Enabled = false;
            BindData();
            buttonLoad.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        #region bar handling
        void GanttView_BarDraw(object sender, BarDrawEventArgs args)
        {
            var bar = args.Bar as LotBar;

            args.Background = Gantt.GetBrushInfo(bar, string.Empty);
            args.DrawFrame = Gantt.EnableSelect && Gantt.SelectedBar != null && !Gantt.CompareToSelectedBar(bar, string.Empty);
            args.FrameColor = Color.White;
            args.DrawFrame = true;

            args.ForeColor = bar.IsGhostBar && bar.State != EqpState.PM ? Color.Gray
                : bar.State == EqpState.DOWN ? Color.White : Color.Black;

            if (bar.State == EqpState.PM)
                args.ForeColor = Color.White;

            if (Gantt.IsOnlyLineMode)
                args.Text = bar.GetTitle(true);
            else
                args.Text = bar.GetTitle(false);

            args.DrawDefault = true;
        }

        void GanttView_BarClick(object sender, BarEventArgs e)
        {
            if (Gantt.ColumnHeader == null)
                return;

            this.grid1.BeginUpdate();

            if (e.Mouse.Button == MouseButtons.Right && e.Bar != null)
            {
                Gantt.TurnOnSelectMode();

                Gantt.SelectedBar = e.Bar as LotBar;
            }
            else
            {
                Gantt.TurnOffSelectMode();
            }

            this.grid1.EndUpdate();
            this.grid1.Refresh();

            var bar = e.Bar as LotBar;

            ViewDispDetail(bar); 
            ViewEqpProcessDetail(bar.LotID);
            HighLightSelectRow(e.RowIndex);
        }

        private void HighLightSelectRow(int rowIndex)
        {
            var worksheet = this.Gantt.Worksheet;

            worksheet.SelectRowUsed(rowIndex, false, 0);
        }

        #endregion

        #region build bars

        void GanttView_BindDone(object sender, EventArgs e)
        {
            var colHeader = Gantt.ColumnHeader;

            // 마지막 Row 값 세팅
            if (this.IsOnlyToolMode == false)
            {
                XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(_startSameEqpRowIdx, ColName.MaskChangeCnt), _subJobChg);
            }

            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(_startSameEqpRowIdx, ColName.TotalRun), _subTotalTO);

            string totalToColName = string.Format("{0} \n({1})", "TOTAL", _totalTO.ToString("#.#"));
            XtraSheetHelper.SetCellFloatValue(colHeader.GetCellInfo(0, ColName.TotalRun), totalToColName);

            for (int i=0; i < Gantt.LastRowIndex; i++)
            {
                colHeader.GetCellInfo(i, ColName.LotID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            }

            colHeader.GetCellInfo(0, ColName.LotID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            colHeader.GetCellInfo(0, ColName.RunQtySum).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            colHeader.GetCellInfo(0, ColName.TotalRun).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;

            colHeader.GetCellInfo(0, ColName.LotID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            colHeader.GetCellInfo(0, ColName.RunQtySum).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            colHeader.GetCellInfo(0, ColName.TotalRun).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

            if (this.IsOnlyToolMode == false)
            {
                colHeader.GetCellInfo(0, ColName.ProductID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                colHeader.GetCellInfo(0, ColName.ProductID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            }

            if (SelectViewMode == LotGantt.ViewMode.LINE)
            {
                int fromRowIdx = this.IsOnlyToolMode == false ? _startSameEqpRowIdx : _startSameRowKeyIdx;
                MergeRows(fromRowIdx, Gantt.LastRowIndex);
            }
            
            PaintTotColumnCell();
        }

        void GanttView_BindBarAdded(object sender, GanttCellEventArgs args)
        {
            args.Bar.CumulateQty(ref _rowsumti, ref _rowsumto);

            _rowsumJobChg += args.Bar.BarList.Where(x => x.State == EqpState.SETUP).Count();

            double loadTime = args.Bar.BarList.Where(x => x.State == EqpState.BUSY || x.State == EqpState.IDLERUN || x.State == EqpState.SETUP)
                .Sum(x => (x.TkoutTime - x.TkinTime).TotalSeconds);

            _rowsumLoadTimeFrBar += loadTime;
        }

        void GanttView_BindRowAdding(object sender, GanttRowEventArgs args)
        {
            var worksheet = this.Gantt.Worksheet;

            var info = args.Item as LotGantt.GanttInfo;
            var colHeader = Gantt.ColumnHeader;

            string lineID = info.LineID;
            string productID = info.ProductID;
            string lotID = info.LotID;

            SetRowHeaderValue(args.RowIndex, lineID, productID, string.IsNullOrEmpty(args.Key) ? "-" : args.Key, lotID);

            this._rowsumti = 0; 
            this._rowsumto = 0; 
            this._rowsumJobChg = 0;
            this._rowsumLoadTimeFrBar = 0;

            if (args.Node == null)
                return;

            var rows = args.Node.LinkedBarList;

            if (rows.Count > 1 && args.Index > 0 && args.Index < rows.Count - 1)
            {
                XtraSheetHelper.SetCellText(colHeader.GetCellInfo(args.RowIndex, ColName.LineID), lineID);
                XtraSheetHelper.SetCellText(colHeader.GetCellInfo(args.RowIndex, ColName.LotID), lotID);
                if (this.IsOnlyToolMode == false)
                    XtraSheetHelper.SetCellText(colHeader.GetCellInfo(args.RowIndex, ColName.ProductID), productID);

                PaintRowKeyedCell(args.RowIndex, _currColor);
            }
        }

        void GanttView_BindRowAdded(object sender, GanttRowEventArgs args)
        {
            var info = args.Item as LotGantt.GanttInfo;
            var colHeader = Gantt.ColumnHeader;

            if (_totalLoadTImeFrBarDic.ContainsKey(info.EqpID) == false)
                _totalLoadTImeFrBarDic.Add(info.EqpID, _rowsumLoadTimeFrBar);

            if (this.IsOnlyToolMode)
            {
                string sLoadRate = string.Empty;
                double loadRate = 0;
                sLoadRate = Math.Round(loadRate, 1).ToString() + "%";
                //XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(args.RowIndex, ColName.MaskChangeCnt), _rowsumJobChg);
            }

            XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(args.RowIndex, ColName.RunQtySum), _rowsumto);

            _subTotalTI += _rowsumti;
            _subTotalTO += _rowsumto;
            _totalTO += _rowsumto;
            _subJobChg += _rowsumJobChg;
            _totalJobChg += _rowsumJobChg;
        }

        void GanttView_BindItemAdded(object sender, GanttItemEventArgs args)
        {
            var info = args.Item as LotGantt.GanttInfo;
            var colHeader = Gantt.ColumnHeader;
        }

        #endregion

        #region build Header
        void GanttView_HeaderDone(object sender, GanttColumnHeaderEventArgs e)
        {
            var colHeader = e.ColumnHeader;

            colHeader.AddColumn(new XtraSheetHelper.SfColumn(ColName.RunQtySum, 70));

            colHeader.AddColumn(new XtraSheetHelper.SfColumn(ColName.TotalRun, 82));

            this.SizeManager.LeftExceptCount = this.Gantt.FixedColCount;
            this.SizeManager.TopExceptCount = this.Gantt.FixedRowCount;

            this.SizeManager.RightExceptCount = 3;
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

            DateTime hstart = MyHelper.DATE.Trim(this.StartDate, "HH");
            DateTime hend = MyHelper.DATE.Trim(this.EndDate, "HH");
            if (hend == this.EndDate)
                hend = hend.AddHours(-1);

            DateTime ss = shiftStart;
            DateTime ee = ss.AddHours(ShopCalendar.ShiftHours - 1);

            if (ss < hstart)
                ss = hstart;

            if (ee > hend)
                ee = hend.AddHours(-1);

            var sColName = ss.ToString(parttern);
            var eColName = ee.ToString(parttern);
            string headText = this.Gantt.GetJobChgShiftCntFormat(ss);

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
        
        #region // BindData

        protected void BindData()
        {
            GenerateLotGantt();

            BindGrid();
        }


        List<LotGantt.GanttInfo> _list;
        private void GenerateLotGantt()
        {
            Gantt.BuildGantt(
                this.IsOnlyToolMode,
                this.TargetLineID,
                this.SelectedProdIDs,
                this.SelectedStepIDs,
                this.StartDate,
                this.EndDate,
                this.PlanStartTime,
                this.EqpIdPattern
            );

            Dictionary<string, LotGantt.GanttInfo> collectData = Gantt.Table;
            _list = new List<LotGantt.GanttInfo>(collectData.Values);

            Gantt.Expand(this.IsStepView);
        }

        #endregion

        #region //BindGrid
        bool _isFirst = true;
        string _prePrcGroup = string.Empty;
        string _preLineID = string.Empty;
        string _preLotID = string.Empty;
        string _preProductID = string.Empty;
        string _preRowKey = string.Empty;

        Color _preColor = XtraSheetHelper.AltColor;
        Color _currColor = XtraSheetHelper.AltColor2;

        int _startSameEqpRowIdx = 0;
        int _startSameRowKeyIdx = 0;
        double _subTotalTI = 0;
        double _subTotalTO = 0;
        double _totalTO = 0;
        double _subJobChg = 0;
        double _totalJobChg = 0;
        Dictionary<string, double> _totalLoadTimeFrStatDic;
        Dictionary<string, double> _totalLoadTImeFrBarDic;

        double _rowsumti = 0;
        double _rowsumto = 0;
        double _rowsumJobChg = 0;
        double _rowsumLoadTimeFrBar = 0;

        public Dictionary<string, int> _moveByLayer;


        private void BindGrid()
        {
            if (IsOnlyToolMode || SelectViewMode == LotGantt.ViewMode.LOT)
                _list.Sort(new LotGantt.CompareGanttInfo(LotGantt.SortOptions.LOT_ID, LotGantt.SortOptions.PRODUCT_ID));
            else
                _list.Sort(new LotGantt.CompareGanttInfo(LotGantt.SortOptions.PRODUCT_ID, LotGantt.SortOptions.LOT_ID));

            Gantt.Workbook.BeginUpdate();
            Gantt.ResetWorksheet();

            Gantt.TurnOffSelectMode();

            SetColumnHeaders();

            var colHeader = Gantt.ColumnHeader;
            Gantt.SchedBarComparer = new CompareMBarList();

            _isFirst = true;
            _preRowKey = string.Empty;

            _preColor = XtraSheetHelper.AltColor;
            _currColor = XtraSheetHelper.AltColor2;

            _startSameEqpRowIdx = 0;
            _startSameRowKeyIdx = 0;
            _subTotalTI = 0;
            _subTotalTO = 0;
            _subJobChg = 0;

            _totalTO = 0;
            _totalJobChg = 0;
            _totalLoadTimeFrStatDic = new Dictionary<string, double>();
            _totalLoadTImeFrBarDic = new Dictionary<string, double>();

            Gantt.Bind(_list);
            Gantt.Workbook.EndUpdate();
        }

        private void SetRowHeaderValue(int rowIndex, string lineID, string productID, string stepSeq, string lotID)
        {
            if (IsOnlyToolMode)
            {
                string curKey = lineID;
                var colHeader = Gantt.ColumnHeader;

                if (_isFirst)
                {
                    _preLineID = lineID;
                    _preLotID = lotID;
                    _preProductID = productID;
                    _preRowKey = curKey;
                    _startSameEqpRowIdx = rowIndex;
                    _startSameRowKeyIdx = rowIndex;

                    _isFirst = false;
                }

                if (_isFirst == false && lotID.Equals(_preLotID) == false)
                {
                    MergeRows(_startSameEqpRowIdx, rowIndex - 1);
                    _startSameEqpRowIdx = rowIndex;
                }

                if (_isFirst == false && lotID.Equals(_preLotID) == false)
                {
                    if (_startSameEqpRowIdx > 1)
                    {
                        XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(_startSameEqpRowIdx - 1, ColName.TotalRun), _subTotalTO);

                        if (this.IsOnlyToolMode == false)
                        {
                            //XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(_startSameEqpRowIdx - 1, ColName.MaskChangeCnt), _subJobChg);
                        }
                    }

                    _preLineID = lineID;
                    _preLotID = lotID;
                    _preProductID = productID;
                    _startSameEqpRowIdx = rowIndex;
                    _subTotalTI = 0;
                    _subTotalTO = 0;
                    _subJobChg = 0;
                }

                if (_isFirst == false && curKey.Equals(_preRowKey) == false)
                {
					MergeRows(_startSameRowKeyIdx, rowIndex - 1);

					Color tmp = _preColor;
                    _preColor = _currColor;
                    _currColor = tmp;
                    _preRowKey = curKey;
					_startSameRowKeyIdx = rowIndex;
				}

                PaintRowKeyedCell(rowIndex, _currColor);

                XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.LineID), lineID);
                Console.WriteLine(lotID);
                XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.LotID), lotID);

                if (this.IsOnlyToolMode == false)
                    XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.ProductID), productID);

                Gantt.Worksheet[rowIndex, colHeader.TryGetColumnIndex(ColName.LineID)].SetCellText(lineID);
                colHeader.GetCellInfo(rowIndex, ColName.LineID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                colHeader.GetCellInfo(rowIndex, ColName.LineID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                Gantt.Worksheet[rowIndex, colHeader.TryGetColumnIndex(ColName.LotID)].SetCellText(lotID);

                colHeader.GetCellInfo(rowIndex, ColName.LotID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                colHeader.GetCellInfo(rowIndex, ColName.LotID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                if (this.IsOnlyToolMode == false)
                {
                    colHeader.GetCellInfo(rowIndex, ColName.ProductID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    colHeader.GetCellInfo(rowIndex, ColName.ProductID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                }
            }
            else
            {
                string curKey = productID;
                string curToolKey = lotID;
                var colHeader = Gantt.ColumnHeader;

                if (_isFirst)
                {
                    _preLotID = lotID;
                    _preProductID = productID;
                    _preRowKey = curKey;
                    _startSameEqpRowIdx = rowIndex;
                    _startSameRowKeyIdx = rowIndex;

                    _isFirst = false;
                }

                if (SelectViewMode == LotGantt.ViewMode.PROD)
                {
                    if (_isFirst == false && (lotID.Equals(_preLotID) == false || productID.Equals(_preProductID) == false))
                    {
                        MergeRows(_startSameEqpRowIdx, rowIndex - 1);
                        _preLotID = lotID;
                        _startSameEqpRowIdx = rowIndex;
                    }

                    if (_isFirst == false && productID.Equals(_preProductID) == false)
                    {
                        if (_startSameEqpRowIdx > 1)
                        {
                            string sLoadRate = string.Empty;
                            XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(_startSameEqpRowIdx - 1, ColName.TotalRun), _subTotalTO);
                        }

                        _preLotID = lotID;
                        _preProductID = productID;
                        _startSameEqpRowIdx = rowIndex;
                        _subTotalTI = 0;
                        _subTotalTO = 0;
                        _subJobChg = 0;
                    }

                    if (_isFirst == false && curKey.Equals(_preRowKey) == false)
                    {
                        MergeRows(_startSameRowKeyIdx, rowIndex - 1);

                        Color tmp = _preColor;
                        _preColor = _currColor;
                        _currColor = tmp;
                        _preRowKey = curKey;
                        _startSameRowKeyIdx = rowIndex;

                    }

                    PaintRowKeyedCell(rowIndex, _currColor);
                                       
                    XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.LotID), lotID);
                    Gantt.Worksheet[rowIndex, colHeader.TryGetColumnIndex(ColName.LotID)].SetCellText(lotID);

                    XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.ProductID), productID);
                    colHeader.GetCellInfo(rowIndex, ColName.ProductID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    colHeader.GetCellInfo(rowIndex, ColName.ProductID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                    colHeader.GetCellInfo(rowIndex, ColName.LotID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    colHeader.GetCellInfo(rowIndex, ColName.LotID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                }

                if (SelectViewMode == LotGantt.ViewMode.LOT)
                {

                    if (_isFirst == false && (lotID.Equals(_preLotID) == false || productID.Equals(_preProductID) == false))
                    {
                        MergeRows(_startSameEqpRowIdx, rowIndex - 1, 1);
                        _preLotID = lotID;
                        _preProductID = productID;
                        _startSameEqpRowIdx = rowIndex;
                    }

                    if (_isFirst == false && productID.Equals(_preLotID) == false)
                    {
                        if (_startSameEqpRowIdx > 1)
                        {
                            string sLoadRate = string.Empty;
                            XtraSheetHelper.SetTotCellValue(colHeader.GetCellInfo(_startSameEqpRowIdx - 1, ColName.TotalRun), _subTotalTO);
                        }

                        _preLotID = lotID;
                        _preProductID = productID;
                        _startSameEqpRowIdx = rowIndex;
                        _subTotalTI = 0;
                        _subTotalTO = 0;
                        _subJobChg = 0;
                    }

                    if (_isFirst == false && curKey.Equals(_preRowKey) == false)
                    {
                        MergeRows(_startSameRowKeyIdx, rowIndex - 1, 1);

                        Color tmp = _preColor;
                        _preColor = _currColor;
                        _currColor = tmp;
                        _preRowKey = curKey;
                        _startSameRowKeyIdx = rowIndex;

                    }

                    PaintRowKeyedCell(rowIndex, _currColor);

                    XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.LotID), lotID);
                    Gantt.Worksheet[rowIndex, colHeader.TryGetColumnIndex(ColName.LotID)].SetCellText(lotID);

                    XtraSheetHelper.SetCellText(colHeader.GetCellInfo(rowIndex, ColName.ProductID), productID);
                    colHeader.GetCellInfo(rowIndex, ColName.ProductID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    colHeader.GetCellInfo(rowIndex, ColName.ProductID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                    colHeader.GetCellInfo(rowIndex, ColName.ProductID).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    colHeader.GetCellInfo(rowIndex, ColName.ProductID).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

                }
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

            int toColIndex = 2;

            for (int colindex = 0; colindex < toColIndex; colindex++)
            {
                worksheet[rowIndex, colindex].FillColor = color;
            }
        }

        private void PaintTotColumnCell()
        {
            var worksheet = this.Gantt.Worksheet;

            var colHeader = this.Gantt.ColumnHeader;

            worksheet.SetUsedColumnFillColor(colHeader.TryGetColumnIndex(ColName.RunQtySum), Color.FromArgb(219, 236, 216));

            worksheet.SetUsedColumnFillColor(colHeader.TryGetColumnIndex(ColName.TotalRun), Color.FromArgb(204, 255, 195));

            var usedRange = worksheet.GetUsedRange();
            Formatting rangeFormatting = usedRange.BeginUpdateFormatting();
            rangeFormatting.Font.Color = Color.Black;
            usedRange.EndUpdateFormatting(rangeFormatting);
        }

        #endregion

        #region ColName

        struct ColName
        {
            public static string LineID = "LINE_ID";
            public static string ToolID = "TOOL_ID";
            public static string EqpId = "EQP_ID";
            public static string State = "State";
            public static string ProductID = "ProductID";
            public static string StartTime = "START_TIME";
            public static string EndTime = "END_TIME";
            public static string TrackInTime = "TRACK_IN_TIME";
            public static string TrackOutTime = "TRACK_OUT_TIME";
            public static string GapTime = "GAP_TIME";
            public static string NextTkinTime = "NEXT_TKIN_TIME";
            public static string StepID = "STEP_ID";
            public static string LotID = "LOTID";
            public static string ProductKind = "PRODUCT_KIND";

            public static string TIQty = "T/I QTY";
            public static string TOQty = "T/O QTY";

            public static string MaskChangeCnt = "MASK CHG";

            public static string TIQtySum = "T/I\nQTY";
            public static string RunQtySum = "RUN QTY";
            public static string TITotal = "T/I\nTOTAL";
            public static string TotalRun = "TOTAL RUN";
        }

        #endregion

        #region // SetColumnHeader

        protected void SetColumnHeaders()
        {
            int colCount = this.TargetAddDays * (int)ShopCalendar.ShiftHours + 4 + 2;
            
            if (this.IsOnlyToolMode)
            {
                Gantt.FixedColCount = 2;
            }
            else
            {
                colCount += 1;
                Gantt.FixedColCount = 2;
            }

            if (this.IsStepView)
                Gantt.FixedRowCount = 3;
            else
                Gantt.FixedRowCount = 2;


            if (this.IsOnlyToolMode)
            {                    
                Gantt.SetColumnHeaders(colCount,
                    new XtraSheetHelper.SfColumn(ColName.LineID, ColName.LineID, 80),
                    new XtraSheetHelper.SfColumn(ColName.LotID, ColName.LotID, 150));
            }
            else
            {
                if (SelectViewMode == LotGantt.ViewMode.PROD)
                {
                    Gantt.SetColumnHeaders(colCount,
                        new XtraSheetHelper.SfColumn(ColName.ProductID, ColName.ProductID, 93),
                        new XtraSheetHelper.SfColumn(ColName.LotID, ColName.LotID, 150));
                }

                if (SelectViewMode == LotGantt.ViewMode.LOT)
                {
                    Gantt.SetColumnHeaders(colCount,
                        new XtraSheetHelper.SfColumn(ColName.LotID, ColName.LotID, 150),
                        new XtraSheetHelper.SfColumn(ColName.ProductID, ColName.ProductID, 93));
                }
            }

            Gantt.Worksheet.Rows[0].Style = Gantt.Workbook.Styles["CustomHeader"];
            Gantt.Worksheet.Rows[1].Style = Gantt.Workbook.Styles["CustomHeader"];

            Gantt.Worksheet.SelectedCell[0, 0].Style = Gantt.Workbook.Styles["CustomHeaderCenter"];
            Gantt.Worksheet.SelectedCell[0, 1].Style = Gantt.Workbook.Styles["CustomHeaderCenter"];
        }

        #endregion

        #region //Event Handlers

        private void editLineId_EditValueChanged(object sender, EventArgs e)
        {
            MyHelper.ENGCONTROL.SetControl_ProductID_Checked(this.editProdId, this.Result);
            MyHelper.ENGCONTROL.SetControl_StepID_Checked(this.editStepId, this.Result);
            //MyHelper.CONTROL.SetControl_Mask_Checked(this.editMask, this.TargetVersionNo, this.TargetLineID);
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControl1.DataSource = null;

            RunQuery();
        }

        private void radioViewGroup_EditValueChanged(object sender, EventArgs e)
        {
            if (Gantt == null)
                return;

            RunQuery();
        }

        private void radioGroup1_EditValueChanged(object sender, EventArgs e)
        {
            if (Gantt == null)
                return;

            Gantt.MouseSelType = this.SelectedMouseSelectType;
        }

        private void ViewDispDetail(LotBar bar = null)
        {
            if (this.DispView == null)
                return;

            this.IsNeedSetFocus = true;
            this.DispView.SetBarInfo(bar);
        }

        #region // GetGridDetail
        private void ViewEqpProcessDetail(string toolID)
        {
            List<LotGantt.GanttInfo> list = GetGanttInfo(toolID);

            if (list == null)
                return;

            DataTable dt = CrateDataTable();

            BindProcessData(dt, list);
        }

        private List<LotGantt.GanttInfo> GetGanttInfo(string toolID)
        {
            List<LotGantt.GanttInfo> result = new List<LotGantt.GanttInfo>();

            foreach (LotGantt.GanttInfo info in _list)
            {
                if (info.LotID == toolID)
                    result.Add(info);
            }
            if (result.Count > 0)
                return result;
            else
                return null;
        }
        #endregion


        private void BindProcessData(DataTable dt, List<LotGantt.GanttInfo> list)
        {
            foreach (LotGantt.GanttInfo info in list)
            {
                foreach (var item in info.Items)
                {
                    string step = item.Key;

                    foreach (LotBar b in item.Value)
                    {
                        if (b.BarKey != step || b.State == EqpState.PM)
                            continue;

                        DataRow drow = dt.NewRow();
                        drow[ColName.LineID] = b.LineID;
                        drow[ColName.ProductID] = b.ProductId;
                        drow[ColName.LotID] = b.LotID;

                        drow[ColName.State] = b.State.ToString();

                        if (b.State == EqpState.BUSY || b.State == EqpState.IDLERUN)
                        {
                            drow[ColName.EqpId] = b.EqpId;
                            drow[ColName.StepID] = b.StepId;
                        }
                        drow[ColName.StartTime] = b.TkinTime.ToString("yyyy-MM-dd HH:mm:ss");
                        drow[ColName.EndTime] = b.TkoutTime.ToString("yyyy-MM-dd HH:mm:ss");


                        if (b.State == EqpState.BUSY)
                        {
                            drow[ColName.TIQty] = b.TIQty;
                        }

                        dt.Rows.Add(drow);
                    }
                }
            }

            detailGridControl.BeginUpdate();
            detailGridControl.DataSource = new DataView(dt, "", ColName.StartTime, DataViewRowState.CurrentRows);
            detailGridControl.EndUpdate();

            detailGridView.BestFitColumns();

            //Globals.GetColumnWeith(detailGridView);
        }

        private DataTable CrateDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(ColName.LineID, typeof(string));
            dt.Columns[ColName.LineID].Caption = ColName.LineID;

            dt.Columns.Add(ColName.LotID, typeof(string));
            dt.Columns[ColName.LotID].Caption = ColName.LotID;
            dt.Columns.Add(ColName.EqpId, typeof(string));
            dt.Columns[ColName.EqpId].Caption = ColName.EqpId;
            dt.Columns.Add(ColName.State, typeof(string));
            dt.Columns[ColName.State].Caption = "STATUS";

            dt.Columns.Add(ColName.StepID, typeof(string));
            dt.Columns[ColName.StepID].Caption = ColName.StepID;

            dt.Columns.Add(ColName.ProductID, typeof(string));
            dt.Columns[ColName.ProductID].Caption = "PRODUCT_ID";
            dt.Columns.Add(ColName.StartTime, typeof(string));
            dt.Columns[ColName.StartTime].Caption = "START_TIME";
            dt.Columns.Add(ColName.EndTime, typeof(string));
            dt.Columns[ColName.EndTime].Caption = "END_TIME";

            dt.Columns.Add(ColName.TIQty, typeof(int));
            dt.Columns[ColName.TIQty].Caption = "QTY";

            return dt;
        }

		private void detailGridView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (this.Gantt == null)
                return;

            LotBar bar = this.Gantt.SelectedBar as LotBar;
            if (bar == null)
                return;

            if (bar.IsGhostBar)
                return;

            var text = detailGridView.GetRowCellDisplayText(e.RowHandle, ColName.StartTime);
            if (string.IsNullOrEmpty(text))
                return;

            if (text == MyHelper.DATE.Format(bar.TkinTime))
            {
                e.Appearance.BackColor = Color.LightCoral;

                if (this.IsNeedSetFocus)
                {
                    detailGridView.FocusedRowHandle = e.RowHandle;
                    this.IsNeedSetFocus = false;
                }
            }
        }

        private void buttonExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Gantt.ExportToExcel(this.Tag + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        private void editCellWidth_EditValueChanged(object sender, EventArgs e)
        {
            this.SizeManager.CellWidth = this.CellWidthSize;
        }

        private void editCellHeight_EditValueChanged(object sender, EventArgs e)
        {
            this.SizeManager.CellHeight = this.CellHeightSize;
        }

        private void buttonSaveSize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.SaveLocalSetting(this.ServiceProvider, this.ViewName + "ganttCellWidth", this.CellWidthSize.ToString());
            this.SaveLocalSetting(this.ServiceProvider, this.ViewName + "ganttCellHeight", this.CellHeightSize.ToString());
        }

		#endregion

	}
}
