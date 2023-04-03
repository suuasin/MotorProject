using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SmartAPS.MozartStudio.Gantts;
using SmartAPS.MozartStudio.Helper;
using SmartAPS.MozartStudio.Utils;
using Template.Lcd.Scheduling.Outputs;
using SmartAPS.UserLibrary.Extensions;
using SmartAPS.UserLibrary.Utils;
using static SmartAPS.MozartStudio.Analysis.DispatchingAnalysisView;

namespace SmartAPS.MozartStudio.Analysis
{
	public partial class DispatchingLogView : MyXtraGridTemplate
    {
        private List<DispatchingInfo> _infos;

        private EqpMaster EqpMgr { get; set; }

        private string TargetShopID
        {
            get
            {
                return this.editShopId.EditValue as string;
            }
            
        }
 
        private DateTime PlanStartTime
        {
            get
            {
                return MyHelper.ENGCONTROL.GetPlanStartTime(this.Result);
            }
        }

        private DateTime FromTime
        {
            get
            {
                return Convert.ToDateTime(this.editDateTime.EditValue).Date;
            }
        }

        private DateTime ToTime
        {
            get
            {
                return this.FromTime.AddDays(Convert.ToDouble(this.editDateSpin.EditValue));
            }
        }

        public DispatchingLogView()
        {
            InitializeComponent();
        }

        public DispatchingLogView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            Initialize();

            this.SetInitializeOption(this.gridControl1);

            SetControls();

            RunQuery();
        }

        private void Initialize()
        {
            GetPreprocessData();
        }

        private void SetControls()
        {
            this.editDateTime.EditValue = this.PlanStartTime;

            this.editDateSpin.EditValue = 1;

            MyHelper.ENGCONTROL.SetControl_ShopID(this.editShopId, this.Result);
            MyHelper.ENGCONTROL.SetControl_EqpGroup_Checked(this.editEqpGroup, this.TargetShopID, this.Result);
        }

        private void GetPreprocessData()
        {
            this.EqpMgr = new EqpMaster(this.Result);
            this.EqpMgr.LoadEqp();
        }

        private List<DispatchingInfo> GetDispatchingInfo()
        {
            if (_infos != null)
                return _infos;

            var list = _infos = new List<DispatchingInfo>();

            var table = MyHelper.DATASVC.GetEntityData<EqpDispatchLog>(this.Result);
            if (table == null || table.Count() == 0)
                return list;

            foreach (EqpDispatchLog row in table)
            {
                var item = new DispatchingInfo(row);
                
                list.Add(item);
            }

            return list;
        }


        protected override void Query()
        {
            BindGrid();

            DesignGrid();
        }

        private DataTable CreateSchema()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(Schema.SHOP_ID, typeof(string));
            dt.Columns.Add(Schema.EQP_GROUP, typeof(string));
            dt.Columns.Add(Schema.EQP_ID, typeof(string));
            dt.Columns.Add(Schema.SUB_EQP_ID, typeof(string));

            dt.Columns.Add(Schema.PRESET_ID, typeof(string));
            dt.Columns.Add(Schema.DISPATCHING_TIME, typeof(string));

            dt.Columns.Add(Schema.SELECTED_LOT, typeof(string));
            dt.Columns.Add(Schema.SELECTED_PRODDUCT, typeof(string));
            dt.Columns.Add(Schema.SELECTED_STEP, typeof(string));

            dt.Columns.Add(Schema.INIT_WIP_CNT, typeof(int));
            dt.Columns.Add(Schema.FILTERED_WIP_CNT, typeof(int));
            dt.Columns.Add(Schema.SELECTED_WIP_CNT, typeof(int));

            dt.Columns.Add(Schema.INFO_TYPE, typeof(string));
            dt.Columns.Add(Schema.LOT_ID, typeof(string));
            dt.Columns.Add(Schema.PRODUCT_ID, typeof(string));
            dt.Columns.Add(Schema.PRODUCT_VER, typeof(string));
            dt.Columns.Add(Schema.STEP_ID, typeof(string));
            dt.Columns.Add(Schema.LOT_QTY, typeof(int));
            dt.Columns.Add(Schema.PRODUCTION_TYPE, typeof(string));
            dt.Columns.Add(Schema.DETAIL, typeof(string));

