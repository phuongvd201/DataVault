using System;

namespace DataVault.Query
{
    public class QueryValue
    {
        public object Value { get; set; }

        public Type Type { get; set; }

        public QueryValue(object value)
        {
            Value = value;
            Type = value.GetType();
        }

        public static string UnHex(string value)
        {
            return $"UNHEX('{value}')";
        }
    }
}