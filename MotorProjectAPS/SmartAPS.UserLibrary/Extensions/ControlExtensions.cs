using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Mozart.SeePlan;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SmartAPS.UserLibrary.Extensions
{
    public static class ControlExtensions
    {
        public static readonly string ALL = "ALL";

        public static void SetComboEdit(this ComboBoxEdit control, ICollection<string> list, bool includeALL = false, TextEditStyles style = TextEditStyles.Standard)
        {
            if (control == null)
                return;

            control.Properties.Items.Clear();

            if (includeALL)
                control.Properties.Items.Add(ALL);

            foreach (string item in list)
                control.Properties.Items.Add(item);

            if (control.Properties.Items.Count > 0)
                control.SelectedIndex = 0;

            control.Properties.TextEditStyle = style;
        }

        public static void SetComboEdit(this RepositoryItemComboBox control, ICollection<string> list, bool includeALL = false, TextEditStyles style = TextEditStyles.Standard)
        {

            if (control == null)
                return;

            control.Items.Clear();

            if (includeALL)
                control.Items.Add(ALL);

            foreach (string item in list)
                control.Items.Add(item);

            control.TextEditStyle = style;
            control.Sorted = true;
        }


        public static void SetCheckedComboEdit(this CheckedComboBoxEdit control, ICollection<string> list, bool includeALL = false, TextEditStyles style = TextEditStyles.Standard)
        {
            if (control == null)
                return;

            control.Properties.Items.Clear();

            if (includeALL)
                control.Properties.Items.Add(ALL);

            foreach (string item in list)
                control.Properties.Items.Add(item);

            if (control.Properties.Items.Count > 0)
            {
                foreach (CheckedListBoxItem item in control.Properties.Items)
                    item.CheckState = CheckState.Checked;
            }
        }

        public static void SetCheckedComboEdit(this RepositoryItemCheckedComboBoxEdit control, ICollection<string> list, bool includeALL = false, TextEditStyles style = TextEditStyles.Standard)
        {
            if (control == null)
                return;

            control.Items.Clear();

            if (includeALL)
                control.Items.Add(ALL);

            foreach (string item in list)
                control.Items.Add(item, CheckState.Checked, true);

        }

        //public static void AddDataToComboBox(this ComboBoxEdit control, IExperimentResultItem result, string tableName, string colName, bool addALL = true)
        //{
        //    var dtable = result.LoadInput(tableName, null);

        //    var datas = dtable.Distinct(colName, "");

        //    foreach (string data in datas)
        //    {
        //        if (control.Properties.Items.Contains(data) == false)
        //            control.Properties.Items.Add(data);
        //    }

        //    //if (addALL)
        //    //    control.Properties.Items.Insert(0, Consts.ALL);

        //    if (control.Properties.Items.Count > 0)
        //        control.SelectedIndex = 0;

        //    control.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        //}

        public static void ShiftName(this ComboBoxEdit control, DateTime planStartTime)
        {
            control.Properties.Items.Clear();
            foreach (string name in ShopCalendar.ShiftNames)
            {
                control.Properties.Items.Add(name);
            }

            if (control.Properties.Items.Count > 2)
            {
                if (planStartTime.Hour >= 6 && planStartTime.Hour < 14)
                    control.SelectedIndex = 0;
                else if (planStartTime.Hour >= 14 && planStartTime.Hour < 22)
                    control.SelectedIndex = 1;
                else
                    control.SelectedIndex = 2;
            }
            else if (control.Properties.Items.Count == 2)
            {
                if (planStartTime.Hour >= 6 && planStartTime.Hour < 18)
                    control.SelectedIndex = 0;
                else
                    control.SelectedIndex = 1;
            }
            else
                control.SelectedIndex = 0;

            control.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        }

        public static void ShiftName(this System.Windows.Forms.ComboBox combo, DateTime planStartTime)
        {
            combo.Items.Clear();
            foreach (string name in ShopCalendar.ShiftNames)
            {
                combo.Items.Add(name);
            }

            if (combo.Items.Count > 2)
            {
                if (planStartTime.Hour >= 6 && planStartTime.Hour < 14)
                    combo.SelectedIndex = 0;
                else if (planStartTime.Hour >= 14 && planStartTime.Hour < 22)
                    combo.SelectedIndex = 1;
                else
                    combo.SelectedIndex = 2;
            }
            else if (combo.Items.Count == 2)
            {
                if (planStartTime.Hour >= 6 && planStartTime.Hour < 18)
                    combo.SelectedIndex = 0;
                else
                    combo.SelectedIndex = 1;
            }
            else
                combo.SelectedIndex = 0;

            //combo.TextEditStyle = TextEditStyles.DisableTextEditor;
        }

        public static List<string> GetCheckedItemsToArray(this CheckedComboBoxEdit control)
        {
            List<string> list = new List<string>();

            var str = control.Properties.GetCheckedItems() as string;
            if (str == null)
                return list;        

            var arr = str.Split(',');
            foreach (var it in arr)
            {
                list.Add(it.Trim());
            }            

            return list;
        }

        public static List<string> GetCheckedItemsToList(this RepositoryItemCheckedComboBoxEdit control)
        {
            List<string> list = new List<string>();

            var str = control.GetCheckedItems() as string;
            if (str == null)
                return list;

            var arr = str.Split(',');
            foreach (var it in arr)
            {
                list.Add(it.Trim());
            }

            return list;
        }
    }
}
