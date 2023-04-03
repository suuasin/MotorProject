using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraPivotGrid;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.TaskModel.UserInterface;
using Mozart.Studio.TaskModel.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SmartAPS.UI.Helper;
using SmartAPS.UserLibrary.Helper;

namespace SmartAPS.UI.Utils
{
	[ToolboxItem(false)]
    public class MyXtraPivotGridTemplate : XtraPivotGridTemplate
    {
        protected string SelectedShopID { get; set; }

        protected MyXtraPivotGridTemplate()
        {
        }

        protected MyXtraPivotGridTemplate(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            MyHelper.SHIFTTIME.Initialize();
        }

        protected override PivotGridControl GetPivotGrid()
        {
            return null;
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
            return null;
        }

        protected override void BindData(DataTable dt)
        {
            base.BindData(dt);
        }

        protected override XtraPivotGridTemplate GetCloneView()
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

        public override string GetLayoutFullPath(PivotGridControl view = null)
        {
            return base.GetLayoutFullPath(view);
        }    
    }
}
