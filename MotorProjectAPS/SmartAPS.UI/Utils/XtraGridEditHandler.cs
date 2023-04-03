using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Template.Lcd.MozartStudio.Helper;
using Template.UserLibrary.Extensions;

namespace Template.Lcd.MozartStudio.Utils
{
    public interface IDragAndDropHandler
    {
        bool CheckValidateDropPosition(DataRow fromRow, DataRow toRow);

        void DragAndDrop_UpdateRows(DataTable sourceTable);
    }

    public class XtraGridEditHandler
    {
        IDragAndDropHandler _dragAndDropOwner;

        protected const string Description = "desc";
        protected const string Hidden = "hidden";
        protected const string Editor = "editor";

        public Dictionary<string, string[]> MSDic;

        public Color AddedRowColor { get; set; }
        public Color ModifiedRowColor { get; set; }
        public Color ModifiedCellColor { get; set; }

        public RepositorySet RepositorySet { get; set; }
        public DragAndDropOptions DragDropOptions { get; set; }
        public DragAndDropInfo DragDropInfo { get; set; }

        private Dictionary<DataRow, int> DropRows { get; set; }

        public XtraGridEditHandler()
        {
            Initialize();
        }

        public XtraGridEditHandler(IDragAndDropHandler owner)
        {
            this._dragAndDropOwner = owner;

            Initialize();
        }

        void Initialize()
        {
            DragDropInfo = new DragAndDropInfo();
            DragDropOptions = new DragAndDropOptions();
            RepositorySet = new RepositorySet();
            MSDic = new Dictionary<string, string[]>();
        }

        #region [Colour]
        public void SetRowColors()
        {
            AddedRowColor = Color.LightGoldenrodYellow;
            ModifiedRowColor = Color.LightCyan;
            ModifiedCellColor = Color.LightSkyBlue;
        }

        public void DisableChangeRowColor(GridView view)
        { 
            view.RowStyle -= RowStyle;
            view.CustomDrawCell -= CustomDrawCell;
        }

        public void EnableChangeRowColor(GridView view)
        {
            view.RowStyle += RowStyle;
            view.CustomDrawCell += CustomDrawCell;
        }


        public void SetUpdateTime(DataRow row, CustomRowCellEditEventArgs e)
        {
            return;
            string updateCol = "UPDATE_TIME";

            if (row == null)
                return;

            if (row.Table.Columns.Contains(updateCol) == false)
                return;

            if (row.RowState == DataRowState.Unchanged)
                return;

            

            bool modified = false;
            foreach(DataColumn column in row.Table.Columns)
            {
                if (column.ColumnName == updateCol)
                    continue;

                if (row.HasModified(column) == true)
                {
                    modified = true;
                    break;
                }
            }

            if (modified == false && row.HasVersion(DataRowVersion.Original))
            {
                row[updateCol] = row[updateCol, DataRowVersion.Original];
                return;
            }

            row[updateCol] = DateTime.Now;
        }

        public void RowStyle(object sender, RowStyleEventArgs e)
        {        
            GridView view = sender as GridView;
            
            DataRowView rowView = view.GetRow(e.RowHandle) as DataRowView;

            if (rowView == null)
                return;

            DataRow row = rowView.Row;

            if (row.RowState == DataRowState.Added)
                e.Appearance.BackColor = AddedRowColor;
            else if (row.RowState == DataRowState.Modified && row.HasModified() == true)
                e.Appearance.BackColor = ModifiedRowColor;
            else
                e.Appearance.BackColor = Color.White;
        }

        public void CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            GridView view = sender as GridView;

            DataRow row = view.GetDataRow(e.RowHandle);

            if (row == null)
                return;

            SetUpdateTime(row, e);
        }

        public void CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            DataRow row = view.GetDataRow(e.RowHandle);

            if (row == null)
                return;

            if (row.RowState != DataRowState.Modified)
                return;

            DataColumn column = row.Table.Columns[e.Column.FieldName];

            if (row.HasModified(column) == false)
                return;

