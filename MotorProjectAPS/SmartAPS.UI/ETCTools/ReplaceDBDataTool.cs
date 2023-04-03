using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Task.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;

namespace SmartAPS.UI.ETCTools
{
	public partial class ReplaceDBDataTool : MyXtraGridTemplate
    {
        string FieldString = "STRING";
        string FieldReplace = "REPLACE";
        string FieldColumn = "COLUMN";

        public enum TableCategory
        {
            INPUT,
            OUTPUT
        }

        Dictionary<string, DataTable> AsisDataTables { get; set; }
        Dictionary<string, DataTable> TobeDataTables { get; set; }
        Dictionary<string, DataTable> ReplaceDataTables { get; set; }

        DataTable StringDataTable { get; set; }
        XtraTabPage StringPage { get; set; }
        HashSet<XtraTabPage> AsisPage { get; set; }
        HashSet<XtraTabPage> TobePage { get; set; }
        Dictionary<string, XtraTabPage> TabPages { get; set; } //Replace change Tab Color
        Dictionary<string, XtraTabControl> TabControls { get; set; }

        Dictionary<string, TableCategory> TableInfos = new Dictionary<string, TableCategory>();
        private string SelectedTableName { get; set; }

        public ReplaceDBDataTool()
        {
            InitializeComponent();
        }

        public ReplaceDBDataTool(IServiceProvider serviceProvider)
           : base(serviceProvider)
        {
            InitializeComponent();
        }

        protected override void LoadDocument()
        {
            this.Result = this.Document.GetResultItem();

            if (this.Result == null)
                return;

            SetTableComboBoxItem();
        }

        private void barButtonItemLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitializeProperty();

            GenerateStringDataTable();

            Dictionary<string, StringInfo> strInfos = CollectString();

            BindStringDataTable(strInfos);

            AddTabPage(this.StringDataTable , GridType.Default);

            AsisAddTabPage();
        }

        private void barButtonItemReplace_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (TobePage == null)
                return;

            foreach (XtraTabPage page in this.TobePage)
            {
                this.xtraTabControl.TabPages.Remove(page);
            }

            Dictionary<string, string> replaceString = GetReplaceString();

