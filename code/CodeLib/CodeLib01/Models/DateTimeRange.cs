using System;
using System.Collections.Generic;

namespace CodeLib01.Models
{
    public struct DateTimeRange
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        /// <summary>
        /// 2022-10-19 Ciaran 获取区间期间的月份
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> InnerMonths()
        {
            DateTime current = new DateTime(Begin.Year, Begin.Month, 1);
            DateTime end = new DateTime(End.Year, End.Month, 1);
            do
            {
                yield return current;
                current = current.AddMonths(1);
            } while (current <= end);
        }

        /// <summary>
        /// 2022-10-19 Ciaran 获取区间期间的日期
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> InnerDates()
        {
            DateTime current = Begin.Date;
            DateTime end = End.Date;
            do
            {
                yield return current;
                current = current.AddDays(1);
            } while (current <= end);
        }
    }
}