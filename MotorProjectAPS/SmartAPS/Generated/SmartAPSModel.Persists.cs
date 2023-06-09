/*------------------------------------------------------------------------------------------------------------------------------------------------------------
 <auto-generated>
     This code was generated by a mozart.

     Changes to this file may cause incorrect behavior and will be lost if
     the code is regenerated.
 </auto-generated>
------------------------------------------------------------------------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Mozart.Common;
using Mozart.Collections;
using Mozart.Extensions;
using Mozart.Mapping;
using Mozart.Data;
using Mozart.Data.Entity;
using Mozart.Task.Execution;
using Mozart.Task.Execution.Persists;
using SmartAPS.Inputs;
using SmartAPS.Outputs;

namespace SmartAPS.Persists
{
    
    /// <summary>
    /// SmartAPSModel Persist Model class
    /// </summary>
    public partial class SmartAPSModel_Persist : PersistModule
    {
        public override string Name
        {
            get
            {
                return "SmartAPSModel";
            }
        }
        protected override void Configure()
        {
            InputPersister input = new InputPersister();
            ServiceLocator.RegisterInstance<IInputPersister> (input);
            OutputPersister output = new OutputPersister();
            ServiceLocator.RegisterInstance<IOutputPersister> (output);
        }
        /// <summary>
        /// persist context class
        /// </summary>
        internal partial class InputPersister : InputPersisterBase
        {
            private SmartAPS.Logic.PersistInputs fPersistInputs = new SmartAPS.Logic.PersistInputs();
            protected override void Configure()
            {
                #region Init Config
                // Init
                this.SetParametersInfo(typeof(GlobalParameters), typeof(ConfigParameters), null, null, null, null);
                #endregion
                #region Set Property
                ThreadCount = 1;
                RetryCount = 3;
                ExceptionPolicy = PersistExceptionPolicy.StopAtThrown;
                #endregion
                #region Inputs Config
                // Inputs
                this.Log("Input loading");
                this.StartPerformance();
                #region * Preloading * Config
                // * Preloading *
                this.StartPreloading();
                this.StartPerformance();
                this.EndPerformance();
                this.EndPreloading();
                // * Preloading * End
                #endregion
                #region EXECUTION Persists Config
                // EXECUTION Persists
                this.StartPerformance();
                this.Load<EXECUTION_OPTION_CONFIG>("EXECUTION_OPTION_CONFIG", false, true, null);
                this.Action("EXECUTION_OPTION_CONFIG", null);
                this.Load<CONFIG>("CONFIG", false, true, null);
                this.Action("CONFIG", null);
                this.Load<PLAN_REPORT_TARGET>("PLAN_REPORT_TARGET", false, true, null);
                this.Action("PLAN_REPORT_TARGET", null);
                this.Load<SHIFT_TIME_CONFIG>("SHIFT_TIME_CONFIG", false, true, null);
                this.Action("SHIFT_TIME_CONFIG", null);
                this.EndPerformance();
                // EXECUTION Persists End
                #endregion
                #region BOP Persists Config
                // BOP Persists
                this.StartPerformance();
                this.Load<STD_STEP_INFO>("STD_STEP_INFO", false, true, fPersistInputs.OnAfterLoad_STD_STEP_INFO);
                this.Action("STD_STEP_INFO", null);
                this.Load<STEP_ROUTE>("STEP_ROUTE", false, true, null);
                this.Action("STEP_ROUTE", fPersistInputs.OnAction_STEP_ROUTE);
                this.Load<STEP_TAT>("STEP_TAT", false, true, null);
                this.Action("STEP_TAT", null);
                this.Load<STEP_YIELD>("STEP_YIELD", false, true, null);
                this.Action("STEP_YIELD", null);
                this.Load<PROCESS>("PROCESS", false, true, fPersistInputs.OnAfterLoad_PROCESS);
                this.Action("PROCESS", null);
                this.Load<PRODUCT>("PRODUCT", false, true, fPersistInputs.OnAfterLoad_PRODUCT);
                this.Action("PRODUCT", null);
                this.Load<PRODUCT_ROUTE>("PRODUCT_ROUTE", false, true, fPersistInputs.OnAfterLoad_PRODUCT_ROUTE);
                this.Action("PRODUCT_ROUTE", null);
                this.Load<VALIDATION_BOP>("VALIDATION_BOP", false, true, null);
                this.Action("VALIDATION_BOP", null);
                this.EndPerformance();
                // BOP Persists End
                #endregion
                #region RESOURCE Persists Config
                // RESOURCE Persists
                this.StartPerformance();
                this.Load<PRESET_INFO>("PRESET_INFO", false, true, fPersistInputs.OnAfterLoad_PRESET_INFO);
                this.Action("PRESET_INFO", fPersistInputs.OnAction_PRESET_INFO);
                this.Load<SETUP_INFO>("SETUP_INFO", false, true, null);
                this.Action("SETUP_INFO", null);
                this.Load<EQUIPMENT>("EQUIPMENT", false, true, fPersistInputs.OnAfterLoad_EQUIPMENT);
                this.Action("EQUIPMENT", null);
                this.Load<LINE_INFO>("LINE_INFO", false, true, null);
                this.Action("LINE_INFO", null);
                this.Load<EQP_ARRANGE>("EQP_ARRANGE", false, true, fPersistInputs.OnAfterLoad_EQP_ARRANGE);
                this.Action("EQP_ARRANGE", null);
                this.Load<MAT_SUPPLIER>("MAT_SUPPLIER", false, true, fPersistInputs.OnAfterLoad_MAT_SUPPLIER);
                this.Action("MAT_SUPPLIER", null);
                this.Load<FACTORY_BREAK>("FACTORY_BREAK", false, true, fPersistInputs.OnAfterLoad_FACTORY_BREAK);
                this.Action("FACTORY_BREAK", null);
                this.Load<PM_PLAN>("PM_PLAN", false, true, null);
                this.Action("PM_PLAN", null);
                this.Load<REPLENISH_PLAN>("REPLENISH_PLAN", false, true, null);
                this.Action("REPLENISH_PLAN", fPersistInputs.OnAction_REPLENISH_PLAN);
                this.Load<MATERIAL>("MATERIAL", false, true, fPersistInputs.OnAfterLoad_MATERIAL);
                this.Action("MATERIAL", null);
                this.Load<MATERIAL_BOM>("MATERIAL_BOM", false, true, fPersistInputs.OnAfterLoad_MATERIAL_BOM);
                this.Action("MATERIAL_BOM", null);
                this.Load<SPLIT_INFO>("SPLIT_INFO", false, true, fPersistInputs.OnAfterLoad_SPLIT_INFO);
                this.Action("SPLIT_INFO", null);
                this.Load<TOOL>("TOOL", false, true, null);
                this.Action("TOOL", null);
                this.Load<TOOL_ARRANGE>("TOOL_ARRANGE", false, true, null);
                this.Action("TOOL_ARRANGE", null);
                this.Load<EQP_UTILIZATION>("EQP_UTILIZATION", false, true, fPersistInputs.OnAfterLoad_EQP_UTILIZATION);
                this.Action("EQP_UTILIZATION", null);
                this.EndPerformance();
                // RESOURCE Persists End
                #endregion
                #region TARGET Persists Config
                // TARGET Persists
                this.StartPerformance();
                this.Load<OUT_ACT>("OUT_ACT", false, true, fPersistInputs.OnAfterLoad_OUT_ACT);
                this.Action("OUT_ACT", null);
                this.Load<DEMAND>("DEMAND", false, true, fPersistInputs.OnAfterLoad_DEMAND);
                this.Action("DEMAND", fPersistInputs.OnAction_DEMAND);
                this.Load<WIP>("WIP", false, true, fPersistInputs.OnAfterLoad_WIP);
                this.Action("WIP", null);
                this.Load<FIRM_PLAN>("FIRM_PLAN", false, true, fPersistInputs.OnAfterLoad_FIRM_PLAN);
                this.Action("FIRM_PLAN", fPersistInputs.OnAction_FIRM_PLAN);
                this.Load<IN_PLAN>("IN_PLAN", false, true, null);
                this.Action("IN_PLAN", null);
                this.Load<PEG_CONDITION>("PEG_CONDITION", false, true, null);
                this.Action("PEG_CONDITION", fPersistInputs.OnAction_PEG_CONDITION);
                this.EndPerformance();
                // TARGET Persists End
                #endregion
                this.EndPerformance();
                // Inputs End
                #endregion
            }
        }
        /// <summary>
        /// persist context class
        /// </summary>
        internal partial class OutputPersister : OutputPersisterBase
        {
            protected override void Configure()
            {
                #region Set Property
                ThreadCount = 1;
                RetryCount = 3;
                ExceptionPolicy = PersistExceptionPolicy.StopAtThrown;
                #endregion
                #region Outputs Config
                // Outputs
                this.Log("Output saving");
                this.StartPerformance();
                #region Simulation Persists Config
                // Simulation Persists
                this.StartPerformance();
                this.Commit<MATERIAL_HISTORY>("MATERIAL_HISTORY", true, true);
                this.Commit<UNLOADED_LOTLOG>("UNLOADED_LOTLOG", true, true);
                this.Commit<PRODUCT_ROUTE_LOG>("PRODUCT_ROUTE_LOG", true, true);
                this.Commit<UNKIT_REMAIN_WIPLOG>("UNKIT_REMAIN_WIPLOG", true, true);
                this.Commit<STEP_WIP>("STEP_WIP", true, true);
                this.Commit<RELEASE_HISTORY>("RELEASE_HISTORY", true, true);
                this.Commit<EQP_PLAN>("EQP_PLAN", true, true);
                this.Commit<LOAD_STAT>("LOAD_STAT", true, true);
                this.Commit<LOAD_HISTORY>("LOAD_HISTORY", true, true);
                this.Commit<STEP_MOVE>("STEP_MOVE", true, true);
                this.Commit<TOOL_SEIZE_LOG>("TOOL_SEIZE_LOG", true, true);
                this.Commit<EQP_DISPATCH_LOG>("EQP_DISPATCH_LOG", true, true);
                this.EndPerformance();
                // Simulation Persists End
                #endregion
                #region Log Persists Config
                // Log Persists
                this.StartPerformance();
                this.Commit<ANALYSIS_PSI>("ANALYSIS_PSI", true, true);
                this.Commit<RUN_HISTORY>("RUN_HISTORY", true, true);
                this.Commit<ERROR_LOG>("ERROR_LOG", true, true);
                this.Commit<INIT_WIP_LOG>("INIT_WIP_LOG", true, true);
                this.EndPerformance();
                // Log Persists End
                #endregion
                #region Pegging Persists Config
                // Pegging Persists
                this.StartPerformance();
                this.Commit<INPUT_PLAN>("INPUT_PLAN", true, true);
                this.Commit<PEG_HISTORY>("PEG_HISTORY", true, true);
                this.Commit<STEP_TARGET>("STEP_TARGET", true, true);
                this.Commit<UNPEG_HISTORY>("UNPEG_HISTORY", true, true);
                this.EndPerformance();
                // Pegging Persists End
                #endregion
                #region InputData Persists Config
                // InputData Persists
                this.StartPerformance();
                this.Commit<WIP_HIS>("WIP_HIS", true, true);
                this.Commit<PRESET_INFO_HIS>("PRESET_INFO_HIS", true, true);
                this.Commit<DEMAND_HIS>("DEMAND_HIS", true, true);
                this.EndPerformance();
                // InputData Persists End
                #endregion
                #region AnomalyDetection Persists Config
                // AnomalyDetection Persists
                this.StartPerformance();
                this.Commit<APS_STATUS_MASTER>("APS_STATUS_MASTER", true, true);
                this.Commit<APS_LOG_HISTORY>("APS_LOG_HISTORY", true, true);
                this.EndPerformance();
                // AnomalyDetection Persists End
                #endregion
                this.EndPerformance();
                // Outputs End
                #endregion
            }
        }
    }
}