            dt.Columns.Add(Schema.F_REASON, typeof(string));

            dt.Columns.Add(Schema.W_SEQ, typeof(int));
            dt.Columns.Add(Schema.W_SCORE_SUM, typeof(float));

            return dt;
        }

        new private List<DispatchingInfo> GetData()
        {
            var infos = GetDispatchingInfo();

            if (infos == null || infos.Count == 0)
                return infos;

            DateTime fromTime = this.FromTime;
            DateTime toTime = this.ToTime;

            string targetShopID = this.TargetShopID;
            List<string> eqpGroupList = (this.editEqpGroup.Edit as RepositoryItemCheckedComboBoxEdit).GetCheckedItemsToList();

            var list = infos.FindAll(t => IsMatched(t, fromTime, toTime, targetShopID, eqpGroupList));

            return list;
        }

        private bool IsMatched(DispatchingInfo info, DateTime fromTime, DateTime toTime, string targetShopID, List<string> eqpGroupList)
        {
            DateTime t = MyHelper.DATE.StringToDateTime(info.DispatchingTime);
            if (t < fromTime || t >= toTime)
                return false;         

            bool isAll = MyHelper.STRING.Equals(targetShopID, "ALL");
            if(isAll == false)
            {
                if (info.ShopID != targetShopID)
                    return false;
            }

            //if (eqpGroupList == null || eqpGroupList.Count == 0)
            //    return false;

            if (!String.IsNullOrWhiteSpace(info.EqpGroupID) && eqpGroupList.Contains(info.EqpGroupID) == false)
                return false;

            return true;
        }

        private void BindGrid()
        {
            var dt = CreateSchema();

            var list = GetData();

            foreach (DispatchingInfo item in list)
            {
                bool existF = AddRow_Filter(item, dt);
                bool existW = AddRow_WeightFactor(item, dt);

                if (existF == false && existW == false)
                    AddRow_Base(item, dt);
            }

            this.gridControl1.DataSource = dt;
        }

        private DataRow AddRow_Base(DispatchingInfo item, DataTable dt)
        {
            DataRow row = dt.NewRow();

            row[Schema.SHOP_ID] = item.ShopID;
            row[Schema.EQP_GROUP] = item.EqpGroupID;
            row[Schema.EQP_ID] = item.EqpID;
            row[Schema.SUB_EQP_ID] = item.SubEqpID;
            row[Schema.PRESET_ID] = item.PresetID;

            row[Schema.DISPATCHING_TIME] = item.DispatchingTime;
            row[Schema.SELECTED_LOT] = item.SeleLotID;
            row[Schema.SELECTED_PRODDUCT] = item.SeleProdID;
            row[Schema.SELECTED_STEP] = item.SeleStepID;

            row[Schema.INIT_WIP_CNT] = item.InitWipCnt;
            row[Schema.FILTERED_WIP_CNT] = item.FilteredWipCnt;
            row[Schema.SELECTED_WIP_CNT] = item.SelectedWipCnt;

            dt.Rows.Add(row);

            return row;
        }

        private bool AddRow_Filter(DispatchingInfo item, DataTable dt)
        {
            var list = FilteredInfo.Parse(item.FilteredWipLog);
            if (list == null || list.Count == 0)
                return false;

            foreach (var it in list)
            {
                DataRow row = AddRow_Base(item, dt);
                
                row[Schema.LOT_ID] = it.LotID;
                row[Schema.STEP_ID] = it.StepID;
                row[Schema.PRODUCT_ID] = it.ProductID;
                row[Schema.PRODUCT_VER] = it.ProductVersion;
                row[Schema.LOT_QTY] = it.LotQty;
                row[Schema.PRODUCTION_TYPE] = it.ProductionType;
                row[Schema.DETAIL] = it.Detail;

                row[Schema.INFO_TYPE] = "FILTERED_WIP";
                row[Schema.F_REASON] = it.Reason;
            }           

            return true;
        }
        
        private bool AddRow_WeightFactor(DispatchingInfo item, DataTable dt)
        {
            var list = WeightFactorInfo.Parse(item.DispatchWipLog);
            if (list == null || list.Count == 0)
                return false;

            foreach (var it in list)
            {
                DataRow row = AddRow_Base(item, dt);
                
                row[Schema.LOT_ID] = it.LotID;
                row[Schema.STEP_ID] = it.StepID;
                row[Schema.PRODUCT_ID] = it.ProductID;
                row[Schema.PRODUCT_VER] = it.ProductVersion;
                row[Schema.LOT_QTY] = it.LotQty;
                row[Schema.PRODUCTION_TYPE] = it.ProductionType;
                row[Schema.DETAIL] = it.Detail;

                row[Schema.INFO_TYPE] = "WEIGHT_FACTOR";
                row[Schema.W_SEQ] = it.Seq;
                row[Schema.W_SCORE_SUM] = it.Sum;
            }

            return true;
        }

        private void DesignGrid()
        {
			//this.gridView1.Columns[Schema.DISPATCHING_TIME].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			//this.gridView1.Columns[Schema.DISPATCHING_TIME].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
			//this.gridView1.Columns[Schema.DISPATCHING_TIME].Resize(135);

			this.gridView1.Columns[Schema.INIT_WIP_CNT].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView1.Columns[Schema.INIT_WIP_CNT].DisplayFormat.FormatString = "###,###";
            this.gridView1.Columns[Schema.FILTERED_WIP_CNT].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView1.Columns[Schema.FILTERED_WIP_CNT].DisplayFormat.FormatString = "###,###";
            this.gridView1.Columns[Schema.SELECTED_WIP_CNT].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView1.Columns[Schema.SELECTED_WIP_CNT].DisplayFormat.FormatString = "###,###";

            this.gridView1.OptionsView.ColumnAutoWidth = false;
            foreach (GridColumn column in gridView1.Columns)
            {
                if (column.GetBestWidth() < 300)
                    column.Width = column.GetBestWidth();
                else
                    column.Width = 300;

            }
        }

        #region Event

        #endregion

        private class Schema
        {
            public const string SHOP_ID = "SHOP_ID";
            public const string EQP_GROUP = "EQP_GROUP";
            public const string EQP_ID = "EQP_ID";
            public const string SUB_EQP_ID = "SUB_EQP_ID";

            public const string PRESET_ID = "PRESET_ID";
            public const string DISPATCHING_TIME = "DISPATCHING_TIME";

            public const string SELECTED_LOT = "SELECTED_LOT";
            public const string SELECTED_PRODDUCT = "SELECTED_PRODDUCT";
            public const string SELECTED_STEP = "SELECTED_STEP";

            public const string INIT_WIP_CNT = "INIT_WIP_CNT";
            public const string FILTERED_WIP_CNT = "FILTERED_WIP_CNT";
            public const string SELECTED_WIP_CNT = "SELECTED_WIP_CNT";

            public const string INFO_TYPE = "INFO_TYPE";
            public const string LOT_ID = "LOT_ID";
            public const string PRODUCT_ID = "PRODUCT_ID";
            public const string PRODUCT_VER = "PRODUCT_VER";
            public const string STEP_ID = "STEP_ID";
            public const string LOT_QTY = "LOT_QTY";
            public const string PRODUCTION_TYPE = "PRODUCTION_TYPE";
            public const string DETAIL = "DETAIL";

            public const string F_REASON = "F_REASON";

            public const string W_SEQ = "W_SEQ";
            public const string W_SCORE_SUM = "W_SCORE_SUM";
        }

        public class FilteredInfo
        {
            public string LotID { get; private set; }
            public string StepID { get; private set; }
            public string ProductID { get; private set; }
            public string ProductVersion { get; private set; }
            public int LotQty { get; private set; }
            public string ProductionType { get; private set; }
            public string Detail { get; private set; }
            public string Reason { get; private set; }

            public FilteredInfo(string log, string reason)
            {
                if (string.IsNullOrEmpty(log))
                    return;

                string[] arr = log.Split('/');

                int count = arr.Length;

                string lotID = arr[0];
                string productID = count >= 2 ? arr[1] : Consts.NULL_ID;
                string productVer = count >= 3 ? arr[2] : Consts.NULL_ID;
                string stepID = count >= 4 ? arr[3] : Consts.NULL_ID;
                int qty = count >= 5 ? !String.IsNullOrWhiteSpace(arr[4]) ? Convert.ToInt32(arr[4]) : 0 : 0;
                string productionType = count >= 6 ? arr[5] : Consts.NULL_ID;
                string detail = count >= 7 ? arr[6] : Consts.NULL_ID;

                this.LotID = lotID;
                this.StepID = stepID;
                this.ProductID = productID;
                this.ProductVersion = productVer;
                this.LotQty = qty;
                this.ProductionType = productionType;
                this.Detail = detail;

                this.Reason = reason;
            }

            public static List<FilteredInfo> Parse(string log)
            {
                List<FilteredInfo> list = new List<FilteredInfo>();
                if (string.IsNullOrEmpty(log))
                    return list;

                string[] arr = log.Split('\t');
                foreach (string str in arr)
                {
                    string[] group = str.Split(':');

                    if (group.Length < 2)
                        continue;

                    string reason = group[0];
                    string[] wips = group[1].Split(';');

                    foreach (string wip in wips)
                    {
                        FilteredInfo item = new FilteredInfo(wip, reason);
                        list.Add(item);
                    }
                }

                return list;
            }
        }

        public class WeightFactorInfo
        {
            public string LotID { get; private set; }
            public string StepID { get; private set; }
            public string ProductID { get; private set; }
            public string ProductVersion { get; private set; }
            public int LotQty { get; private set; }
            public string ProductionType { get; private set; }
            public string Detail { get; private set; }
            public int Seq { get; private set; }
            public float Sum { get; private set; }

            public WeightFactorInfo(string log)
            {
                if (string.IsNullOrEmpty(log))
                    return;

                string[] arr = log.Split('/');

                int count = arr.Length;

                string lotID = arr[0];
                string productID = count >= 2 ? arr[1] : Consts.NULL_ID;
                string productVer = count >= 3 ? arr[2] : Consts.NULL_ID;
                string stepID = count >= 4 ? arr[3] : Consts.NULL_ID;
                int qty = count >= 5 ? Convert.ToInt32(arr[4]) : 0;
                string productionType = count >= 6 ? arr[5] : Consts.NULL_ID;
                string detail = count >= 7 ? arr[6] : Consts.NULL_ID;

                this.LotID = lotID;
                this.StepID = stepID;
                this.ProductID = productID;
                this.ProductVersion = productVer;
                this.LotQty = qty;
                this.ProductionType = productionType;
                this.Detail = detail;
            }

            public static List<WeightFactorInfo> Parse(string log)
            {
                List<WeightFactorInfo> list = new List<WeightFactorInfo>();
                if (string.IsNullOrEmpty(log))
                    return list;

                string[] arr = log.Split(';');

                int count = arr.Length;
                for (int i = 0; i < count; i++)
                {
                    string str = arr[i];

                    var item = new WeightFactorInfo(str);

                    item.Seq = i + 1;
                    item.Sum = GetWeightSum(str);

                    list.Add(item);
                }

                return list;
            }

            private static float GetWeightSum(string log)
            {
                if (string.IsNullOrEmpty(log))
                    return 0;

                string[] arr = log.Split('/');

                int count = arr.Length;
                int startIndex = 7;

                float sum = 0;
                for (int i = startIndex; i < count; i++)
                {
                    var str = arr[i];
                    var values = str.Split('@');

                    //string desc = string.Empty;
                    //if (arrVaules.Length > 1)
                    //    desc = arrVaules[1];

                    float score = MyHelper.CONVERT.ToFloat(values[0]);

                    //마이너스(-) 값 존재시 대상제외 (sum -1 표시로 처리)
                    if (score < 0)
                        return score;

                    sum += score;
                }

                return sum;
            }
        }

        private void buttonLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunQuery();
        }

		private void buttonExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MyHelper.GRIDVIEWEXPORT.ExportToExcel(this.gridView1);
        }


        private void editShopId_SelectedIndexChanged(object sender, EventArgs e)
        {
            MyHelper.ENGCONTROL.SetControl_EqpGroup_Checked(this.editEqpGroup, this.TargetShopID, this.Result);
        }

	}
}
