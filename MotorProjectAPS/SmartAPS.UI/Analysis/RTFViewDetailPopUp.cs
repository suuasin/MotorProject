using Mozart.Data.Entity;
using Mozart.Studio.TaskModel.Projects;
using SmartAPS.Outputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SmartAPS.UI.Analysis
{
    public partial class RTFViewDetailPopUp : MyXtraGridTemplate
    {
        string DemandID;
        private IEnumerable<STEP_TARGET> _stepTarget; // step target 가져오기
        private IEnumerable<PEG_HISTORY> _pegHistory; //peg history 가져오기

        public RTFViewDetailPopUp()
        {
            InitializeComponent();
        }

        public RTFViewDetailPopUp(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }


        public RTFViewDetailPopUp(IExperimentResultItem result, string demandID)
        {
            InitializeComponent();

            this.Result = result;
            this.DemandID = demandID;
            LoadDocument();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();
            RunQuery();
        }

        protected override void Query()
        {
            LoadData();
            BindTable();
        }

        private void LoadData()
        {
            this.barStaticItem1.Caption = String.Format("DEMAND_ID : {0}", DemandID);
            _stepTarget = MyHelper.DATASVC.GetEntityData<STEP_TARGET>(this.Result);
            _pegHistory = MyHelper.DATASVC.GetEntityData<PEG_HISTORY>(this.Result);
        }

        private void BindTable()
        {

            var stepTargets = _stepTarget.Where(x => x.MO_DEMAND_ID == this.DemandID);
            //var pegHistorys = _pegHistory.Where(x => x.MO_DEMAND_ID == stepTargets.FirstOrDefault().MO_DEMAND_ID && x.STEP_ID == stepTargets.FirstOrDefault().STEP_ID && x.PRODUCT_ID == stepTargets.FirstOrDefault().PRODUCT_ID);

            DataTable dt = new DataTable();
            dt.Columns.Add("PRODUCT_ID", typeof(string));
            dt.Columns.Add("STEP_ID", typeof(string));
            dt.Columns.Add("IN_QTY", typeof(double));
            dt.Columns.Add("OUT_QTY", typeof(double));
            dt.Columns.Add("PEG_QTY", typeof(double));
            dt.Columns.Add("TARGET_DATE", typeof(DateTime));
            dt.Columns.Add("MO_DEMAND_ID", typeof(string));
            dt.Columns.Add("MO_PRODUCT_ID", typeof(string));
            dt.Columns.Add("MO_DUE_DATE", typeof(DateTime));



            foreach (var step in stepTargets)
            {
                var pegHistorys = _pegHistory.Where(x => x.MO_DEMAND_ID == step.MO_DEMAND_ID && x.STEP_ID == step.STEP_ID && x.PRODUCT_ID == step.PRODUCT_ID && step.IN_OUT == x.IN_OUT);

                DataRow row = dt.NewRow();
                row["PRODUCT_ID"] = step.PRODUCT_ID;
                row["STEP_ID"] = step.STEP_ID;
                row["IN_QTY"] = step.IN_QTY;
                row["OUT_QTY"] = step.OUT_QTY;
                //row["PEG_QTY"] = pegHistorys.Count() > 0 ? pegHistorys.FirstOrDefault().PEG_QTY : null;
                row["TARGET_DATE"] = step.TARGET_DATE;
                row["MO_DEMAND_ID"] = step.MO_DEMAND_ID;
                row["MO_PRODUCT_ID"] = step.MO_PRODUCT_ID;
                row["MO_DUE_DATE"] = step.MO_DUE_DATE;
                
                if (pegHistorys.Count() > 0)
                {
                    //row["PEG_QTY"] = pegHistorys.FirstOrDefault().PEG_QTY;
                    row["PEG_QTY"] = pegHistorys.Sum(x => x.PEG_QTY); 
                }

                dt.Rows.Add(row);
            }

            gridDetail.DataSource = dt;
            //SetGridDesign();
            gridDetail.EndUpdate();

        }



        //private DataTable ToDataTable<T>(IEnumerable<T> list)
        //{
        //	if (list == null)
        //		return null;
        //	PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
        //	DataTable table = new DataTable();
        //	for (int i = 0; i < props.Count; i++)
        //	{
        //		PropertyDescriptor prop = props[i];
        //		table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //	}
        //	object[] values = new object[props.Count];
        //	foreach (T item in list)
        //	{
        //		for (int i = 0; i < values.Length; i++)
        //			values[i] = props[i].GetValue(item) ?? DBNull.Value;
        //		table.Rows.Add(values);
        //	}
        //	return table;
        //}

        //private string[] SplitEqpID(string eqp_id)
        //{
        //	string chamberId;
        //	string eqpId;
        //	List<string> idList = new List<string>();

        //	string[] id = eqp_id.Split('-');
        //	eqpId = id[0];
        //	if (id.Length > 1)
        //	{
        //		string[] arr = id[1].Split('(');
        //		chamberId = arr[0];
        //	}
        //	else chamberId = eqpId;

        //	idList.Add(eqpId);
        //	idList.Add(chamberId);

        //	return idList.ToArray();
        //}
    }
}

