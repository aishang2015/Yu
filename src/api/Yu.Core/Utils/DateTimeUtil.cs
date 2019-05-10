using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.Utils
{
    public static class DateTimeUtil
    {
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
