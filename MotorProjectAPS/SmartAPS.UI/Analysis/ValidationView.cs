using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Mozart.Collections;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Task.Model;
using SmartAPS.Inputs;
using SmartAPS.UI.Helper;
using SmartAPS.UI.Utils;
using SmartAPS.UserLibrary.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using SmartAPS.UserLibrary.Extensions; 

namespace SmartAPS.UI.Analysis
{
    public partial class ValidationView : MyXtraGridTemplate
    {
        DataSet _validationViewDataSet = new DataSet();
        DataTable _validationViewDataTable = new DataTable();
        DataTable _inputValidationDataTable;
        ModelEngine engine; 
        string _colorFlag;
        string _colorFlag2; 

        public Dictionary<string, string[]> CategoryDic = new Dictionary<string, string[]>();
        public DoubleDictionary<string, string, string[]> TableKeyDic = new DoubleDictionary<string, string, string[]>();
        public Dictionary<string, string[]> ColorDic = new Dictionary<string, string[]>();

        public ValidationView()
        {
            InitializeComponent();            
        }

        public ValidationView(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitializeComponent();
        }

        public ValidationView(IExperimentResultItem targetResult)
        {
            InitializeComponent();
            this.Result = targetResult;
            LoadDocument();
        }

        protected override void LoadDocument()
        {
            base.LoadDocument();
            SetModelEngine();
            InitializeDictionary(); 
            InItializeControls();
        }

        private void InItializeControls()
        {
            SetValidationCombo(this.barEditItem1);
            SetTableRelationCombo(barEditItem2, this.barEditItem1.EditValue.ToString());
            _validationViewDataSet = GetValidationViewDataSet();
            _validationViewDataTable = GetValidationViewDataTable(); 
            Query(this.barEditItem1.EditValue.ToString());   
        }

        private void Query(string ValidationComboValue)
        {
            //SetGrid(this.CategoryDic["ARRANGE"]);
            SetGrid(this.TableKeyDic.GetDictionary(ValidationComboValue));

            BindData(this.TableKeyDic.GetDictionary(ValidationComboValue));
            _colorFlag = ValidationComboValue;
            _colorFlag2 = this.barEditItem2.EditValue.ToString(); 
            (this.gridControl1.MainView as GridView).RowCellStyle += gridView1_RowCellStyle;

            //SetBackColor(this.barEditItem2.EditValue.ToString());
        }

        private void SetGrid(Dictionary<string, string[]> SelectedTableKeyDic)
        {
            this.gridControl1.MainView = AddBands(SelectedTableKeyDic);
            GridView gridView = (this.gridControl1.MainView as GridView);
            gridView.OptionsSelection.MultiSelect = true;
            gridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
            gridView.OptionsBehavior.Editable = false; 
        }
        private void BindData(Dictionary<string, string[]> SelectedTableKeyDic)
        {
            this.gridControl1.BeginUpdate();
            DataColumnCollection allCols = _validationViewDataTable.Copy().Columns;
            DataTable bindDataTable = _validationViewDataTable.Copy();
           
            foreach (DataColumn col in allCols)
            {
                string tableName = col.ColumnName.ToString().Split(this.TableDelimiter, StringSplitOptions.None)[0];
                if (!SelectedTableKeyDic.ContainsKey(tableName))
                    bindDataTable.Columns.Remove(col.ColumnName);
            }

            //데이터가 모두 있는 경우는 이미 기준정보가 완벽한 경우이므로 조회할 필요가 없다. 
            foreach (DataRow dr in bindDataTable.Select())
            {
                if (AreAllColumnsFullorEmpty(dr))
                    dr.Delete();
            }
            bindDataTable.AcceptChanges(); 
            this.gridControl1.DataSource = bindDataTable;
            var bgv = (this.gridControl1.MainView as BandedGridView);

            using (var g = gridControl1.CreateGraphics())
            {
                foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand band in bgv.Bands)
                    band.MinWidth = (int)g.MeasureString(band.Caption, band.AppearanceHeader.Font).Width + 20;
            }
            bgv.BestFitColumns();
            bgv.OptionsView.ColumnAutoWidth = false;
            this.gridControl1.EndUpdate();

        }


