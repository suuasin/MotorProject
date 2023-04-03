using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAPS.DataModel
{
    public interface IMatBom
    {
        string MaterialType { get; set; }

        SmartAPSProduct Product { get; set; }

        SmartAPSStep Step { get; set; }

        double CompQty { get; set; }

        List<IMatPlan> MatPlans { get; }
    }
}
