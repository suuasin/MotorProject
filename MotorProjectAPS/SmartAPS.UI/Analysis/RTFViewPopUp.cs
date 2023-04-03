using DevExpress.XtraVerticalGrid.Rows;
using Mozart.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using SmartAPS.Inputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using SmartAPS.UI.Properties;
using Mozart.Studio.TaskModel.Projects;
using SmartAPS.Outputs;

namespace SmartAPS.UI.Analysis
{
	public partial class RTFViewPopUp : MyXtraGridTemplate
	{
        string ProductID;
        private IEnumerable<ERROR_LOG> _errorLog; //에러로그가져오기
	

		public RTFViewPopUp()
		{
			InitializeComponent();
		}

		public RTFViewPopUp(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			InitializeComponent();
		}


		public RTFViewPopUp(IExperimentResultItem result,  string productID)
		{
			InitializeComponent();

			this.Result = result;
            this.ProductID = productID;
            //string[] idArr = SplitEqpID(targetId);
            //this.TargetEqpId = idArr[0];
            //this.TargetChamberId = idArr[1];

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
			this.barStaticItem1.Caption = String.Format("PRODUCT_ID : {0}", ProductID);
            _errorLog = MyHelper.DATASVC.GetEntityData<ERROR_LOG>(this.Result);
        }

        private void BindTable()
        {

            var errorLogs = _errorLog.Where(x => x.PRODUCT_ID == this.ProductID); //PRODUCTID를 기준으로 가져오기

            DataTable dt = new DataTable();
            dt.Columns.Add("ERR_CODE", typeof(string));
            dt.Columns.Add("SEVERITY", typeof(string));
            dt.Columns.Add("CATEGORY", typeof(string));
            dt.Columns.Add("REASON", typeof(string));
            dt.Columns.Add("ITEM", typeof(string));
            dt.Columns.Add("DEMAND_ID", typeof(string));
            //dt.Columns.Add("PRODUCT_ID", typeof(string));
            dt.Columns.Add("STEP_ID", typeof(string));
            dt.Columns.Add("EQP_ID", typeof(string));
            dt.Columns.Add("LOT_ID", typeof(string));
            dt.Columns.Add("LOG_TIME", typeof(DateTime));
            dt.Columns.Add("ERROR_LOG_ID", typeof(string));


            foreach (var error in errorLogs)
            {
             
                DataRow row = dt.NewRow();
                row["ERR_CODE"] = error.ERR_CODE;
                row["SEVERITY"] = error.SEVERITY;
                row["CATEGORY"] = error.CATEGORY;
                row["REASON"] = error.REASON;
                row["ITEM"] = error.ITEM;
                row["DEMAND_ID"] = error.DEMAND_ID;
                //row["PRODUCT_ID"] = error.PRODUCT_ID;
                row["STEP_ID"] = error.STEP_ID;
                row["EQP_ID"] = error.EQP_ID;
                row["LOT_ID"] = error.LOT_ID;
                row["LOG_TIME"] = error.LOG_TIME ;
                row["ERROR_LOG_ID"] = error.ERROR_LOG_ID;

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