        private BandedGridView AddBands(Dictionary<string, string[]> SelectedTableKeyDic)
        {
            int nrOfTables = SelectedTableKeyDic.Keys.Count(); 
            BandedGridView bandedView = new BandedGridView();

            foreach (KeyValuePair<string, string[]> kvp in SelectedTableKeyDic)
            {
                var tableGridBand = new GridBand();
                tableGridBand.Caption = kvp.Key.ToString();
                tableGridBand.Name = kvp.Key.ToString();

                BandedGridColumn[] bandedColumns = new BandedGridColumn[kvp.Value.Length];
                
                for (int i = 0; i < kvp.Value.Length; i++)
                {
                    bandedColumns[i] = bandedView.Columns.AddField(kvp.Key.ToString() + this.TableDelimiter[0] + kvp.Value[i].ToString());
                    bandedColumns[i].Caption = kvp.Value[i].ToString();
                    bandedColumns[i].OwnerBand = tableGridBand;
                    bandedColumns[i].Visible = true;
                    bandedColumns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    bandedColumns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }

                bandedView.Bands.Add(tableGridBand); 
            }
            return bandedView;
        }
        public void SetModelEngine()
        {
            string path = this.Document.ProjectItem.Project.GetFilePath();
            this.engine = ModelEngine.Load(path);
        }
        public IEnumerable<T> GetData<T>(string itemName)
        {
            IEnumerable<T> dt;
            using (var acc = this.engine.LocalAccessorFor(itemName))
            {
                dt = acc.Query<T>(string.Empty, -1, -1);
            }
            return dt;
        }


        public DataSet GetValidationViewDataSet()
        {
            DataSet validDataSet = new DataSet();
            //IEnumerable<VALIDATION_BOP> data = MyHelper.DATASVC.GetEntityData<VALIDATION_BOP>(this.Result); 
            IEnumerable<FullValidationBop> data = JoinFullValidationBopData(); 

            if (data == null)
                return null;
            _inputValidationDataTable = ConvertHelper.ToDataTable(data);

            foreach (DataColumn col in _inputValidationDataTable.Columns)
            {
                if (col.ColumnName == "ObjectState")
                    continue; 

                string tableName = col.ToString().Split(this.TableDelimiter, StringSplitOptions.None)[0];
                //Create Tables 
                if (!validDataSet.Tables.Contains(tableName))
                    validDataSet.Tables.Add(tableName);
                
                string colNameStrings = col.ToString().Split(this.TableDelimiter, StringSplitOptions.None)[1];
                string[] colNames = colNameStrings.Split(this.ColumnDelimiter, StringSplitOptions.None);
                int colCount = 0; 

                foreach (var colName in colNames)
                {
                    string insert_colName = tableName + this.TableDelimiter[0] + colName; 
                    //Create Columns 
                    if (!validDataSet.Tables[tableName].Columns.Contains(insert_colName))
                        validDataSet.Tables[tableName].Columns.Add(insert_colName);

                    //Insert Data 
                    if (_inputValidationDataTable.Rows.Count < 1)
                        continue; 

                    for (int i = 0; i < _inputValidationDataTable.Rows.Count; i++)
                    {
                        DataRow dr;
                        if (validDataSet.Tables[tableName].Rows.Count < i + 1)
                        {
                            dr = validDataSet.Tables[tableName].NewRow();
                            validDataSet.Tables[tableName].Rows.Add(dr);
                        }
                        string insertValue = ""; 
                        string valueStrings = _inputValidationDataTable.Rows[i][col.ColumnName].ToString();
                        if (!string.IsNullOrEmpty(valueStrings))
                        {
                            string[] valueStringArr = valueStrings.ToString().Split(this.KeyDelimiter, StringSplitOptions.None);
                            insertValue = valueStringArr[colCount]; 
                        }
                        validDataSet.Tables[tableName].Rows[i][insert_colName] = insertValue;
                    }
                    colCount++; 
                }
            }
            return validDataSet; 
        }



