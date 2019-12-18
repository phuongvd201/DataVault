using System;

namespace DataVault.Common.Extensions
{
    public static class StringExtension
    {
        public static string LastChars(this string text, int numberOfChars)
        {
            string lastChars = text.Length > numberOfChars ? text.Substring(text.Length - numberOfChars, numberOfChars) : text;
            return lastChars;
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool EqualsIgnoreCase(this string firstValue, string secondValue)
        {
            return string.Equals(firstValue, secondValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (str.EndsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return str + c;
        }

        public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (str.StartsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return c + str;
        }
    }
}