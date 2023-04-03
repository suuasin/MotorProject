using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Outputs;
using Mozart.SeePlan.Pegging;

namespace SmartAPS
{
    [MyComponent("PegOutputMapper", Root = "Pegging", Order = 2)]
    public class PegOutputMapper : IModelController
    {
        public static PegOutputMapper Instance
        {
            get { return ServiceLocator.Resolve<PegOutputMapper>(); }
        }

        #region IModelController 멤버
        Type IModelController.ControllerType
        {
            get { return typeof(PegOutputMapper); }
        }
        #endregion

        [FEAction]
        public virtual STEP_TARGET OnWriteStepTarget(STEP_TARGET entity, SmartAPSPegTarget pt)
        {
            return entity;
        }
        [FEAction]
        public virtual PEG_HISTORY OnWritePegHistory(PEG_HISTORY entity, PegTarget target, SmartAPSPlanWip peggedwip)
        {
            return entity;
        }
        [FEAction]
        public virtual UNPEG_HISTORY OnWriteUnPegHistory(UNPEG_HISTORY entity, SmartAPSPlanWip wip)
        {
            return entity;
        }

    }
}