        public IEnumerable<FullValidationBop> JoinFullValidationBopData()
        {
            IEnumerable<PROCESS> inputProcess = GetData<PROCESS>("PROCESS");
            IEnumerable<PRODUCT> inputProduct = GetData<PRODUCT>("PRODUCT");
            IEnumerable<STD_STEP_INFO> inputStdStepInfo = GetData<STD_STEP_INFO>("STD_STEP_INFO");
            IEnumerable<STEP_ROUTE> inputStepRoute = GetData<STEP_ROUTE>("STEP_ROUTE");
            IEnumerable<EQUIPMENT> inputEquipment = GetData<EQUIPMENT>("EQUIPMENT");
            IEnumerable<EQP_ARRANGE> inputEqpArrange = GetData<EQP_ARRANGE>("EQP_ARRANGE");
            IEnumerable<DEMAND> inputDemand = GetData<DEMAND>("DEMAND");
            var demandProductId = inputDemand.Select(x => x.PRODUCT_ID).Distinct();
            #region FullJoinQuery 
            var j11 = from proc in inputProcess
                      join prod in inputProduct
                      on proc.PROCESS_ID equals prod.PROCESS_ID
                      into joinTable
                      from prod in joinTable.DefaultIfEmpty()
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = proc.PROCESS_ID,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = prod != null ? string.Concat(prod?.PRODUCT_ID, " & ", prod?.PROCESS_ID) : default,
                          PRODUCT___PRODUCT_ID = prod != null ? prod?.PRODUCT_ID : default,
                          PRODUCT___PROCESS_ID = prod != null ? prod?.PROCESS_ID : default
                      };
            var j12 = from prod in inputProduct
                      join proc in inputProcess
                      on prod.PROCESS_ID equals proc.PROCESS_ID
                      into joinTable
                      from proc in joinTable.DefaultIfEmpty()
                      where proc == null
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = proc != null ? proc?.PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = string.Concat(prod.PRODUCT_ID, " & ", prod.PROCESS_ID),
                          PRODUCT___PRODUCT_ID = prod?.PRODUCT_ID,
                          PRODUCT___PROCESS_ID = prod?.PROCESS_ID
                      };

            var full1 = j11.Union(j12);

            var j21 = from f1 in full1
                      join stepRoute in inputStepRoute
                      on f1.PROCESS___PROCESS_ID equals stepRoute.PROCESS_ID
                      into joinTable
                      from stepRoute in joinTable.DefaultIfEmpty()
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f1.PROCESS___PROCESS_ID,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f1.PRODUCT___PRODUCT_ID__PROCESS_ID,
                          PRODUCT___PRODUCT_ID = f1.PRODUCT___PRODUCT_ID,
                          PRODUCT___PROCESS_ID = f1.PRODUCT___PROCESS_ID,
                          STEP_ROUTE___PROCESS_ID = stepRoute != null ? stepRoute?.PROCESS_ID : default,
                          STEP_ROUTE___STEP_ID = stepRoute != null ? stepRoute?.STEP_ID : default,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = stepRoute != null ? string.Concat(stepRoute?.PROCESS_ID, " & ", stepRoute?.STEP_ID) : default,
                      };
            var j22 = from stepRoute in inputStepRoute
                      join f1 in full1
                      on stepRoute.PROCESS_ID equals f1.PROCESS___PROCESS_ID
                      into joinTable
                      from f1 in joinTable.DefaultIfEmpty()
                      where f1 == null
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f1 != null ? f1.PROCESS___PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f1 != null ? f1.PRODUCT___PRODUCT_ID__PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID = f1 != null ? f1.PRODUCT___PRODUCT_ID : default,
                          PRODUCT___PROCESS_ID = f1 != null ? f1.PRODUCT___PROCESS_ID : default,
                          STEP_ROUTE___PROCESS_ID = stepRoute?.PROCESS_ID,
                          STEP_ROUTE___STEP_ID = stepRoute?.STEP_ID,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = string.Concat(stepRoute.PROCESS_ID, " & ", stepRoute.STEP_ID),
                      };

            var full2 = j21.Union(j22);

            var j31 = from f2 in full2
                      join stdStepInfo in inputStdStepInfo
                      on f2.STEP_ROUTE___STEP_ID equals stdStepInfo.STD_STEP_ID
                      into joinTable
                      from stdStepInfo in joinTable.DefaultIfEmpty()
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f2.PROCESS___PROCESS_ID,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f2.PRODUCT___PRODUCT_ID__PROCESS_ID,
                          PRODUCT___PRODUCT_ID = f2.PRODUCT___PRODUCT_ID,
                          PRODUCT___PROCESS_ID = f2.PRODUCT___PROCESS_ID,
                          STEP_ROUTE___PROCESS_ID = f2.STEP_ROUTE___PROCESS_ID,
                          STEP_ROUTE___STEP_ID = f2.STEP_ROUTE___STEP_ID,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = f2.STEP_ROUTE___PROCESS_ID__STEP_ID,
                          STD_STEP_INFO___STD_STEP_ID = stdStepInfo != null ? stdStepInfo?.STD_STEP_ID : default
                      };
            var j32 = from stdStepInfo in inputStdStepInfo
                      join f2 in full2
                      on stdStepInfo.STD_STEP_ID equals f2.STEP_ROUTE___STEP_ID
                      into joinTable
                      from f2 in joinTable.DefaultIfEmpty()
                      where f2 == null
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f2 != null ? f2.PROCESS___PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f2 != null ? f2.PRODUCT___PRODUCT_ID__PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID = f2 != null ? f2.PRODUCT___PRODUCT_ID : default,
                          PRODUCT___PROCESS_ID = f2 != null ? f2.PRODUCT___PROCESS_ID : default,
                          STEP_ROUTE___PROCESS_ID = f2 != null ? f2.STEP_ROUTE___PROCESS_ID : default,
                          STEP_ROUTE___STEP_ID = f2 != null ? f2.STEP_ROUTE___STEP_ID : default,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = f2 != null ? f2.STEP_ROUTE___PROCESS_ID__STEP_ID : default,
                          STD_STEP_INFO___STD_STEP_ID = stdStepInfo?.STD_STEP_ID
                      };

            var full3 = j31.Union(j32);

            var j41 = from f3 in full3
                      join eqparrange in inputEqpArrange
                      on new { A = f3.PRODUCT___PROCESS_ID, B = f3.PRODUCT___PRODUCT_ID, C = f3.STEP_ROUTE___STEP_ID, D = f3.STEP_ROUTE___PROCESS_ID }
                      equals new { A = eqparrange.PROCESS_ID, B = eqparrange.PRODUCT_ID, C = eqparrange.STEP_ID, D = eqparrange.PROCESS_ID }
                      into joinTable
                      from eqparrange in joinTable.DefaultIfEmpty()
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f3.PROCESS___PROCESS_ID,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f3.PRODUCT___PRODUCT_ID__PROCESS_ID,
                          PRODUCT___PRODUCT_ID = f3.PRODUCT___PRODUCT_ID,
                          PRODUCT___PROCESS_ID = f3.PRODUCT___PROCESS_ID,
                          STEP_ROUTE___PROCESS_ID = f3.STEP_ROUTE___PROCESS_ID,
                          STEP_ROUTE___STEP_ID = f3.STEP_ROUTE___STEP_ID,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = f3.STEP_ROUTE___PROCESS_ID__STEP_ID,
                          STD_STEP_INFO___STD_STEP_ID = f3.STD_STEP_INFO___STD_STEP_ID,
                          EQP_ARRANGE___PRODUCT_ID__PROCESS_ID = eqparrange != null ? string.Concat(eqparrange.PRODUCT_ID, " & ", eqparrange.PROCESS_ID) : default,
                          EQP_ARRANGE___PROCESS_ID__STEP_ID = eqparrange != null ? string.Concat(eqparrange.PROCESS_ID, " & ", eqparrange.STEP_ID) : default,
                          EQP_ARRANGE___PROCESS_ID = eqparrange != null ? eqparrange?.PROCESS_ID : default,
                          EQP_ARRANGE___PRODUCT_ID = eqparrange != null ? eqparrange?.PRODUCT_ID : default,
                          EQP_ARRANGE___STEP_ID = eqparrange != null ? eqparrange?.STEP_ID : default,
                          EQP_ARRANGE___EQP_ID = eqparrange != null ? eqparrange?.EQP_ID : default
                      };

            var j42 = from eqparrange in inputEqpArrange
                      join f3 in full3
                      on new { A = eqparrange.PROCESS_ID, B = eqparrange.PRODUCT_ID, C = eqparrange.STEP_ID, D = eqparrange.PROCESS_ID }
                      equals new { A = f3.PRODUCT___PROCESS_ID, B = f3.PRODUCT___PRODUCT_ID, C = f3.STEP_ROUTE___STEP_ID, D = f3.STEP_ROUTE___PROCESS_ID }
                      into joinTable
                      from f3 in joinTable.DefaultIfEmpty()
                      where f3 == null
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f3 != null ? f3.PROCESS___PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f3 != null ? f3.PRODUCT___PRODUCT_ID__PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID = f3 != null ? f3.PRODUCT___PRODUCT_ID : default,
                          PRODUCT___PROCESS_ID = f3 != null ? f3.PRODUCT___PROCESS_ID : default,
                          STEP_ROUTE___PROCESS_ID = f3 != null ? f3.STEP_ROUTE___PROCESS_ID : default,
                          STEP_ROUTE___STEP_ID = f3 != null ? f3.STEP_ROUTE___STEP_ID : default,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = f3 != null ? f3.STEP_ROUTE___PROCESS_ID__STEP_ID : default,
                          STD_STEP_INFO___STD_STEP_ID = f3 != null ? f3.STD_STEP_INFO___STD_STEP_ID : default,
                          EQP_ARRANGE___PRODUCT_ID__PROCESS_ID = string.Concat(eqparrange.PRODUCT_ID, " & ", eqparrange.PROCESS_ID),
                          EQP_ARRANGE___PROCESS_ID__STEP_ID = string.Concat(eqparrange.PROCESS_ID, " & ", eqparrange.STEP_ID),
                          EQP_ARRANGE___PROCESS_ID = eqparrange?.PROCESS_ID,
                          EQP_ARRANGE___PRODUCT_ID = eqparrange?.PRODUCT_ID,
                          EQP_ARRANGE___STEP_ID = eqparrange?.STEP_ID,
                          EQP_ARRANGE___EQP_ID = eqparrange?.EQP_ID
                      };

            var full4 = j41.Union(j42);

            var j51 = from f4 in full4
                      join eqp in inputEquipment
                      on f4.EQP_ARRANGE___EQP_ID equals eqp.EQP_ID
                      into joinTable
                      from eqp in joinTable.DefaultIfEmpty()
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f4.PROCESS___PROCESS_ID,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f4.PRODUCT___PRODUCT_ID__PROCESS_ID,
                          PRODUCT___PRODUCT_ID = f4.PRODUCT___PRODUCT_ID,
                          PRODUCT___PROCESS_ID = f4.PRODUCT___PROCESS_ID,
                          STEP_ROUTE___PROCESS_ID = f4.STEP_ROUTE___PROCESS_ID,
                          STEP_ROUTE___STEP_ID = f4.STEP_ROUTE___STEP_ID,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = f4.STEP_ROUTE___PROCESS_ID__STEP_ID,
                          STD_STEP_INFO___STD_STEP_ID = f4.STD_STEP_INFO___STD_STEP_ID,
                          EQP_ARRANGE___PRODUCT_ID__PROCESS_ID = f4.EQP_ARRANGE___PRODUCT_ID__PROCESS_ID,
                          EQP_ARRANGE___PROCESS_ID__STEP_ID = f4.EQP_ARRANGE___PROCESS_ID__STEP_ID,
                          EQP_ARRANGE___PROCESS_ID = f4.EQP_ARRANGE___PROCESS_ID,
                          EQP_ARRANGE___PRODUCT_ID = f4.EQP_ARRANGE___PRODUCT_ID,
                          EQP_ARRANGE___STEP_ID = f4.EQP_ARRANGE___STEP_ID,
                          EQP_ARRANGE___EQP_ID = f4.EQP_ARRANGE___EQP_ID,
                          EQUIPMENT___EQP_ID = eqp != null ? eqp.EQP_ID : default
                      };

            var j52 = from eqp in inputEquipment
                      join f4 in full4
                      on eqp.EQP_ID equals f4.EQP_ARRANGE___EQP_ID
                      into joinTable
                      from f4 in joinTable.DefaultIfEmpty()
                      where f4 == null
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f4 != null ? f4.PROCESS___PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f4 != null ? f4.PRODUCT___PRODUCT_ID__PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID = f4 != null ? f4.PRODUCT___PRODUCT_ID : default,
                          PRODUCT___PROCESS_ID = f4 != null ? f4.PRODUCT___PROCESS_ID : default,
                          STEP_ROUTE___PROCESS_ID = f4 != null ? f4.STEP_ROUTE___PROCESS_ID : default,
                          STEP_ROUTE___STEP_ID = f4 != null ? f4.STEP_ROUTE___STEP_ID : default,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = f4 != null ? f4.STEP_ROUTE___PROCESS_ID__STEP_ID : default,
                          STD_STEP_INFO___STD_STEP_ID = f4 != null ? f4.STD_STEP_INFO___STD_STEP_ID : default,
                          EQP_ARRANGE___PRODUCT_ID__PROCESS_ID = f4 != null ? f4.EQP_ARRANGE___PRODUCT_ID__PROCESS_ID : default,
                          EQP_ARRANGE___PROCESS_ID__STEP_ID = f4 != null ? f4.EQP_ARRANGE___PROCESS_ID__STEP_ID : default,
                          EQP_ARRANGE___PROCESS_ID = f4 != null ? f4.EQP_ARRANGE___PROCESS_ID : default,
                          EQP_ARRANGE___PRODUCT_ID = f4 != null ? f4.EQP_ARRANGE___PRODUCT_ID : default,
                          EQP_ARRANGE___STEP_ID = f4 != null ? f4.EQP_ARRANGE___STEP_ID : default,
                          EQP_ARRANGE___EQP_ID = f4 != null ? f4.EQP_ARRANGE___EQP_ID : default,
                          EQUIPMENT___EQP_ID = eqp.EQP_ID
                      };
            var full5 = j51.Union(j52);

            var j61 = from f5 in full5
                      join demandProdId in demandProductId
                      on f5.PRODUCT___PRODUCT_ID equals demandProdId
                      into joinTable
                      from demandProdId in joinTable.DefaultIfEmpty()
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f5.PROCESS___PROCESS_ID,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f5.PRODUCT___PRODUCT_ID__PROCESS_ID,
                          PRODUCT___PRODUCT_ID = f5.PRODUCT___PRODUCT_ID,
                          PRODUCT___PROCESS_ID = f5.PRODUCT___PROCESS_ID,
                          STEP_ROUTE___PROCESS_ID = f5.STEP_ROUTE___PROCESS_ID,
                          STEP_ROUTE___STEP_ID = f5.STEP_ROUTE___STEP_ID,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = f5.STEP_ROUTE___PROCESS_ID__STEP_ID,
                          STD_STEP_INFO___STD_STEP_ID = f5.STD_STEP_INFO___STD_STEP_ID,
                          EQP_ARRANGE___PRODUCT_ID__PROCESS_ID = f5.EQP_ARRANGE___PRODUCT_ID__PROCESS_ID,
                          EQP_ARRANGE___PROCESS_ID__STEP_ID = f5.EQP_ARRANGE___PROCESS_ID__STEP_ID,
                          EQP_ARRANGE___PROCESS_ID = f5.EQP_ARRANGE___PROCESS_ID,
                          EQP_ARRANGE___PRODUCT_ID = f5.EQP_ARRANGE___PRODUCT_ID,
                          EQP_ARRANGE___STEP_ID = f5.EQP_ARRANGE___STEP_ID,
                          EQP_ARRANGE___EQP_ID = f5.EQP_ARRANGE___EQP_ID,
                          EQUIPMENT___EQP_ID = f5.EQUIPMENT___EQP_ID,
                          DEMAND___PRODUCT_ID = demandProdId != null ? demandProdId : default
                      };

            var j62 = from demandProdId in demandProductId
                      join f5 in full5
                      on demandProdId equals f5.EQP_ARRANGE___EQP_ID
                      into joinTable
                      from f5 in joinTable.DefaultIfEmpty()
                      where f5 == null
                      select new FullValidationBop
                      {
                          PROCESS___PROCESS_ID = f5 != null ? f5.PROCESS___PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID__PROCESS_ID = f5 != null ? f5.PRODUCT___PRODUCT_ID__PROCESS_ID : default,
                          PRODUCT___PRODUCT_ID = f5 != null ? f5.PRODUCT___PRODUCT_ID : default,
                          PRODUCT___PROCESS_ID = f5 != null ? f5.PRODUCT___PROCESS_ID : default,
                          STEP_ROUTE___PROCESS_ID = f5 != null ? f5.STEP_ROUTE___PROCESS_ID : default,
                          STEP_ROUTE___STEP_ID = f5 != null ? f5.STEP_ROUTE___STEP_ID : default,
                          STEP_ROUTE___PROCESS_ID__STEP_ID = f5 != null ? f5.STEP_ROUTE___PROCESS_ID__STEP_ID : default,
                          STD_STEP_INFO___STD_STEP_ID = f5 != null ? f5.STD_STEP_INFO___STD_STEP_ID : default,
                          EQP_ARRANGE___PRODUCT_ID__PROCESS_ID = f5 != null ? f5.EQP_ARRANGE___PRODUCT_ID__PROCESS_ID : default,
                          EQP_ARRANGE___PROCESS_ID__STEP_ID = f5 != null ? f5.EQP_ARRANGE___PROCESS_ID__STEP_ID : default,
                          EQP_ARRANGE___PROCESS_ID = f5 != null ? f5.EQP_ARRANGE___PROCESS_ID : default,
                          EQP_ARRANGE___PRODUCT_ID = f5 != null ? f5.EQP_ARRANGE___PRODUCT_ID : default,
                          EQP_ARRANGE___STEP_ID = f5 != null ? f5.EQP_ARRANGE___STEP_ID : default,
                          EQP_ARRANGE___EQP_ID = f5 != null ? f5.EQP_ARRANGE___EQP_ID : default,
                          EQUIPMENT___EQP_ID = f5 != null ? f5.EQUIPMENT___EQP_ID : default,
                          DEMAND___PRODUCT_ID = demandProdId
                      };
            var full6 = j61.Union(j62); //join data 전체 full join 
            #endregion

            //Validation에 문제 되는 데이터만 필터링 
            var ret = full6.Where(x => x.PROCESS___PROCESS_ID == null || x.PRODUCT___PRODUCT_ID == null || x.PRODUCT___PROCESS_ID == null ||
                                  x.STEP_ROUTE___PROCESS_ID == null || x.STEP_ROUTE___STEP_ID == null || x.STD_STEP_INFO___STD_STEP_ID == null ||
                                  x.EQP_ARRANGE___EQP_ID == null || x.EQP_ARRANGE___PROCESS_ID == null || x.EQP_ARRANGE___PRODUCT_ID == null ||
                                  x.EQP_ARRANGE___STEP_ID == null || x.EQUIPMENT___EQP_ID == null || x.DEMAND___PRODUCT_ID == null
                                  );
            return ret;
        }
       


        public DataTable GetValidationViewDataTable()
        {
            DataTable allDataTable = new DataTable();
            int rowNum = _validationViewDataSet.Tables[0].Rows.Count;
            for (int i = 0; i < rowNum; i++)
                allDataTable.Rows.Add();
            
            for (int i = 0; i < _validationViewDataSet.Tables.Count; i++)
                allDataTable = MergeTablesByIndex(allDataTable, _validationViewDataSet.Tables[i]);
            
            return allDataTable; 
        }

        public void SetValidationCombo(BarEditItem ValidationCombo)
        {
            ValidationCombo.BeginUpdate();
            repositoryItemComboBox1.Items.Clear();
            foreach (KeyValuePair<string, string[]> kvp in this.CategoryDic)
                repositoryItemComboBox1.Items.Add(kvp.Key);
            ValidationCombo.EditValue = this.CategoryDic.Keys.First(); //set initial value
            ValidationCombo.EndUpdate();
        }
        private void SetTableRelationCombo(BarEditItem TableRelaionCombo, string ValidationComboValue)
        {
            TableRelaionCombo.BeginUpdate();
            repositoryItemComboBox2.Items.Clear();
            
            foreach (string relation in this.CategoryDic[ValidationComboValue])
                repositoryItemComboBox2.Items.Add(relation);
            TableRelaionCombo.EditValue = this.CategoryDic[ValidationComboValue][0]; //set initial value
            TableRelaionCombo.EndUpdate();
        }
        public static DataTable MergeTablesByIndex(DataTable t1, DataTable t2)
        {
            if (t1 == null || t2 == null) throw new ArgumentNullException("t1 or t2", "Both tables must not be null");

            DataTable t3 = t1.Clone();  // first add columns from table1
            foreach (DataColumn col in t2.Columns)
            {
                string newColumnName = col.ColumnName;
                int colNum = 1;
                while (t3.Columns.Contains(newColumnName))
                {
                    newColumnName = string.Format("{0}_{1}", col.ColumnName, ++colNum);
                }
                t3.Columns.Add(newColumnName, col.DataType);
            }
            var mergedRows = t1.AsEnumerable().Zip(t2.AsEnumerable(),
                (r1, r2) => r1.ItemArray.Concat(r2.ItemArray).ToArray());
            foreach (object[] rowFields in mergedRows)
                t3.Rows.Add(rowFields);

            return t3;
        }



        public static bool AreAllColumnsFullorEmpty(DataRow dr)
        {
            if (dr == null || dr.ItemArray.All(r => string.IsNullOrEmpty(r.ToString())) || dr.ItemArray.All(r => !string.IsNullOrEmpty(r.ToString())))
                return true;
            else
                return false;
        }

        private void barEditItem1_SelectedValueChanged(object sender, EventArgs e)
        {
            var cBox = sender as BarEditItem;
            SetTableRelationCombo(barEditItem2, cBox.EditValue.ToString());
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            Query(barEditItem1.EditValue.ToString());
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;

            if (view == null)
                return;
            string[] colorArr = this.ColorDic[_colorFlag2];
            foreach (var color in colorArr)
            {
                if (color == e.Column.FieldName && _colorFlag == this.barEditItem1.EditValue.ToString())
                    e.Appearance.BackColor = Color.MediumSpringGreen;
            }
        }



        public class FullValidationBop
        {
            public string PROCESS___PROCESS_ID { get; set; }
            public string PRODUCT___PRODUCT_ID { get; set; }
            public string PRODUCT___PROCESS_ID { get; set; }
            public string PRODUCT___PRODUCT_ID__PROCESS_ID { get; set; }
            public string EQP_ARRANGE___PRODUCT_ID__PROCESS_ID { get; set; } 
            public string EQP_ARRANGE___PROCESS_ID__STEP_ID { get; set; }
            public string EQP_ARRANGE___PROCESS_ID{ get; set; }
            public string EQP_ARRANGE___STEP_ID { get; set; }
            public string EQP_ARRANGE___PRODUCT_ID{ get; set; }
            public string STEP_ROUTE___PROCESS_ID__STEP_ID { get; set; }
            public string STEP_ROUTE___STEP_ID { get; set; }
            public string STEP_ROUTE___PROCESS_ID { get; set; }
            public string STD_STEP_INFO___STD_STEP_ID { get; set; }
            public string EQP_ARRANGE___EQP_ID { get; set; }
            public string EQUIPMENT___EQP_ID { get; set; }
            public string DEMAND___PRODUCT_ID { get; set; }
        }
        


        //Validation Category  
        public const string DEMAND = "DEMAND";
        public const string BOP = "BOP";
        public const string ARRANGE = "ARRANGE";

        public static string[] DemandRelation = new string[] { "DEMAND-PRODUCT" };
        public static string[] BopRelation = new string[] { "PRODUCT-PROCESS",
                                                            "PROCESS-STEP_ROUTE",
                                                            "STEP_ROUTE-STD_STEP_INFO",
                                                            };
        public static string[] ArrangeRelation = new string[] { "EQP_ARRANGE-STEP_ROUTE",
                                                                "EQP_ARRANGE-PRODUCT",
                                                                "EQP_ARRANGE-EQUIPMENT",
                                                            };
        public static string[] BopColorArr = new string[]{ "PROCESS___PROCESS_ID",
                                                           "PRODUCT___PROCESS_ID",
                                                           "PROCESS___PROCESS_ID",
                                                           "STEP_ROUTE___PROCESS_ID",
                                                           "STEP_ROUTE___STEP_ID",
                                                           "STD_STEP_INFO___STD_STEP_ID"
                                                            };

        public static string[] ArrangeColorArr = new string[]{ "EQP_ARRANGE___PROCESS_ID",
                                                               "EQP_ARRANGE___STEP_ID",
                                                               "STEP_ROUTE___PROCESS_ID",
                                                               "STEP_ROUTE___STEP_ID",
                                                               "EQP_ARRANGE___PROCESS_ID",
                                                               "EQP_ARRANGE___PRODUCT_ID",
                                                               "PRODUCT___PROCESS_ID",
                                                               "PRODUCT___PRODUCT_ID",
                                                               "EQP_ARRANGE___EQP_ID",
                                                               "EQUIPMENT___EQP_ID"
                                                            };

        public static string[] DemandColorArr = new string[]{ "DEMAND___PRODUCT_ID",
                                                              "PRODUCT___PRODUCT_ID"
                                                            };

        public const string EQP_ARRANGE = "EQP_ARRANGE";
        public const string PRODUCT = "PRODUCT";
        public const string PROCESS = "PROCESS";
        public const string STEP_ROUTE = "STEP_ROUTE";
        public const string EQUIPMENT = "EQUIPMENT";
        public const string PRODUCT_ID = "PRODUCT_ID";
        public const string PROCESS_ID = "PROCESS_ID";
        public const string STEP_ID = "STEP_ID";
        public const string STD_STEP_INFO = "STD_STEP_INFO";
        public const string STD_STEP_ID = "STD_STEP_ID";
        public const string EQP_ID = "EQP_ID";

        public static string[] EQP_ARRANGE_KEYS = new string[] { PRODUCT_ID, PROCESS_ID, STEP_ID, EQP_ID };
        public static string[] PROCESS_KEYS = new string[] { PROCESS_ID };
        public static string[] PRODUCT_KEYS = new string[] { PRODUCT_ID, PROCESS_ID };
        public static string[] STEP_ROUTE_KEYS = new string[] { PROCESS_ID, STEP_ID };
        public static string[] STD_STEP_INFO_KEYS = new string[] { STD_STEP_ID };
        public static string[] EQUIPMENT_KEYS = new string[] { EQP_ID };
        public static string[] DEMAND_KEYS = new string[] { PRODUCT_ID };

        string[] ColumnDelimiter = new string[] { "__" };
        string[] TableDelimiter = new string[] { "___" };
        string[] KeyDelimiter = new string[] { " & " };

        public void InitializeDictionary()
        {
            this.CategoryDic.Add(BOP, BopRelation);
            this.CategoryDic.Add(ARRANGE, ArrangeRelation);
            this.CategoryDic.Add(DEMAND, DemandRelation);

            this.TableKeyDic.Add(BOP, PRODUCT, PRODUCT_KEYS);
            this.TableKeyDic.Add(BOP, PROCESS, PROCESS_KEYS);
            this.TableKeyDic.Add(BOP, STEP_ROUTE, STEP_ROUTE_KEYS);
            this.TableKeyDic.Add(BOP, STD_STEP_INFO, STD_STEP_INFO_KEYS);

            this.TableKeyDic.Add(ARRANGE, EQP_ARRANGE, EQP_ARRANGE_KEYS);
            this.TableKeyDic.Add(ARRANGE, PRODUCT, PRODUCT_KEYS);
            this.TableKeyDic.Add(ARRANGE, STEP_ROUTE, STEP_ROUTE_KEYS);
            this.TableKeyDic.Add(ARRANGE, EQUIPMENT, EQUIPMENT_KEYS);

            this.TableKeyDic.Add(DEMAND, DEMAND, DEMAND_KEYS);
            this.TableKeyDic.Add(DEMAND, PRODUCT, PRODUCT_KEYS);

            this.ColorDic.Add(BopRelation[0], new ArraySegment<string>(BopColorArr, 0, 2).ToArray());
            this.ColorDic.Add(BopRelation[1], new ArraySegment<string>(BopColorArr, 2, 2).ToArray());
            this.ColorDic.Add(BopRelation[2], new ArraySegment<string>(BopColorArr, 4, 2).ToArray());

            this.ColorDic.Add(ArrangeRelation[0], new ArraySegment<string>(ArrangeColorArr, 0, 4).ToArray());
            this.ColorDic.Add(ArrangeRelation[1], new ArraySegment<string>(ArrangeColorArr, 4, 4).ToArray());
            this.ColorDic.Add(ArrangeRelation[2], new ArraySegment<string>(ArrangeColorArr, 8, 2).ToArray());


            this.ColorDic.Add(DemandRelation[0], DemandColorArr);

        }

        private void buttonExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            MyHelper.GRIDVIEWEXPORT.ExportToExcel(this.gridView1);
        }
    }
}