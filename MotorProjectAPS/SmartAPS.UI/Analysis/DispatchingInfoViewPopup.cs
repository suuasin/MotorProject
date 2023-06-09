﻿using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Mozart.Studio.TaskModel.UserInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.UI.Gantts;
using SmartAPS.UI.Helper;
using SmartAPS.UserLibrary.Utils;
using Mozart.Studio.TaskModel.Projects;

namespace SmartAPS.UI.Analysis
{
    public partial class DispatchingInfoViewPopup : DevExpress.XtraEditors.XtraForm
    {
        private EQP_DISPATCH_LOG _dispInfo;
        private EqpMaster.Eqp _eqpInfo;
        private IExperimentResultItem Result;

        private string _lastWip;

        private List<PRESET_INFO_HIS> _wpList;
        private Dictionary<string, PRESET_INFO_HIS> _wpInfos;

        private string TargetVersionNo { get; set; }

        private bool IsRotate
        {
            get
            {
                if (isRotateView.Checked == false)
                    return false;
                return true;
            }
        }

        private bool ShowOnlyTop_Dispatch
        {
            get { return this.chkOnlyTopDisp.Checked; }
        }

        private bool ShowOnlyTop_Filter
        {
            get { return this.chkOnlyTopFilter.Checked; }
        }

        public DispatchingInfoViewPopup(IExperimentResultItem result, EQP_DISPATCH_LOG dispInfo, EqpMaster.Eqp eqpInfo, List<PRESET_INFO_HIS> wpList)
        {
            InitializeComponent();

            this.Result = result;

            _dispInfo = dispInfo;
            _eqpInfo = eqpInfo;
            _wpList = wpList;

            SetControls();
            Query();
        }

        private void SetControls()
        {
            var log = _dispInfo;
            this.Text = string.Format("Dispatching Information - {0}", log.DISPATCHING_TIME);

            StringBuilder sb = new StringBuilder();

            if (_eqpInfo != null)
                sb.AppendLine(_eqpInfo.ToString());

            sb.AppendFormat("Initial Wip Count : {0}\r\n", log.INIT_WIP_CNT);
            sb.AppendFormat("Filtered Wip Count : {0}\r\n", log.FILTERED_WIP_CNT);
            sb.AppendFormat("Selected Wip Count : {0}\r\n", log.SELECTED_WIP_CNT);
            //sb.AppendFormat("Last Run : {0}\r\n", log.LAST_WIP);

            this.eqpInfoTextBox.Text = sb.ToString();
        }

        private void Query()
        {
            BindData();
            DesignGrid_Dispatch();
        }

        private void FillGrid(DataTable resultTable)
        {
            gridControl1.DataSource = resultTable;

            gridView1.PopulateColumns();
            gridView1.BestFitColumns();
        }

        private void BindGrid_Filter(DataTable resultTable)
        {
            gridControl3.DataSource = resultTable;
            gridView3.PopulateColumns();

            foreach (GridColumn column in gridView3.Columns)
            {
                if (column.ColumnType == typeof(double))
                {
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    column.DisplayFormat.FormatString = "{0:##0.0%}";
                }
            }

            gridView3.BestFitColumns();
        }

        private void BindData()
        {
            var log = _dispInfo;

            BindData_Dispatch(log);
            BindData_Filter(log);
        }

