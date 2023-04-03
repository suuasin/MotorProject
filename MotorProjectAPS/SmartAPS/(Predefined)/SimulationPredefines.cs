using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using Mozart.SeePlan.DataModel;
using Mozart.SeePlan.Simulation;
using System.ComponentModel;
using Mozart.Simulation.Engine;
using SmartAPS.Logic;
using SmartAPS.Inputs;

namespace SmartAPS
{
    [FeatureBind()]
    [MyDefault]
    public class SimulationPredefines
    {
        internal static SimulationPredefines Instance
        {
            get { return ServiceLocator.Resolve<SimulationPredefines>(); }
        }

        #region SimInit

        [FeatureAttachTo("SimInit/OnCreateLot", Root = "Simulation", Bind = true)]
        public SmartAPSLot CREATE_LOT_DEF(SmartAPSWipInfo info, bool isWip, ref bool handled, SmartAPSLot prevReturnValue)
        {
            return CreateHelper.CreateLot(info, isWip);
        }



        [FeatureAttachTo("SimInit/OnGetReleaseTerm", Root = "Simulation", Bind = true)]
        public double GET_RELEASE_TERM_DEF(int lotSize, ref bool handled, double prevReturnValue)
        {
            double defTerm = ConfigHelper.GetConfig<double>(Constants.DEFAULT_RELEASE_TERM);

            return defTerm == 0 ? 24 / (double)lotSize : defTerm;
        }

        [FeatureAttachTo("SimInit/OnGetLotSize", Root = "Simulation", Bind = true)]
        public int GET_LOT_SIZE_DEF(SmartAPSProduct prod, ref bool handled, int prevReturnValue)
        {
            if (prod.LotSize == 0)
            {
                int defSize = ConfigHelper.GetConfig<int>(Constants.DEFAULT_LOT_SIZE);

                return defSize > 0 ? defSize : SeeplanConfiguration.Instance.LotUnitSize;
            }
            else
            {
                return prod.LotSize;
            }
        }

        [FeatureAttachTo("SimInit/OnGetStateTime", Root = "Simulation", Bind = true)]
        public DateTime GET_STATE_TIME_DEF(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, DateTime prevReturnValue)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            DateTime time = aeqp.NowDT;

            bool val = ConfigHelper.GetConfig<bool>(Constants.USE_CALC_STATE_TIME);

            if (val == true)
            {
                SmartAPSWipInfo wip = lot.WipInfo as SmartAPSWipInfo;

                ProcTimeInfo info = TimeHelper.GetProcessTime(hb, aeqp);
                double sec = info.FlowTime.TotalSeconds * wip.OutQty;

                time = aeqp.NowDT.AddSeconds(-sec);
            }
            else
            {
                if (lot.WipInfo.WipStateTime > SimHelper.MinDateTime)
                    time = lot.StateTime;
            }

            time = time < aeqp.NowDT ? time : aeqp.NowDT;

            return time;
        }
        [FeatureAttachTo("SimInit/OnGetBlockInTargetLots", Root = "Simulation", Bind = true)]
        public bool GET_BLOCK_IN_TARGET_LOTS(ref bool handled, bool prevReturnValue)
        {
            bool val = ConfigHelper.GetConfig<bool>(Constants.BLOCK_IN_TARGET_LOTS);

            return val;
        }
        [FeatureAttachTo("SimInit/OnGetReleaseLotOption", Root = "Simulation", Bind = true)]
        public string GET_RELEASE_LOT_OPTION_DEF(ref bool handled, string prevReturnValue)
        {
            string val = ConfigHelper.GetConfig<string>(Constants.RELEASE_LOT_OPTION);

            if (string.IsNullOrWhiteSpace(val))
                return prevReturnValue;

            return val;
        }

        [FeatureAttachTo("SimInit/OnGetUseUnpegWipOption", Root = "Simulation", Bind = true)]
        public bool GET_USE_UNPEG_WIP_OPTION_DEF(ref bool handled, bool prevReturnValue)
        {
            bool val = ConfigHelper.GetConfig<bool>(Constants.USE_UNPEG_WIP_OPTION);

            return val;
        }

        [FeatureAttachTo("SimInit/OnGetReleaseTimeOption", Root = "Simulation", Bind = true)]
        public string GET_RELEASE_TIME_OPTION_DEF(ref bool handled, string prevReturnValue)
        {
            string val = ConfigHelper.GetConfig<string>(Constants.RELEASE_TIME_OPTION);

            if (string.IsNullOrWhiteSpace(val))
                return prevReturnValue;

            return val;
        }

