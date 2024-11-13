using System;


namespace XEngine.Engine
{
    public static class TimeUtil
    {
        public static DateTime NowTime;

        /// <summary>
        /// 用法见飞书《时间类可传参数表》：https://cx0ifc6fadv.feishu.cn/wiki/XIKbwtwpwi4Nwkk29hZcAcv6nUc?from=from_copylink
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DateTimeToAny(string format)
        {
            NowTime = DateTime.Now;
            return NowTime.ToString(format);
        }

        /// <summary>
        /// string:"AM/PM"
        /// </summary>
        /// <returns></returns>
        public static string GetNowAMPM()
        {
            NowTime = DateTime.Now;
            if (int.Parse(NowTime.ToString("HH")) < 12)
            {
                return "AM";
            }
            else
            {
                return "PM";
            }
        }

        /// <summary>
        /// HH:MM
        /// </summary>
        /// <returns></returns>
        public static string GetNowTimeHM()
        {
            NowTime = DateTime.Now;
            return NowTime.ToString("HH:mm");
        }

        /// <summary>
        /// HH:MM:SS
        /// </summary>
        /// <returns></returns>
        public static string GetNowTimeHMS()
        {
            NowTime = DateTime.Now;
            return NowTime.ToLongTimeString();
        }

        /// <summary>
        /// YY/MM/DD
        /// </summary>
        /// <returns></returns>
        public static string GetDateTodayYMD()
        {
            NowTime = DateTime.Now;
            return NowTime.ToShortDateString();
        }

        /// <summary>
        /// YY年MM月DD日
        /// </summary>
        /// <returns></returns>
        public static string GetDateTodayYMDzhCN()
        {
            NowTime = DateTime.Now;
            return NowTime.ToLongDateString();
        }

        /// <summary>
        /// 毫秒级别时间戳转字符串 日期 时:分
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string TimestampToDateTimeString(long timestamp)
        {
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(timestamp);
            DateTime dateTime = epoch.Add(timeSpan);
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 毫秒级别时间戳转字符串 时:分
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string TimestampToTimeString(long timestamp)
        {
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(timestamp);
            DateTime dateTime = epoch.Add(timeSpan);
            return dateTime.ToString("HH:mm");
        }

        /// <summary>
        /// 获取秒级别时间戳（13位）
        /// </summary>
        /// <returns></returns>
        public static long GetTimestampToSeconds()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 获取毫秒级别时间戳（13位）
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampToMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        /// <summary>
        /// 获取当前时间的毫秒级别时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetNowTimeMilliSeconds()
        {
            return System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
