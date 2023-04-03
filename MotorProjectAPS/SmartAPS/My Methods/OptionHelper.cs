using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Mozart.Common;
using Mozart.Collections;
using Mozart.Extensions;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using SmartAPS.Inputs;
using SmartAPS.Outputs;
using SmartAPS.Persists;
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class OptionHelper
    {
        public static bool UseHardPegging()
        {
            bool val = ConfigHelper.GetConfig<bool>(Constants.USE_HARD_PEGGING_OPTION);
            
            return val;
        }
    }
}