        [FeatureAttachTo("SimInit/OnGetKitOption", Root = "Simulation", Bind = true)]
        public string GET_KIT_OPTION_DEF(ref bool handled, string prevReturnValue)
        {
            string val = ConfigHelper.GetConfig<string>(Constants.KIT_OPTION);

            if (string.IsNullOrWhiteSpace(val))
                return prevReturnValue;

            return val;
        }

        [FeatureAttachTo("SimInit/OnGetReleaseTimeBuffer", Root = "Simulation", Bind = true)]
        public int GET_RELEASE_TIME_BUFFER_DEF(ref bool handled, int prevReturnValue)
        {
            int val = ConfigHelper.GetConfig<int>(Constants.RELEASE_TIME_BUFFER);

            return val;
        }

        [FeatureAttachTo("SimInit/OnGetSpecificReleaseTime", Root = "Simulation", Bind = true)]
        public List<int> GET_SPECIFIC_RELEASE_TIME_DEF(ref bool handled, List<int> prevReturnValue)
        {
            List<int> retValue = new List<int>();
            string val = ConfigHelper.GetConfig<string>(Constants.SPECIFIC_RELEASE_TIME);
            if (string.IsNullOrWhiteSpace(val))
                return null;
            else
            {
                foreach (var item in val.Split(','))
                {
                    if (int.TryParse(item.Trim(), out int num))
                    {
                        if (num > 24 || num < 0)
                            return null;
                        else if (num == 24)
                            num = 0;
                        retValue.Add(num);
                    }
                    else
                        return null;
                }
            }

            return retValue;
        }

        [FeatureAttachTo("SimInit/OnUsePMSchedule", Root = "Simulation", Bind = true)]
        public bool USE_PM_SCHEDULE_DEF(PMSchedule pm, SmartAPSEqp eqp, ref bool handled, bool prevReturnValue)
        {
            return true;
        }

        #endregion SimInit

        #region SimRun

        [FeatureAttachTo("SimRun/OnGetTactTime", Root = "Simulation", Bind = true)]
        public double GET_TACT_TIME_DEF(ref bool handled, double prevReturnValue)
        {
            double defTime = ConfigHelper.GetConfig<double>(Constants.DEFAULT_TACT_TIME);

            return defTime;
        }

        [FeatureAttachTo("SimRun/OnGetProcTime", Root = "Simulation", Bind = true)]
        public double GET_PROC_TIME_DEF(ref bool handled, double prevReturnValue)
        {
            double defTime = ConfigHelper.GetConfig<double>(Constants.DEFAULT_PROC_TIME);

            return defTime;
        }
        [FeatureAttachTo("SimRun/OnGetDefaultUtilization", Root = "Simulation", Bind = true)]
        public double GET_UTILIZATION_DEF(ref bool handled, double prevReturnValue)
        {
            double defUtil = ConfigHelper.GetConfig<double>(Constants.DEFAULT_UTILIZATION);
            if (defUtil == null || defUtil <= 0)
                defUtil = 100; 

            return defUtil;
        }
        [FeatureAttachTo("SimRun/OnGetDefaultEfficiency", Root = "Simulation", Bind = true)]
        public double GET_EFFICIENCY_DEF(ref bool handled, double prevReturnValue)
        {
            double defEff = ConfigHelper.GetConfig<double>(Constants.DEFAULT_EFFICIENCY);
            if (defEff == null || defEff <= 0)
                defEff = 100; 

            return defEff;
        }

        [FeatureAttachTo("SimRun/OnIsPartProdSplit", Root = "Simulation", Bind = true)]
        public bool IS_PART_PROD_SPLIT_DEF(ref bool handled, bool prevReturnValue)
        {
            bool val = ConfigHelper.GetConfig<bool>(Constants.PART_PROD_SPLIT_OPTION);

            return val;
        }

        [FeatureAttachTo("SimRun/OnIsAddLotsAssyConstraint", Root = "Simulation", Bind = true)]
        public bool IS_ADD_LOTS_ASSY_CONSTRAINT_DEF(ref bool handled, bool prevReturnValue)
        {
            bool val = ConfigHelper.GetConfig<bool>(Constants.ADD_LOTS_ASSY_CONSTRAINT);

            return val;
        }

