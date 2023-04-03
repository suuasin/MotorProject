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
using Mozart.Utility;
namespace SmartAPS
{
    [FeatureBind()]
    public static partial class ConfigHelper
    {
        public static object GetConfig(string key)
        {
            var opt = InputMart.Instance.EXECUTION_OPTION_CONFIGView.FindRows(key).FirstOrDefault();

            if (opt == null)
                return null;

            return opt.OPTION_VALUE;
        }

        public static T GetConfig<T>(string key)
        {
            var opt = InputMart.Instance.EXECUTION_OPTION_CONFIGView.FindRows(key).FirstOrDefault();

            if (opt == null)
                return default(T);

            object obj = opt.OPTION_VALUE;

            if (typeof(T) == typeof(string) ||
                    typeof(T) == typeof(String))
                return (T)obj;

            return ConvertUtility.ChangeType<T>(obj);
        }

        public static double DecimalFormatHalper(double qty, int decimalPoint)
        {
            double convertQty = 0;
            switch (decimalPoint)
            {
                case 1:
                    convertQty = Math.Ceiling(qty);
                    break;
                case -1:
                    convertQty = Math.Truncate(qty);
                    break;
                case 2:
                    convertQty = Math.Round(qty);
                    break;
                default:
                    convertQty = qty;
                    break;
            }
                
            return convertQty;
        }
    }
}
