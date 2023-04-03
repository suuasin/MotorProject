using DevExpress.LookAndFeel;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.UserLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Gantts;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using SmartAPS.UserLibrary.Utils;
using SmartAPS.UI.Properties;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserInterface;

namespace SmartAPS.UI.Analysis
{
    public partial class DispatchingAnalysisView : MyXtraGridTemplate
    {
        private Dictionary<string, EQUIPMENT> _eqpList;

        private List<PRESET_INFO_HIS> _piList;

        private List<DispatchingInfo> _infos;

        private bool isChild = false;

        private string PrevVersionNo { get; set; }

        private EqpMaster EqpMgr { get; set; }

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

        private DateTime PlanStartTime
        {
            get
            {
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
            }
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
                return this.TargetStartDate.AddHours(ShopCalendar.ShiftHours * Convert.ToInt32(this.TargetAddDays));
            }
        }

        private int TargetAddDays
        {
            get
            {
                return Convert.ToInt32(this.editDateSpin.EditValue);
            }
        }

        public DispatchingAnalysisView()
        {
            InitializeComponent();
        }

        public DispatchingAnalysisView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        public DispatchingAnalysisView(IExperimentResultItem result, DateTime startTime, int shifts)
        {
            InitializeComponent();
            this.Result = result;
            this.isChild = true;

            this.editDateTime.EditValue = startTime;
            this.editDateSpin.EditValue = shifts;

            LoadDocument();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetInitializeOption(this.gridControl1);

            Initialize();
            SetControls();
            //if (!isChild)
            //    RunQuery();
        }

        private void Initialize()
        {
            GetPreprocessData();
            GetEqpData();
        }

        private void GetPreprocessData()
        {
            this.EqpMgr = new EqpMaster(this.Result);
            this.EqpMgr.LoadEqp();

            _piList = new List<PRESET_INFO_HIS>();

            var table = MyHelper.DATASVC.GetEntityData<PRESET_INFO_HIS>(this.Result);

            foreach (PRESET_INFO_HIS wpLog in table)
            {
                _piList.Add(wpLog);
            }
        }

        private void GetEqpData()
        {
            _eqpList = new Dictionary<string, EQUIPMENT>();

            var dt = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(this.Result);
            if (dt == null)
                return;

            foreach (EQUIPMENT eqp in dt)
                _eqpList.Add(eqp.EQP_ID, eqp);
        }

        private void SetControls()
        {
            if (this.isChild)
            {
                this.gridView1.OptionsView.ShowGroupPanel = false;
                this.ribbonControl1.Minimized = true;
            }
            else
            {
                this.editDateSpin.EditValue = 1 * ShopCalendar.ShiftCount;
                this.editDateTime.EditValue = this.PlanStartTime;
            }

            MyHelper.ENGCONTROL.SetControl_EqpGroup_Checked(this.editEqpGroup, this.Result);
        }

        private List<DispatchingInfo> GetDispatchingInfo()
        {
            var list = _infos = new List<DispatchingInfo>();

            var table = MyHelper.DATASVC.GetEntityData<EQP_DISPATCH_LOG>(this.Result);
            if (table == null || table.Count() == 0)
                return list;

            foreach (EQP_DISPATCH_LOG row in table)
            {
                var item = new DispatchingInfo(row);

                list.Add(item);
            }

            return list;
        }

        protected override void Query()
        {
            var list = GetData();
            BindGrid(list);
            DesignGrid();
        }

        public void Query(string eqpGroupID, string eqpID)
        {
            if (this.WaitDlg == null)
                this.WaitDlg = new WaitDialog("Query", string.Empty, new Size(260, 50), this.ParentForm);

            try
            {
                SetControlValue(eqpGroupID);

                var list = GetData();
                if (list == null)
                    return;
                BindGrid(list);
                DesignGrid();

                SetFilter(eqpGroupID, eqpID);
            }
            finally
            {
                if (this.WaitDlg != null)
                    this.WaitDlg.Close();

                this.WaitDlg = null;
            }

            this.Cursor = Cursors.Default;
        }

