using System;
using System.Collections.Generic;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    public static class Extensions
    {
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> items, bool condition, Func<TSource, bool> predicate)
            => condition ? items.Where(predicate) : items;
        
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => DistinctBy(source, keySelector, null);

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source.IsEmpty())
            {
                return source;
            }

            return DistinctByIterator(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var set = new HashSet<TKey>(32, comparer);
                    do
                    {
                        TSource element = enumerator.Current;
                        if (set.Add(keySelector(element)))
                        {
                            yield return element;
                        }
                    } while (enumerator.MoveNext());
                }
            }
        }
    }
}