using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Mozart.Data.Entity;
using Mozart.SeePlan;
using Mozart.Studio.TaskModel.Projects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SmartAPS.Outputs;
using EngInputs = SmartAPS.Inputs;


namespace SmartAPS.UI.Helper
{
    public class EngControlHelper
    {
        enum AREA
        {
            TFT = 0,
            CF,
            CELL,
            NONE
        }

        public static void SetControl_ComboBase(BarEditItem control, bool useAll, List<object> items)
        {
            if (control == null)
                return;
            control.BeginUpdate();

            try
            {
                var cbEdit = control.Edit as RepositoryItemComboBox;
                if (cbEdit == null)
                    return;

                cbEdit.Items.Clear();

                if (useAll)
                    cbEdit.Items.Add("ALL");

                if (items != null)
                    cbEdit.Items.AddRange(items);

                if (!cbEdit.Items.Contains(control.EditValue))
                {
                    if (cbEdit.Items.Count > 0)
                        control.EditValue = cbEdit.Items[0];
                    else
                        control.EditValue = null;
                }
            }
            catch
            {
                control.EditValue = null;
            }
            finally
            {
                control.EndUpdate();
                control.Refresh();
            }
        }

        public static void SetControl_CheckedComboBase(BarEditItem control, List<object> items)
        {
            if (control == null)
                return;
            control.BeginUpdate();

            try
            {
                var cbEdit = control.Edit as RepositoryItemCheckedComboBoxEdit;
                if (cbEdit == null)
                    return;

                cbEdit.TextEditStyle = TextEditStyles.Standard;
                cbEdit.Items.Clear();

                items.Sort();
                items.ForEach(item => cbEdit.Items.Add(item));

                cbEdit.Items.ToList<CheckedListBoxItem>().ForEach(item => item.CheckState = CheckState.Checked);
                control.EditValue = cbEdit.GetCheckedItems();
            }
            catch
            {
                control.EditValue = null;
            }
            finally
            {
                control.EndUpdate();
                control.Refresh();
            }
        }

        public static void SetControl_LineID(BarEditItem control, IExperimentResultItem resultItem, bool useAll = false)
        {
            SetControl_ComboBase(control, useAll, MyHelper.FILTER.GetLineId_Line(resultItem));
        }

        public static void SetControl_ProductID(BarEditItem control, IExperimentResultItem resultItem, bool useAll = false)
        {
            SetControl_ComboBase(control, useAll, MyHelper.FILTER.GetProductId_Demand(resultItem));
        }

        public static void SetControl_StepID(BarEditItem control, IExperimentResultItem resultItem, bool useAll = false)
        {
            SetControl_ComboBase(control, useAll, MyHelper.FILTER.GetStepId_StdStep(resultItem));
        }

        public static void SetControl_ProductID_Checked(BarEditItem control, IExperimentResultItem resultItem)
        {
            SetControl_CheckedComboBase(control, MyHelper.FILTER.GetProductId_Demand(resultItem));
        }

        public static void SetControl_WIP_ProductID_Checked(BarEditItem control, IExperimentResultItem resultItem)
        {
            SetControl_CheckedComboBase(control, MyHelper.FILTER.GetProductId_Wip(resultItem));
        }

        public static void SetControl_StepID_Checked(BarEditItem control, IExperimentResultItem resultItem)
        {
            SetControl_CheckedComboBase(control, MyHelper.FILTER.GetStepId_StdStep(resultItem));
        }

        public static void SetControl_EqpID_Checked(BarEditItem control, IExperimentResultItem resultItem)
        {
            SetControl_CheckedComboBase(control, MyHelper.FILTER.GetEqpId_Equipment(resultItem));
        }

        public static void SetControl_EqpGroup_Checked(BarEditItem control, IExperimentResultItem resultItem)
        {
            SetControl_CheckedComboBase(control, MyHelper.FILTER.GetEqpGroup_Equipment(resultItem));
        }

        public static void SetControl_EqpModel_Checked(BarEditItem control, IExperimentResultItem resultItem)
        {
            SetControl_CheckedComboBase(control, MyHelper.FILTER.GetEqpModel_Equipment(resultItem));
        }

        public static DateTime GetPlanStartTime(IExperimentResultItem result)
        {
            var startTime = MyHelper.DATASVC.GetPlanStartTime(result);
            if (startTime != null)
                return startTime;

            return ShopCalendar.StartTimeOfDayT(DateTime.Now);
        }

        public static DateTime GetPlanEndTime(IExperimentResultItem result)
        {
            var endTime = MyHelper.DATASVC.GetPlanEndTime(result);
            if (endTime != null)
                return endTime;

            return ShopCalendar.EndTimeOfDayT(DateTime.Now);
        }

        public static void SetGridViewColumnWidth(GridView grid)
        {
            grid.OptionsView.ColumnAutoWidth = false;

            foreach (GridColumn col in grid.Columns)
            {
                string key = col.Name;
                int width = col.Width;

                switch (key)
                {
                    case "EQP_ID": width = 80; break;
                    case "SUB_EQP_ID": width = 80; break;
                    case "STATE": width = 50; break;
                    case "LOT_ID": width = 100; break;
                    case "LAYER": width = 80; break;
                    case "OWNER_TYPE": width = 100; break;
                    case "OPERATION_NAME": width = 60; break;
                    case "MASK_ID": width = 100; break;
                    case "TOOL_ID": width = 100; break;
                    case "PRODUCT_ID": width = 100; break;
                    case "PRODUCT_VERSION": width = 140; break;
                    case "START_TIME": width = 140; break;
                    case "END_TIME": width = 140; break;
                    case "QTY": width = 60; break;
                    case "IN_QTY": width = 60; break;
                    case "OUT_QTY": width = 60; break;
                    case "GAP_TIME": width = 120; break;
                    case "LOT_PRIORITY": width = 100; break;
                    case "EQP_RECIPE": width = 80; break;
                    case "WIP_INIT_RUN": width = 100; break;
                }

                col.Width = width;
            }
        }
    }
}
