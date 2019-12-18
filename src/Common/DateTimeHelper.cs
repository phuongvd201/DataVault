using System;

namespace DataVault.Common
{
    public static class DateTimeHelper
    {
        public const string DATE_ISO_FORMAT = "yyyyMMdd";
        public const string DATE_TIME_ISO_FORMAT = "yyyyMMddhhmmss";
        public const string DATE_TIME_ISO_FORMAT_WITH_MICRO_SECOND = "yyyyMMddhhmmssffffff";
        public const string DATE_TIME_STD_FORMAT = "yyyy/MM/dd HH:mm:ss";

        /// <summary>
        /// Return a DateTime string in ISO format yyyyMMddHHmmss
        /// </summary>
        public static string ToISOString(this DateTime dateTime) => dateTime.ToString(DATE_TIME_ISO_FORMAT);

        public static string ToISOStringWithMicrosecond(this DateTime dateTime) => dateTime.ToString(DATE_TIME_ISO_FORMAT_WITH_MICRO_SECOND);

        public static string ConvertToSaleForceDatetimeString(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime()
                         .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }
    }
}
