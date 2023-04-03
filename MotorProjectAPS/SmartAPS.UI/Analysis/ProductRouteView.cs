using DevExpress.XtraBars;
using DevExpress.XtraDiagram;
using DevExpress.XtraPivotGrid;
using Mozart.Studio.TaskModel.UserLibrary;
using Mozart.Studio;
using Mozart.Common;
using SmartAPS.Inputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.XtraEditors.Repository;
using SmartAPS.Outputs;
using DevExpress.Office.Drawing;
using System.Collections.Specialized;
using DevExpress.Utils.DPI;
using DevExpress.Diagram.Core.Native;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.PivotGrid.OLAP.Mdx;
using System.Windows.Forms;
using System.Drawing;

namespace SmartAPS.UI.Analysis
{
    public partial class ProductRouteView : MyXtraPivotGridTemplate
    {

        private List<PRODUCT_ROUTE> _prodRouteList;
        private Dictionary<string, PRODUCT> _productDict;
        private Dictionary<string, string> _RouteRatioDict;
        private Dictionary<string, List<STEP_ROUTE>> _stepRouteDict;
        private Dictionary<string, ResultItem> _stepAllDic;
        private Dictionary<string, List<STEP_MOVE>> _stepMoveDict;
        private Dictionary<string, List<STEP_TARGET>> _stepTargetDict;
        private Dictionary<string, double> _categoryIndexs;
     
        private List<BOMProduct> _allProdList;
        public string targetData = "Target Date";
        public string targetQty = "Target Qty";
        public string planQty = "Plan Qty";
        public string onTimePlanQty = "On-Time Plan Qty";
        public string rtfRate = "RTF Rate";
        public string fulfillmentRate = "Fulfillment Rate";


        public class BOMProduct
        {
            public string ID { get; set; }
            public string ProcessID { get; set; }
            public string StepRoute { get; set; }
            public List<BOMProduct> Children { get; set; } = new List<BOMProduct>();

            public List<BOMProduct> AddChildren(BOMProduct product)
            {
                Children.Add(product);
                return Children;
            }
        }

        public class ResultItem
        {
            public string StepID { get; private set; }
            public string Category { get; private set; }
            public string InOutCate { get; private set; }
            public DateTime InTime { get; set; }
            public DateTime OutTime { get; set; }
            public double TargetInQty { get; set; }
            public double TargetOutQty { get; set; }
            public double MoveDateInQty { get; set; }
            public double MoveDateOutQty { get; set; }
            public double MoveInQty { get; set; }
            public double MoveOutQty { get; set; }
            public double Value { get; private set; }

            public ResultItem(DateTime intime, DateTime outTime, double inQty, double outQty, double moveInQty, double moveOutQty, double movedataInQty, double moveDataOutQty)
            {
                this.InTime = intime;
                this.OutTime = outTime;
                this.TargetInQty = inQty;
                this.TargetOutQty = outQty;
                this.MoveDateInQty = movedataInQty;
                this.MoveDateOutQty = moveDataOutQty;
                this.MoveInQty = moveInQty;
                this.MoveOutQty = moveOutQty;
            }
            public ResultItem(string stepID, string inOutcate, string category, double value)
            {
                this.StepID = stepID;
                this.InOutCate = inOutcate;
                this.Category = category;
                this.Value = value;
            }
        }

        private string TargetProductID
        {
            get
            {
                return this.editProdId.EditValue as string;
            }
        }

        private string TargetDemandID
        {
            get
            {
                return this.editDemandId.EditValue as string;
            }
        }

        public ProductRouteView()
        {
            InitializeComponent();

        }

        public ProductRouteView(IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            InitializeComponent();
        }

        private void SetControls()
        {
            //var facStartTime = MyHelper.DATASVC.GetPlanStartTime(this.Result);
            //MyHelper.SHIFTTIME.SetFactoryStartTime(facStartTime);

            MyHelper.ENGCONTROL.SetControl_ProductID(this.editProdId, this.Result);
            SetControl_DemandID(this.editDemandId);
            this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            ShowTotal(this.detailGrid);
            makeCategoryDic();

        }

