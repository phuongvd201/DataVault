using System;
using System.Globalization;

namespace DataVault.Common.Extensions
{
    /// <summary>
    /// Converts a string data type to another base data type using a safe conversion method.
    /// </summary>
    public static class StringConvertExtensions
    {
        public static bool ToBoolean(this string value)
        {
            if (value == null)
                return false;

            bool result;
            if (bool.TryParse(value, out result))
                return result;

            string v = value.Trim();

            if (string.Equals(v, "t", StringComparison.OrdinalIgnoreCase)
                || string.Equals(v, "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(v, "y", StringComparison.OrdinalIgnoreCase)
                || string.Equals(v, "yes", StringComparison.OrdinalIgnoreCase)
                || string.Equals(v, "1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(v, "x", StringComparison.OrdinalIgnoreCase)
                || string.Equals(v, "on", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public static byte ToByte(this string value)
        {
            if (value == null)
                return 0;

            byte result;
            byte.TryParse(value, out result);

            return result;
        }

        public static byte ToByte(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0;

            byte result;
            byte.TryParse(value, NumberStyles.Integer, provider, out result);

            return result;
        }

        public static DateTime ToDateTime(this string value)
        {
            if (value == null)
                return new DateTime(0L);

            DateTime result;
            DateTime.TryParse(value, out result);

            return result;
        }

        public static DateTime ToDateTime(this string value, IFormatProvider provider)
        {
            if (value == null)
                return new DateTime(0L);

            DateTime result;
            DateTime.TryParse(value, provider, DateTimeStyles.None, out result);

            return result;
        }

        public static decimal ToDecimal(this string value)
        {
            if (value == null)
                return 0M;

            decimal result;
            decimal.TryParse(value, out result);

            return result;
        }

        public static decimal ToDecimal(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0M;

            decimal result;
            decimal.TryParse(value, NumberStyles.Number, provider, out result);

            return result;
        }

        public static double ToDouble(this string value)
        {
            if (value == null)
                return 0.0;

            double result;
            double.TryParse(value, out result);

            return result;
        }

        public static double ToDouble(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0.0;

            double result;
            double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

            return result;
        }

        public static short ToInt16(this string value)
        {
            if (value == null)
                return 0;

            short result;
            short.TryParse(value, out result);

            return result;
        }

        public static short ToInt16(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0;

            short result;
            short.TryParse(value, NumberStyles.Integer, provider, out result);

            return result;
        }

        public static int ToInt32(this string value)
        {
            if (value == null)
                return 0;

            int result;
            int.TryParse(value, out result);

            return result;
        }

        public static int ToInt32(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0;

            int result;
            int.TryParse(value, NumberStyles.Integer, provider, out result);

            return result;
        }

        public static long ToInt64(this string value)
        {
            if (value == null)
                return 0L;

            long result;
            long.TryParse(value, out result);

            return result;
        }

        public static long ToInt64(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0L;

            long result;
            long.TryParse(value, NumberStyles.Integer, provider, out result);

            return result;
        }

        public static float ToSingle(this string value)
        {
            if (value == null)
                return 0f;

            float result;
            float.TryParse(value, out result);

            return result;
        }

        public static float ToSingle(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0F;

            float result;
            float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

            return result;
        }

        public static ushort ToUInt16(this string value)
        {
            if (value == null)
                return 0;

            ushort result;
            ushort.TryParse(value, out result);

            return result;
        }

        public static ushort ToUInt16(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0;

            ushort result;
            ushort.TryParse(value, NumberStyles.Integer, provider, out result);

            return result;
        }

        public static uint ToUInt32(this string value)
        {
            if (value == null)
                return 0;

            uint result;
            uint.TryParse(value, out result);

            return result;
        }

        public static uint ToUInt32(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0;

            uint result;
            uint.TryParse(value, NumberStyles.Integer, provider, out result);

            return result;
        }

        public static ulong ToUInt64(this string value)
        {
            if (value == null)
                return 0L;

            ulong result;
            ulong.TryParse(value, out result);

            return result;
        }

        public static ulong ToUInt64(this string value, IFormatProvider provider)
        {
            if (value == null)
                return 0L;

            ulong result;
            ulong.TryParse(value, NumberStyles.Integer, provider, out result);

            return result;
        }

        public static TimeSpan ToTimeSpan(this string value)
        {
            if (value == null)
                return TimeSpan.Zero;

            TimeSpan result;
            TimeSpan.TryParse(value, out result);

            return result;
        }

        public static Guid ToGuid(this string value)
        {
            if (value == null)
                return Guid.Empty;

            Guid result;
            Guid.TryParse(value, out result);

            return result;
        }

        public static T ToEnum<T>(this string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnumOrDefault<T>(this string value, T defaultValue = default) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse(value, true, out result) ? result : defaultValue;
        }

        /// <summary>
        /// Tries to convert the <paramref name="input"/> to the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="input">The input to convert.</param>
        /// <param name="type">The type to convert to.</param>
        /// <param name="value">The converted value.</param>
        /// <returns><c>true</c> if the vaule was converted; otherwise <c>false</c>.</returns>
        public static bool TryConvert(this string input, Type type, out object value)
        {
            // first try string
            if (type == typeof(string))
            {
                value = input;
                return true;
            }

            // check nullable
            if (input == null && type.IsNullable())
            {
                value = null;
                return true;
            }

            Type underlyingType = type.GetUnderlyingType();

            // convert by type
            if (underlyingType == typeof(bool))
            {
                value = input.ToBoolean();
                return true;
            }

            if (underlyingType == typeof(byte))
            {
                value = input.ToByte();
                return true;
            }

            if (underlyingType == typeof(DateTime))
            {
                value = input.ToDateTime();
                return true;
            }

            if (underlyingType == typeof(decimal))
            {
                value = input.ToDecimal();
                return true;
            }

            if (underlyingType == typeof(double))
            {
                value = input.ToDouble();
                return true;
            }

            if (underlyingType == typeof(short))
            {
                value = input.ToInt16();
                return true;
            }

            if (underlyingType == typeof(int))
            {
                value = input.ToInt32();
                return true;
            }

            if (underlyingType == typeof(long))
            {
                value = input.ToInt64();
                return true;
            }

            if (underlyingType == typeof(float))
            {
                value = input.ToSingle();
                return true;
            }

            if (underlyingType == typeof(ushort))
            {
                value = input.ToUInt16();
                return true;
            }

            if (underlyingType == typeof(uint))
            {
                value = input.ToUInt32();
                return true;
            }

            if (underlyingType == typeof(ulong))
            {
                value = input.ToUInt64();
                return true;
            }

            if (underlyingType == typeof(TimeSpan))
            {
                value = input.ToTimeSpan();
                return true;
            }

            if (underlyingType == typeof(Guid))
            {
                value = input.ToGuid();
                return true;
            }

            value = input;
            return true;
        }

        /// <summary>
        /// Converts the result to the TValue type.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="result">The result to convert.</param>
        /// <param name="convert">The optional convert function.</param>
        /// <returns>The converted value.</returns>
        public static TValue ConvertValue<TValue>(this object result, Func<object, TValue> convert = null)
        {
            TValue value;

            if (result == null || result == DBNull.Value)
                value = default(TValue);
            else if (result is TValue)
                value = (TValue)result;
            else if (convert != null)
                value = convert(result);
            else
                value = (TValue)Convert.ChangeType(result, typeof(TValue));

            return value;
        }
    }
}