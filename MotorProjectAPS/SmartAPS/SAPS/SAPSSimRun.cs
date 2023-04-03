using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.Simulation.Engine;

namespace SmartAPS
{
    [MyComponent("SimRun", Root = "Simulation", Order = 1)]
    public class SimRun : IModelController
    {
        public static SimRun Instance
        {
            get { return ServiceLocator.Resolve<SimRun>(); }
        }

        #region IModelController 멤버
        Type IModelController.ControllerType
        {
            get { return typeof(SimRun); }
        }
        #endregion

        [FEAction]
        public virtual double OnGetTactTime()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_TACT_TIME_DEF(ref handled, 0);
        }

        [FEAction]
        public virtual double OnGetProcTime()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_PROC_TIME_DEF(ref handled, 0);
        }

        [FEAction]
        public virtual double OnGetDefaultUtilization()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_UTILIZATION_DEF(ref handled, 0);
        }

        [FEAction]
        public virtual double OnGetDefaultEfficiency()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_EFFICIENCY_DEF(ref handled, 0);
        }

        [FEAction]
        public virtual bool OnIsPartProdSplit()
        {
            var handled = false;
            return SimulationPredefines.Instance.IS_PART_PROD_SPLIT_DEF(ref handled, false);
        }

        [FEAction]
        public virtual bool OnIsAddLotsAssyConstraint()
        {
            var handled = false;
            return SimulationPredefines.Instance.IS_ADD_LOTS_ASSY_CONSTRAINT_DEF(ref handled, false);
        }

        [FEAction]
        public virtual string OnGetNotFoundDestOption(DispatchingAgent da, IHandlingBatch hb)
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_NOT_FOUNT_DEST_OPTION_DEF(da, hb, ref handled, Constants.DUMMY);
        }

        [FEAction]
        public virtual string OnGetSetupOption(AoEquipment aeqp, IHandlingBatch hb)
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_SETUP_OPTION_DEF(aeqp, hb, ref handled, Constants.NONE);
        }

        [FEAction]
        public virtual Mozart.Simulation.Engine.Time OnGetEqpSetupTime(AoEquipment aeqp, IHandlingBatch hb)
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_EQP_SETUP_TIME_DEF(aeqp, hb, ref handled, Time.Zero);
        }

        [FEAction]
        public virtual Time OnGetCustomSetupTime(AoEquipment aeqp, IHandlingBatch hb)
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_CUSTOM_SETUP_TIME_DEF(aeqp, hb, ref handled, Time.Zero);
        }

        [FEAction]
        public virtual bool OnIsNeedSetup(AoEquipment aeqp, IHandlingBatch hb)
        {
            var handled = false;
            return SimulationPredefines.Instance.IS_NEED_SETUP_DEF(aeqp, hb, ref handled, false);
        }

        [FEAction]
        public virtual void OnReplenishEvent(AoFactory facotry, IMatPlan mat)
        {
            var handled = false;
            SimulationPredefines.Instance.REPLENISH_EVENT_DEF(facotry, mat, ref handled);
        }
    }
}