        private void ShowTotal(PivotGridControl pivot, bool isCheck = false)
        {
            pivot.OptionsView.ShowRowTotals = false;
            pivot.OptionsView.ShowRowGrandTotals = false;
            pivot.OptionsView.ShowColumnTotals = false;
            pivot.OptionsView.ShowColumnGrandTotals = false;
        }

        public void SetControl_DemandID(BarEditItem control)
        {
            control.BeginUpdate();

            var cbEdit = control.Edit as RepositoryItemComboBox;
            if (cbEdit == null)
                return;

            cbEdit.Items.Clear();

            List<object> list = GetList_DemandID();

            cbEdit.Items.AddRange(list);

            if (!cbEdit.Items.Contains(control.EditValue))
            {
                if (cbEdit.Items.Count > 0)
                    control.EditValue = cbEdit.Items[0];
                else
                    control.EditValue = null;
            }

            control.EndUpdate();
            control.Refresh();
        }

        private List<object> GetList_DemandID()
        {
            List<object> demandIdList = new List<object>();

            var demand = MyHelper.DATASVC.GetEntityData<DEMAND_HIS>(this.Result);

            foreach (var item in demand)
            {
                if (item.PRODUCT_ID != this.TargetProductID)
                    continue;

                string value = item.DEMAND_ID;

                if (string.IsNullOrEmpty(value) == false && demandIdList.Contains(item.DEMAND_ID) == false)
                    demandIdList.Add(value);
            }

            demandIdList.Sort();
            return demandIdList;
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();

            this.LoadData();

            this.SetMainPivotGrid(this.detailGrid);

            this.AddExtendPivotGridMenu(this.detailGrid);

            SetControls();

            this.diagramControl1.ApplyMindMapTreeLayout();

            //RunQuery();
        }

        private void LoadData()
        {
            _prodRouteList = MyHelper.DATASVC.GetEntityData<PRODUCT_ROUTE>(this.Result).ToList();
            _RouteRatioDict = new Dictionary<string, string>();
            foreach (var route in _prodRouteList) 
            {
                string key = route.FROM_PRODUCT_ID +"@"+ route.TO_PRODUCT_ID;
                string InOutQtyValue = Convert.ToString(route.IN_QTY);
                _RouteRatioDict.Add(key, InOutQtyValue);
            }

            _productDict = new Dictionary<string, PRODUCT>();
            var prodTable = MyHelper.DATASVC.GetEntityData<PRODUCT>(this.Result).ToList();
            foreach (var prod in prodTable)
                _productDict.Add(prod.PRODUCT_ID, prod);

            _stepRouteDict = new Dictionary<string, List<STEP_ROUTE>>();
            var routeTable = MyHelper.DATASVC.GetEntityData<STEP_ROUTE>(this.Result).ToList();
            foreach (var step in routeTable)
            {
                if (_stepRouteDict.ContainsKey(step.PROCESS_ID))
                    _stepRouteDict[step.PROCESS_ID].Add(step);
                else
                    _stepRouteDict.Add(step.PROCESS_ID, new List<STEP_ROUTE>() { step });
            }
            
            _stepTargetDict = new Dictionary<string, List<STEP_TARGET>>();
            var targetTable = MyHelper.DATASVC.GetEntityData<STEP_TARGET>(this.Result).ToList();
            foreach (var target in targetTable.OrderBy(x => x.TARGET_DATE))
            {
                var key = target.STEP_ID + "@" + target.PRODUCT_ID;

                if (_stepTargetDict.ContainsKey(key))
                    _stepTargetDict[key].Add(target);
                else
                    _stepTargetDict.Add(key, new List<STEP_TARGET>() { target });
            }

            _stepMoveDict = new Dictionary<string, List<STEP_MOVE>>();
            var moveTable = MyHelper.DATASVC.GetEntityData<STEP_MOVE>(this.Result).ToList();
            foreach (var move in moveTable)
            {
                var key = move.STEP_ID + "@" + move.PRODUCT_ID;

                if (_stepMoveDict.ContainsKey(key))
                    _stepMoveDict[key].Add(move);
                else
                    _stepMoveDict.Add(key, new List<STEP_MOVE>() { move });
            }
        }

