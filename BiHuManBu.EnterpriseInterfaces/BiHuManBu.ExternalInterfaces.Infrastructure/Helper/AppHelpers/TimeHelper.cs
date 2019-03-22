using System;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers
{
    public static class TimeHelper
    {
        public static DateTime UnixTimeToDateTime(this string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public static long DateTimeToUnixTime(this DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
            //return (time.ToUniversalTime().Ticks - 621355968000000000)/10000000;
        }

        /// <summary>
        /// 两个时间的差值
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public static int GetDayMinus(DateTime dateTime1, DateTime dateTime2)
        {
            TimeSpan ts = dateTime1 - dateTime2;
            if (ts.Days == 0)
            {
                if (dateTime1 < dateTime2)
                {
                    return 1;
                }
            }
            return ts.Days;
        }
    }
}
