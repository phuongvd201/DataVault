using System;
using System.Collections.Generic;
using System.Data;

namespace DataVault.Internal
{
    internal static class DataRecordExtensions
    {
        internal static DateTime? GetNullableDateTime(this IDataRecord record, int column)
        {
            return record.IsDBNull(column) ? null : (DateTime?)record.GetDateTime(column);
        }

        internal static int? GetNullableInt32(this IDataRecord record, int column)
        {
            return record.IsDBNull(column) ? null : (int?)record.GetInt32(column);
        }

        internal static int? GetNullableInt64(this IDataRecord record, int column)
        {
            return record.IsDBNull(column) ? null : (int?)record.GetInt64(column);
        }

        internal static string GetString(this IDataRecord record, int column)
        {
            return record.IsDBNull(column) ? null : record.GetString(column);
        }

        internal static bool? GetNullableBoolean(this IDataRecord record, int column)
        {
            return record.IsDBNull(column) ? null : (bool?)record.GetBoolean(column);
        }

        internal static string GetSafeString(this IDataRecord record, int column)
        {
            return record.IsDBNull(column) ? null : record.GetString(column);
        }

        internal static int? GetNullableInt16(this IDataRecord record, int column)
        {
            return record.IsDBNull(column) ? null : (int?)record.GetInt16(column);
        }

        internal static TimeSpan GetTimeSpan(this IDataReader record, int column)
        {
            return (TimeSpan)record.GetValue(column);
        }

        internal static TimeSpan? GetNullableTimeSpan(this IDataRecord record, int column)
        {
            return record.IsDBNull(column) ? null : (TimeSpan?)record.GetValue(column);
        }

        internal static int GetCount(this IDataReader record, Dictionary<string, int> columns)
        {
            return record.IsDBNull(0) ? 0 : int.Parse(record.GetValue(0).ToString());
        }
    }
}