        protected override void Query()
        {
            var facStartTime = MyHelper.DATASVC.GetPlanStartTime(this.Result);

            this.DrawBOMMap();
         
            this.diagramControl1.FitToPage();
            this.diagramControl1.AlignPage(DevExpress.Utils.HorzAlignment.Center);
            this.diagramControl1.Refresh();
        }

        private void DrawBOMMap()
        {
            _allProdList = new List<BOMProduct>();
            if (string.IsNullOrEmpty(this.TargetProductID))
                return;

            List<BOMProduct> bomSource;
            PRODUCT targetProduct;

            if (_productDict.TryGetValue(this.TargetProductID, out targetProduct))
                bomSource = new List<BOMProduct>() { new BOMProduct() { ID = targetProduct.PRODUCT_ID,
                    ProcessID = targetProduct.PROCESS_ID,
                    StepRoute = GetStepRouteByProcId(targetProduct.PROCESS_ID, targetProduct.PRODUCT_ID)
                } };
                    
            else
                bomSource = new List<BOMProduct>() { new BOMProduct() { ID = this.TargetProductID } };

            GetAllChildProduct(bomSource);

            this.diagramControl1.BeginUpdate();
            //diagramOrgChartController1.LayoutKind = DevExpress.Diagram.Core.DiagramLayoutKind;
      
            diagramOrgChartController1.DataSource = bomSource;
          
            this.diagramControl1.EndUpdate();
            foreach (var item in this.diagramControl1.Items)
            {
             
                if (item.GetType() == typeof(DiagramConnector))
                {
                    DiagramConnector connector = item as DiagramConnector;

                    var changeArrow = connector.BeginArrow;
                    connector.BeginArrow = connector.EndArrow;
                    connector.EndArrow = changeArrow;
                    
                    var beginBOM = connector.BeginItem.DataContext as BOMProduct;
                    var endBOM = connector.EndItem.DataContext as BOMProduct;
                    string key = endBOM.ID + "@" + beginBOM.ID;
                    string value;
                    if(_RouteRatioDict.TryGetValue(key, out value))
                        connector.Content = value;
                }
            }

            this.diagramControl1.CalcBestSize();
        }

        private string GetStepRouteByProcId(string processId, string productId)
        {
            List<STEP_ROUTE> stepRouteList;

            double rtf = 0;
            double fulfill = 0;

            if (_stepRouteDict.TryGetValue(processId, out stepRouteList) == false)
                return null;

            var stepRoute = stepRouteList.OrderBy(r => r.STEP_SEQ).LastOrDefault();

            string rtfKey = stepRoute.STEP_ID + rtfRate+"OUT";
            string fuliKey = stepRoute.STEP_ID + fulfillmentRate + "OUT";

            var result = SelectedLoadData(productId, processId, new List<STEP_ROUTE>() { stepRoute });

            if(result.Count() == 0)
                return string.Format("RTF Rate : {0}% \r\n Fulfillment Rate : {1}%", rtf, fulfill);

            rtf = result[rtfKey].Value;
  
            fulfill = result[fuliKey].Value;

            return string.Format("RTF Rate : {0}% \r\n Fulfillment Rate : {1}%", rtf, fulfill);
        }

