// Note: Several of the below implementations are closely inspired by the corefx source code for FirstOrDefault, etc.

using System;
using System.Collections.Generic;

namespace DataVault.Common.Optional
{
    public static class OptionalCollectionExtensions
    {
        public static IEnumerable<T> Values<T>(this IEnumerable<Optional<T>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var option in source)
            {
                if (option.HasValue)
                {
                    yield return option.Value;
                }
            }
        }

        public static Optional<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is IList<TSource> list)
            {
                if (list.Count > 0)
                {
                    return list[0].ToOptional();
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        return enumerator.Current.ToOptional();
                    }
                }
            }

            return Optional<TSource>.Undefined;
        }

        public static Optional<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var element in source)
            {
                if (predicate(element))
                {
                    return element.ToOptional();
                }
            }

            return Optional<TSource>.Undefined;
        }

        public static Optional<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is IList<TSource> list)
            {
                var count = list.Count;
                if (count > 0)
                {
                    return list[count - 1].ToOptional();
                }
            }
#if !NET35
            else if (source is IReadOnlyList<TSource> readOnlyList)
            {
                var count = readOnlyList.Count;
                if (count > 0)
                {
                    return readOnlyList[count - 1].ToOptional();
                }
            }
#endif
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        TSource result;
                        do
                        {
                            result = enumerator.Current;
                        }
                        while (enumerator.MoveNext());

                        return result.ToOptional();
                    }
                }
            }

            return Optional<TSource>.Undefined;
        }

        public static Optional<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (source is IList<TSource> list)
            {
                for (var i = list.Count - 1; i >= 0; --i)
                {
                    var result = list[i];
                    if (predicate(result))
                    {
                        return result.ToOptional();
                    }
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var result = enumerator.Current;
                        if (predicate(result))
                        {
                            while (enumerator.MoveNext())
                            {
                                var element = enumerator.Current;
                                if (predicate(element))
                                {
                                    result = element;
                                }
                            }

                            return result.ToOptional();
                        }
                    }
                }
            }

            return Optional<TSource>.Undefined;
        }

        public static Optional<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is IList<TSource> list)
            {
                switch (list.Count)
                {
                    case 0: return Optional<TSource>.Undefined;
                    case 1: return list[0].ToOptional();
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                    {
                        return Optional<TSource>.Undefined;
                    }

                    var result = enumerator.Current;
                    if (!enumerator.MoveNext())
                    {
                        return result.ToOptional();
                    }
                }
            }

            return Optional<TSource>.Undefined;
        }

        public static Optional<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var result = enumerator.Current;
                    if (predicate(result))
                    {
                        while (enumerator.MoveNext())
                        {
                            if (predicate(enumerator.Current))
                            {
                                return Optional<TSource>.Undefined;
                            }
                        }

                        return result.ToOptional();
                    }
                }
            }

            return Optional<TSource>.Undefined;
        }

        public static Optional<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (index >= 0)
            {
                if (source is IList<TSource> list)
                {
                    if (index < list.Count)
                    {
                        return list[index].ToOptional();
                    }
                }
                else
                {
                    using (var enumerator = source.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (index == 0)
                            {
                                return enumerator.Current.ToOptional();
                            }

                            index--;
                        }
                    }
                }
            }

            return Optional<TSource>.Undefined;
        }
    }
}