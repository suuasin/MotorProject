using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using Mozart.Studio.TaskModel.UserInterface;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio.UserInterface;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SmartAPS.UserLibrary.Helper
{
    public static class PivotHelper
    {
        public static PivotGridControl SetPivotGrid(this PivotGridControl pivot, DataTable dataTable)
        {
            pivot.BeginUpdate();

            pivot.ClearPivotGridFields();

            SetPivotField(pivot, dataTable);

            pivot.DataSource = dataTable;

            pivot.EndUpdate();

            return pivot;
        }

        private static void SetPivotField(PivotGridControl pivot, DataTable dataTable)
        {
            if (dataTable == null)
                return;
            foreach (DataColumn column in dataTable.Columns)
            {
                PivotGridField field = pivot.AddPivotGridField(column.ColumnName, PivotArea.FilterArea);
                field.CellFormat.FormatType = FormatType.Numeric;
                field.CellFormat.FormatString = "#,###";
                field.Visible = false;
                field.Options.AllowRunTimeSummaryChange = true;
            }
        }

        public static string GetLayoutFilePath(string layoutID)
        {
            string dirPath = string.Format("{0}\\DefaultLayOut", Application.StartupPath);

            string fileName = string.Format("{0}.xml", layoutID);

            string filePath = string.Format("{0}\\{1}", dirPath, fileName);

            return filePath;
        }

        public static void SavePivotGridControlLayout(this PivotGridControl pivot, string layoutFilePath)
        {
            try
            {
                string message = "Do you want to save PivotGridLayOut?";
                DialogResult result = MessageBox.Show(message, "Save Current LayOut", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    pivot.SaveLayoutToXml(layoutFilePath, OptionsLayoutBase.FullLayout);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("Exception {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ShowPivotGridFieldOptionControl(this PivotGridControl pivot, PivotGridField field)
        {
            PivotGridFieldOptionControl fieldOption = new PivotGridFieldOptionControl(field);
            fieldOption.StartPosition = FormStartPosition.Manual;
            fieldOption.Location = Control.MousePosition;
            fieldOption.ShowDialog();
        }

        public static bool IsUnboundField(this PivotGridField field)
        {
            if (field.UnboundType == DevExpress.Data.UnboundColumnType.Bound)
                return false;

            return true;
        }

        public static string GetSelectedCellSumValue(this PivotGridControl pivot, SummaryItemType summaryType, out int validCount)
        {
            switch (summaryType)
            {
                case SummaryItemType.Min:
                    return GetSelectedCellSummary_Min(pivot, out validCount);
                case SummaryItemType.Max:
                    return GetSelectedCellSummary_Max(pivot, out validCount);
                case SummaryItemType.Average:
                    return GetSelectedCellSummary_Average(pivot, out validCount).ToString("###,###,##0.###");
                case SummaryItemType.Count:
                    return GetSelectedCellSummary_Count(pivot, out validCount).ToString("###,###,###");
            }

            return GetSelectedCellSummary_Sum(pivot, out validCount).ToString("###,###,##0.###");
        }

        private static double GetSelectedCellSummary_Sum(PivotGridControl pivot, out int validCount)
        {
            validCount = 0;
            double sum = 0;

            var selectedCells = pivot.Cells.MultiSelection.SelectedCells;
            foreach (var point in selectedCells)
            {
                object value = pivot.GetCellValue(point.X, point.Y);
                if (value == null)
                    continue;

                double cellValue = 0;
                if (double.TryParse(value.ToString(), out cellValue))
                {
                    validCount += 1;
                    sum += cellValue;
                }
            }

            return sum;
        }

        private static string GetSelectedCellSummary_Min(PivotGridControl pivot, out int validCount)
        {
            validCount = 0;
            double min = double.MaxValue;

            var selectedCells = pivot.Cells.MultiSelection.SelectedCells;
            foreach (var point in selectedCells)
            {
                object value = pivot.GetCellValue(point.X, point.Y);
                if (value == null)
                    continue;

                double cellValue = 0;
                if (double.TryParse(value.ToString(), out cellValue))
                {
                    validCount += 1;
                    min = Math.Min(cellValue, min);
                }
            }

            if (min == double.MaxValue)
                return string.Empty;

            return min.ToString("###,###,##0.###");
        }

        private static string GetSelectedCellSummary_Max(PivotGridControl pivot, out int validCount)
        {
            validCount = 0;
            double max = double.MinValue;

            var selectedCells = pivot.Cells.MultiSelection.SelectedCells;
            foreach (var point in selectedCells)
            {
                object value = pivot.GetCellValue(point.X, point.Y);
                if (value == null)
                    continue;

                double cellValue = 0;
                if (double.TryParse(value.ToString(), out cellValue))
                {
                    validCount += 1;
                    max = Math.Max(cellValue, max);
                }
            }

            if (max == double.MinValue)
                return string.Empty;

            return max.ToString("###,###,##0.###");
        }

        private static double GetSelectedCellSummary_Average(PivotGridControl pivot, out int validCount)
        {
            double sum = GetSelectedCellSummary_Sum(pivot, out validCount);

            return sum / validCount;
        }

        private static int GetSelectedCellSummary_Count(PivotGridControl pivot, out int validCount)
        {
            int count = 0;

            var selectedCells = pivot.Cells.MultiSelection.SelectedCells;
            if (selectedCells != null)
                count = selectedCells.Count;

            validCount = count;

            return count;
        }
    }
}
