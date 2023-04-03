using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAPS.DataModel
{
    public interface IMatPlan
    {
        string MaterialID { get; set; }

        string MaterialType { get; set; }

        double Qty { get; set; }

        bool IsInfinite { get; set; }

        DateTime ReplenishDate { get; set; }

        bool IsEmpty { get; }

        MatType MatType { get; set; }
    }

    public enum MatType
    {
        Inv,
        Plan
    }
}
