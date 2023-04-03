using Mozart.SeePlan.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAPS.DataModel
{
    public interface IReplenishEvents
    {
        void OnEvent(AoFactory factory, IEnumerable<IMatPlan> mats);
    }
}
