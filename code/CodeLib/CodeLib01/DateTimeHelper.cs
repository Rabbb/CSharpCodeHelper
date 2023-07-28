using System;
using System.Collections.Generic;
using System.Globalization;
using CodeLib01.Models;
using Newtonsoft.Json.Linq;
#pragma warning disable CS8625

namespace CodeLib01
{
    /// <summary>
    /// 时间部分
    /// <br/>2022-11-30 Ciaran 
    /// </summary>
    public enum DatePart
    {
        Year,
        Month,
        Week,
        Day,
        Hour,
        Minute,
        Second,
        Millisecond,
        Tick,
    }

    public static class DateTimeHelper
    {
        /// <summary>
        /// MSSQL DateTime类型最小值 1753-01-01 00:00:00
        /// </summary>
        public static readonly DateTime MsSqlMinDateTime = new DateTime(1753, 1, 1);

        /// <summary>
        /// 获取范围内的时间值
        /// <br/>2022-11-30 Ciaran 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step">位移函数</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> Range(DateTime start, DateTime end, Func<DateTime, DateTime>? step = null)
        {
            step ??= d => d.AddDays(1);
            var start2 = start.Date;
            while (start2 <= end)
            {
                yield return start2;
                start2 = step(start2);
            }
        }

        /// <summary>
        /// 获取范围内的时间值
        /// <br/>2022-11-30 Ciaran 
        /// </summary>
        public static IEnumerable<DateTime> Range(DateTime start, DateTime end, DatePart part, double step)
        {
            // 2022-11-30 Ciaran 如果step 小于零, 则 开始值 大于 结束值
            if (step < 0 && start < end)
            {
                var tmp = start;
                start = end;
                end = tmp;
            }

            if (part == DatePart.Day)
                return Range(start, end, d => d.AddDays(step));
            if (part == DatePart.Hour)
                return Range(start, end, d => d.AddHours(step));
            if (part == DatePart.Minute)
                return Range(start, end, d => d.AddMinutes(step));
            if (part == DatePart.Second)
                return Range(start, end, d => d.AddSeconds(step));
            if (part == DatePart.Year)
                return Range(start, end, d => d.AddYears((int)step));
            if (part == DatePart.Month)
                return Range(start, end, d => d.AddMonths((int)step));
            if (part == DatePart.Millisecond)
                return Range(start, end, d => d.AddMilliseconds(step));
            if (part == DatePart.Tick)
                return Range(start, end, d => d.AddTicks((int)step));
            throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }

        /// <summary>
        /// 获取当前星期的开始和结束的日期区间
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTimeRange GetWeekDateRange(DateTime day)
        {
            var day_of_week = (int)day.DayOfWeek;
            //if (day_of_week == 0)
            //{
            //    day_of_week = 7;
            //}
            var first_day = day.AddDays(-day_of_week);
            var last_day = day.AddDays(6 - day_of_week);
            return new DateTimeRange { Begin = first_day, End = last_day };
        }

        /// <summary>
        /// 获取当前月份的开始和结束的日期区间
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTimeRange GetMonthDateRange(DateTime day)
        {
            int year = day.Year;
            int month = day.Month;
            var first_day = new DateTime(year, month, 1);
            var last_day = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return new DateTimeRange { Begin = first_day, End = last_day };
        }

        /// <summary>
        /// 此方法转换失败时, 会发生异常
        /// </summary>
        /// <param name="time_str"></param>
        /// <returns></returns>
        public static DateTime? Parse(string time_str)
        {
            return !string.IsNullOrWhiteSpace(time_str) ? new JValue(time_str).ToObject<DateTime>() : null;
        }