            ReplaceData(replaceString);
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string message = "Do you want to save data?";
            DialogResult result = MessageBox.Show(message, "Save", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                Save();

                barButtonItemLoad.PerformClick();
            }
        }

        protected override DataTable GetData()
        {
            string tableName = this.barEditItemTable.EditValue.ToString();

            this.SelectedTableName = tableName;

            DataTable dataTable = GetDataTable(tableName);

            return dataTable;
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
      
        private void AsisAddTabPage()
        {
            foreach (DataTable dt in this.AsisDataTables.Values)
            {
                XtraTabControl control = new XtraTabControl();
                control.Dock = DockStyle.Fill;

                XtraTabPage page = new XtraTabPage();
                page.Text = dt.TableName;
                page.Controls.Add(control);

                this.xtraTabControl.TabPages.Add(page);
                this.TabControls.Add(dt.TableName, control);
                this.TabPages.Add(dt.TableName, page);

                AddTabPage(dt, GridType.Asis);
            }
        }

        private void GenerateStringDataTable()
        {
            DataTable strDataTable = new DataTable();
            strDataTable.TableName = this.FieldString;

            DataColumn strCol = new DataColumn(this.FieldString);
            DataColumn replaceCol = new DataColumn(this.FieldReplace);
            DataColumn colCol = new DataColumn(this.FieldColumn);

            strDataTable.Columns.Add(colCol);
            strDataTable.Columns.Add(strCol);
            strDataTable.Columns.Add(replaceCol);

            this.StringDataTable = strDataTable;
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

            inputItems.Sort((x, y) => x.CompareTo(y));

            this.repoEditItemTable.Items.AddRange(inputItems.ToArray());
        }

        private void InitializeProperty()
        {
            this.xtraTabControl.TabPages.Clear();
            this.AsisPage = new HashSet<XtraTabPage>();
            this.TobePage = new HashSet<XtraTabPage>();
            this.AsisDataTables = new Dictionary<string, DataTable>();
            this.TobeDataTables = new Dictionary<string, DataTable>();
            this.TabControls = new Dictionary<string, XtraTabControl>();
            this.TabPages = new Dictionary<string, XtraTabPage>();  //Replace Change Header Color
        }

        private void BindStringDataTable(Dictionary<string, StringInfo> strInfos)
        {
            foreach (KeyValuePair<string, StringInfo> info in strInfos)
            {
                DataRow row = this.StringDataTable.NewRow();

                row[this.FieldColumn] = info.Value.Column;
                row[this.FieldString] = info.Key;

                this.StringDataTable.Rows.Add(row);
            }

            this.StringDataTable.AcceptChanges();
        }

        private Dictionary<string, StringInfo> CollectString()
        {
            Dictionary<string, StringInfo> strInfos = new Dictionary<string, StringInfo>();

            foreach (string item in this.repoEditItemTable.Items.GetCheckedValues())
            {
                DataTable dt = GetDataTable(item);
               
                if (dt != null)
                {
                    dt.TableName = item;

                    this.AsisDataTables.Add(item, dt);

                    DataTable tobeDt = dt.Copy();

                    this.TobeDataTables.Add(item, tobeDt);

                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (column.DataType == typeof(string))
                            {
                                string str = row.GetString(column);

                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    StringInfo info;
                                    if(strInfos.TryGetValue(str, out info) == false)
                                    {
                                        info = new StringInfo(str);
                                        strInfos.Add(str, info);
                                    }

                                    if(info.Columns.ContainsKey(column.ColumnName) == false)
                                        info.Columns.Add(column.ColumnName, 0);

                                    info.Columns[column.ColumnName]++;
                                }
                            }
                        }
                    }
                }
            }

            return strInfos;
        }

        private GridControl GenerateGridControl(DataTable dt, DockStyle style, GridType type)
        {
            GridControl grid = new GridControl();

            grid.Dock = style;
            grid.BindingContext = new BindingContext();

            GridView view = new GridView();
            view.GridControl = grid;
            grid.MainView = view;
            grid.DataSource = dt;

            if (type == GridType.Default)
            {
                string[] disableColumns = new string[]
                {
                    this.FieldString
                };

                this.DisableEditColumn(view, disableColumns);
            }
            else
            {
                view.OptionsBehavior.Editable = false;
            }

            //this.EnableChangeRowColor(view);

            view.BestFitColumns();
            view.OptionsView.ColumnAutoWidth = false;

            return grid;
        }

        private void ReplaceData(Dictionary<string, string> replaceString)
        {
            foreach(DataTable dt in this.TobeDataTables.Values)
            {
                foreach (DataRow row in dt.Rows)
                {
                    foreach(DataColumn column in dt.Columns)
                    {
                        if(column.DataType == typeof(string))
                        {
                            string str = row.GetString(column);

                            string replaceStr;
                            if(replaceString.TryGetValue(str, out replaceStr))
                            {
                                row[column] = replaceStr;
                                this.TabControls[dt.TableName].AppearancePage.Header.BackColor = Color.Red;
                                this.TabPages[dt.TableName].Appearance.Header.BackColor = Color.Purple;
                            }
                        }
                    }
                }

                AddTabPage(dt, GridType.Tobe);
            }
        }

        private void AddTabPage(DataTable dt, GridType type)
        {
            XtraTabPage page = new XtraTabPage();
            string pageName = type == GridType.Default ? dt.TableName : string.Format("{0}_{1}",type.ToString(), dt.TableName);
            page.Text = pageName;

            GridControl grid = GenerateGridControl(dt, DockStyle.Fill, type);

            this.SetInitializeOption(grid);
            page.Controls.Add(grid);

            if (type == GridType.Asis)
                this.AsisPage.Add(page);
            else if (type == GridType.Tobe)
                this.TobePage.Add(page);

            if (type == GridType.Default)
                this.xtraTabControl.TabPages.Add(page);
            else
                this.TabControls[dt.TableName].TabPages.Add(page);
        }

        private Dictionary<string, string> GetReplaceString()
        {
            Dictionary<string, string> replaceString = new Dictionary<string, string>();
            foreach (DataRow row in this.StringDataTable.Rows)
            {
                string str = row.GetString(this.FieldString);
                string replace = row.GetString(this.FieldReplace);

                if (string.IsNullOrEmpty(replace) == false && str != replace)
                {
                    if(replaceString.ContainsKey(str) == false)
                        replaceString.Add(str, replace);
                }
            }

            return replaceString;
        }

        private void Save()
        {
            if (TobeDataTables == null)
                return;

            foreach (var key in this.TobeDataTables.Keys)
            {
                DataTable targetTable = TobeDataTables[key];
                MyHelper.DATASVC.SaveLocal(this.Result, key, targetTable);
            }
        }

        public enum GridType
        {
            Default,
            Asis,
            Tobe
        }

        public class StringInfo
        {
            public string String { get; private set; }
            public Dictionary<string, int> Columns { get; set; }

            public string Column
            {
                get
                {
                    string column = string.Empty;

                    int maxCnt = 0;
                    foreach(KeyValuePair<string, int> colInfo in this.Columns)
                    {
                        if(string.IsNullOrEmpty(column) || maxCnt < colInfo.Value)
                        {
                            column = colInfo.Key;
                            maxCnt = colInfo.Value;
                        }
                    }

                    return column;
                }
            }

            public StringInfo(string str)
            {
                this.String = str;
                this.Columns = new Dictionary<string, int>();
            }
        }
    }
}
