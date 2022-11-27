using System.Collections.Generic;
using System.Linq;

namespace NRules.Utilities;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T element)
    {
        return source.Concat(Enumerable.Repeat(element, 1));
    }

#if NETSTANDARD2_0
    public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
    {
        return new HashSet<TSource>(source);
    }
#endif
}
