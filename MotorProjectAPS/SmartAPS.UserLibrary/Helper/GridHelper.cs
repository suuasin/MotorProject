using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Data;
using System.Windows.Forms;

namespace SmartAPS.UserLibrary.Helper
{
    public static class GridHelper
    {
        public static bool IsUnboundColumn(this GridColumn col)
        {
            if (col == null)
                return false;

            if (col.UnboundType != UnboundColumnType.Bound)
                return true;

            return false;
        }

        public static void SaveLayoutLocal(this GridView gridView, string filePath)
        {
            string message = "Do you want to save GridLayOut?";
            DialogResult result = MessageBox.Show(message, "Save Current LayOut", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                gridView.SaveLayoutToXml(filePath, OptionsLayoutBase.FullLayout);
            }
        }

        public static void SetDataSource(this GridControl gridControl, DataTable dt)
        {
            GridView view = gridControl.MainView as GridView;

            gridControl.DataSource = null;
            view.Columns.Clear();
            gridControl.DataSource = dt;

            view.BestFitColumns();
        }
    }
}
