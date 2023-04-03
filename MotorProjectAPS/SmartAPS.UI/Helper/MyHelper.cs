using System;
using System.Globalization;

namespace SmartAPS.UI.Helper
{
    public partial class MyHelper
    {
        public class COMMON : SmartAPS.UserLibrary.Helper.CommonHelper
        {
        }

        public class DATE : SmartAPS.UserLibrary.Helper.DateHelper
        {
            public static DateTime GetFirstDayOfWeek(int year, int weekNumber)
            {
                System.Globalization.Calendar calendar = CultureInfo.CurrentCulture.Calendar;
                DateTime firstOfYear = new DateTime(year, 1, 1, calendar);
                DateTime targetDay = calendar.AddWeeks(firstOfYear, weekNumber - 1);
                DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

                while (targetDay.DayOfWeek != firstDayOfWeek)
                {
                    targetDay = targetDay.AddDays(-1);
                }

                return targetDay;
            }
        }

        public class STRING : SmartAPS.UserLibrary.Helper.StringHelper
        {
        }

        public class ENUM : SmartAPS.UserLibrary.Helper.EnumHelper
        {
        }

        public class SHOPCALENDAR : SmartAPS.UserLibrary.Utils.ShopCalendarLoader
        {
        }

        public class CONVERT : SmartAPS.UserLibrary.Helper.ConvertHelper
        {
        }


        public class PIVOTGRID : SmartAPS.UserLibrary.Helper.PivotGridHelper
        {
        }

        public class PIVOTEXPORT : Mozart.Studio.TaskModel.Utility.PivotGridExporter
        {
        }

        public class GRIDVIEW : SmartAPS.UserLibrary.Helper.GridViewHelper
        {
        }

        public class GRIDVIEWEXPORT : Mozart.Studio.TaskModel.Utility.GridExporter
        {
        }

        public class SHIFTTIME : SmartAPS.UserLibrary.Helper.ShiftTimeHelper
        {
        }

        public class DATASVC : SmartAPS.UI.Helper.DataService
        {
        }

        public class ENGCONTROL : SmartAPS.UI.Helper.EngControlHelper
        {
        }

        public class FILTER : SmartAPS.UI.Helper.ConditionHelper
        {
        }
    }
}
