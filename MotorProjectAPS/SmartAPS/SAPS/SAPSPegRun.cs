using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;

namespace SmartAPS
{
    [MyComponent("PegRun", Root = "Pegging", Order = 1)]
    public class PegRun : IModelController
    {
        public static PegRun Instance
        {
            get { return ServiceLocator.Resolve<PegRun>(); }
        }

        #region IModelController 멤버
        Type IModelController.ControllerType
        {
            get { return typeof(PegRun); }
        }
        #endregion

        [FEAction]
        public virtual bool OnGetInPlanOption()
        {
            var handled = false;
            return PeggingPredefines.Instance.GET_USE_IN_PLAN_OPTION_DEF(ref handled, false);
        }


    }
}
