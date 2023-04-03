using Mozart.SeePlan.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAPS.DataModel
{
    public class MatFilter
    {
        public IMatBom Bom { get; set; }
        public IHandlingBatch Hb { get; set; }
        public AoEquipment Aeqp { get; set; }
    }
}
