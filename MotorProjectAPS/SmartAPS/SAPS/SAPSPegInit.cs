using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;
using SmartAPS.DataModel;

namespace SmartAPS
{[MyComponent("PegInit", Root = "Pegging", Order = 1)]
    public class PegInit : IModelController
    {
        public static PegInit Instance
        {
            get { return ServiceLocator.Resolve<PegInit>(); }
        }

        #region IModelController 멤버
        Type IModelController.ControllerType
        {
            get { return typeof(PegInit); }
        }
        #endregion

        [FEAction]
        public virtual void OnCreatePegPart(SmartAPSMoMaster mm, SmartAPSPegPart pp)
        {
        }

        [FEAction]
        public virtual void OnCreatePegTarget(SmartAPSPegPart pp, SmartAPSMoPlan mo, SmartAPSPegTarget pt)
        {
        }

        [FEAction]
        public virtual void OnCreatePlanWip(SmartAPSWipInfo info, SmartAPSPlanWip wip)
        {
        }
    }
}
