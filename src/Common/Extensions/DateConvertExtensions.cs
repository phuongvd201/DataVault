using System;

namespace DataVault.Common.Extensions
{
    public static class DateConvertExtensions
    {
        public static DateTime? FromUnixTime(this long? unixTime)
        {
            return unixTime.HasValue ? unixTime.Value.FromUnixTime() : (DateTime?)null;
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long? ToUnixTime(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToUnixTime() : (long?)null;
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }
    }
}