        private void GetAllChildProduct(List<BOMProduct> parentProds)
        {
            foreach (var parent in parentProds)
            {
                var childProds = _prodRouteList.Where(x => x.TO_PRODUCT_ID == parent.ID);

                foreach (var child in childProds)
                {
                    if (parent.Children != null && parent.Children.Count > 0)
                        if (parent.Children.Where(p => p.ID == child.FROM_PRODUCT_ID).Count() > 0)
                            continue;

                    List<BOMProduct> children;
                    PRODUCT childInfo;
                    if (_productDict.TryGetValue(child.FROM_PRODUCT_ID, out childInfo))
                        children = parent.AddChildren(new BOMProduct()
                        {
                            ID = childInfo.PRODUCT_ID,
                            ProcessID = childInfo.PROCESS_ID,
                            StepRoute = GetStepRouteByProcId(childInfo.PROCESS_ID, childInfo.PRODUCT_ID),
                        });
                    else
                        children = parent.AddChildren(new BOMProduct() { ID = child.FROM_PRODUCT_ID });

                    GetAllChildProduct(children);
                }
            }
        }

        private Dictionary<string, ResultItem> SelectedLoadData(string productID, string processID, List<STEP_ROUTE> stepList)
        {
            List<STEP_TARGET> targetList;
            List<STEP_MOVE> moveList;
            var eqpPlans = MyHelper.DATASVC.GetEntityData<EQP_PLAN>(this.Result).ToList();

            _stepAllDic = new Dictionary<string, ResultItem>();

            List<string> inOut = new List<string> { "IN", "OUT" };
            Dictionary<string, DateTime> timeDic = new Dictionary<string, DateTime>();

            foreach (var step in stepList.OrderBy(x => x.STEP_SEQ))
            {
                string key = step.STEP_ID + "@" + productID;
                if (_stepTargetDict.TryGetValue(key, out targetList) == false)
                    continue;

                var stepTarget = targetList.Where(t => t.MO_DEMAND_ID == this.TargetDemandID);

                double inQty = 0;
                double outQty = 0;
                double dateInQty = 0;
                double dateOutQty = 0;

                if (stepTarget.Count() == 0)
                    continue;

                foreach (var stTarget in stepTarget)
                {
                    ResultItem re;
                    if (_stepAllDic.TryGetValue(stTarget.STEP_ID, out re) == false)
                        _stepAllDic.Add(stTarget.STEP_ID, new ResultItem(stTarget.TARGET_DATE, stTarget.TARGET_DATE, stTarget.IN_QTY, stTarget.OUT_QTY, 0, 0, 0, 0));
                    else
                    {
                        if (stTarget.IN_QTY > stTarget.OUT_QTY)
                        {
                            re.TargetInQty = stTarget.IN_QTY;
                        }
                        else
                        {
                            re.TargetOutQty = stTarget.OUT_QTY;
                            re.OutTime = stTarget.TARGET_DATE;
                        }
                    }
                }

                DateTime inTime = _stepAllDic[step.STEP_ID].InTime;
                DateTime outTime = _stepAllDic[step.STEP_ID].OutTime;

                if (_stepMoveDict.TryGetValue(key, out moveList) == false)
                    continue;

                var stepMove = moveList.Where(t => t.DEMAND_ID == this.TargetDemandID);

                if (stepMove.Count() == 0)
                    continue;

                //foreach (var stMove in stepMove)
                //{
                //    inQty += stMove.IN_QTY;
                //    outQty += stMove.OUT_QTY;

                //    if (stMove.PLAN_DATE < intime)
                //        dateInQty += stMove.IN_QTY;

                //    if (stMove.PLAN_DATE < outtime)
                //        dateOutQty += stMove.OUT_QTY;
                //}

                foreach (var eqpPlan in eqpPlans)
                {
                    if (eqpPlan.EQP_STATE_CODE != "BUSY")
                        continue;

                    if (eqpPlan.STEP_ID != step.STEP_ID)
                        continue;

                    if (eqpPlan.PRODUCT_ID != productID)
                        continue;

                    if (eqpPlan.DEMAND_ID != this.TargetDemandID)
                        continue;

                    inQty += eqpPlan.PROCESS_QTY;

                    if (eqpPlan.EQP_START_TIME <= inTime)
                        dateInQty += eqpPlan.PROCESS_QTY;

                    if (eqpPlan.EQP_END_TIME <= outTime)
                        dateOutQty += eqpPlan.PROCESS_QTY;
                }

                _stepAllDic[step.STEP_ID].MoveInQty = inQty;  // eqp plan에서 토탈 시뮬레이션 기간 동안 해당 step에 몇 개가 투입 되었는 지
                _stepAllDic[step.STEP_ID].MoveOutQty = inQty; //outQty;
                _stepAllDic[step.STEP_ID].MoveDateInQty = dateInQty;  // eqp plan에서 target date까지 해당 step에 몇 개가 투입 되었는 지
                _stepAllDic[step.STEP_ID].MoveDateOutQty = dateOutQty;
            }

            List<string> list = new List<string>() { targetData, targetQty, planQty, onTimePlanQty, rtfRate, fulfillmentRate };
            Dictionary<string, ResultItem> items = new Dictionary<string, ResultItem>();

            foreach (var step in _stepAllDic.Keys)
            {
                foreach (var category in list)
                {
                    foreach (var io in inOut)
                    {
                        string stepKey = step + category + io;
                        if (!items.ContainsKey(stepKey))
                        {
                            var value = ReturnValue(step, category, io);
                            items.Add(stepKey, new ResultItem(step, io, category, value));
                        }

                    }
                }
            }
            return items;
        }

