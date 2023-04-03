using System;
using Mozart.SeePlan;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace SmartAPS.UserLibrary.Helper
{
    public class DateHelper
    {
        /// <summary>
        /// yyyyMMddHHmmss
        /// </summary>
        public static readonly string dbDateTimeFormatOld = "yyyyMMddHHmmss";
        /// <summary>
        /// yyyyMMdd HHmmss
        /// </summary>
        private static readonly string dbDateTimeFormatNew = "yyyyMMdd HHmmss";

        public static bool UseSpacedDbDateTimeFormat = true;

        /// <summary>
        /// yyyyMMdd
        /// </summary>
        public static string DbDateFormat = "yyyyMMdd";
        /// <summary>
        /// HHmmss
        /// </summary>
        public static string DbTimeFormat = "HHmmss";

        public static string DbDateTimeFormat
        {
            get { return UseSpacedDbDateTimeFormat ? dbDateTimeFormatNew : dbDateTimeFormatOld; }
        }

        public static String GetNowString()
        {
            return DateTime.Now.ToString(DbDateTimeFormat);
        }

        public static DateTime StringToDate(string value)
        {
            return StringToDateTime(value, false);
        }

        public static DateTime StringToDateTime(string value)
        {
            return StringToDateTime(value, true);
        }
        static public DateTime StringToDateTime(string value, bool withTime)
        {
            if (value == null)
                return DateTime.MinValue;

            value = StringHelper.Trim(value);
            int length = value.Length;

            if (length < 8)
                return DateTime.MinValue;

            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            try
            {
                year = int.Parse(value.Substring(0, 4));
                month = int.Parse(value.Substring(4, 2));
                day = int.Parse(value.Substring(6, 2));
                if (withTime)
                {
                    int t = 8;

                    if (length >= 10)
                    {
                        if (value[8] == ' ')
                            t++;

                        hour = int.Parse(value.Substring(t + 0, 2));
                    }

                    if (length >= 12)
                    {
                        minute = int.Parse(value.Substring(t + 2, 2));
                    }

                    if (length >= 14)
                    {
                        second = int.Parse(value.Substring(t + 4, 2));
                    }
                }
            }
            catch
            {

            }
            return new DateTime(year, month, day, hour, minute, second);
        }
        static public TimeSpan StringToTime(string value)
        {
            if (value == null)
                return TimeSpan.Zero;

            value = value.Trim();
            int length = value.Length;

            if (length < 4)
                return TimeSpan.Zero;


            int hour = 0;
            int minute = 0;
            int second = 0;
            try
            {
                hour = int.Parse(value.Substring(0, 2));
                minute = int.Parse(value.Substring(2, 2));

                if (length >= 6)
                {
                    second = int.Parse(value.Substring(4, 2));
                }
            }
            catch
            {
            }
            return new TimeSpan(hour, minute, second);
        }

        public static string DateToString(DateTime dateTime)
        {
            return DateTimeToString(dateTime, false);
        }

        public static string DateTimeToString(DateTime dateTime)
        {
            return DateTimeToString(dateTime, true);
        }
        public static string DateTimeToString(DateTime dateTime, bool withTime)
        {
            if (dateTime == DateTime.MinValue)
                return "0";
            if (withTime)
                return dateTime.ToString(DbDateTimeFormat);
            else return dateTime.ToString(DbDateFormat);
        }
        public static string DateTimeToStringTrimSec(DateTime dateTime)
        {
            int sec = dateTime.Second;
            if (sec > 0) dateTime = dateTime.AddSeconds(-sec);
            return DateTimeToString(dateTime, true);
        }
        public static DateTime DateTimeToStringTrimMilliSec(DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerSecond), dateTime.Kind);
        }

        public static string TimeToStringTrimSec(TimeSpan time)
        {
            int sec = time.Seconds;
            if (sec > 0) time = time.Add(TimeSpan.FromSeconds(-sec));
            return TimeToString(time);
        }
        public static string TimeToString(TimeSpan time)
        {
            if (time == TimeSpan.Zero)
                return "0";
            return String.Format("{0:00}{1:00}{2:00}", time.Hours, time.Minutes, time.Seconds);
        }

        public static DateTime StringToDTime(string value)
        {
            return (new DateTime(1900, 1, 1)) + StringToTime(value);
        }
        public static string DTimeToStringTrimSec(DateTime time)
        {
            int sec = time.Second;
            if (sec > 0) time = time.AddSeconds(-sec);
            return DTimeToString(time);
        }
        public static string DTimeToString(DateTime time)
        {
            return time.ToString(DbTimeFormat);
        }

        public static object DBNullable(DateTime dt)
        {
            return dt == DateTime.MinValue ? DBNull.Value : (object)dt;
        }

        public static string Format(string dbdt)
        {
            return Format(StringToDateTime(dbdt));
        }

        public static string Format(string dbdt, bool noTime)
        {
            return Format(StringToDate(dbdt), noTime);
        }

        public static string Format(DateTime dt)
        {
            return Format(dt, false);
        }

        public static string Format(DateTime dt, bool noTime)
        {
            if (noTime)
                return dt.ToString("yyyy-MM-dd");
            else
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string SplitDateToString(DateTime dt)
        {
            //TODO : 확인 필요
            return DateToString(ShopCalendar.SplitDate(dt));
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(Calendar cal, DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = cal.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return cal.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        // 확인요
        public static void GetWeekRange(string weekNo, out DateTime start, out DateTime end)
        {
            int year = int.Parse(weekNo.Substring(0, 4));
            int week = int.Parse(weekNo.Substring(4, 2));

            CultureInfo ci = CultureInfo.InvariantCulture;
            DateTime yearStart = new DateTime(year, 1, 1);

            start = DateHelper.StartDayOfWeek(yearStart);
            start = ci.Calendar.AddWeeks(start, week - 1);
            end = DateHelper.EndDayOfWeek(start);

            if (weekNo.Length > 6)
            {
                if (weekNo[6] == 'a')
                {
                    end = new DateTime(start.Year, start.Month,
                        ci.Calendar.GetDaysInMonth(start.Year, start.Month));
                }
                else if (weekNo[6] == 'b')
                {
                    start = new DateTime(end.Year, end.Month, 1);
                }
            }
        }

        public static string[] ParseModItemRoute(string routeID)
        {
            string[] routes = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };

            if (string.IsNullOrEmpty(routeID))
                return routes;

            string[] items = routeID.Split('_');

            if (items.Length >= 1)
                routes[0] = items[0].Trim();    //FA

            if (items.Length >= 2)
                routes[1] = items[1].Trim();    //LAMI

            if (items.Length >= 3)
                routes[2] = items[2].Trim();    //OLB

            if (items.Length >= 4)
                routes[3] = items[3].Trim();    //CP

            return routes;
        }

        public static string GetWeekNo(DateTime date)
        {
            return WeekNo(date).ToString();
        }

        public static DateTime GetStartDateWithMonthNo(string monthNo)
        {
            if (monthNo.Length != 6)
                return DateTime.MaxValue;

            int year = Convert.ToInt32(monthNo.Substring(0, 4));
            int month = Convert.ToInt32(monthNo.Substring(4, 2));

            DateTime dt = new DateTime(year, month, 01);
            return DateHelper.GetStartDayOfMonth(dt);
        }

        public static DateTime GetStartDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetEndDateWithMonthNo(string monthNo)
        {
            DateTime dt = GetStartDateWithMonthNo(monthNo);
            if (dt == DateTime.MaxValue)
                return dt;

            return dt.AddDays(GetDaysInMonth(monthNo) - 1);
        }

        public static int GetDaysInMonth(string monthNo)
        {
            if (monthNo.Length > 6)
                return 30;

            int year = Convert.ToInt32(monthNo.Substring(0, 4));
            int month = Convert.ToInt32(monthNo.Substring(4, 2));

            CultureInfo ci = CultureInfo.InvariantCulture;

            return ci.Calendar.GetDaysInMonth(year, month);
        }

        public static DateTime Trim(DateTime date, string format)
        {
            if (string.IsNullOrEmpty(format))
                return date;

            switch (format)
            {
                case "yyyy":
                    return new DateTime(date.Year, 0, 0);
                case "MM":
                    return new DateTime(date.Year, date.Month, 0);
                case "dd":
                    return new DateTime(date.Year, date.Month, date.Day);
                case "HH":
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
                case "mm":
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
                case "m0":
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, ((date.Minute / 10) * 10), 0);
                case "ss":
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                case "s0":
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, ((date.Second / 10) * 10));
            }
            return date;
        }

        public static DateTime GetRptDate_1Hour(DateTime t, int baseMinute)
        {
            //1시간 단위
            int baseHours = 1;
            
            //ex) HH:30:00
            DateTime rptDate = DateHelper.Trim(t, "HH").AddMinutes(baseMinute);

            //baseMinute(ex.30분) 이상인 경우 이후 시간대 baseMinute의 실적
            //07:30 = 06:30(초과) ~ 07:30(이하)인경우, 06:40 --> 07:30, 07:30 --> 07:30, 07:40 --> 08:30
            if (t.Minute > baseMinute)
            {
                rptDate = rptDate.AddHours(baseHours);
            }

            return rptDate;
        }

        #region [WEEK]
        internal static string GetYearMon(DateTime dt)
        {
            DateTimeFormatInfo en_US = new CultureInfo("en-US", false).DateTimeFormat;
            string year = dt.Year.ToString();
            return "" + year[year.Length - 1] + dt.ToString("MMM", en_US)[0];
        }

        //목요일 기준 주차
        public static int WeekNo(DateTime date)
        {
            DateTime baseDay = BaseDayOfWeek(date);

            return baseDay.Year * 100 + WeekOfYear(baseDay);

            //date = StartDayOfWeek(date);

            //CultureInfo ci = CultureInfo.InvariantCulture;

            //return date.Year * 100 + ci.Calendar.GetWeekOfYear(date,
            //    CalendarWeekRule.FirstFullWeek,
            //    DayOfWeek.Monday);

        }

        public static DateTime BaseDayOfWeek(DateTime date)
        {
            //목요일 기준으로 주차 결정
            return StartDayOfWeek(date).AddDays(3);
        }

        /// <summary>
        /// yyyyMM 형태의 년도+월을 반환합니다. 예)201601
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetMonthNo(DateTime dt)
        {
            return ShopCalendar.SplitDate(dt).ToString("yyyyMM");
        }

        public static string GetWeekPlanNo(DateTime planDate)
        {
            planDate = StartDayOfWeek(planDate);

            //목요일 기준으로 주차 생성
            DateTime wThursDay = planDate.AddDays(3);

            return wThursDay.ToString("yyyy") + WeekOfYear(wThursDay).ToString("0#");
        }

        public static string GetWeekPlanNo(DateTime date, bool isFull)
        {
            DateTime wStart = StartDayOfWeek(date);
            DateTime wEnd = wStart.AddDays(6);

            string sWeekNo = GetWeekPlanNo(wStart);

            //월경계 포함된 주차인 경우
            if (wStart.Month != wEnd.Month)
            {
                string str = date.Month == wStart.Month ? "a" : "b";
                int iDays = 0;

                if (str == "a")
                {
                    DateTime monthEndDay = EndDayOfMonth(wStart);
                    iDays = (monthEndDay.Date - wStart.Date).Days + 1;
                }
                else if (str == "b")
                {
                    iDays = wEnd.Day;
                }

                sWeekNo = isFull ? string.Format("{0}{1}({2})", sWeekNo, str, iDays) : string.Format("{0}{1}", sWeekNo, str);
            }

            return sWeekNo;
        }

        public static string GetMonthBoundaryWeekPlanNo(DateTime planDate, string sWeekPlanNo)
        {
            sWeekPlanNo = sWeekPlanNo.Substring(0, 6);
            DateTime wStart = StartDayOfWeek(planDate);
            DateTime wEnd = wStart.AddDays(6);

            string sWeekNo = sWeekPlanNo;

            //월경계 포함된 주차인 경우
            if (wStart.Month != wEnd.Month)
            {
                string str = planDate.Month == wStart.Month ? "a" : "b";
                int iDays = 0;

                if (str == "a")
                {
                    DateTime monthEndDay = EndDayOfMonth(wStart);
                    iDays = (monthEndDay.Date - wStart.Date).Days + 1;
                }
                else if (str == "b")
                {
                    iDays = wEnd.Day;
                }

                sWeekNo = string.Format("{0}{1}({2})", sWeekPlanNo, str, iDays);
            }

            return sWeekNo;
        }

        public static string GetMonthBoundaryWeekPlanNo(string sPlanDate, string sWeekPlanNo)
        {
            sWeekPlanNo = sWeekPlanNo.Substring(0, 6);
            DateTime planDate = DateHelper.StringToDate(sPlanDate);
            DateTime wStart = StartDayOfWeek(planDate);
            DateTime wEnd = wStart.AddDays(6);

            string sWeekNo = sWeekPlanNo;

            //월경계 포함된 주차인 경우
            if (wStart.Month != wEnd.Month)
            {
                string str = planDate.Month == wStart.Month ? "a" : "b";
                int iDays = 0;

                if (str == "a")
                {
                    DateTime monthEndDay = EndDayOfMonth(wStart);
                    iDays = (monthEndDay.Date - wStart.Date).Days + 1;
                }
                else if (str == "b")
                {
                    iDays = wEnd.Day;
                }

                sWeekNo = string.Format("{0}{1}({2})", sWeekPlanNo, str, iDays);
            }

            return sWeekNo;
        }

        //시작주 이거나 마지막 주 일때 주 시작일이 속하는 월 체크
        public static string GetMonthContainCheck(DateTime dWeekDate)
        {
            DateTime wStart = StartDayOfWeek(dWeekDate);
            DateTime wEnd = wStart.AddDays(6);
            string str = null;

            //월경계 구분
            if (wStart.Month != wEnd.Month)
                str = dWeekDate.Month == wStart.Month ? "a" : "b";

            return str;
        }

        public static DateTime FirstWeekOfYear(int year)
        {
            DateTime d1 = new DateTime(year, 1, 1);

            DateTime dt = StartDayOfWeek(d1);

            int weekOfYear = WeekOfYear(d1);
            if (weekOfYear != 1) dt = dt.AddDays(7);

            return dt;
        }

        public static int WeekOfYear(DateTime date)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;

            return ci.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime StartDayOfWeek(string week)
        {
            int year = Convert.ToInt32(week.Substring(0, 4));
            int w = Convert.ToInt32(week.Substring(4, 2)) - 1;

            DateTime sdt = new DateTime(year, 1, 1);
            DateTime sow = StartDayOfWeek(sdt);

            int rw = WeekNo(sdt);
            if (rw / 100 < year)
                w++;

            return sow.AddDays(w * 7);
        }

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

        public static DateTime StartDayOfWeek(int week)
        {
            string weekStr = week.ToString();

            return StartDayOfWeek(weekStr);
            //int year = Convert.ToInt32(weekStr.Substring(0, 4));
            //int w = Convert.ToInt32(weekStr.Substring(4, 2)) - 1;

            //DateTime sdt = new DateTime(year, 1, 1);
            //DateTime sow = StartDayOfWeek(sdt);

            //int rw = WeekNo(sdt);
            //if (rw / 100 < year)
            //    w++;

            //return sow.AddDays(w * 7);
        }

        public static DateTime StartDayOfWeek(DateTime date)
        {
            int dayOfWeek = (int)date.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;
            return date.AddDays((int)DayOfWeek.Monday - dayOfWeek);
        }

        public static DateTime StartDayOfWeek(DateTime date, int days, bool isConsiderMonth)
        {
            DateTime weekStartDate = StartDayOfWeek(date.AddDays(days));
            DateTime monthStartDate = date.AddDays(date.Day - 1);

            if (isConsiderMonth)
                return (weekStartDate <= monthStartDate) ? weekStartDate : monthStartDate;

            return weekStartDate;
        }

        public static DateTime EndDayOfWeek(DateTime date)
        {
            int dayOfWeek = (int)date.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;

            return date.AddDays(7 - dayOfWeek);
        }

        public static DateTime EndDayOfWeek(DateTime date, int days, bool isConsiderMonth)
        {
            DateTime weekEndDate = EndDayOfWeek(date.AddDays(days));
            DateTime monthEndDate = date.AddDays(DateTime.DaysInMonth(date.Year, date.Month) - date.Day);

            if (isConsiderMonth)
                return (weekEndDate <= monthEndDate) ? weekEndDate : monthEndDate;

            return weekEndDate;
        }

        public static DateTime StartTimeOfNextWeek(DateTime date)
        {
            return ShopCalendar.StartTimeOfNextDayT(EndDayOfWeek(date));
        }

        public static DateTime StartTimeOfNextWeek(DateTime date, int days, bool isConsiderMonth)
        {
            return ShopCalendar.StartTimeOfNextDayT(EndDayOfWeek(date, days, isConsiderMonth));
        }

        public static DateTime AddWeeks(DateTime date, int weeks)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;

            return ci.Calendar.AddWeeks(date, weeks);

        }

        public static DateTime StartDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndDayOfMonth(DateTime date)
        {
            date = date.AddMonths(1);
            date = new DateTime(date.Year, date.Month, 1);

            return date.AddDays(-1);
        }

        public static DateTime EndDayOfMonth(string monthNo)
        {
            if (monthNo.Length < 6)
                return EndDayOfMonth(DateTime.MinValue);

            int year = Convert.ToInt32(monthNo.Substring(0, 4));
            int month = Convert.ToInt32(monthNo.Substring(4, 2));

            DateTime date = new DateTime(year, month, 1);

            return EndDayOfMonth(date);
        }

        public static DateTime WeekNoToStartDayOfWeek(string sWeekNo)
        {
            string sYear = sWeekNo.Substring(0, 4);
            string sWeek = sWeekNo.Substring(4, 2);

            DateTime firstWeekDay = FirstWeekOfYear(Convert.ToInt32(sYear));
            DateTime wStartDate = AddWeeks(firstWeekDay, Convert.ToInt32(sWeek) - 1);

            //주차중 b인 경우는 월경계(차월) 
            if (sWeekNo.IndexOf('b') != -1)
            {
                wStartDate = StartDayOfMonth(wStartDate.AddDays(6));
            }

            return wStartDate;
        }

        internal static DateTime WeekNoToStartDayOfWeek(int iWeekNo)
        {
            string sWeekNo = iWeekNo.ToString();

            string sYear = sWeekNo.Substring(0, 4);
            string sWeek = sWeekNo.Substring(4, 2);

            DateTime firstWeekDay = FirstWeekOfYear(Convert.ToInt32(sYear));
            DateTime wStartDate = AddWeeks(firstWeekDay, Convert.ToInt32(sWeek) - 1);

            //주차중 b인 경우는 월경계(차월) 
            if (sWeekNo.IndexOf('b') != -1)
            {
                wStartDate = StartDayOfMonth(wStartDate.AddDays(6));
            }

            return wStartDate;
        }

        public static DateTime WeekNoToEndDayOfWeek(string sWeekNo)
        {
            string sYear = sWeekNo.Substring(0, 4);
            string sWeek = sWeekNo.Substring(4, 2);

            DateTime firstWeekDay = FirstWeekOfYear(Convert.ToInt32(sYear));
            DateTime wEndDate = AddWeeks(firstWeekDay, Convert.ToInt32(sWeek)).AddDays(-1);

            //주차중 a인 경우는 월경계(월말) 
            if (sWeekNo.IndexOf('a') != -1)
            {
                wEndDate = EndDayOfMonth(wEndDate.AddDays(-6));
            }

            return wEndDate;
        }

        public static DateTime WeekNoToEndDayOfWeek(int iWeekNo)
        {
            string sWeekNo = iWeekNo.ToString();
            string sYear = sWeekNo.Substring(0, 4);
            string sWeek = sWeekNo.Substring(4, 2);

            DateTime firstWeekDay = FirstWeekOfYear(Convert.ToInt32(sYear));
            DateTime wEndDate = AddWeeks(firstWeekDay, Convert.ToInt32(sWeek)).AddDays(-1);

            //주차중 a인 경우는 월경계(월말) 
            if (sWeekNo.IndexOf('a') != -1)
            {
                wEndDate = EndDayOfMonth(wEndDate.AddDays(-6));
            }

            return wEndDate;
        }

        public static DayOfWeek DiffWeek(DayOfWeek dow, int d)
        {
            int i = (int)(dow - d);

            return i < 0 ? (DayOfWeek)(7 + i) : (DayOfWeek)i;
        }

        /// <summary>
        /// 주차간의 간격을 구합니다
        /// </summary>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns>예외상황에서 int.MaxValue를 리턴하므로, 이런 경우 주차 관련 연산을 수행하면 예외가 발생할 수 있음</returns>
        public static int DiffWeek(string w1, string w2)
        {
            try
            {
                DateTime d1 = StartDayOfWeek(w1);
                DateTime d2 = StartDayOfWeek(w2);

                TimeSpan diffDates = d1 >= d2 ? d1 - d2 : d2 - d1;
                return (int)diffDates.TotalDays / 7;
            }
            catch
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// 주차간의 간격을 구합니다
        /// </summary>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns>예외상황에서 int.MaxValue를 리턴하므로, 이런 경우 주차 관련 연산을 수행하면 예외가 발생할 수 있음</returns>
        public static int DiffWeek(int w1, int w2)
        {
            return DiffWeek(w1.ToString(), w2.ToString());

            //try
            //{
            //    DateTime d1 = StartDayOfWeek(w1);
            //    DateTime d2 = StartDayOfWeek(w2);

            //    TimeSpan diffDates = d1 - d2;
            //    return (int)diffDates.TotalDays / 7;
            //}
            //catch
            //{
            //    return int.MaxValue;
            //}

            //int weeks = int.MaxValue;
            //TimeSpan diffDates = StartDayOfWeek(w1) - StartDayOfWeek(w2);
            //weeks = (int)diffDates.TotalDays / 7;

            //return weeks;
        }

        #region 시작일 부터 종요일 까지 주차 반환

        public static ListDictionary GetWeekList(DateTime startDate, DateTime endDate, bool isSection, bool isFull)
        {
            ListDictionary list = new ListDictionary();

            for (DateTime date = startDate; date.Date <= endDate.Date; date = date.AddDays(1))
            {
                string sKey = isSection ? GetWeekPlanNo(date, false) : GetWeekPlanNo(date);
                string sValue = GetWeekValue(date, isSection, isFull);

                if (!list.Contains(sKey)) list.Add(sKey, sValue);
            }

            return list;
        }

        public static List<string> GetWeekList(DateTime startDate, DateTime endDate, bool isSection)
        {
            List<string> list = new List<string>();

            for (DateTime date = startDate; date.Date <= endDate.Date; date = date.AddDays(1))
            {
                string value = isSection ? GetWeekPlanNo(date, false) : GetWeekPlanNo(date);

                if (list.Contains(value) == false) list.Add(value);
            }

            return list;
        }

        private static string GetWeekValue(DateTime date, bool isSection, bool isFull)
        {
            if (isSection) return isFull ? GetWeekPlanNo(date, true) : GetWeekPlanNo(date, false);

            return GetWeekPlanNo(date);
        }
        #endregion 시작일 부터 종요일 까지 주차 반환

        /// <summary> 전 주 일요일 반환 </summary>
        public static DateTime GetPriorSunDay(DateTime date)
        {
            int iDayWeek = (int)date.DayOfWeek;

            return (iDayWeek == 0) ? date.AddDays(-7) : date.AddDays(iDayWeek * -1);
        }

        /// <summary> 주 일요일 반환  (* 주 시작일은 월요일)</summary>
        public static DateTime GetWeekSunday(DateTime date)
        {
            int iDayWeek = (int)date.DayOfWeek;
            int diffSunday = (iDayWeek == 0) ? iDayWeek : 7 - iDayWeek;
            return date.AddDays(diffSunday);
        }

        public static DateTime GetWeekMonday(DateTime date)
        {
            int iDayWeek = (int)date.DayOfWeek;
            int diffMonday = (iDayWeek == 0) ? 6 : iDayWeek - 1;
            return date.AddDays(-diffMonday);
        }

        public static string LastMonthWeekYear(string sYear)
        {
            DateTime date = new DateTime(Convert.ToInt32(sYear), 12, 1);
            DateTime endDate = EndDayOfMonth(date);

            return GetWeekPlanNo(endDate);
        }

        public static string LastMonthWeekNo(string sYear)
        {
            string sWeek = LastMonthWeekYear(sYear);

            return sWeek.Substring(sWeek.Length - 2);
        }

        public static bool GetWeekValidation(string sWeek)
        {
            //유효하지 않은 형식인 경우 제외
            Regex r = new Regex(@"^\d{6}[a-b]");

            int sWeekLength = sWeek.Length;
            if (sWeekLength >= 6 && sWeekLength <= 7 && CommonHelper.IsDigit(sWeek.Substring(0, 6)))
            {
                int iWeek = (sWeekLength == 6) ? Convert.ToInt32(sWeek) : Convert.ToInt32(sWeek.Substring(0, 6));
                int iYearLastWeek = Convert.ToInt32(LastMonthWeekYear(sWeek.Substring(0, 4)));

                if (iYearLastWeek < iWeek) return false;

                if (sWeek.Length > 6)
                {
                    Match m = r.Match(sWeek.Trim());
                    if (!m.Success) return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        internal static int AddMonthNo(int yearMonth, int addMonth)
        {
            return DateUtility.AddMonths(yearMonth, addMonth);
        }

        internal static int AddWeekNo(int iWeekNo, int addWeek)
        {
            int addDays = addWeek * 7;
            string weekNoAdded = GetWeekPlanNo(WeekNoToEndDayOfWeek(iWeekNo).AddDays(addDays));
            return int.Parse(weekNoAdded);
        }

        /// <summary>
        /// 해당 월의 첫번째 주차를 반환합니다.
        /// </summary>
        /// <param name="startMonthNo"></param>
        internal static string MonthNoToStartWeek(int startMonthNo, bool considerBoundary = false)
        {
            int year = (int)(startMonthNo / 100);
            int month = startMonthNo - year * 100;

            DateTime startDayOfMonth = StartDayOfMonth(new DateTime(year, month, 1));

            string weekNo;
            if (considerBoundary)
                weekNo = GetWeekPlanNo(startDayOfMonth, false);
            else
                weekNo = GetWeekPlanNo(startDayOfMonth);

            return weekNo;
        }

        /// <summary>
        /// 해당 월의 마지막 주차를 반환합니다.
        /// </summary>
        /// <param name="startMonthNo"></param>
        internal static string MonthNoToEndWeek(int startMonthNo, bool considerBoundary = false)
        {
            int year = (int)(startMonthNo / 100);
            int month = startMonthNo - year * 100;

            DateTime endDayOfMonth = EndDayOfMonth(new DateTime(year, month, 1));

            string weekNo;
            if (considerBoundary)
                weekNo = GetWeekPlanNo(endDayOfMonth, false);
            else
                weekNo = GetWeekPlanNo(endDayOfMonth);

            return weekNo;
        }
        #endregion
    }
}
