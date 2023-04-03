using Mozart.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace SmartAPS.UserLibrary.Helper
{
    public class CommonHelper
    {
        public static bool IsDigit( object oValue )
        {
            if ( IsNullOrDBNullValue( oValue ) ) return false;

            string sValue = oValue.ToString().Trim();
            Regex r = new Regex( @"^\d+$" );
            Match m = r.Match( sValue );
            return m.Success;
        }
        public static bool IsNullOrDBNullValue( object oValue )
        {
            return ( oValue == null || oValue == DBNull.Value );
        }

        public static void AddSort<T>(List<T> list, T item, Comparison<T> compare)
        {
            var index = list.BinarySearch(item, compare);
            if (index < 0)
                index = ~index;

            list.Insert(index, item);
        }
    }    
}
