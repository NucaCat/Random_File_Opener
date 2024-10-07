using System;
using System.Collections.Generic;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    public static class Extensions
    {
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> items, bool condition, Func<TSource, bool> predicate)
            => condition ? items.Where(predicate) : items;
    }
}