using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartAPS.UserLibrary.Helper
{
    public class EnumHelper
	{
        public static string ToEnumName(object name)
        {
            return Enum.GetName(name.GetType(), name);
        }

        public static List<string> ToEnumNames<T>()
        {
            return Enum.GetNames(typeof(T)).ToList();
        }

        public static T ToEnum<T>(string name)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), name);
            }
            catch
            {
                return default(T);
            }
        }

        public static T ToEnum<T>(string name, T defaultValue) where T : struct, IConvertible
        {
            try
            {
                T result;

                if (Enum.TryParse<T>(name, true, out result) == false)
                    return defaultValue;

                return result;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