        private void BindData_Dispatch(EQP_DISPATCH_LOG log)
        {
            Dictionary<string, List<float>> summaryDic = new Dictionary<string, List<float>>();
            List<PRESET_INFO_HIS> matchedList = new List<PRESET_INFO_HIS>();

            var wpList = _wpList.Where(x => x.PRESET_ID == log.PRESET_ID).ToList();

            //TODO : bong (정렬 확인 필요)
            //wpList.Sort(t => t.SEQUENCE);

            _wpInfos = new Dictionary<string, PRESET_INFO_HIS>();
            foreach (var item in wpList)
            {
                matchedList.Add(item);
                _wpInfos[item.FACTOR_ID] = item;
            }

            string dispatchWipLog = log.DISPATCH_WIP_LOG;

            if (string.IsNullOrEmpty(dispatchWipLog))
                return;
            string[] infoList = dispatchWipLog.Split(';');

            var sample = infoList[0].Split('/');

            bool showProductInfo = sample.Length - matchedList.Count > 1;
            DataTable dt = CreateSchema_Dispatch(matchedList, showProductInfo);

            foreach (string info in infoList)
            {
                try
                {
                    string[] split = info.Split('/');
                    if (split.Count() <= 1)
                        continue;

                    DataRow row = dt.NewRow();
                    string prodID = split[1];
                    string stepID = split[2];

                    int index = 0;

                    row[ColName.LOT_ID] = split[0];

                    if (showProductInfo)
                    {
                        row[ColName.PRODUCT_ID] = prodID;
                        row[ColName.STEP_ID] = stepID;

                        int qty;
                        if (int.TryParse(split[3], out qty) == false)
                            qty = 0;

                        row[ColName.LOT_QTY] = qty;

                        string key = MyHelper.STRING.CreateKey(prodID, stepID);
                        List<float> summaryList;
                        if (summaryDic.TryGetValue(key, out summaryList) == false)
                            summaryDic[key] = summaryList = new List<float>();
                        summaryList.Add(qty);
                    }

                    int diffIndex = showProductInfo ? 4 : 1;
                    float sum = 0.0f;
                    for (int i = diffIndex; i < split.Length; i++)
                    {
                        var orgValue = split[i];
                        var arrVaules = orgValue.Split('@');

                        string sScore = arrVaules[0];
                        string desc = string.Empty;
                        if (arrVaules.Length > 1)
                            desc = arrVaules[1];

                        float score;
                        if (float.TryParse(sScore, out score) == false)
                            score = 0;

                        if (matchedList.Count >= (i - diffIndex + 1))
                        {
                            string factorID = matchedList[i - diffIndex].FACTOR_ID;
                            row[factorID] = string.IsNullOrEmpty(desc) ? sScore : sScore + " " + desc;

                            sum += score;
                            index++;
                        }
                    }

                    row[ColName.WEIGHTED_SUM] = string.Format("{0}", sum);
                    dt.Rows.Add(row);
                }
                catch
                {
                    continue;
                }
            }

            if (this.ShowOnlyTop_Dispatch)
                dt = SummaryOnlyTop_Dispatch(summaryDic, dt);

            dt = CreateResultDt(dt, _wpInfos);

            FillGrid(dt);
            BindData_Summary(summaryDic);

            DesignGrid_Dispatch();
        }

        private static DataTable SummaryOnlyTop_Dispatch(Dictionary<string, List<float>> summaryDic, DataTable result)
        {
            DataTable dt = result.Clone();
            dt.Clear();

            foreach (var item in summaryDic)
            {
                string[] keySplit = item.Key.Split('@');

                if (keySplit.Count() <= 1)
                    continue;

                string productID = keySplit[0];
                string stepID = keySplit[1];

                foreach (DataRow row in result.Rows)
                {
                    if (row[ColName.PRODUCT_ID].ToString() == productID
                        && row[ColName.STEP_ID].ToString() == stepID)
                    {
                        DataRow sumRow = row;
                        string lotID = row[ColName.DISPATCHING_WIP].ToString();
                        int lotCount = item.Value.Count();
                        float sumLotQty = item.Value.Sum();
                        sumRow[ColName.DISPATCHING_WIP] = string.Format("{0} [{1}]", lotID, lotCount);
                        sumRow[ColName.LOT_QTY] = sumLotQty;

                        dt.ImportRow(sumRow);
                        break;
                    }
                }
            }

            return dt;
        }

        private DataTable CreateResultDt(DataTable dt, Dictionary<string, PRESET_INFO_HIS> wpInfo)
        {
            if (this.IsRotate)
            {
                return CreateResultDt_Rotate(dt, wpInfo);
            }
            else
            {
                return CreateResultDt_Default(dt, wpInfo);
            }
        }

        private DataTable CreateResultDt_Default(DataTable dt, Dictionary<string, PRESET_INFO_HIS> wpInfo)
        {
            DataTable resultDt = new DataTable();

            resultDt.Columns.Add("ColumnName", typeof(string));

            for (int i = 0; i < dt.Rows.Count; i++)
                resultDt.Columns.Add("Column" + (i + 1).ToString(), typeof(string));

            foreach (DataColumn col in dt.Columns)
            {
                string colName = col.ColumnName;

                PRESET_INFO_HIS wp;
                if (wpInfo.TryGetValue(colName, out wp))
                {
                    //if (wp.FACTOR_WEIGHT == 0)
                    //    continue;
                }

                DataRow newRow = resultDt.NewRow();
                newRow["ColumnName"] = wp == null ? colName : string.Format("{0} [{1}]", colName, wp.FACTOR_WEIGHT);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    newRow[i + 1] = row.GetString(col);
                }

                resultDt.Rows.Add(newRow);
            }