        [FeatureAttachTo("SimRun/OnGetNotFoundDestOption", Root = "Simulation", Bind = true)]
        public string GET_NOT_FOUNT_DEST_OPTION_DEF(DispatchingAgent da, IHandlingBatch hb, ref bool handled, string prevReturnValue)
        {
            string val = ConfigHelper.GetConfig<string>(Constants.NOT_FOUND_DEST_OPTION);

            if (string.IsNullOrWhiteSpace(val))
            {
                return Constants.DUMMY;
            }

            return val;
        }

        [FeatureAttachTo("SimRun/OnGetSetupOption", Root = "Simulation", Bind = true)]
        public string GET_SETUP_OPTION_DEF(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, string prevReturnValue)
        {
            string val = ConfigHelper.GetConfig<string>(Constants.SETUP_TIME_OPTION);

            if (string.IsNullOrWhiteSpace(val))
            {
                return Constants.NONE;
            }

            return val;
        }

        [FeatureAttachTo("SimRun/OnGetEqpSetupTime", Root = "Simulation", Bind = true)]
        public Time GET_EQP_SETUP_TIME_DEF(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, Time prevReturnValue)
        {
            SmartAPSLot lot = hb.Sample as SmartAPSLot;
            SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;
            SmartAPSPlanInfo info = aeqp.LastPlan as SmartAPSPlanInfo;
            
            double defTime = ConfigHelper.GetConfig<double>(Constants.DEFAULT_SETUP_TIME);

            Time retValue = new Time(0);
            if (eqp.SetupInfo.Count() > 0)
            {
                if (eqp.SetupInfo.Count() == 1) {
                    SETUP_INFO setup = eqp.SetupInfo.FirstOrDefault();
                    if (SimHelper.IsItemChanged(lot, info, setup.ITEM, setup.ITEM_DETAIL))
                        retValue = TimeHelper.CalcTime(setup.SETUP_TIME, setup.TIME_UNIT);
                }
                else
                {
                    int beforePriority = eqp.SetupInfo.Min(x => x.PRIORITY);
                    foreach (var setup in eqp.SetupInfo.OrderBy(x => x.PRIORITY))
                    {
                        if (retValue.TotalSeconds != 0)
                        {
                            if (beforePriority >= setup.PRIORITY)
                            {
                                if (SimHelper.IsItemChanged(lot, info, setup.ITEM, setup.ITEM_DETAIL))
                                    retValue += TimeHelper.CalcTime(setup.SETUP_TIME, setup.TIME_UNIT);
                            }
                            else
                                break;
                        }
                        else
                        {
                            if (SimHelper.IsItemChanged(lot, info, setup.ITEM, setup.ITEM_DETAIL))
                                retValue = TimeHelper.CalcTime(setup.SETUP_TIME, setup.TIME_UNIT);
                        }

                        beforePriority = setup.PRIORITY;
                    }
                }
            }
            else
                return Time.FromMinutes(defTime);
            
            return retValue;
        }

        [FeatureAttachTo("SimRun/OnGetCustomSetupTime", Root = "Simulation", Bind = false)]
        public Time GET_CUSTOM_SETUP_TIME_DEF(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, Time prevReturnValue)
        {
            double defTime = ConfigHelper.GetConfig<double>(Constants.DEFAULT_SETUP_TIME);

            return Time.FromMinutes(defTime);
        }

        [FeatureAttachTo("SimRun/OnIsNeedSetup", Root = "Simulation", Bind = true)]
        public bool IS_NEED_SETUP_DEF(AoEquipment aeqp, IHandlingBatch hb, ref bool handled, bool prevReturnValue)
        {
            if (aeqp.LastPlan == null)
                return false;
            
            SmartAPSEqp eqp = aeqp.Target as SmartAPSEqp;
            if (eqp.SetupInfo.Count() > 0) {
                SmartAPSLot lot = hb.Sample as SmartAPSLot;
                SmartAPSPlanInfo info = aeqp.LastPlan as SmartAPSPlanInfo;

                foreach (SETUP_INFO setup in eqp.SetupInfo)
                {
                    if (SimHelper.IsItemChanged(lot, info, setup.ITEM, setup.ITEM_DETAIL))
                        return true;
                }
            }
            return false;
        }


        [FeatureAttachTo("SimRun/OnReplenishEvent", Root = "Simulation", Bind = true)]
        public void REPLENISH_EVENT_DEF(AoFactory factory, IMatPlan mat, ref bool handled)
        {
            var mfs = MaterialManager.Instance.FindFilter(mat);

            if(mfs != null && mfs.Count() > 0)
            {
                foreach(var mf in mfs)
                {
                    mf.Aeqp.WakeUp();
                }
            }
        }

        #endregion SimRun

    }
}
