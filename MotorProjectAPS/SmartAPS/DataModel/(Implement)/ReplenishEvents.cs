using Mozart.SeePlan.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAPS.DataModel
{
    [Mozart.Task.Execution.FEBaseClassAttribute(Root = "SAPS", Category = "SAPS", IsTypeBinding = true, DiscardBase = true, Mandatory = false, Description = null)]
    public class ReplenishEvents : IReplenishEvents
    {
        protected virtual void OnReplenishEvent(AoFactory factory, IMatPlan mat)
        {
            SimRun.Instance.OnReplenishEvent(factory, mat);
        }

        #region IReplenishEvents 멤버

        void IReplenishEvents.OnEvent(AoFactory factory, IEnumerable<IMatPlan> mats)
        {
            foreach (var mat in mats)
            {
                if (!(mat is MatPlan))
                    return;

                this.OnReplenishEvent(factory, mat);
            }
        }
        #endregion
    }
}
