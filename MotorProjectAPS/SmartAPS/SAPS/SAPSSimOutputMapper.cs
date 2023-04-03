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
    [MyComponent("SimOutputMapper", Root = "Simulation", Order = 2)]
    public class SimOutputMapper : IModelController
    {
        public static SimOutputMapper Instance
        {
            get { return ServiceLocator.Resolve<SimOutputMapper>(); }
        }

        #region IModelController 멤버
        Type IModelController.ControllerType
        {
            get { return typeof(SimOutputMapper); }
        }
        #endregion
    }
}