            e.Appearance.BackColor = ModifiedCellColor;           
        }

        #endregion

        #region Set Columns
        public void SetGridViewColumnsBySchema(GridControl gridControl, DataTable schema, string targetShopID, bool isInit)
        {
            GridView view = gridControl.MainView as GridView;
            view.OptionsView.ColumnAutoWidth = false;

            gridControl.DataSource = schema;
            if (schema == null || schema.Columns.Count == 0)
                return;

            if (isInit)
                return;

            foreach (DataColumn column in schema.Columns)
            {
                if (view.Columns.Any(x => column.ColumnName == x.FieldName) == false)
                    return;

                SetDateTimeColumnFormat(view, column);

                SetColumnToolTips(view, column);

                SetHiddenColumn(view, column);

                SetCheckBoxColumn(view, column);

                SetReadOnlyColumn(view, column);

                SetMSListControl(view, column);
            }
        }
        private void SetDateTimeColumnFormat(GridView view, DataColumn column)
        {
            if (column.DataType != typeof(DateTime))
                return;

            view.Columns[column.ColumnName].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            view.Columns[column.ColumnName].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";

        }

        private void SetColumnToolTips(GridView view, DataColumn column)
        {
            if (column.ExtendedProperties.ContainsKey(Description) == false)
                return;

            string descEntry = column.ExtendedProperties[Description].ToString();

            view.Columns[column.ColumnName].ToolTip = descEntry;
        }

        private void SetHiddenColumn(GridView view, DataColumn column)
        {
            if (column.ExtendedProperties.ContainsKey(Hidden) == false)
                return;

            string descEntry = column.ExtendedProperties[Hidden].ToString();

            if (descEntry != "True")
                return;

            view.Columns[column.ColumnName].Visible = false;
        }

        private void SetCheckBoxColumn(GridView view, DataColumn column)
        {
            if (column.ExtendedProperties.ContainsKey(Editor) == false)
                return;

            string edit = column.ExtendedProperties[Editor].ToString();

            if (edit != "CheckBox")
                return;

            MyHelper.GRIDVIEW.SetCheckBoxColumnType(view, column.ColumnName);
        }

        private void SetReadOnlyColumn(GridView view, DataColumn column)
        {
            if (column.ExtendedProperties.ContainsKey(Editor) == false)
                return;

            string edit = column.ExtendedProperties[Editor].ToString();

            if (edit != "ReadOnly")
                return;

            DisableEditColumn(view, column.ColumnName);
        }

        private void SetMSListControl(GridView view, DataColumn column)
        {
            if (column.ExtendedProperties.ContainsKey(Editor) == false)
                return;

            string editEntry = column.ExtendedProperties[Editor].ToString();

            if (editEntry.Contains("MSList") == false)
                return;                
                
            int startIndex = editEntry.IndexOf('(');
            int lastIndex = editEntry.LastIndexOf(')');
            int length = lastIndex - startIndex - 1;

            if (length < 0)
                return;

            string contents = editEntry.Substring(startIndex + 1, length);
            contents = contents.Replace("'", "");
            contents = contents.Replace(" ", "");

            string[] splitStr = contents.Split(',');
            
            if (splitStr.Length < 1)
                return;

            string dataActionName = splitStr[0];
            MSDic[column.ColumnName] = splitStr;

            DisableEditColumn(view, column.ColumnName);
        }

        #endregion

        #region Repositories
        public RepositoryItemComboBox GetCellRepositoryItem(string columnName, string value)
        {
            return this.RepositorySet.GetItem(columnName, value) as RepositoryItemComboBox;
        }

        //public void InheritRepositoryItem(RepositorySet set)
        //{
        //    RepositorySet.Clear();
        //    RepositorySet = set;
        //}

        #endregion

        #region Drag And Drop

        public DragDropBehavior SetGridViewDragAndDropBehavior(BehaviorManager behaviorManager, GridView gridView, IContainer components)
        {
            DragDropEvents dragDropEvents = new DragDropEvents(components);

            behaviorManager.SetBehaviors(gridView, new Behavior[] {
            DragDropBehavior.Create(typeof(DevExpress.XtraGrid.Extensions.ColumnViewDragDropSource), true, true, true, true, dragDropEvents)});

            DragDropBehavior gridControlBehavior = behaviorManager.GetBehavior<DragDropBehavior>(gridView);

            gridControlBehavior.DragOver += Behavior_DragOver;
            gridControlBehavior.DragDrop += Behavior_DragDrop;

            this.DragDropOptions.UseChangeValue = true;
            this.DragDropOptions.CheckValidateDropPosition = true;

            MyHelper.GRIDVIEW.DisableColumnSort(gridView);

            DropRows = new Dictionary<DataRow, int>();

            return gridControlBehavior;
        }

        public void DisableDragAndDrop(BehaviorManager behaviorManager, GridView gridView)
        {
            DragDropBehavior gridControlBehavior = behaviorManager.GetBehavior<DragDropBehavior>(gridView);

            if (gridControlBehavior == null)
                return;

            gridControlBehavior.DragOver -= Behavior_DragOver;
            gridControlBehavior.DragDrop -= Behavior_DragDrop;
            DropRows = null;

            MyHelper.GRIDVIEW.EnableColumnSort(gridView);
        }

        public void Behavior_DragOver(object sender, DragOverEventArgs e)
        {
            DragOverGridEventArgs args = DragOverGridEventArgs.GetDragOverGridEventArgs(e);
            e.InsertType = args.InsertType;
            e.InsertIndicatorLocation = args.InsertIndicatorLocation;
            e.Action = args.Action;
            Cursor.Current = args.Cursor;
            args.Handled = true;
        }

        public void Behavior_DragDrop(object sender, DevExpress.Utils.DragDrop.DragDropEventArgs e)
        {
            GridView targetGrid = e.Target as GridView;
            GridView sourceGrid = e.Source as GridView;

            if (e.Action == DragDropActions.None || targetGrid != sourceGrid)
                return;

            DataTable sourceTable = sourceGrid.GridControl.DataSource as DataTable;

            Point hitPoint = targetGrid.GridControl.PointToClient(Cursor.Position);
            GridHitInfo hitInfo = targetGrid.CalcHitInfo(hitPoint);

            int[] sourceHandles = e.GetData<int[]>();

            int targetRowHandle = hitInfo.RowHandle;
            int targetRowIndex = targetGrid.GetDataSourceRowIndex(targetRowHandle);

            int newRowIndex;
            int oldRowIndex;

            List<DataRow> draggedRows = new List<DataRow>();
            foreach (int sourceHandle in sourceHandles)
            {
                oldRowIndex = sourceGrid.GetDataSourceRowIndex(sourceHandle);
                DataRow oldRow = sourceTable.Rows[oldRowIndex];
                draggedRows.Add(oldRow);
            }


            switch (e.InsertType)
            {
                case InsertType.Before:
                    newRowIndex = targetRowIndex > sourceHandles[sourceHandles.Length - 1] ? targetRowIndex - 1 : targetRowIndex;
                    break;
                case InsertType.After:
                    newRowIndex = targetRowIndex < sourceHandles[0] ? targetRowIndex + 1 : targetRowIndex;
                    break;
                default:
                    newRowIndex = -1;
                    break;
            }

            DataRow newRow = sourceTable.Rows[newRowIndex];

            foreach (DataRow oldRow in draggedRows)
            {
                if (DragAndDrop_ValidateLocation(oldRow, newRow) == false)
                    return;
            }

            switch (e.InsertType)
            {
                case InsertType.Before:
                    for (int i = draggedRows.Count - 1; i >= 0; i--)
                        DragAndDrop_Excute(sourceTable, draggedRows[i], newRowIndex);
                    break;
                case InsertType.After:
                    for (int i = 0; i < draggedRows.Count; i++)
                        DragAndDrop_Excute(sourceTable, draggedRows[i], newRowIndex);
                    break;
                default:
                    newRowIndex = -1;
                    break;
            }

            int insertedIndex = targetGrid.GetRowHandle(newRowIndex);
            targetGrid.FocusedRowHandle = insertedIndex;
            targetGrid.SelectRow(targetGrid.FocusedRowHandle);
        }

        public void DragAndDrop_Excute(DataTable sourceTable, DataRow oldRow, int newRowIndex)
        {            
            int oldRowIndex = sourceTable.Rows.IndexOf(oldRow);
            DataRow newRow = sourceTable.NewRow();
            newRow.ItemArray = oldRow.ItemArray;            

            DataRow toRow = sourceTable.Rows[newRowIndex];

            if (DropRows.ContainsKey(oldRow) == false)
                DropRows.Add(oldRow, oldRowIndex);

            sourceTable.Rows.Remove(oldRow);
            sourceTable.Rows.InsertAt(newRow, newRowIndex);

            DataRow row = sourceTable.NewRow();
            row.ItemArray = newRow.ItemArray;

            if (DropRows.ContainsKey(newRow) == false && DropRows.ContainsKey(oldRow) == true)
                DropRows.Add(newRow, DropRows[oldRow]);

            newRow.AcceptChanges();

            this.DragDropInfo.FromIndex = oldRowIndex;
            this.DragDropInfo.ToIndex = newRowIndex;
            this.DragDropInfo.FromRow = oldRow;
            this.DragDropInfo.ToRow = toRow;

            DragAndDrop_ChangeValues(sourceTable);

            if (DropRows.ContainsKey(oldRow) == true)
            {
                if (newRowIndex == DropRows[oldRow])
                    newRow.AcceptChanges();

            }
        }

        private void DragAndDrop_ChangeValues(DataTable table)
        {
            if (this.DragDropOptions.UseChangeValue == false)
                return;

            _dragAndDropOwner.DragAndDrop_UpdateRows(table);
        }

        private bool DragAndDrop_ValidateLocation(DataRow fromRow, DataRow toRow)
        {
            if (DragDropOptions.CheckValidateDropPosition == false)
                return true;

            return _dragAndDropOwner.CheckValidateDropPosition(fromRow, toRow);
        }
        public void DisableEditColumn(GridView view, params string[] columnNames)
        {
            MyHelper.GRIDVIEW.DisableEditColumn(view, columnNames);
        }

        public void EnableEditColumn(GridView view, params string[] columnNames)
        {
            MyHelper.GRIDVIEW.EnableEditColumn(view, columnNames);
        }

        public class DragAndDropInfo
        {
            public int FromIndex { get; set; }
            public int ToIndex { get; set; }
            public DataRow FromRow { get; set; }
            public DataRow ToRow { get; set; }
        }

        #endregion

        
        public class DragAndDropOptions
        {
            public bool UseChangeValue { get; set; }

            public bool CheckValidateDropPosition { get; set; }
            public string[] CheckColumns { get; set; }
        }
    }

    public class RepositorySet
    {
        Dictionary<string, Dictionary<string, RepositoryItem>> _dic;

        public RepositorySet()
        {
            _dic = new Dictionary<string, Dictionary<string, RepositoryItem>>();
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public bool ContainsColumn(string column)
        {
            return _dic.ContainsKey(column);
        }

        public void AddItem(string column, string value, RepositoryItem item)
        {
            if (_dic.ContainsKey(column) == false)
                _dic[column] = new Dictionary<string, RepositoryItem>();

            _dic[column][value] = item;
        }

        public RepositoryItem GetItem(string columnName, string value)
        {
            if (_dic.ContainsKey(columnName) == false)
                return null;

            if (_dic[columnName].ContainsKey(value) == false)
                return null;

            return _dic[columnName][value];
        }

        public List<KeyValuePair<string, RepositoryItem>> GetItemsByValue(string value)
        {
            List<KeyValuePair<string, RepositoryItem>> results = new List<KeyValuePair<string, RepositoryItem>>();

            foreach (string key in _dic.Keys)
            {
                if (_dic[key].ContainsKey(value) == false)
                    continue;
                results.Add(new KeyValuePair<string, RepositoryItem>(key, _dic[key][value]));
            }

            return results;
        }
    }
}