            return resultDt;
        }

        private DataTable CreateResultDt_Rotate(DataTable dt, Dictionary<string, PRESET_INFO_HIS> wpInfos)
        {
            DataTable resultDt = new DataTable();

            foreach (DataColumn col in dt.Columns)
            {
                string colName = col.ColumnName;

                PRESET_INFO_HIS wp;
                if (wpInfos.TryGetValue(colName, out wp))
                {
                    if (wp.FACTOR_WEIGHT == 0)
                        continue;
                }

                var newCol = resultDt.Columns.Add(colName, col.DataType);
                newCol.Caption = wp == null ? colName : string.Format("{0} [{1}]", colName, wp.FACTOR_WEIGHT);
            }

            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = resultDt.NewRow();

                foreach (DataColumn col in resultDt.Columns)
                {
                    string colName = col.ColumnName;
                    if (dt.Columns.Contains(colName) == false)
                        continue;

                    newRow[colName] = row[colName];
                }

                resultDt.Rows.Add(newRow);
            }

            return resultDt;
        }

        private void BindData_Filter(EQP_DISPATCH_LOG log)
        {
            var dt = CreateSchema_FilteredWip();

            string filteredWipLog = log.FILTERED_WIP_LOG;
            if (string.IsNullOrEmpty(filteredWipLog))
                return;

            Dictionary<string, List<int>> sumFilterWipDic = new Dictionary<string, List<int>>();

            string[] blobs = filteredWipLog.Split('\t');
            foreach (string blob in blobs)
            {
                string[] group = blob.Split(':');

                if (group.Length < 2)
                    continue;

                string reason = group[0];

                string[] wips = group[1].Split(';');

                foreach (string wip in wips)
                {
                    DataRow frow = dt.NewRow();

                    string[] wipArr = wip.Split('/');
                    int wipCnt = wipArr.Length;

                    string lotID = wipArr[0];
                    string productID = wipCnt >= 2 ? wipArr[1] : Consts.NULL_ID;
                    string productVer = wipCnt >= 3 ? wipArr[2] : Consts.NULL_ID;
                    string stepID = wipCnt >= 4 ? wipArr[3] : Consts.NULL_ID;
                    int qty = wipCnt >= 5 ? Convert.ToInt32(wipArr[4]) : 0;
                    string productionType = wipCnt >= 6 ? wipArr[5] : Consts.NULL_ID;

                    frow[ColName.LOT_ID] = lotID;
                    frow[ColName.PRODUCT_ID] = productID;
                    frow[ColName.STEP_ID] = stepID;
                    frow[ColName.Qty] = qty;
                    frow[ColName.REASON] = reason;

                    dt.Rows.Add(frow);

                    if (this.ShowOnlyTop_Filter)
                    {
                        string key = MyHelper.STRING.CreateKey(productID, productVer, stepID, productionType);
                        List<int> sumFilterWipList;
                        if (sumFilterWipDic.TryGetValue(key, out sumFilterWipList) == false)
                            sumFilterWipDic[key] = sumFilterWipList = new List<int>();

                        sumFilterWipList.Add(qty);
                    }
                }
            }

            if (this.ShowOnlyTop_Filter)
                dt = SummaryOnlyTop_Filter(sumFilterWipDic, dt);

            BindGrid_Filter(dt);
            DesignGrid_Filter();
        }

        private static DataTable SummaryOnlyTop_Filter(Dictionary<string, List<int>> sumFilterWipDic, DataTable filterWipTable)
        {
            DataTable filterSumResult = filterWipTable.Clone();
            filterSumResult.Clear();
            foreach (var item in sumFilterWipDic)
            {
                string[] keySplit = item.Key.Split('@');

                if (keySplit.Count() <= 1)
                    continue;

                string productID = keySplit[0];
                string productVer = keySplit[1];
                string stepID = keySplit[2];
                string productionType = keySplit[3];

                foreach (DataRow row in filterWipTable.Rows)
                {
                    if (row[ColName.PRODUCT_ID].ToString() == productID
                        && row[ColName.STEP_ID].ToString() == stepID)
                    {
                        DataRow sumRow = row;
                        string lotID = row[ColName.LOT_ID].ToString();
                        int wipCount = item.Value.Count();
                        int sumQty = item.Value.Sum();
                        sumRow[ColName.LOT_ID] = string.Format("{0} [{1}]", lotID, wipCount);
                        sumRow[ColName.Qty] = sumQty;
                        sumRow[ColName.REASON] = row[ColName.REASON].ToString();

                        filterSumResult.ImportRow(sumRow);
                        break;
                    }
                }
            }

            return filterSumResult;
        }

        private void DesignGrid_Dispatch()
        {
            var gv1 = this.gridView1;

            if (this.IsRotate)
            {
                gv1.OptionsView.ShowColumnHeaders = true;

                foreach (GridColumn col in this.gridView1.Columns)
                {
                    string fieldName = col.FieldName;

                    if (fieldName == ColName.WEIGHTED_SUM || fieldName == ColName.PRODUCT_ID
                        || fieldName == ColName.STEP_ID)
                    {
                        col.Fixed = FixedStyle.Left;
                    }

                    col.BestFit();
                    col.Width = Math.Min(col.Width, 300);
                }
            }
            else
            {
                gv1.OptionsView.ShowColumnHeaders = false;

                foreach (GridColumn col in gv1.Columns)
                {
                    string fieldName = col.FieldName;
                    if (fieldName == "ColumnName" || fieldName == "Column1")
                    {
                        col.Fixed = FixedStyle.Left;

                    }

                    col.BestFit();
                    col.Width = Math.Min(col.Width, 300);
                }
            }
        }

        private void DesignGrid_Filter()
        {
            var grid = this.gridView3;

            Color custPurple = Color.FromArgb(242, 243, 255);

            int index = grid.Columns[ColName.REASON].AbsoluteIndex;
            int count = grid.Columns.Count;
            for (int i = 0; i < count; i++)
            {
                if (i < index)
                    continue;

                var col = grid.Columns[i];
                col.OptionsColumn.AllowEdit = false;
                col.AppearanceCell.BackColor = custPurple;
            }
        }

        private DataTable CreateSchema_Dispatch(List<PRESET_INFO_HIS> wpList, bool showProductInfo)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(ColName.WEIGHTED_SUM, typeof(string));
            dt.Columns.Add(ColName.DISPATCHING_WIP, typeof(string));

            if (showProductInfo)
            {
                dt.Columns.Add(ColName.PRODUCT_ID, typeof(string));
                dt.Columns.Add(ColName.STEP_ID, typeof(string));
                dt.Columns.Add(ColName.LOT_QTY, typeof(float));
                //dt.Columns.Add(ColName.DETAIL, typeof(string));
            }

            foreach (var wp in wpList)
                dt.Columns.Add(wp.FACTOR_ID, typeof(string));

            return dt;
        }

        private DataTable CreateSchema_FilteredWip()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(ColName.LOT_ID, typeof(string));
            dt.Columns.Add(ColName.PRODUCT_ID, typeof(string));
            dt.Columns.Add(ColName.STEP_ID, typeof(string));
            dt.Columns.Add(ColName.Qty, typeof(int));
            dt.Columns.Add(ColName.REASON, typeof(string));

            return dt;
        }

        private DataTable CreateSchema_Summary()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(ColName.PRODUCT_ID, typeof(string));
            dt.Columns.Add(ColName.STEP_ID, typeof(string));

            dt.Columns.Add(ColName.COUNT, typeof(int));

            dt.Columns.Add(ColName.LOT_QTY, typeof(float));

            return dt;
        }

        private void BindData_Summary(Dictionary<string, List<float>> summary)
        {
            DataTable result = CreateSchema_Summary();

            foreach (var it in summary)
            {
                string key = it.Key;
                var value = it.Value;

                var arr = key.Split('@');

                var productID = arr[0];
                var stepID = arr[1];

                var count = value.Count;
                var sum = value.Sum();

                var row = result.NewRow();
                row[ColName.PRODUCT_ID] = productID;
                row[ColName.STEP_ID] = stepID;
                row[ColName.COUNT] = count;

                row[ColName.LOT_QTY] = sum;

                result.Rows.Add(row);
            }

            gridControl2.DataSource = result;

            gridView2.BestFitColumns();
            gridView2.Columns[ColName.COUNT].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
        }

        #region ColumnName

        public struct ColName
        {
            public static string EQP_GRP = "EQP_GRP";
            public static string EQP_ID = "EQP_ID";
            public static string EQP_TYPE = "EQP_TYPE";
            public static string TARGET_DATE = "TARGET_DATE";
            public static string S_BUSY = "S_BUSY";
            public static string DISP_TIME = "DISPATCHING_TIME";
            public static string SEL_WIP = "SELECTED_WIP";
            public static string FACTOR_LIST = "FACTOR_LIST";
            public static string FILTERED_WIP = "FILTERED_WIP";
            public static string INIT_WIP = "INIT_WIP";
            public static string DISPATCHING_WIP = "LOT_ID";
            public static string WEIGHTED_SUM = "WEIGHTED_SUM";

            public static string PRODUCT_ID = "PRODUCT_ID";
            public static string STEP_ID = "STEP_ID";
            public static string LOT_QTY = "LOT_QTY";
            public static string COUNT = "COUNT";
            public static string DUE_DATE = "LOT_DUE_DATE";

            public static string INFO = "INFO";
            public static string INIT_CNT = "INIT_CNT";
            public static string FilterCount = "FILTERED_CNT";
            public static string DISPATCHING_CNT = "DISPATCHING_CNT";
            public static string SELECTED_CNT = "SELECTED_CNT";

            public static string LOT_ID = "LOT_ID";
            public static string Qty = "QTY";
            public static string REASON = "REASON";

            public static string DETAIL = "DETAIL";
        }

        #endregion ColumnName

        Color _custSky = Color.FromArgb(233, 251, 254);
        Color _custPink = Color.FromArgb(246, 217, 221);
        Color _custGreen = Color.FromArgb(204, 255, 195);
        Color _custYellow = Color.FromArgb(252, 254, 203);
        Color _custPurple = Color.FromArgb(242, 243, 255);

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            int rowHandle = e.RowHandle;
            int columnHandle = e.Column.ColumnHandle;

            if (rowHandle < 0 || columnHandle < 0)
                return;

            var gv = this.gridView1;

            bool isLast = IsMatched_Last(gv, rowHandle, columnHandle, this.IsRotate);
            if (isLast)
                e.Appearance.ForeColor = Color.Red;

            //string selWip = _selectedWip;
            //var cellText = gv1.GetRowCellValue(1, e.Column) as string;            
            //bool isSelectedlot = selWip != null && cellText != null && selWip.Contains(cellText);

            if (this.IsRotate == false)
            {
                if (e.Column.FieldName == "ColumnName")
                {
                    if (rowHandle <= 4)
                    {
                        e.Appearance.BackColor = _custYellow;
                        e.Appearance.Font = new Font(DefaultFont, FontStyle.Italic);
                        e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    }
                    else
                    {
                        e.Appearance.BackColor = _custSky;
                    }
                }
                else
                {
                    if (rowHandle == 0)
                    {
                        e.Appearance.BackColor = _custPurple;
                        e.Appearance.BackColor2 = Color.White;
                        e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        e.Appearance.Font = new Font(DefaultFont, FontStyle.Bold);
                    }
                }

                if (rowHandle == 0)
                {
                    e.Appearance.BackColor = _custGreen;
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }

                if (columnHandle == 1)
                {
                    e.Appearance.BackColor = _custPink;
                    e.Appearance.Font = new Font(DefaultFont, FontStyle.Bold);
                }

                if (columnHandle >= 2 && rowHandle >= 1 && rowHandle <= 7)
                    e.Appearance.BackColor = _custPurple;
            }
            else
            {
                e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;

                if (rowHandle == 0)
                {
                    e.Appearance.BackColor = _custPink;
                    e.Appearance.Font = new Font(DefaultFont, FontStyle.Bold);
                }

                else
                {
                    if (IsHeadOfRoate(e.Column))
                    {
                        e.Appearance.BackColor = _custSky;
                    }

                    if (e.Column.FieldName == ColName.WEIGHTED_SUM)
                    {
                        e.Appearance.BackColor = _custGreen;
                        e.Appearance.Font = new Font(DefaultFont, FontStyle.Bold);
                    }
                }
            }
        }

        private bool IsMatched_Last(GridView gv, int rowHandle, int columnHandle, bool isRotate)
        {
            string lastWip = _lastWip;
            if (string.IsNullOrEmpty(lastWip))
                return false;

            int iproductID = 1;
            int istepID = 2;

            int vauleIndex = isRotate ? columnHandle : rowHandle;
            if (vauleIndex != iproductID
                && vauleIndex != istepID)
            {
                return false;
            }

            string pattern = null;
            if (isRotate)
            {
                string productID = gv.GetRowCellValue(rowHandle, gv.Columns[iproductID]) as string;
                string stepID = gv.GetRowCellValue(rowHandle, gv.Columns[istepID]) as string;

                pattern = string.Format("%{0}%{1}%", productID, stepID);
            }
            else
            {
                string productID = gv.GetRowCellValue(iproductID, gv.Columns[columnHandle]) as string;
                string stepID = gv.GetRowCellValue(istepID, gv.Columns[columnHandle]) as string;

                pattern = string.Format("%{0}%{1}%", productID, stepID);
            }

            if (pattern == null)
                return false;

            return MyHelper.STRING.Like(lastWip, pattern);
        }

        private bool IsHeadOfRoate(GridColumn col)
        {
            if (col.FieldName == ColName.DISPATCHING_WIP)
                return true;

            if (col.FieldName == ColName.PRODUCT_ID)
                return true;

            if (col.FieldName == ColName.STEP_ID)
                return true;

            if (col.FieldName == ColName.LOT_QTY)
                return true;

            if (col.FieldName == ColName.WEIGHTED_SUM)
                return true;

            return false;
        }

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (this.IsRotate == false)
                return;

            if (e.Column == null)
                return;

            if (IsHeadOfRoate(e.Column))
            {
                e.Appearance.BackColor = _custYellow;
                e.Appearance.Font = new Font(DefaultFont, FontStyle.Italic);
            }
            else
            {
                e.Appearance.BackColor = _custPurple;
            }
        }

        private void isRotateView_CheckedChanged(object sender, EventArgs e)
        {
            BindData_Dispatch(_dispInfo);
        }

        private void isShowTypicalLot_CheckedChanged(object sender, EventArgs e)
        {
            BindData_Dispatch(_dispInfo);
        }

        private void showTypicalFilterWip_CheckedChanged(object sender, EventArgs e)
        {
            BindData_Filter(_dispInfo);
        }

        private void GridView3_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            int rowHandle = e.RowHandle;
            int columnHandle = e.Column.ColumnHandle;

            if (rowHandle < 0 || columnHandle < 0)
                return;

            var gv = this.gridView3;

            bool isLast = IsMatched_Last(gv, rowHandle, columnHandle);
            if (isLast)
                e.Appearance.ForeColor = Color.Red;
        }

        private bool IsMatched_Last(GridView gv, int rowHandle, int columnHandle)
        {
            string lastWip = _lastWip;
            if (string.IsNullOrEmpty(lastWip))
                return false;

            int iproductID = gv.Columns[ColName.PRODUCT_ID].ColumnHandle;
            int istepID = gv.Columns[ColName.STEP_ID].ColumnHandle;

            int vauleIndex = columnHandle;
            if (vauleIndex != iproductID
                && vauleIndex != istepID)
            {
                return false;
            }

            string productID = gv.GetRowCellValue(rowHandle, gv.Columns[iproductID]) as string;
            string stepID = gv.GetRowCellValue(rowHandle, gv.Columns[istepID]) as string;

            string pattern = string.Format("%{0}%{1}%", productID, stepID);

            return MyHelper.STRING.Like(lastWip, pattern);
        }

        private void gridView3_DoubleClick(object sender, EventArgs e)
        {
            //GridView view = (GridView)sender;
            //Point pt = view.GridControl.PointToClient(Control.MousePosition);
            //GridHitInfo info = view.CalcHitInfo(pt);

            //if (info.InRowCell == false)
            //    return;

            //DataRow selectRow = view.GetFocusedDataRow();
            //if (selectRow == null)
            //    return;

            //string reason = selectRow.GetString(ColName.REASON);
            //if (MyHelper.STRING.Equals(reason, "MASK") == false)
            //    return;

            //var log = _dispInfo;

            //string eqpID = log.EQP_ID;

            //string stepID = selectRow.GetString(ColName.STEP_ID);
            //string productID = selectRow.GetString(ColName.PRODUCT_ID);
            //string productVersion = selectRow.GetString(ColName.PROD_VER);

            //string dispatchTime = log.DISPATCHING_TIME;

            //try
            //{
            //    ToolUseStateView control = new ToolUseStateView(this.TargetVersionNo);

            //    var dialog = new PopUpForm(control);
            //    dialog.StartPosition = FormStartPosition.CenterParent;
            //    dialog.Show();

            //    control.Query(eqpID, stepID, productID, productVersion, dispatchTime);
            //}
            //catch { }
        }
    }
}
