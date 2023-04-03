using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.SeePlan.DataModel;

namespace SmartAPS
{
    [MyComponent("SimInit", Root = "Simulation", Order = 1)]
    public class SimInit : IModelController
    {
        public static SimInit Instance
        {
            get { return ServiceLocator.Resolve<SimInit>(); }
        }

        #region IModelController 멤버
        Type IModelController.ControllerType
        {
            get { return typeof(SimInit); }
        }
        #endregion

        [FEAction]
        public virtual SmartAPSLot OnCreateLot(SmartAPSWipInfo info, bool isWip)
        {
            var handled = false;
            return SimulationPredefines.Instance.CREATE_LOT_DEF(info, isWip, ref handled, null);
        }

        [FEAction]
        public virtual int OnGetLotSize(SmartAPSProduct prod)
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_LOT_SIZE_DEF(prod, ref handled, 0);
        }

        [FEAction]
        public virtual double OnGetReleaseTerm(int lotSize)
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_RELEASE_TERM_DEF(lotSize, ref handled, 0);
        }

        [FEAction]
        public virtual DateTime OnGetStateTime(AoEquipment aeqp, IHandlingBatch hb)
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_STATE_TIME_DEF(aeqp, hb, ref handled, aeqp.NowDT);
        }

        [FEAction]
        public virtual bool OnGetBlockInTargetLots() {
            var handled = false;
            return SimulationPredefines.Instance.GET_BLOCK_IN_TARGET_LOTS(ref handled, true);
        }
        [FEAction]
        public virtual string OnGetReleaseLotOption()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_RELEASE_LOT_OPTION_DEF(ref handled, Constants.AT_ENGINE_START);
        }
        [FEAction]
        public virtual bool OnGetUseUnpegWipOption()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_USE_UNPEG_WIP_OPTION_DEF(ref handled, false);
        }

        [FEAction]
        public virtual string OnGetKitOption()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_KIT_OPTION_DEF(ref handled, Constants.DEFAULT);
        }

        [FEAction]
        public virtual string OnGetReleaseTimeOption()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_RELEASE_TIME_OPTION_DEF(ref handled, Constants.NORMAL);
        }

        [FEAction]
        public virtual int OnGetReleaseTimeBuffer()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_RELEASE_TIME_BUFFER_DEF(ref handled, 0);
        }

        [FEAction]
        public virtual List<int> OnGetSpecificReleaseTime()
        {
            var handled = false;
            return SimulationPredefines.Instance.GET_SPECIFIC_RELEASE_TIME_DEF(ref handled, null);
        }

        [FEAction]
        public virtual bool OnUsePMSchedule(PMSchedule pm, SmartAPSEqp eqp)
        {
            var handled = false;
            return SimulationPredefines.Instance.USE_PM_SCHEDULE_DEF(pm, eqp, ref handled, true);
        }
    }
}