        /// <summary>
        /// 此方法转换失败时, 返回空时间
        /// </summary>
        /// <param name="time_str"></param>
        /// <returns></returns>
        public static DateTime? AsDateTime(this string time_str)
        {
            try
            {
                return Parse(time_str);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 根据开始于结束时间获取时间段内所经过的周
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="weeks"></param>
        public static void GetWeeksByStartEndTime(DateTime startDate, DateTime endDate, out List<int> weeks)
        {
            weeks = new List<int>();
            int diff = (7 + (startDate.DayOfWeek - DayOfWeek.Sunday)) % 7;
            var weekStartDate = startDate.AddDays(-1 * diff).Date;
            var weekEndDate = DateTime.MinValue;
            while (weekEndDate < endDate)
            {
                weekEndDate = weekStartDate.AddDays(6);
                var shownStartDate = weekStartDate < startDate ? startDate : weekStartDate;
                var shownEndDate = weekEndDate > endDate ? endDate : weekEndDate;
                //Console.WriteLine($"Week {i++}: {shownStartDate:dd MMMM yyyy} - {shownEndDate:dd MMMM yyyy}");
                var week = GetWeekIndexByYear(shownStartDate);
                weeks.Add(week);
                weekStartDate = weekStartDate.AddDays(7);
            }
        }

        /// <summary>
        /// 由于一年的最后一周和下年的第一周存在重叠, 则存在重复周, 使用周末来唯一化周<br/>
        /// 2022-12-15 Ciaran
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="weekend"></param>
        /// <returns></returns>
        public static int GetWeekendWeekOfYear(DateTime dt, out DateTime weekend)
        {
            dt = dt.Date;
            weekend = dt.AddDays((int)DayOfWeek.Saturday - (int)dt.DayOfWeek);
            return GetWeekIndexByYear(weekend);
        }


        /// <summary>
        /// 获取当前时间是当年的第几周
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetWeekIndexByYear(DateTime dt)
        {
            DateTime curDay = dt;

            int firstdayofweek = Convert.ToInt32(Convert.ToDateTime(curDay.Year + "-1-1").DayOfWeek);

            int days = curDay.DayOfYear;
            int daysOutOneWeek = days - (7 - firstdayofweek);

            if (daysOutOneWeek <= 0)
            {
                return 1;
            }

            int weeks = daysOutOneWeek / 7;
            if (daysOutOneWeek % 7 != 0)
                weeks++;

            return weeks + 1;
            ////当前时间当年的第一天
            //DateTime time = Convert.ToDateTime(dt.ToString("yyyy") + "-01-01");
            //TimeSpan ts = dt - time;
            ////当年第一天是星期几
            //int firstDayOfWeek = (int)time.DayOfWeek;
            ////获取当前时间已过的总天数（四舍五入）
            //int day = int.Parse(ts.TotalDays.ToString("F0")) + 1;
            ////今年第一天星期日
            //if (firstDayOfWeek == 0)
            //{
            //    //总天数减1
            //    day--;
            //}
            //else
            //{
            //    //减去第一周的天数
            //    day = day - (7 - firstDayOfWeek + 1);
            //}
            ////当前日期的星期
            //int thisDayOfWeek = (int)dt.DayOfWeek;
            ////星期日直接减7天
            //if (thisDayOfWeek == 0)
            //{
            //    day = day - 7;
            //}
            //else
            //{
            //    day = day - thisDayOfWeek;
            //}
            ////第一个星期完整的7天+ 当前时间这个星期的7天 除以7
            //int week = (day + 7 + 7) / 7;
            //return week;
        }

        /// <summary>
        /// 得到一年中的某周的起始日和截止日(从周日开始计算)
        /// 年 nYear
        /// 周数 nNumWeek
        /// 周始 out dtWeekStart
        /// 周终 out dtWeekeEnd
        /// </summary>
        /// <param name="nYear"></param>
        /// <param name="nNumWeek"></param>
        /// <param name="dtWeekStart"></param>
        /// <param name="dtWeekeEnd"></param>
        public static void GetWeek(int nYear, int nNumWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
        {
            //DateTime dt = new DateTime(nYear, 1, 1);
            //dt = dt + new TimeSpan((nNumWeek - 1) * 7, 0, 0, 0);
            //dtWeekStart = dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Sunday);
            //dtWeekeEnd = dt.AddDays((int)DayOfWeek.Friday - (int)dt.DayOfWeek + 1).AddDays(1);
            GetFirstEndDayOfWeek(nYear, nNumWeek, CultureInfo.CurrentCulture, out dtWeekStart, out dtWeekeEnd);
        }


        /// <summary>
        /// 根据一年中的第几周获取该周的开始日期与结束日期
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekNumber"></param>
        /// <param name="culture"></param>
        /// <param name="start_time"></param>
        /// <param name="end_time"></param>
        /// <returns></returns>
        public static void GetFirstEndDayOfWeek(int year, int weekNumber, CultureInfo culture, out DateTime start_time, out DateTime end_time)
        {
            Calendar calendar = culture.Calendar;

            DateTime firstOfYear = new DateTime(year, 1, 1);

            DateTime targetDay = calendar.AddWeeks(firstOfYear, weekNumber - 1);


            while (targetDay.DayOfWeek != DayOfWeek.Sunday)

            {
                targetDay = targetDay.AddDays(-1);
            }

            start_time = targetDay;
            end_time = targetDay.AddDays(6);

            //return Tuple.Create<DateTime, DateTime>(targetDay, targetDay.AddDays(6));
        }

        /// <summary>
        /// 得到一年中的某周的起始日和截止日(从周日开始计算)
        /// 年 nYear
        /// 周数 nNumWeek
        /// 周始 out dtWeekStart
        /// 周终 out dtWeekeEnd
        /// </summary>
        /// <param name="nYear"></param>
        /// <param name="nNumWeek"></param>
        /// <param name="dtWeekStart"></param>
        /// <param name="dtWeekeEnd"></param>
        public static void GetWeekAll(int nYear, int nNumWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
        {
            //DateTime dt = new DateTime(nYear, 1, 1);
            //dt = dt + new TimeSpan((nNumWeek - 1) * 7, 0, 0, 0);
            //dtWeekStart = dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Sunday);
            //dtWeekeEnd = dt.AddDays((int)DayOfWeek.Friday - (int)dt.DayOfWeek + 1).AddDays(1).AddSeconds(-1);
            GetFirstEndDayOfWeek(nYear, nNumWeek, CultureInfo.CurrentCulture, out dtWeekStart, out dtWeekeEnd);
            dtWeekeEnd = dtWeekeEnd.AddSeconds(-1);
        }

        /// <summary>
        /// 获取一年中某个月的最后一天
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(int Year, int Month)
        {
            //这里的关键就是 DateTime.DaysInMonth 获得一个月中的天数          
            int Days = DateTime.DaysInMonth(Year, Month);
            return Convert.ToDateTime(Year + "-" + Month + "-" + Days);
        }

        /// <summary>
        /// 获取一年中某个月的最后一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            int year = date.Year, month = date.Month;
            return new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }

        /// <summary>
        /// 获取一年中某个月的第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// 获取两个日期之间的所有日期
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static List<DateTime> GetDaysBetweenStartDateEndDate(DateTime startdate, DateTime enddate)
        {
            var DateSum = new List<DateTime>();
            for (var date = startdate; date <= enddate; date = date.AddDays(1))
            {
                DateSum.Add(date);
            }

            return DateSum;
        }

        /// <summary>
        /// 获取两个日期之间的所有日期的最后一秒
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static List<DateTime> GetLastDaysBetweenStartDateEndDate(DateTime startdate, DateTime enddate)
        {
            var DateSum = new List<DateTime>();
            for (var date = startdate; date <= enddate; date = date.AddDays(1))
            {
                DateSum.Add(Convert.ToDateTime(date.ToString("yyyy-MM-dd  23:59:59")));
            }

            return DateSum;
        }

        /// <summary>
        /// 获取两个日期之间的所有月份的第一天
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static List<DateTime> GetMonthsBetweenStartDateEndDate(DateTime startdate, DateTime enddate)
        {
            var DateSum = new List<DateTime>();
            for (var date = startdate; date <= enddate; date = date.AddMonths(1))
            {
                DateSum.Add(date);
            }

            return DateSum;
        }

        /// <summary>
        /// 获取两个日期之间的所有月份最后一天
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static List<DateTime> GetLastMonthsBetweenStartDateEndDate(DateTime startdate, DateTime enddate)
        {
            var DateSum = new List<DateTime>();
            for (var date = startdate; date <= enddate; date = date.AddMonths(1))
            {
                //int year = date.Year, month = date.Month;
                //var lastdate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                DateTime lastdate = date.AddDays(1 - date.Day).Date.AddMonths(1).AddSeconds(-1);
                DateSum.Add(lastdate);
            }

            return DateSum;
        }

        /// <summary>
        /// 获取两个日期之间的所有日期
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static List<DateTime> GetDaysBetweenStartDateEndDateNotIncludeLast(DateTime startdate, DateTime enddate)
        {
            var DateSum = new List<DateTime>();
            for (var date = startdate; date < enddate; date = date.AddDays(1))
            {
                DateSum.Add(date);
            }

            return DateSum;
        }

        #region MyRegion

        /// <summary>
        /// 生成十位的 Unix 时间戳
        /// </summary>
        /// <param name="date_time"></param>
        /// <returns></returns>
        public static int ToUnixTimeStamp(DateTime date_time)
        {
            DateTime date_start = new DateTime(1970, 1, 1).ToLocalTime();
            int timestamp = Convert.ToInt32((date_time - date_start).TotalSeconds);
            return timestamp;
        }

        /// <summary>
        /// 十位的 Unix 时间戳 转 DateTime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeStamp(int timestamp)
        {
            DateTime start_time = new DateTime(1970, 1, 1);
            DateTime date_time = start_time.AddSeconds(timestamp);
            return date_time.ToLocalTime();
        }

        /// <summary>  
        /// 将c# DateTime时间格式转换为JS时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(DateTime time)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            long t = (time.Ticks - startTime.Ticks) / 10000; //除10000调整为13位      
            return t;
        }

        /// <summary>
        /// Unix(10位)/JS(13位)时间戳转换为DateTime时间
        /// </summary>
        /// <param name="timeStamp">输入时间戳</param>
        /// <param name="applytime">输出DateTime类型时间</param>
        public static void ConvertToDate(string timeStamp, out DateTime applytime)
        {
            applytime = DateTime.Now;

            DateTime startTime = new DateTime(1970, 1, 1).ToLocalTime(); // 当地时区
            long lTime = long.Parse(timeStamp + (timeStamp.Length == 13 ? "0000" : "0000000"));
            TimeSpan toNow = new TimeSpan(lTime);
            applytime = startTime.Add(toNow);
        }

        #endregion

        public static DateTime? yyyyMMddHHmmss(string? s)
        {
            if (s is null)
                return null;

            if (s.Length == "yyyyMMddHHmmss".Length)
                return ParseExact(s, "yyyyMMddHHmmss");
            if (s.Length == "yyyyMMddHHmm".Length)
                return ParseExact(s, "yyyyMMddHHmm");
            if (s.Length == "yyyyMMdd".Length)
                return ParseExact(s, "yyyyMMdd");

            return null;
        }

        public static DateTime? yyyyMMddHHmm(string? s)
        {
            return ParseExact(s, "yyyyMMddHHmm");
        }

        public static DateTime? yyyyMMdd(string? s)
        {
            return ParseExact(s, "yyyyMMdd");
        }

        public static DateTime? ParseExact(string? s, string format)
        {
            if (string.IsNullOrEmpty(value: s))
            {
                return null;
            }

            try
            {
                return DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DateTime? ParseExact(string s, string[] formats)
        {
            if (string.IsNullOrEmpty(value: s))
            {
                return null;
            }

            try
            {
                return DateTime.ParseExact(s, formats, CultureInfo.CurrentCulture, DateTimeStyles.None);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当年的周数
        /// </summary>
        public static int GetWeeksInYear(int year)
        {
            return DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(new DateTime(year, 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static int GetDaysInMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }

        /// <summary>
        /// 获取日期所在年多少周
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetWeeksOfYear(this DateTime date)
        {
            return DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(new DateTime(date.Year, 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        /// <summary>
        /// 获取日期是第几周
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime date)
        {
            return DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}