        #region Event

        private void buttonLoad_ItemClick(object sender, ItemClickEventArgs e)
        {
            Query();
        }

        private void buttonExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            //MyHelper.GRIDVIEWEXPORT.ExportToExcel(this.gridView1);
        }


        #endregion

        private void diagramControl1_SelectionChanged(object sender, DiagramSelectionChangedEventArgs e)
        {
            var selectItem = diagramControl1.SelectedItems;

            if (selectItem.Count() != 0)
                this.dockPanel1.Show();
            else
                this.dockPanel1.HideSliding();

            foreach (var item in selectItem)
            {
                var itemData = item.DataContext as BOMProduct;
                if (itemData == null)
                    return;

                if (_stepRouteDict.ContainsKey(itemData.ProcessID) == false)
                    return;
                List<STEP_ROUTE> stepList = new List<STEP_ROUTE>();

                if (_stepRouteDict.TryGetValue(itemData.ProcessID, out stepList) == false)
                    return;

                var resultData = SelectedLoadData(itemData.ID, itemData.ProcessID, stepList);
                XtraPivotGridHelper.DataViewTable table = CreateDataViewSchema();
                FillData(table , resultData);
                BindData(table);
            }
        }

        private XtraPivotGridHelper.DataViewTable CreateDataViewSchema()
        {
            XtraPivotGridHelper.DataViewTable dt = new XtraPivotGridHelper.DataViewTable();

            dt.AddColumn("STEP_ID", "STEP_ID", typeof(string), PivotArea.ColumnArea, null, null);
            dt.AddColumn("INOUT", "INOUT", typeof(string), PivotArea.ColumnArea, null, null);
            dt.AddColumn("CATEGORY", "CATEGORY", typeof(string), PivotArea.RowArea, null, null);
            dt.AddColumn("VALUE", "VALUE", typeof(double), PivotArea.DataArea, null, null);

            dt.AddDataTablePrimaryKey(
                    new DataColumn[]
                    {
                        dt.Columns["STEP_ID"],
                        dt.Columns["INOUT"],
                        dt.Columns["CATEGORY"]
                    }
                );

            return dt;
        }

        private void detailGrid_CellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
        {
            if (e.DataField == null || e.RowField == null)
                return;

            var dataField = e.GetCellValue(e.DataField);
            var rowField = e.GetFieldValue(e.RowField);
            if (rowField as string == rtfRate || rowField as string == fulfillmentRate)
                e.DisplayText = string.Format("{0}%", dataField);
            if (rowField as string == targetData)
            {
                var c = GetDateString(Convert.ToString(dataField));
                e.DisplayText = c;
            }
        }

