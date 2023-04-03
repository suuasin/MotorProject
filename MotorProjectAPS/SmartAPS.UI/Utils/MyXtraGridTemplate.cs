using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Mozart.Studio.Application;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserInterface;
using Mozart.Studio.TaskModel.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SmartAPS.UI.Helper;
using SmartAPS.UserLibrary.Helper;

namespace SmartAPS.UI.Utils
{
    [ToolboxItem(false)]
    public class MyXtraGridTemplate : XtraGridTemplate
    {
        protected string SelectedShopID { get; set; }

        protected MyXtraGridTemplate()
        {
        }

        protected MyXtraGridTemplate(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            MyHelper.SHIFTTIME.Initialize();
        }

        public override void LoadDocument(IExperimentResultItem result)
        {
            this.Result = result;

            this.LoadDocument();
        }

        protected override void SetInitializeOption(GridControl gridControl)
        {
            base.SetInitializeOption(gridControl);
        }

        protected override void RunQuery()
        {
            base.RunQuery();
        }

        protected override void Query()
        {
            base.Query();
        }

        protected override DataTable GetData()
        {
            return base.GetData();
        }

        protected override void BindData(DataTable dt)
        {
            base.BindData(dt);
        }

        protected override XtraGridTemplate GetCloneView()
        {
            return base.GetCloneView();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void SetGridLayout(GridView gridView)
        {
            base.SetGridLayout(gridView);           
        }

        public override string GetLayoutFullPath(GridView view = null)
        {
            return base.GetLayoutFullPath(view);
        }

        public override string GetLayoutID(GridView view)
        {
            if (view == null)
                return string.Empty;

            string layoutID = string.Format("{0}{1}", this.GetType().Name, view.Name);

            return layoutID;
        }

        protected virtual string GetLayoutID(DevExpress.XtraSpreadsheet.SpreadsheetControl spreadSheet)
        {
            string layoutID = string.Format("{0}{1}", this.ViewName, spreadSheet.Name);

            return layoutID;
        }

        protected void DisableEditColumn(GridView view, params string[] columnNames)
        {
            foreach (string columnName in columnNames)
            {
                if (view.Columns.Any(x => x.FieldName != columnName) == false)
                    return;

                view.Columns[columnName].OptionsColumn.AllowEdit = false;
            }
        }

        public void SaveLocalSetting(IServiceProvider serviceProvider, string settingName, string settingValue)
        {
            IVsApplication application = (IVsApplication)serviceProvider.GetService(typeof(IVsApplication));
            application.SetSetting(settingName, settingValue);
        }

        public string GetLocalSetting(IServiceProvider serviceProvider, string loadingName)
        {
            IVsApplication application = (IVsApplication)serviceProvider.GetService(typeof(IVsApplication));
            string loadingValue = application.GetSetting(loadingName);
            return loadingValue;
        }
    }
}
