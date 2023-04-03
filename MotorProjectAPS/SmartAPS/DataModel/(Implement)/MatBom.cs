using Mozart.SeePlan.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAPS.DataModel
{
    [Mozart.Task.Execution.FEBaseClassAttribute(Root = "SAPS", Category = "SAPS", IsTypeBinding = true, DiscardBase = true, Mandatory = false, Description = null)]
    public class MatBom : IMatBom
    {
        public string MaterialType { get; set; }

        public SmartAPSProduct Product { get; set; }

        public SmartAPSStep Step { get; set; }

        public double CompQty { get; set; }

        private List<IMatPlan> _matPlans = new List<IMatPlan>();
        public List<IMatPlan> MatPlans
        {
            get
            {
                return _matPlans;
            }
        }
    }
}