        private string GetDateString(string value, bool withTime = true)
        {
            DateTime primary;

            if (value == null)
            {
                primary = DateTime.MinValue;
                return primary.ToString("yyyyMMddHHmm");
            }

            int start = 0;
            int num = 0;//중간 띄어쓰기 위치
            string tmp = value;
            while (tmp.IndexOf(" ") > 0)
            {
                num = tmp.IndexOf(" ");
                string tmp1 = tmp.Substring(0, num);
                start = num + 1;
                tmp1 += tmp.Substring(num + 1);
                tmp = tmp1;
            }

            value = tmp; //MyHelper.STRING.Trim(value);
            int length = value.Length;

            if (length < 8)
            {
                primary = DateTime.MinValue;
                return primary.ToString("yyyyMMddHHmm");
            }

            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            try
            {
                year = int.Parse(value.Substring(0, 4));
                month = int.Parse(value.Substring(4, 2));
                day = int.Parse(value.Substring(6, 2));
                if (withTime)
                {
                    int t = 8;

                    if (length >= 10)
                    {
                        if (value[8] == ' ')
                            t++;

                        hour = int.Parse(value.Substring(t + 0, 2));
                    }

                    if (length >= 12)
                    {
                        if (value[8] == ' ')
                            t++;

                        minute = int.Parse(value.Substring(t + 2, 2));
                    }

                    if (length >= 14)
                    {
                        second = int.Parse(value.Substring(t + 4, 2));
                    }
                }
            }
            catch
            {
            }
            primary = new DateTime(year, month, day, hour, minute, second);
            return primary.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void FillData(XtraPivotGridHelper.DataViewTable dt, Dictionary<string, ResultItem> items)
        {
            if (items == null)
                return;

            var resultData = items.Values;
            var table = dt.DataTable;
            
            int digitStep = MyHelper.DATASVC.GetEntityData<STEP_ROUTE>(this.Result).Max(x => x.STEP_SEQ);
            digitStep = digitStep < 10 ? 1 : digitStep < 100 ? 2 : 3;
            int stepSeq = 0;
            string prevStepID = string.Empty;
            string strSeq = string.Empty;

            foreach (var data in resultData)
            {
                if (prevStepID != data.StepID)
                {
                    prevStepID = data.StepID;
                    stepSeq++;
                    strSeq = "[" + stepSeq.ToString("D" + digitStep.ToString()) +"] ";
                }

                table.Rows.Add(
                                strSeq + data.StepID,
                                data.InOutCate,
                                data.Category,
                                data.Value
                            );
            }
        }

        private void BindData(XtraPivotGridHelper.DataViewTable table)
        {
            this.detailGrid.BeginInit();
            this.detailGrid.BeginUpdate();

            this.detailGrid.ClearPivotGridFields();
            this.detailGrid.CreatePivotGridFields(table);

            this.detailGrid.DataSource = table.DataTable;

            detailGrid.CustomCellDisplayText += detailGrid_CellDisplayText;

            detailGrid.Fields["VALUE"].CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            detailGrid.Fields["VALUE"].CellFormat.FormatString = "#,##0";

            this.detailGrid.Fields["CATEGORY"].SortMode = PivotSortMode.Custom;

            this.detailGrid.EndUpdate();
            this.detailGrid.EndInit();
            detailGrid.OptionsCustomization.AllowDrag = false;
            detailGrid.BestFit();
            detailGrid.BestFitColumnArea();
        }

        private double ReturnValue(string step, string category, string inOut)
        {
            double value;
            if (step == null && category == null && inOut == null)
                return 0;

            if (inOut == "IN")
            {
                if (category == targetData)
                {
                    string datestr = _stepAllDic[step].InTime.ToString("yyyyMMddHHmmss");
                    value = Convert.ToDouble(datestr);
                }
                else if (category == targetQty)
                    value = _stepAllDic[step].TargetInQty;
                else if (category == planQty)
                    value = _stepAllDic[step].MoveInQty;
                else if (category == onTimePlanQty)
                    value = _stepAllDic[step].MoveDateInQty;
                else if (category == rtfRate)
                    value = Math.Round((_stepAllDic[step].MoveDateInQty / _stepAllDic[step].TargetInQty) * 100);
                else
                    value = Math.Round((_stepAllDic[step].MoveInQty / _stepAllDic[step].TargetInQty) * 100);
            }

            else
            {
                if (category == targetData)
                {
                    string datestr = _stepAllDic[step].OutTime.ToString("yyyyMMddHHmmss");
                    value = Convert.ToDouble(datestr);
                }
                else if (category == targetQty)
                    value = _stepAllDic[step].TargetOutQty;
                else if (category == planQty)
                    value = _stepAllDic[step].MoveOutQty;
                else if (category == onTimePlanQty)
                    value = _stepAllDic[step].MoveDateOutQty;
                else if (category == rtfRate)
                    value = Math.Round((_stepAllDic[step].MoveDateOutQty / _stepAllDic[step].TargetOutQty) * 100);
                else
                    value = Math.Round((_stepAllDic[step].MoveOutQty / _stepAllDic[step].TargetOutQty) * 100);
            }
            if (Convert.ToString(value).Equals("NaN"))
                value = 100;

            return value;
        }

        private void makeCategoryDic()
        {
            _categoryIndexs = new Dictionary<string, double>();
            _categoryIndexs.Add(targetData, 0);
            _categoryIndexs.Add(targetQty, 1);
            _categoryIndexs.Add(planQty, 2);
            _categoryIndexs.Add(onTimePlanQty, 3);
            _categoryIndexs.Add(rtfRate, 4);
            _categoryIndexs.Add(fulfillmentRate, 5);
        }

        private double ConvertCategoryIndex(string category)
        {
            if (category == null)
                return 999;
            double seq = 0;
            if (_categoryIndexs.TryGetValue(category, out seq))
                return seq;
            return 999;
        }

        private void editProdId_EditValueChanged(object sender, EventArgs e)
        {
            SetControl_DemandID(this.editDemandId);
        }

        private void detailGrid_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
        {
            if (e.Field.FieldName == "CATEGORY")
            {
                double c1 = ConvertCategoryIndex(e.Value1 as string);
                double c2 = ConvertCategoryIndex(e.Value2 as string);
                if (c1 == c2)
                    return;
                e.Result = c1.CompareTo(c2);

                e.Handled = true;
            }
        }

        private void diagramControl1_ItemDrawing(object sender, DiagramItemDrawingEventArgs e)
        {
            //e.Item.Appearance.BackColor = Color.Red;
        }

        private void diagramControl1_CustomDrawItem(object sender, CustomDrawItemEventArgs e)
        {
            Color color;
            if (e.Item.DataContext.GetType() == typeof(BOMProduct))
                color = GetBoxColor(((BOMProduct)e.Item.DataContext).StepRoute);
            else
                color = GetBoxColor(((Connection<BOMProduct, BOMProduct>)e.Item.DataContext).From.StepRoute);
            
            e.Item.Appearance.BackColor = color;
        }

        private Color GetBoxColor(string str)
        {
            Color color = Color.Aquamarine;

            float fulfill = float.Parse(str.Split(':').Last().Trim().Replace("%", ""));
            if (fulfill < 100)
                return Color.Yellow;

            return color;
        }

        private void detailGrid_CustomDrawCell(object sender, PivotCustomDrawCellEventArgs e)
        {
            string rowField = e.GetFieldValue(e.RowField).ToString();
            if (rowField == "Fulfillment Rate")
            {
                if(float.Parse(e.Value.ToString()) < 100)
                    e.Appearance.BackColor = Color.Yellow;
            }
        }
    }
}
