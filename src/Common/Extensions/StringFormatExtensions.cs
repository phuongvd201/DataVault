using System;
using System.Linq;

namespace DataVault.Common.Extensions
{
    public static class StringFormatExtensions
    {
        public static string ToUnderscoreCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
        }

        public static string ToPascalCase(this string str)
        {
            var words = str.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(
                    word => word.Substring(0, 1).ToUpper() +
                            word.Substring(1).ToLower());

            var result = string.Concat(words);
            return result;
        }

        public static string WrapCurlyBracket(this string value)
        {
            return $"({value})";
        }
    }
}