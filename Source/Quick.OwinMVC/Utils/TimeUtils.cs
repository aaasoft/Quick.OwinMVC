using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC.Utils
{
    public class TimeUtils
    {
        //基础时间
        private static readonly DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 返回时间与1970年1月1日之间的毫秒数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>指定的日期和时间据 GMT 时间 1970 年 1 月 1 日午夜之间的毫秒数。</returns>
        public static double GetTime(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime() - baseTime).TotalMilliseconds;
        }

        /// <summary>
        /// 根据与1970年1月1日之间的毫秒数得到时间
        /// </summary>
        /// <param name="milliseconds">要设置的日期和时间据 GMT 时间 1970 年 1 月 1 日午夜之间的毫秒数。</param>
        /// <returns></returns>
        public static DateTime GetDateTime(double milliseconds)
        {
            return baseTime.AddMilliseconds(milliseconds).ToLocalTime();
        }
    }
}
