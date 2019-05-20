using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.Utils
{
    public static class DateTimeUtil
    {
        /// <summary>
        /// 时间戳转日期
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns>日期</returns>
        public static DateTime GetDateTime(string timeStamp)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            var dtStart = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), timezone);
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }
}
