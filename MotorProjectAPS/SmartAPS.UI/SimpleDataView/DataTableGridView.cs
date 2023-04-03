using DevExpress.XtraGrid.Views.Grid;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Task.Model;
using System;
using System.Collections.Generic;
using System.Data;
using SmartAPS.UI.Utils;

namespace SmartAPS.UI.SimpleDataView
{
	public partial class DataTableGridView : MyXtraGridTemplate
    {
        private IExperimentResultItem Result { get; set; }
        Dictionary<string, TableCategory> TableInfos = new Dictionary<string, TableCategory>();
        private string SelectedTableName { get; set; }

        public DataTableGridView()
        {
            InitializeComponent();
        }

        public DataTableGridView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            this.Result = this.Document.GetResultItem();

            if (this.Result == null)
                return;

            InitializeControl();
        }

        private void InitializeControl()
        {
            SetInitializeOption(this.gridControl1);

            SetTableComboBoxItem();
        }

        private void SetTableComboBoxItem()
        {
            ModelEngine engine = this.Result.Model.TargetObject as ModelEngine;
            if (engine == null)
                return;

            List<string> inputItems = new List<string>();

            foreach (var item in engine.Inputs.GetItems())
            {
                this.TableInfos.Add(item.Name, TableCategory.INPUT);
                inputItems.Add(item.Name);
            }

            List<string> outputItems = new List<string>();

            foreach (var item in engine.Outputs.GetItems())
            {
                this.TableInfos.Add(item.Name, TableCategory.OUTPUT);
                outputItems.Add(item.Name);
            }

            inputItems.Sort((x, y) => x.CompareTo(y));
            outputItems.Sort((x, y) => x.CompareTo(y));

            this.repoTable.Items.AddRange(inputItems);
            this.repoTable.Items.AddRange(outputItems);

            this.editTable.EditValue = this.repoTable.Items[0];

            this.SelectedTableName = this.editTable.EditValue.ToString();
        }

        private void barButtonItemQuery_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Query();
        }

        protected override DataTable GetData()
        {
            string tableName = this.editTable.EditValue.ToString();

            this.SelectedTableName = tableName;

            DataTable dataTable = GetDataTable(tableName);

            return dataTable;
        }

        public override string GetLayoutID(GridView grid)
        {
            return string.Format("{0}{1}{2}", this.GetType().Name, grid.Name, this.SelectedTableName);
        }

        private DataTable GetDataTable(string tableName)
        {
            DataTable dt = null;

            if (string.IsNullOrEmpty(tableName))
                return dt;

            TableCategory category;
            if (this.TableInfos.TryGetValue(tableName, out category) == false)
                return dt;

            if (category == TableCategory.INPUT)
                dt = this.Result.LoadInput(tableName);
            else
                dt = this.Result.LoadOutput(tableName);

            return dt;
        }

        public enum TableCategory
        {
            INPUT,
            OUTPUT
        }
    }
}
