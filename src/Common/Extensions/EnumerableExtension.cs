using System;
using System.Collections.Generic;
using System.Linq;

using DataVault.Common.Optional;

namespace DataVault.Common.Extensions
{
    public static class EnumerableExtension
    {
        public static TResult[] ConvertArray<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> toResult)
        {
            return items == null ? Array.Empty<TResult>() : items.Select(toResult).Where(x => x != null).ToArray();
        }

        public static TResult[] ConvertArray<TSource, TResult>(this Optional<TSource[]> items, Func<TSource, TResult> toResult)
        {
            return !items.HasValue || items.Value == null ? Array.Empty<TResult>() : items.Value.Select(toResult).Where(x => x != null).ToArray();
        }

        public static List<TResult> ConvertList<TSource, TResult>(this Optional<List<TSource>> items, Func<TSource, TResult> toResult)
        {
            return !items.HasValue || items.Value == null ? new List<TResult>() : items.Value.Select(toResult).Where(x => x != null).ToList();
        }

        public static List<TResult> ConvertList<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> toResult)
        {
            return items == null ? new List<TResult>() : items.Select(toResult).Where(x => x != null).ToList();
        }

        public static string JoinNotEmpty(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        public static string JoinNotEmpty(this IEnumerable<string> values)
        {
            return values.JoinNotEmpty(string.Empty);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static TSource[] EmptyIfNull<TSource>(this IEnumerable<TSource> source)
        {
            return source == null ? Array.Empty<TSource>() : source.ToArray();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}