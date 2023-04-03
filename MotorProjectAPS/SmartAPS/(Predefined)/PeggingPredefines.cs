using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using Mozart.SeePlan.DataModel;

namespace SmartAPS
{
    [FeatureBind()]
    [MyDefault]
    public class PeggingPredefines
    {
        internal static PeggingPredefines Instance
        {
            get { return ServiceLocator.Resolve<PeggingPredefines>(); }
        }

        [FeatureAttachTo("PegPost/OnGetInputBatchSize", Root = "Pegging", Bind = true)]
        public int GET_INPUT_BATCH_SIZE_DEF(SmartAPSProduct prod, ref bool handled, int prevReturnValue)
        {
            return prod.InputBatchSize > 0 ? prod.InputBatchSize : ConfigHelper.GetConfig<int>(Constants.DEFAULT_INPUT_BATCH_SIZE);
        }

        [FeatureAttachTo("PegPost/OnGetLotSize", Root = "Pegging", Bind = true)]
        public int GET_LOT_SIZE_DEF(SmartAPSProduct prod, ref bool handled, int prevReturnValue)
        {
            if (prod.LotSize == 0)
            {
                int defSize = ConfigHelper.GetConfig<int>(Constants.DEFAULT_LOT_SIZE);

                return defSize > 0 ? defSize : SeeplanConfiguration.Instance.LotUnitSize;
            }
            else
            {
                return prod.LotSize;
            }
        }

        #region pegrun
        [FeatureAttachTo("PegRun/OnGetInPlanOption", Root = "Pegging", Bind = true)]
        public bool GET_USE_IN_PLAN_OPTION_DEF(ref bool handled, bool prevReturnValue)
        {
            return ConfigHelper.GetConfig<bool>(Constants.USE_IN_PLAN_OPTION);
        }
        #endregion
    }
}