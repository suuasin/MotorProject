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

namespace SmartAPS.UI.Gantts
{
	public partial class EqpGanttPopUpView : MyXtraGridTemplate
	{

		string TargetVersionNo;
		string TargetEqpId;
		string TargetChamberId;
		string TargetId;
		DataTable eqpDt;
		//DataTable eqpStatDt;

		public EqpGanttPopUpView()
		{
			InitializeComponent();
		}

		public EqpGanttPopUpView(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			InitializeComponent();
		}

		public EqpGanttPopUpView(IExperimentResultItem result, string targetId)
		{
			InitializeComponent();

			this.Result = result;
			this.TargetId = targetId;
			string[] idArr = SplitEqpID(targetId);
			this.TargetEqpId = idArr[0];
			this.TargetChamberId = idArr[1];

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
			this.barStaticItem1.Caption = String.Format("EQP_ID : {0}", TargetId);
			//var engStat = LoadEqpStat();
			//eqpStatDt = ToDataTable(engStat);
			var engEqp = LoadEqp();
			eqpDt = ToDataTable(engEqp);
		}

		private void BindTable()
		{

			vGridControl1.RowHeaderWidth = 150;
			vGridControl1.RecordWidth = vGridControl1.Width - vGridControl1.RowHeaderWidth;

			AddNewCategoryRow("ENG_EQP");
			AddNewCategoryRow("ENG_EQP_STATUS");

			if (eqpDt == null)
				return;

			//List<DataColumn> eqpStatlist = new List<DataColumn>();
			//foreach (DataColumn it in eqpStatDt.Columns)
			//{
			//	if (!eqpDt.Columns.Contains(Convert.ToString(it)))
			//	{
			//		eqpStatlist.Add(it);
			//	}
			//}

			foreach (DataColumn item in eqpDt.Columns)
			{
				AddNewChildRow(Convert.ToString(item), "ENG_EQP");
			}
			//foreach (DataColumn item in eqpStatlist)
			//{
			//	AddNewChildRow(Convert.ToString(item), "ENG_EQP_STATUS");
			//}

			foreach (DataColumn eqpDc in eqpDt.Columns)
			{
				foreach (DataRow eqpDr in eqpDt.Rows)
				{
					vGridControl1.SetCellValue(vGridControl1.GetRowByCaption(Convert.ToString(eqpDc)), 0, eqpDr[eqpDc]);

					if ((vGridControl1.GetRowByCaption(Convert.ToString(eqpDc)).Properties.RowType.Name) == "DateTime")
					{
						vGridControl1.GetRowByCaption(Convert.ToString(eqpDc)).Properties.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
						vGridControl1.GetRowByCaption(Convert.ToString(eqpDc)).Properties.Format.FormatString = "yyyy-MM-dd HH:mm:ss";
					}
				}
			}

			//foreach (DataColumn statDc in eqpStatDt.Columns)
			//{
			//	if (eqpDt.Columns.Contains(statDc.ToString()))
			//		continue;
			//	foreach (DataRow statDr in eqpStatDt.Rows)
			//	{
			//		vGridControl1.SetCellValue(vGridControl1.GetRowByCaption(Convert.ToString(statDc)), 0, statDr[statDc]);

			//		if ((vGridControl1.GetRowByCaption(Convert.ToString(statDc)).Properties.RowType.Name) == "DateTime")
			//		{
			//			vGridControl1.GetRowByCaption(Convert.ToString(statDc)).Properties.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
			//			vGridControl1.GetRowByCaption(Convert.ToString(statDc)).Properties.Format.FormatString = "yyyy-MM-dd HH:mm:ss";
			//		}
			//	}
			//}
		}

		private IEnumerable<EQUIPMENT> LoadEqp()
		{
			var dt = MyHelper.DATASVC.GetEntityData<EQUIPMENT>(this.Result);
			var find = dt.Where(t => t.EQP_ID as string == TargetEqpId);

			return find;
		}

		//private IEnumerable<EqpStatus> LoadEqpStat()
		//{
		//	var dt = MyHelper.DATASVC.GetEntityData<EqpStatus>(this.TargetVersionNo);
		//	var find = dt.Where(t => t.EQP_ID as string == TargetChamberId);

		//	return find;
		//}

		private void AddNewCategoryRow(string caption = "")
		{
			CategoryRow newCategoryRow = new CategoryRow(caption) { Height = 30 };
			newCategoryRow.Properties.Caption = caption;
			vGridControl1.Rows.Add(newCategoryRow);
		}

		private void AddNewChildRow(string childRowCaption, string categoryRowCaption = "")
		{
			EditorRow childRow = new EditorRow(childRowCaption) { Height = 30 };
			childRow.Properties.Caption = childRowCaption;
			childRow.Properties.Value = "";
			childRow.Name = childRowCaption;
			BaseRow parentRow = vGridControl1.Rows["category" + categoryRowCaption];
			vGridControl1.Rows.Add(childRow);
			vGridControl1.MoveRow(childRow, parentRow, false);
			childRow.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			vGridControl1.BestFit();
		}

		private DataTable ToDataTable<T>(IEnumerable<T> list)
		{
			if (list == null)
				return null;
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
			DataTable table = new DataTable();
			for (int i = 0; i < props.Count; i++)
			{
				PropertyDescriptor prop = props[i];
				table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
			}
			object[] values = new object[props.Count];
			foreach (T item in list)
			{
				for (int i = 0; i < values.Length; i++)
					values[i] = props[i].GetValue(item) ?? DBNull.Value;
				table.Rows.Add(values);
			}
			return table;
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

		private void vGridControl1_Resize(object sender, EventArgs e)
		{
			vGridControl1.RecordWidth = vGridControl1.Width - vGridControl1.RowHeaderWidth;
		}
	}
}

