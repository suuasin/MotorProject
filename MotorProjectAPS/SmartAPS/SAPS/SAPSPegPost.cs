using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;
using SmartAPS.DataModel;

namespace SmartAPS
{
    [MyComponent("PegPost", Root = "Pegging", Order = 3)]
    public class PegPost : IModelController
    {
        public static PegPost Instance
        {
            get { return ServiceLocator.Resolve<PegPost>(); }
        }

        #region IModelController 멤버
        Type IModelController.ControllerType
        {
            get { return typeof(PegPost); }
        }
        #endregion

        //[FEAction]
        //public virtual int OnGetInputBatchSize(SmartAPSProduct prod)
        //{
        //    var handled = false;
        //    return PeggingPredefines.Instance.GET_INPUT_BATCH_SIZE_DEF(prod, ref handled, 0);
        //}

        [FEAction]
        public virtual int OnGetLotSize(SmartAPSProduct prod)
        {
            var handled = false;
            return PeggingPredefines.Instance.GET_LOT_SIZE_DEF(prod, ref handled, 0);
        }
    }
}
