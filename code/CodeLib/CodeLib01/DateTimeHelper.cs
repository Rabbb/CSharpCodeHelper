using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using CodeLib01.Models;
using Newtonsoft.Json.Linq;

#pragma warning disable CS0219,CS8600,CS8602,CS8603,CS8604,CS8618,CS8619,CS8625,CS8714

namespace CodeLib01;

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
    /// 停止计时, 并返回计时
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    public static TimeSpan StopTime(this Stopwatch timer)
    {
        timer.Stop();
        return timer.Elapsed;
    }


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
    public static DateRange GetWeekDateRange(DateTime day)
    {
        var day_of_week = (int)day.DayOfWeek;
        var first_day = day.AddDays(-day_of_week);
        var last_day = day.AddDays(6 - day_of_week);
        return new DateRange { Begin = first_day, End = last_day };
    }
        
    /// <summary>
    /// 获取当前月份的开始和结束的日期区间
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public static DateRange GetMonthDateRange(DateTime day)
    {
        int year = day.Year;
        int month = day.Month;
        var first_day = new DateTime(year, month, 1);
        var last_day = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        return new DateRange { Begin = first_day, End = last_day };
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
            var week = WeekOfYear(shownStartDate);
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
        return WeekOfYear(weekend);
    }


    /// <summary>
    /// 获取当前时间是当年的第几周
    /// </summary>
    /// <param name="curDay"></param>
    /// <returns></returns>
    public static int WeekOfYear(this DateTime curDay) => (int)Math.Ceiling((curDay.DayOfYear + (int)new DateTime(curDay.Year, 1, 1).DayOfWeek) / 7.0);
    // public static int WeekOfYear(this DateTime curDay) => DateTimeFormatInfo.InvariantInfo.Calendar.GetWeekOfYear(curDay, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

    /// <summary>
    /// 得到一年中的某周的起始日和截止日(从周日开始计算)
    /// </summary>
    /// <param name="nYear">年</param>
    /// <param name="nNumWeek">第几周</param>
    /// <param name="dtWeekStart">开始日期</param>
    /// <param name="dtWeekeEnd">结束日期</param>
    public static void GetWeek(int nYear, int nNumWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
    {
        var firstOfYear = new DateTime(nYear, 1, 1);
        dtWeekStart = firstOfYear.AddDays(-(int)firstOfYear.DayOfWeek).AddDays((nNumWeek - 1) * 7);
        dtWeekeEnd = dtWeekStart.AddDays(6);
    }

    /// <summary>
    /// 得到一年中的某周的起始日和截止日(从周日开始计算)
    /// </summary>
    /// <param name="nYear">年</param>
    /// <param name="nNumWeek">第几周</param>
    /// <param name="dtWeekStart">开始日期</param>
    /// <param name="dtWeekeEnd">结束日期</param>
    public static void GetWeekAll(int nYear, int nNumWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
    {
        GetWeek(nYear, nNumWeek, out dtWeekStart, out dtWeekeEnd);
        dtWeekeEnd = dtWeekeEnd.AddDays(1).AddSeconds(-1);
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


    #region timestamp

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

    #region format exact

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

    #endregion

    /// <summary>
    /// 获取当年的周数
    /// </summary>
    public static int WeeksInYear(int year) => new DateTime(year, 12, 31).WeekOfYear();

    /// <summary>
    /// 获取当年的周数
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int WeeksInYear(this DateTime date) => WeeksInYear(date.Year);

    public static int DaysInMonth(this DateTime date)
    {
        return DateTime.DaysInMonth(date.Year, date.Month);
    }

}