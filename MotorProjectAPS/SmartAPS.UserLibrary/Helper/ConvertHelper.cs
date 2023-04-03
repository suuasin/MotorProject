using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace SmartAPS.UserLibrary.Helper
{
    public class ConvertHelper
    {
        #region NumberParse

        public static double ToDouble(string s)
        {
            return ToDouble(s, 0);
        }

        public static double ToDouble(string s, double defaultValue)
        {
            try
            {
                double vaule;
                if (double.TryParse(s, out vaule))
                    return vaule;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float ToFloat(string s)
        {
            return ToFloat(s, 0);
        }

        public static float ToFloat(string s, float defaultValue)
        {
            try
            {
                float vaule;
                if (float.TryParse(s, out vaule))
                    return vaule;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int ToInt32(string s)
        {
            return ToInt32(s, 0);
        }

        public static int ToInt32(string s, int defaultValue)
        {
            try
            {
                int vaule;
                if (int.TryParse(s, out vaule))
                    return vaule;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion NumberParse

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        public static DataTable ToDataTable<T>(List<T> items, Dictionary<string, int> prevDic)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length - 1; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                string stage = values[0].ToString();
                string siteID = values[1].ToString();
                string stepID = values[2].ToString();
                string prodID = values[3].ToString();
                string modelCode2 = values[4].ToString();
                string mpSalesID = values[5].ToString();
                string mpProductID = values[6].ToString();
                string eqpID = values[7].ToString();

                string key = StringHelper.CreateKey(stage, siteID, stepID, prodID, modelCode2, mpSalesID, mpProductID, eqpID);
                if (prevDic.Keys.Contains(key) == false)
                    values[props.Length - 1] = 0;
                else
                    values[props.Length - 1] = prevDic[key];

                tb.Rows.Add(values);
            }

            return tb;
        }
        public static DataTable ToDataTable<T>(IEnumerable<T> list)
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

        public static List<T> ConvertData<T>(DataTable dt) where T : new()
        {
            List<T> results = new List<T>();
            if (dt == null || dt.Rows.Count == 0)
                return results;

            foreach(DataRow row in dt.Rows)
            {
                T t = ConvertData<T>(row);

                if (t != null)
                    results.Add(t);
            }

            return results;
        }

        public static T ConvertData<T>(DataRow row) where T : new()
        {
            T t = new T();

            Type type = t.GetType();

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.CanWrite && row.Table.Columns.Contains(property.Name) == true)
                {
                    if (row[property.Name] != DBNull.Value)
                    {
                        Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        property.SetValue(t, Convert.ChangeType(row[property.Name], propertyType), null);
                    }
                }
            }

            return t;
        }

        public static T StringToEnum<T>(string src, T defValue)
        {
            if (string.IsNullOrEmpty(src))
                return defValue;

            foreach (string en in Enum.GetNames(typeof(T)))
            {
                if (en.Equals(src, StringComparison.CurrentCultureIgnoreCase))
                {
                    defValue = (T)Enum.Parse(typeof(T), src, true);
                    return defValue;
                }
            }

            return defValue;
        }
    }
}
