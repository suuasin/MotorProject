using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAPS.DataModel
{
    [Mozart.Task.Execution.FEBaseClassAttribute(Root = "SAPS", Category = "SAPS", IsTypeBinding = true, DiscardBase = true, Mandatory = false, Description = null)]
    public class MatPlan : IMatPlan
    {
        public string MaterialID { get; set; }

        public string MaterialType { get; set; }

        public double Qty { get; set; }

        public bool IsInfinite { get; set; }

        public DateTime ReplenishDate { get; set; }

        public bool IsEmpty
        {
            get
            {
                return Qty <= 0;
            }
        }

        public MatType MatType { get; set; }

        #region Method

        public MatPlan ShallowCopy()
        {
            var x = (MatPlan)this.MemberwiseClone();
            return x;
        }

        #endregion //Methods


        #region ToString

        public override string ToString()
        {
            return $"{this.MaterialID}/{this.MaterialType}";
        }

        #endregion //ToString
    }
}
