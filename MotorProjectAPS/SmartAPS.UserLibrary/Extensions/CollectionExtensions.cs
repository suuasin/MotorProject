using Mozart.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SmartAPS.UserLibrary.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => { if (!dic.ContainsKey(x.Key)) dic.Add(x.Key, x.Value); });
        }

        //public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        //{
        //    foreach (var item in source)
        //        action(item);
        //}

        #region [DataTable]
        public static void SetColumnValue(this DataTable dt, string columnName, string value)
        {
            if (string.IsNullOrEmpty(columnName) == true || dt.Columns.Contains(columnName) == false)
                return;

            object changedValue;
            foreach (DataRow row in dt.Rows)
            {
                Type type = row[columnName].GetType();

                try
                {
                    changedValue = Convert.ChangeType(value, type);
                }
                catch
                {
                    return;
                }

                row[columnName] = changedValue;
            }
        }
        public static IEnumerable<object> GetDistinctRowValues(this DataTable dt, string columnName)
        {
            if (dt.Columns.Contains(columnName) == false)
                return null;

            return dt.AsEnumerable().Select(x => x[columnName]).Distinct();
        }

        public static ICollection<string> Distinct(this DataTable dtable, string columnName, string filter)
        {
            var dict = new SortedSet<string>();

            if (string.IsNullOrEmpty(columnName) || dtable == null)
                return dict;

            if (dtable.Columns.Contains(columnName) == false)
                return dict;

            var dview = new DataView(dtable, filter, columnName, DataViewRowState.CurrentRows);
            if (dview == null)
                return dict;

            int column = dtable.Columns.IndexOf(columnName);
            foreach (DataRowView drow in dview)
            {
                if (drow.Row.IsNull(column))
                    continue;

                string value = Convert.ToString(drow[column]);
                if (value == null || dict.Contains(value))
                    continue;

                dict.Add(value);
            }

            return dict;
        }

        public static int GetChangesCount(this DataTable dt, DataRowState state)
        {
            DataTable changeDt = dt.GetChanges(state);

            if (changeDt == null)
                return 0;

            return changeDt.Rows.Count;
        }
        #endregion

        #region [DataRow]
        public static bool HasModified(this DataRow row)
        {
            if (row.HasVersion(DataRowVersion.Original) == false)
                return false;

            DataTable dt = row.Table;
            foreach (DataColumn column in dt.Columns)
            {
                if (row[column, DataRowVersion.Original].ToString() != row[column, DataRowVersion.Current].ToString())
                    return true;
            }
            return false;
        }

        public static bool HasModified(this DataRow row, DataColumn column)
        {
            if (row.HasVersion(DataRowVersion.Original) == false)
                return false;

            if (row[column, DataRowVersion.Original].ToString() != row[column, DataRowVersion.Current].ToString())
                return true;

            return false;
        }
        #endregion
    }
}