        private void SetControlValue(string eqpGroupID)
        {
            this.editEqpGroup.EditValue = eqpGroupID;
        }

        private void SetFilter(string eqpGroupID, string eqpID)
        {
            var gv = this.gridView1;

            if (gv.ActiveFilter != null)
                gv.ActiveFilter.Clear();

            var condition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Equals;

            if (string.IsNullOrEmpty(eqpGroupID) == false)
                gv.SetAutoFilterValue(gv.Columns[Schema.EQP_GROUP], eqpGroupID, condition);

            if (string.IsNullOrEmpty(eqpID) == false)
                gv.SetAutoFilterValue(gv.Columns[Schema.EQP_ID], eqpID, condition);
        }


        private DataTable CreateSchema()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(Schema.EQP_GROUP, typeof(string));
            dt.Columns.Add(Schema.EQP_ID, typeof(string));
            dt.Columns.Add(Schema.DISPATCHING_TIME, typeof(string));
            dt.Columns.Add(Schema.SELECTED_LOT, typeof(string));
            dt.Columns.Add(Schema.SELECTED_PRODDUCT, typeof(string));
            dt.Columns.Add(Schema.SELECTED_STEP, typeof(string));

            dt.Columns.Add(Schema.INIT_WIP_CNT, typeof(int));
            dt.Columns.Add(Schema.FILTERED_WIP_CNT, typeof(int));
            dt.Columns.Add(Schema.SELECTED_WIP_CNT, typeof(int));

            dt.Columns.Add(Schema.FILTERED_REASON, typeof(string));
            dt.Columns.Add(Schema.FILTERED_PRODUCT, typeof(string));
            dt.Columns.Add(Schema.PRESET_ID, typeof(string));
            dt.Columns.Add(Schema.DISPATCH_PRODUCT, typeof(string));

