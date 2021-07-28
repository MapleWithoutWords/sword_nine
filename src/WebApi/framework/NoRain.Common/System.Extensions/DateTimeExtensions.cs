using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
  public static  class DateTimeExtensions
    {
        public static long ConvertDateTimeToInt13(this DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        public static long ConvertDateTimeToInt10(this DateTime dt)
        {

            DateTime time = new DateTime(1970, 1, 1);
            TimeSpan ts = dt.ToUniversalTime().Subtract(time);
            return (long)ts.TotalSeconds;
        }

        /// <summary>
        /// 是否时默认时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsDefaultValue(this DateTime time)
        {
            return time == default(DateTime);
        }

        /// <summary>
        /// 是否是1900/01/01 00:00:00
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool Is1900Time(this DateTime time)
        {
            return time == new DateTime(1900,1,1);
        }

        /// <summary>
        /// 是否是1900/01/01 00:00:00或者默认时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsDefaultOr1900Time(this DateTime time)
        {
            if (time != default(DateTime))
            {
                return time == new DateTime(1900, 1, 1);
            }
            return true;
        }
        /// <summary>
        /// 是否不是1900/01/01 00:00:00或者默认时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsNoDefaultAndNo1900Time(this DateTime time)
        {
            return time != default(DateTime) && time == new DateTime(1900, 1, 1);
        }

    }
}
