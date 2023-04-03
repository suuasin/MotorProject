using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mozart.Studio.TaskModel.Projects;

using Template.Lcd.Scheduling.Outputs;// CSOT->Templete으로 변경
using Template.Lcd.Scheduling.Inputs;// CSOT->Templete으로 변경
using SmartAPS.UserLibrary.Utils;

using DevExpress.XtraEditors;
using Mozart.Data.Entity;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using Mozart.Extensions;

using SmartAPS.MozartStudio.Utils;
using SmartAPS.MozartStudio.Helper;

namespace SmartAPS.MozartStudio.Analysis
{
    public partial class ToolUseStateView : MyXtraGridTemplate
    {
        enum LimitType
        {
            NONE,
            L,
            P,
            O,
            B,
            M,
        }

        public const string TFT = "TFT";
        public const string ARRAY = "ARRAY";
        public const string CF = "CF";
        public const string CELL = "CELL";

        public readonly Dictionary<string, string> areaDict = new Dictionary<string, string>() {
            { TFT, ARRAY },
            { CF, CF },
            { CELL, CELL }
        };

        private List<ResultData> _datas;

        //Dictionary<eqpID, List<EAItem>>
        private Dictionary<string, List<EAItem>> _eaInfos;

        //EqpID+StepID+ProductID+ProductVersion+ToolID (LPOBM)
        private Dictionary<string, List<EAItem>> _eaMatchInfos;
        private Dictionary<string, List<MaskHistory>> _mhInfos;

        //EqpArrange(LimitType = M), Key = MaskID
        private Dictionary<string, List<EAItem>> _eamMatchInfos;

        private DateTime PlanStartTime
        {
            get
            {
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result); //planstart
            }
        }

        private DateTime PlanEndTime { get; set; }

        private string TargetAreaID
        {
            get { return this.editAreaId.EditValue as string; }
            set { this.editAreaId.EditValue = value; }
        }

        private string TargetProductID
        {
            get { return this.editProdId.EditValue as string; }
            set { this.editProdId.EditValue = value; }
        }

        private string TargetStepID
        {
            get { return this.editStepId.EditValue as string; }
            set { this.editStepId.EditValue = value; }
        }

        private string TargetDate
        {
            get { return this.editDateTime.EditValue as string; }
            set { this.editDateTime.EditValue = value; }
        }

        public ToolUseStateView()
        {
            InitializeComponent();
		}

        public ToolUseStateView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.SetInitializeOption(this.gridControl1);

            Initialize();

            SetControls();