            return dt;
        }

        new private List<DispatchingInfo> GetData()
        {
            var infos = GetDispatchingInfo();

            if (infos == null || infos.Count == 0)
                return infos;

            DateTime fromTime = this.TargetStartDate;
            DateTime toTime = this.TargetEndDate;

            var list = infos.FindAll(t => IsMatched(t, fromTime, toTime, SelectedEqpGroups));

            return list;
        }

        private bool IsMatched(DispatchingInfo info, DateTime fromTime, DateTime toTime, List<string> eqpGroupList)
        {
            DateTime t = MyHelper.DATE.StringToDateTime(info.DispatchingTime);
            if (t < fromTime || t >= toTime)
                return false;

            if (eqpGroupList == null || eqpGroupList.Count == 0)
                return false;

            EQUIPMENT eqpInfo;
            if (string.IsNullOrEmpty(info.EqpID) == false && _eqpList.TryGetValue(info.EqpID, out eqpInfo))
            {
                if (string.IsNullOrEmpty(eqpInfo.EQP_GROUP) == false && eqpGroupList.Contains(eqpInfo.EQP_GROUP) == false)
                    return false;
            }
            else
                return false;

            return true;
        }


        private void BindGrid(List<DispatchingInfo> dspData)
        {
            var dt = CreateSchema();
            var list = dspData;

            foreach (DispatchingInfo item in list)
            {
                DataRow row = dt.NewRow();

                string eqpGroupId = string.Empty;
                EQUIPMENT eqpInfo;
                if (_eqpList.TryGetValue(item.EqpID, out eqpInfo))
                    eqpGroupId = eqpInfo.EQP_GROUP;
                row[Schema.EQP_GROUP] = eqpGroupId;
                row[Schema.EQP_ID] = item.EqpID;

                row[Schema.DISPATCHING_TIME] = item.DispatchingTime;
                row[Schema.SELECTED_LOT] = item.SeleLotID;
                row[Schema.SELECTED_PRODDUCT] = item.SeleProdID;
                row[Schema.SELECTED_STEP] = item.SeleStepID;

                row[Schema.INIT_WIP_CNT] = item.InitWipCnt;
                row[Schema.FILTERED_WIP_CNT] = item.FilteredWipCnt;
                row[Schema.SELECTED_WIP_CNT] = item.SelectedWipCnt;

                row[Schema.FILTERED_REASON] = item.FilteredReason;
                row[Schema.FILTERED_PRODUCT] = item.FilteredProdID;
                row[Schema.DISPATCH_PRODUCT] = item.DispatchProdID;
                row[Schema.PRESET_ID] = item.PresetID;

                dt.Rows.Add(row);
            }

            // GridControl1
            this.gridControl1.DataSource = dt;
            this.gridView1.BestFitColumns();

            SetColumnCaption();
        }

        private void SetColumnCaption()
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView1.Columns)
            {
                col.Caption = col.FieldName.Replace('_', ' ');
            }
        }

        protected void DesignGrid()
        {
            var grid = this.gridView1;

            grid.Columns[Schema.INIT_WIP_CNT].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grid.Columns[Schema.INIT_WIP_CNT].DisplayFormat.FormatString = "###,###";
            grid.Columns[Schema.FILTERED_WIP_CNT].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grid.Columns[Schema.FILTERED_WIP_CNT].DisplayFormat.FormatString = "###,###";
            grid.Columns[Schema.SELECTED_WIP_CNT].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grid.Columns[Schema.SELECTED_WIP_CNT].DisplayFormat.FormatString = "###,###";

            grid.Columns[Schema.DISPATCHING_TIME].Resize(135);

            int index = grid.Columns[Schema.INIT_WIP_CNT].AbsoluteIndex;

            int count = grid.Columns.Count;
            for (int i = 0; i < count; i++)
            {
                if (i < index)
                    continue;

                var col = grid.Columns[i];
                col.OptionsColumn.AllowEdit = false;
                col.AppearanceCell.BackColor = DXSkinColors.FillColors.Warning;
                col.AppearanceCell.ForeColor = Color.Black;
            }

            MyHelper.ENGCONTROL.SetGridViewColumnWidth(grid);
        }

        #region Event

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo info = view.CalcHitInfo(pt);
            if (info.InRowCell == false)
                return;

            DataRow selectRow = gridView1.GetFocusedDataRow();

            if (selectRow == null)
                return;

            string eqpID = selectRow.GetString(Schema.EQP_ID);

            string dispatchTime = selectRow.GetString(Schema.DISPATCHING_TIME);

            DispatchingInfo resultInfo = _infos.Where(t => t.EqpID == eqpID && t.DispatchingTime == dispatchTime).FirstOrDefault();

            if (resultInfo == null)
            {
                MessageBox.Show("Information not available");
                return;
            }

            EqpMaster.Eqp eqp = this.EqpMgr.FindEqp(eqpID);

            gridView1.FocusedRowHandle = -1;

            DispatchingInfoViewPopup dialog = new DispatchingInfoViewPopup(this.Result, resultInfo.DispInfo, eqp, _piList);

            dialog.Show();
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

        private void buttonExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.GRIDVIEWEXPORT.ExportToExcel(this.gridView1);
        }

        #endregion

        #region PresetInfo

        public class PresetInfo
        {
            public string PresetID;
            public string FactorID;
            public string FactorType;
            public string OrderType;
            public double FactorWeight;
            public float Sequence;
            public string Criteria;

            public PresetInfo(PRESET_INFO_HIS row)
            {
                PresetID = string.Empty;
                FactorID = string.Empty;
                FactorType = string.Empty;
                OrderType = string.Empty;
                FactorWeight = 0.0f;
                Sequence = 0.0f;
                Criteria = string.Empty;

                ParseRow(row);
            }

            private void ParseRow(PRESET_INFO_HIS row)
            {
                // PresetID
                PresetID = row.PRESET_ID;

                // FactorID
                FactorID = row.FACTOR_ID;

                FactorType = row.FACTOR_TYPE;

                OrderType = row.ORDER_TYPE;

                FactorWeight = row.FACTOR_WEIGHT;

                Sequence = row.SEQUENCE;

                Criteria = row.CRITERIA;
            }

            public class Comparer : IComparer<PresetInfo>
            {
                public int Compare(PresetInfo x, PresetInfo y)
                {
                    if (object.ReferenceEquals(x, y))
                        return 0;

                    int cmp = x.Sequence.CompareTo(y.Sequence);

                    if (cmp == 0)
                    {
                        cmp = string.Compare(x.FactorID, y.FactorID);
                    }

                    return cmp;
                }

                public static Comparer Default = new Comparer();
            }
        }

        #endregion

        #region DispatchingInfo

        public class DispatchingInfo
        {
            public EQP_DISPATCH_LOG DispInfo { get; private set; }

            public string SeleLotID { get; private set; }
            public string SeleProdID { get; private set; }
            public string SeleStepID { get; private set; }
            public string FilteredReason { get; private set; }
            public string FilteredLotID { get; private set; }
            public string FilteredProdID { get; private set; }
            public string DispatchLotID { get; private set; }
            public string DispatchProdID { get; private set; }

            public string EqpID
            {
                get { return this.DispInfo.EQP_ID; }
            }

            //public string EqpGroupID
            //{
            //	get { return this.DispInfo.EQP_GROUP; }
            //}

            public string PresetID
            {
                get { return this.DispInfo.PRESET_ID; }
            }
            public string DispatchingTime
            {
                get { return this.DispInfo.DISPATCHING_TIME; }
            }

            public int InitWipCnt
            {
                get { return this.DispInfo.INIT_WIP_CNT; }
            }

            public int FilteredWipCnt
            {
                get { return this.DispInfo.FILTERED_WIP_CNT; }
            }

            public int SelectedWipCnt
            {
                get { return this.DispInfo.SELECTED_WIP_CNT; }
            }

            public string FilteredWipLog
            {
                get { return this.DispInfo.FILTERED_WIP_LOG; }
            }
            public string DispatchWipLog
            {
                get { return this.DispInfo.DISPATCH_WIP_LOG; }
            }

            public DispatchingInfo(EQP_DISPATCH_LOG dispInfo)
            {
                this.DispInfo = dispInfo;

                string seleLotID = string.Empty;
                // TODO --------------------------
                if (dispInfo.SELECTED_WIP == null)
                    return;

                string[] seleLotInfos = dispInfo.SELECTED_WIP.Split(';');
                foreach (string info in seleLotInfos)
                {
                    if (string.IsNullOrEmpty(info))
                        continue;

                    seleLotID += string.IsNullOrEmpty(seleLotID) ? info.Split('/')[0] : ";" + info.Split('/')[0];
                }

                this.SeleLotID = seleLotID;

                //string seleProdID = string.Empty;
                //foreach (string info in seleLotInfos)
                //{
                //    if (string.IsNullOrEmpty(info))
                //        continue;

                //    seleProdID += string.IsNullOrEmpty(seleProdID) ? info.Split('/')[1] : ";" + info.Split('/')[1];
                //}

                //this.SeleProdID = seleProdID;

                //string seleStepID = string.Empty;
                //foreach (string info in seleLotInfos)
                //{
                //    if (string.IsNullOrEmpty(info))
                //        continue;

                //    seleStepID += string.IsNullOrEmpty(seleStepID) ? info.Split('/')[3] : ";" + info.Split('/')[3];
                //}

                //this.SeleStepID = seleStepID;

                string filteredReasons = string.Empty;
                string filteredLotID = string.Empty;
                string filteredProdID = string.Empty;

                List<string> filteredReasonList = new List<string>();
                List<string> filteredProdList = new List<string>();

                // TODO --------------------------
                if (dispInfo.FILTERED_WIP_LOG == null)
                    return;
                string[] blobs = dispInfo.FILTERED_WIP_LOG.Split('\t');
                foreach (string blob in blobs)
                {
                    string[] group = blob.Split(':');

                    if (group.Length < 2)
                        continue;

                    string reason = group[0];

                    string[] wips = group[1].Split(';');

                    foreach (string wip in wips)
                    {
                        string[] wipArr = wip.Split('/');

                        string lotID = wipArr[0];

                        filteredLotID += string.IsNullOrEmpty(filteredLotID) ? lotID : ";" + lotID;

                        if (filteredReasonList.Contains(reason) == false)
                        {
                            filteredReasonList.Add(reason);
                            filteredReasons += string.IsNullOrEmpty(filteredReasons) ? reason : ";" + reason;
                        }

                        string prodID = wipArr.Count() >= 2 ? wip.Split('/')[1] : Consts.NULL_ID;

                        if (filteredProdList.Contains(prodID) == false)
                        {
                            filteredProdList.Add(prodID);
                            filteredProdID += string.IsNullOrEmpty(filteredProdID) ? prodID : ";" + prodID;
                        }
                    }
                }

                this.FilteredReason = filteredReasons;
                this.FilteredLotID = filteredLotID;
                this.FilteredProdID = filteredProdID;

                string dispatchLotIDs = string.Empty;
                string dispatchProdIDs = string.Empty;

                List<string> dispatchProdList = new List<string>();

                string dispatchWipLog = dispInfo.DISPATCH_WIP_LOG;         // DISPATCH_WIP_LOG
                string[] dispatchInfoList = dispatchWipLog.Split(';');

                foreach (string info in dispatchInfoList)
                {
                    string[] split = info.Split('/');

                    string lotID = split.Count() >= 1 ? split[0] : string.Empty;
                    string prodID = split.Count() >= 2 ? split[1] : string.Empty; ;

                    dispatchLotIDs += string.IsNullOrEmpty(dispatchLotIDs) ? lotID : ";" + lotID;

                    if (dispatchProdList.Contains(prodID) == false)
                    {
                        dispatchProdList.Add(prodID);
                        dispatchProdIDs += string.IsNullOrEmpty(dispatchProdIDs) ? prodID : ";" + prodID;
                    }
                }

                this.DispatchLotID = dispatchLotIDs;
                this.DispatchProdID = dispatchProdIDs;
            }
        }

        #endregion

        #region Schema

        public class Schema
        {
            public const string EQP_GROUP = "EQP_GROUP";
            public const string EQP_ID = "EQP_ID";

            public const string DISPATCHING_TIME = "DISPATCHING_TIME";
            public const string SELECTED_LOT = "SELECTED_LOT";
            public const string SELECTED_PRODDUCT = "SELECTED_PRODDUCT";
            public const string SELECTED_STEP = "SELECTED_STEP";

            public const string INIT_WIP_CNT = "INIT_WIP_CNT";
            public const string FILTERED_WIP_CNT = "FILTERED_WIP_CNT";
            public const string SELECTED_WIP_CNT = "SELECTED_WIP_CNT";

            public const string FILTERED_REASON = "FILTERED_REASON";
            public const string FILTERED_LOT = "FILTERED_LOT";
            public const string FILTERED_PRODUCT = "FILTERED_PRODUCT";
            public const string DISPATCH_LOT = "DISPATCH_LOT";
            public const string DISPATCH_PRODUCT = "DISPATCH_PRODUCT";
            public const string PRESET_ID = "PRESET_ID";

            public const string FACTOR_ID = "FACTOR_ID";
            public const string FACTOR_TYPE = "FACTOR_TYPE";
            public const string ORDER_TYPE = "ORDER_TYPE";
            public const string FACTOR_WEIGHT = "FACTOR_WEIGHT";
            public const string SEQUENCE = "SEQUENCE";
            public const string FACTOR_NAME = "FACTOR_NAME";
            public const string CRITERIA = "CRITERIA";
        }

        #endregion

        #region OutputName

        public class OutputName
        {
            public const string EqpPlan = "EqpPlan";
            public const string StepMove = "StepMove";
            public const string StepWip = "StepWip";
            public const string EqpDispatchLog = "EqpDispatchLog";
            public const string LoadHistory = "LoadHistory";
            public const string LoadStat = "LoadStat";
            public const string MaskHistory = "MaskHistory";
            public const string SetupHistory = "SetupHistory";
            public const string WeightPresetLog = "WeightPresetLog";
        }

        #endregion
    }
}
