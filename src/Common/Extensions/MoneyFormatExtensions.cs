using System;
using System.Globalization;

namespace DataVault.Common.Extensions
{
    public static class MoneyFormatExtensions
    {
        public static decimal StandardizeAmount(this decimal value)
        {
            return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public static decimal StandardizeAmount(this string value)
        {
            var amount = value.ToDecimal();

            return StandardizeAmount(amount);
        }

        public static string ToStandardizeAmountString(this decimal value)
        {
            return StandardizeAmount(value).ToString(CultureInfo.InvariantCulture);
        }
    }
}