            RunQuery();
        }

        private void SetControls()
        {
            MyHelper.ENGCONTROL.SetControl_AreaID(this.editAreaId, this.Result); //this.TargetVersionNo
            SetControl_ProductID(this.editProdId);
			SetControl_StepID(this.editStepId);

            this.PlanEndTime = this.PlanStartTime.AddDays(7);

            this.editDateTime.EditValue = this.PlanStartTime;
            this.repoDateTime.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            this.repoDateTime.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
        }

		private void SetControl_StepID(BarEditItem control)
        {
            control.BeginUpdate();

            var cbEdit = control.Edit as RepositoryItemComboBox;
            if (cbEdit == null)
                return;

            cbEdit.Items.Clear();

            List<string> list = new List<string>();

            var table = MyHelper.DATASVC.GetEntityData<StdStep>(this.Result);
            if (table != null)
            {
                foreach (var item in table)
                {
                    if (this.TargetAreaID != item.AREA_ID)
                        continue;
                    if (item.IS_USE_MASK != "Y" && item.IS_USE_JIG != "Y")
                        continue;
                    string value = item.STEP_ID;
                    if (value != null && !cbEdit.Items.Contains(value))
                        list.Add(value);
                }
            }

            list.Sort();
            cbEdit.Items.AddRange(list);

            if (!cbEdit.Items.Contains(control.EditValue))
            {
                if (cbEdit.Items.Count > 0)
                    control.EditValue = cbEdit.Items[0];
                else
                    control.EditValue = null;
            }

            control.EndUpdate();
            control.Refresh();
        }

        public void SetControl_ProductID(BarEditItem control, bool useAll = false)
        {
            control.BeginUpdate();

            var cbEdit = control.Edit as RepositoryItemComboBox;
            if (cbEdit == null)
                return;

            cbEdit.Items.Clear();

            if (useAll)
                cbEdit.Items.Add("ALL");

            List<string> list = new List<string>();

            var table = MyHelper.DATASVC.GetEntityData<Product>(this.Result);
            if (table != null)
            {
                foreach (var item in table)
                {
                    if (areaDict[this.TargetAreaID] != item.SHOP_ID)
                        continue;
                    string value = item.PRODUCT_ID;
                    if (value != null && !cbEdit.Items.Contains(value))
                        list.Add(value);
                }
            }

            list.Sort();
            cbEdit.Items.AddRange(list);

            if (!cbEdit.Items.Contains(control.EditValue))
            {
                if (cbEdit.Items.Count > 0)
                    control.EditValue = cbEdit.Items[0];
                else
                    control.EditValue = null;
            }

            control.EndUpdate();
            control.Refresh();
        }

        private void Initialize()
        {
            _datas = new List<ResultData>();
            _eaInfos = new Dictionary<string, List<EAItem>>();
            _eaMatchInfos = new Dictionary<string, List<EAItem>>();
            _mhInfos = new Dictionary<string, List<MaskHistory>>();
            _eamMatchInfos = new Dictionary<string, List<EAItem>>();
        }

        protected override void Query()
        {
            string areaID = GetAreaIDByProductID(this.TargetProductID);
            if (string.IsNullOrEmpty(areaID) == false)
                this.TargetAreaID = areaID;

            if (this.TargetAreaID == null
                || this.TargetProductID == null || this.TargetStepID == null)
            {
                MessageBox.Show("Invalid Query");
                gridView1.Columns.Clear();
                return;
            }

            LoadData();
            BindData();
        }

        public void Query(string eqpID, string stepID, string productID, string productVersion, string targetDate)
        {
            this.TargetStepID = stepID;
            this.TargetProductID = productID;
            this.TargetDate = targetDate;

            string areaID = GetAreaIDByProductID(this.TargetProductID);
            if (string.IsNullOrEmpty(areaID) == false)
                this.TargetAreaID = areaID;

            RunQuery();
            SetFilter(eqpID, productVersion);
        }

        private void SetFilter(string eqpID, string productVersion)
        {
            var gv = this.gridView1;

            if (gv.ActiveFilter != null)
                gv.ActiveFilter.Clear();

            var condition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Equals;

            if (string.IsNullOrEmpty(eqpID) == false)
                gv.SetAutoFilterValue(gv.Columns[Schema.EQP_ID], eqpID, condition);

            if (string.IsNullOrEmpty(productVersion) == false)
                gv.SetAutoFilterValue(gv.Columns[Schema.PRODUCT_VERSION], productVersion, condition);
        }

        private string GetAreaIDByProductID(string productID)
        {
            if (string.IsNullOrEmpty(productID))
                return null;

            var table = MyHelper.DATASVC.GetEntityData<Product>(this.Result);
            //check product table
            var finds = table.Where(t => t.PRODUCT_ID == productID);
            if (finds != null)
            {
                var list = finds.ToList();
                if (list.Find(t => t.SHOP_ID == ARRAY) != null)
                    return TFT.ToString();
                else if (list.Find(t => t.SHOP_ID == CF) != null)
                    return CF.ToString();
                else if (list.Find(t => t.SHOP_ID == CELL) != null)
                    return CELL.ToString();
            }

            //check product code
            var c = productID[0];
            if (c == 'T')
                return TFT.ToString();
            else if (c == 'F')
                return CF.ToString();
            else if (c == 'C')
                return CELL.ToString();

            return null;
        }

        private void LoadData()
        {
            _datas.Clear();
            _eaInfos.Clear();
            _eaMatchInfos.Clear();
            _eamMatchInfos.Clear();
            _mhInfos.Clear();

            ImportData_EA();
            ImportData_EAM();
            ImportData_MH();

            var datas = _datas;
            var maInfos = ImportData_MA();

            DateTime targetDate = MyHelper.DATE.StringToDateTime(this.TargetDate);

            foreach (var it in maInfos)
            {
                var item = it.Value.FirstOrDefault();
                if (item == null)
                    continue;

                if (IsNeedCheck_EA(item.ShopID, item.StepID))
                {
                    var list = GetMatchedList_EA(item);
                    int count = list == null ? 0 : list.Count;

                    item.Issue_EAM = count == 0;
                    if (item.Issue_EAM == false)
                    {
                        var find = list.FirstOrDefault(t => t.Target.MASK_ID == item.MaskID && t.IsLoadable());
                        if (find == null)
                            item.Issue_EAM = true;
                    }
                }

                if (item.Issue_EAM == false)
                {
                    var list = GetMatchedList_EAM(item.MaskID);
                    if (list != null && list.Count > 0)
                    {
                        //check limit
                        var notloadables = list.FirstOrDefault(t => t.IsLoadable() == false);
                        if (notloadables != null)
                        {
                            item.Issue_EAM = true;
                        }
                    }
                }

                var mhlist = GetMatchedList_MH(item.MaskID, targetDate);
                MaskHistory last = mhlist == null ? null : mhlist.LastOrDefault();
                if (last != null)
                {
                    item.UsedLast = last;
                    item.UsedCount = mhlist.GroupBy(t => t.LOT_ID).Count();
                }

                datas.Add(item);
            }
        }

        private void ImportData_EA()
        {
            var infos = _eaInfos;
            var table = MyHelper.DATASVC.GetEntityData<EqpArrangeLimit>(this.Result);

            if (table == null)
                return;

            foreach (var it in table)
            {
                string eqpID = it.EQP_ID;
                if (string.IsNullOrEmpty(eqpID))
                    continue;

                List<EAItem> list;
                if (infos.TryGetValue(eqpID, out list) == false)
                    infos.Add(eqpID, list = new List<EAItem>());

                EAItem item = new EAItem(it);
                list.Add(item);
            }
        }

        private void ImportData_EAM()
        {
            var infos = _eamMatchInfos;

            var eqpArranges = MyHelper.DATASVC.GetEntityData<EqpArrangeLimit>(this.Result);

            foreach (var it in eqpArranges)
            {
                if (MyHelper.STRING.Equals(it.LIMIT_TYPE, "M") == false)
                    continue;

                string maskID = it.MASK_ID;
                if (string.IsNullOrEmpty(maskID))
                    continue;

                List<EAItem> list;
                if (infos.TryGetValue(maskID, out list) == false)
                    infos.Add(maskID, list = new List<EAItem>());

                EAItem item = new EAItem(it);
                list.Add(item);
            }
        }

        private void ImportData_MH()
        {
            var infos = _mhInfos;
            var table = MyHelper.DATASVC.GetEntityData<MaskHistory>(this.Result);

            if (table == null)
                return;

            foreach (var item in table)
            {
                string toolID = item.TOOL_ID;
                if (string.IsNullOrEmpty(toolID))
                    continue;

                if (item.START_TIME == null || item.END_TIME == null)
                    continue;

                if (toolID == "T6451D02-6BA28CSNH") {
                    Console.WriteLine();
				}

                List<MaskHistory> list;
                if (infos.TryGetValue(toolID, out list) == false)
                    infos.Add(toolID, list = new List<MaskHistory>());

				MyHelper.COMMON.AddSort<MaskHistory>(list, item, (x, y) => Comparer_MH(x, y));
			}
        }

        private Dictionary<string, List<ResultData>> ImportData_MA()
        {
            Dictionary<string, List<ResultData>> datas = new Dictionary<string, List<ResultData>>();

            string targetProductID = this.TargetProductID;
            string targetStepID = this.TargetStepID;

            var table = MyHelper.DATASVC.GetEntityData<ToolArrangeLimit>(this.Result);
            if (table == null)
                return datas;

            foreach (var item in table)
            {
                string eqpID = item.EQP_ID;
                if (string.IsNullOrEmpty(eqpID))
                    continue;

                string stepID = item.STEP_ID;
                if (targetStepID.ToUpper().Trim() != "ALL" && stepID != targetStepID)
                    continue;

                string pridID = item.PRODUCT_ID;
                if (targetProductID.ToUpper().Trim() != "ALL" && pridID != targetProductID)
                    continue;

                List<ResultData> list;
                string key = ResultData.CreateKey(item.EQP_ID, item.STEP_ID, pridID, item.PRODUCT_VERSION, item.TOOL_ID);
                if (datas.TryGetValue(key, out list) == false)
                    datas.Add(key, list = new List<ResultData>());

                ResultData info = new ResultData(item.SHOP_ID,
                                                 eqpID,
                                                 stepID,
                                                 pridID,
                                                 item.PRODUCT_VERSION,
                                                 item.TOOL_ID,
                                                 item.ACTIVATE_TYPE,
                                                 item.PRIORITY);

                list.AddSort(info, ResultData.Comparer.Default);
            }

            return datas;
        }

        private List<EAItem> GetMatchedList_EA(ResultData info)
        {
            var map = _eaMatchInfos;

            string eqpID = info.EqpID;
            if (string.IsNullOrEmpty(eqpID))
                return null;

            string key = info.MatchKey;

            //cache
            List<EAItem> finds;
            if (map.TryGetValue(key, out finds))
                return finds;

            var eaInfos = _eaInfos;
            if (eaInfos == null || eaInfos.Count == 0)
                return null;

            List<EAItem> list;
            if (eaInfos.TryGetValue(eqpID, out list) == false)
                return null;

            if (list == null || list.Count == 0)
                return null;

            finds = list.FindAll(t => t.IsMatched(info));
            if (finds == null && finds.Count == 0)
                return null;

            map.Add(key, finds);

            return finds;
        }

        private bool IsNeedCheck_EA(string shopID, string stepID)
        {
            if (MyHelper.STRING.Equals(shopID, ARRAY) == false)
                return false;

            if (MyHelper.STRING.Equals(stepID, "1300") == false)
                return false;

            return true;
        }

        private List<EAItem> GetMatchedList_EAM(string key)
        {
            var infos = _eamMatchInfos;

            if (infos == null || infos.Count == 0)
                return null;

            List<EAItem> list;
            if (infos.TryGetValue(key, out list))
                return list;

            return null;
        }

        private int Comparer_MH(MaskHistory x, MaskHistory y)
        {
            int cmp = DateTime.Compare(x.START_TIME.GetValueOrDefault(), y.START_TIME.GetValueOrDefault());

            if (cmp == 0)
            {
                cmp = DateTime.Compare(x.END_TIME.GetValueOrDefault(), y.END_TIME.GetValueOrDefault());
            }

            return cmp;
        }

        private List<MaskHistory> GetMatchedList_MH(string maskID, DateTime targetDate)
        {
            var infos = _mhInfos;

            List<MaskHistory> list;
            if (infos.TryGetValue(maskID, out list) == false)
                return null;

            var finds = list.FindAll(t => t.START_TIME <= targetDate && targetDate < t.END_TIME);

            return finds;
        }

        private DataTable CreateSchema()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(Schema.SHOP_ID, typeof(string));
            dt.Columns.Add(Schema.STEP_ID, typeof(string));
            dt.Columns.Add(Schema.PRODUCT_ID, typeof(string));
            dt.Columns.Add(Schema.PRODUCT_VERSION, typeof(string));
            dt.Columns.Add(Schema.EQP_ID, typeof(string));
            dt.Columns.Add(Schema.MASK_ID, typeof(string));

            dt.Columns.Add(Schema.USABLE, typeof(string));
            dt.Columns.Add(Schema.ACTIVATE_TYPE, typeof(string));
            dt.Columns.Add(Schema.EAM_ISSUE, typeof(string));

            dt.Columns.Add(Schema.USED_CNT, typeof(int));
            dt.Columns.Add(Schema.USED_EQP_ID, typeof(string));
            dt.Columns.Add(Schema.USED_LOT_ID, typeof(string));
            dt.Columns.Add(Schema.USED_STEP_ID, typeof(string));
            dt.Columns.Add(Schema.USED_PRODUCT_ID, typeof(string));
            dt.Columns.Add(Schema.USED_START_TIME, typeof(string));
            dt.Columns.Add(Schema.USED_END_TIME, typeof(string));

            return dt;
        }

        private void BindData()
        {
            DataTable dt = CreateSchema();

            var datas = _datas;
            if (datas != null)
            {
                foreach (var item in datas)
                {
                    DataRow row = dt.NewRow();

                    row[Schema.SHOP_ID] = item.ShopID;
                    row[Schema.STEP_ID] = item.StepID;
                    row[Schema.EQP_ID] = item.EqpID;
                    row[Schema.PRODUCT_ID] = item.ProductID;
                    row[Schema.PRODUCT_VERSION] = item.ProductVersion;
                    row[Schema.MASK_ID] = item.MaskID;

                    row[Schema.USABLE] = MyHelper.STRING.ToStringYN(item.IsUsable());
                    row[Schema.ACTIVATE_TYPE] = item.ActivateType;
                    row[Schema.EAM_ISSUE] = MyHelper.STRING.ToStringYN(item.Issue_EAM);

                    row[Schema.USED_CNT] = item.UsedCount;

                    var last = item.UsedLast;
                    if (last != null)
                    {
                        row[Schema.USED_EQP_ID] = last.EQP_ID;
                        row[Schema.USED_LOT_ID] = last.LOT_ID;
                        row[Schema.USED_STEP_ID] = last.STEP_ID;
                        row[Schema.USED_PRODUCT_ID] = last.PRODUCT_ID;

                        DateTime startTime = last.START_TIME.GetValueOrDefault();
                        if (startTime == DateTime.MinValue)
                            startTime = this.PlanStartTime;

                        DateTime endTime = last.END_TIME.GetValueOrDefault();
                        if (endTime == DateTime.MinValue)
                            endTime = this.PlanEndTime;

                        row[Schema.USED_START_TIME] = MyHelper.DATE.DateTimeToString(startTime);
                        row[Schema.USED_END_TIME] = MyHelper.DATE.DateTimeToString(endTime);
                    }

                    dt.Rows.Add(row);
                }
            }

            this.gridControl1.DataSource = dt;

            DesignGrid();
        }

        private void DesignGrid()
        {
            var grid = this.gridView1;

            grid.BestFitColumns();

            MyHelper.ENGCONTROL.SetGridViewColumnWidth(grid); //CONTROL
        }

        public static string CreateKey_Match(string eqpID, string stepID, string productID, string productVer, string maskID)
        {
            string key = MyHelper.STRING.CreateKey(eqpID,
                                                stepID,
                                                productID,
                                                productVer,
                                                maskID);

            return key;
        }

        private static bool HasLimitType(List<LimitType> list, LimitType limitType)
        {
            if (list == null || list.Count == 0)
                return false;

            if (list.Contains(limitType))
                return true;

            return false;
        }

        private static List<LimitType> ParseLimitType(string limitTypeStr)
        {
            List<LimitType> list = new List<LimitType>();

            if (string.IsNullOrEmpty(limitTypeStr))
                return list;

            var arr = limitTypeStr.ToCharArray();
            foreach (var c in arr)
            {
                var limitType = MyHelper.ENUM.ToEnum(c.ToString(), LimitType.NONE);
                if (limitType == LimitType.NONE)
                    continue;

                list.Add(limitType);
            }

            return list;
        }

        public class Schema
        {
            public const string SHOP_ID = "SHOP_ID";
            public const string STEP_ID = "STEP_ID";
            public const string EQP_ID = "EQP_ID";
            public const string PRODUCT_ID = "PRODUCT_ID";
            public const string PRODUCT_VERSION = "PRODUCT_VERSION";
            public const string MASK_ID = "MASK_ID";

            public const string USABLE = "USABLE";
            public const string ACTIVATE_TYPE = "ACTIVATE_TYPE";
            public const string EAM_ISSUE = "EAM_ISSUE";

            public const string USED_CNT = "USED_CNT";
            public const string USED_EQP_ID = "USED_EQP_ID";
            public const string USED_LOT_ID = "USED_LOT_ID";
            public const string USED_STEP_ID = "USED_STEP_ID";
            public const string USED_PRODUCT_ID = "USED_PRODUCT_ID";
            public const string USED_START_TIME = "USED_START_TIME";
            public const string USED_END_TIME = "USED_END_TIME";
        }

        public class ResultData
        {
            public string MatchKey { get; set; }

            public string ShopID { get; private set; }
            public string EqpID { get; private set; }
            public string StepID { get; private set; }
            public string ProductID { get; private set; }
            public string ProductVersion { get; private set; }
            public string MaskID { get; private set; }
            public string ActivateType { get; private set; }
            public int Priority { get; private set; }

            public bool Issue_EAM { get; set; }

            public MaskHistory UsedLast { get; set; }
            public int UsedCount { get; set; }

            public ResultData(string shopID, string eqpID, string stepID,
                string productID, string productVer, string maskID, string activateType, int priority)
            {
                this.ShopID = shopID;
                this.EqpID = eqpID;
                this.StepID = stepID;
                this.ProductID = productID;
                this.ProductVersion = productVer;
                this.MaskID = maskID;
                this.ActivateType = activateType;
                this.Priority = priority;

                this.MatchKey = CreateKey_Match(this.EqpID,
                                this.StepID,
                                this.ProductID,
                                this.ProductVersion,
                                this.MaskID);
            }

            public bool IsUsable()
            {
                if (MyHelper.STRING.Equals(this.ActivateType, "Y") == false)
                    return false;

                if (this.Issue_EAM)
                    return false;

                if (this.UsedCount > 0)
                    return false;

                return true;
            }

            public static string CreateKey(string eqpID, string stepID, string productID, string productVer, string maskID)
            {
                string key = MyHelper.STRING.CreateKey(eqpID,
                                                    stepID,
                                                    productID,
                                                    productVer,
                                                    maskID);

                return key;
            }

            public class Comparer : IComparer<ResultData>
            {
                public int Compare(ResultData x, ResultData y)
                {
                    if (object.ReferenceEquals(x, y))
                        return 0;

                    int cmp = x.Priority.CompareTo(y.Priority);

                    return cmp;
                }

                public static IComparer<ResultData> Default = new Comparer();
            }
        }

        private class EAItem
        {
            public string Key { get; set; }
            public EqpArrangeLimit Target { get; set; }

            public List<LimitType> LimitTypeList { get; set; }

            public EAItem(EqpArrangeLimit item)
            {
                this.Target = item;

                this.Key = item.EQP_ID;
                this.LimitTypeList = ParseLimitType(item.LIMIT_TYPE);
            }

            public bool IsMatched(ResultData info)
            {
                var item = this.Target;
                var limitTypeList = this.LimitTypeList;

                if (HasLimitType(limitTypeList, LimitType.L))
                {
                    if (IsMatched(item.EQP_ID, info.EqpID) == false)
                        return false;
                }

                if (HasLimitType(limitTypeList, LimitType.O))
                {
                    if (IsMatched(item.STEP_ID, info.StepID) == false)
                        return false;
                }

                if (HasLimitType(limitTypeList, LimitType.P))
                {
                    if (IsMatched(item.PRODUCT_ID, info.ProductID) == false)
                        return false;
                }

                if (HasLimitType(limitTypeList, LimitType.B))
                {
                    if (IsMatched(item.PRODUCT_VERSION, info.ProductVersion) == false)
                        return false;
                }

                return true;
            }

            public bool IsLoadable()
            {
                var item = this.Target;

                if (MyHelper.STRING.Equals(item.ACTIVATE_TYPE, "Y") || MyHelper.STRING.Equals(item.ACTIVATE_TYPE, "M"))
                    return true;

                return false;
            }

            private bool IsMatched(string arrangeStr, string targetStr)
            {
                if (MyHelper.STRING.Equals(arrangeStr, "ALL"))
                    return true;

                return arrangeStr == targetStr;
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e == null || e.Column == null)
                return;

            string usable = (string)this.gridView1.GetRowCellValue(e.RowHandle, Schema.USABLE);
            if (MyHelper.STRING.Equals(usable, "Y") == false)
                return;

            //Color custPurple = Color.FromArgb(242, 243, 255);
            Color custYellow = Color.FromArgb(252, 254, 203);
            //Color custSky = Color.FromArgb(233, 251, 254);

            e.Appearance.BackColor = custYellow;
        }

		private void btnQeury_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
            RunQuery();
		}

		private void editAreaId_EditValueChanged(object sender, EventArgs e)
        {
            //this.SetControl_ProductID(this.editProdId);
            //this.SetControl_StepID(this.editStepId);
        }

    }
}
