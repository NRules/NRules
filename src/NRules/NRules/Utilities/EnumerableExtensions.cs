using System.Collections.Generic;
using System.Linq;

namespace NRules.Utilities;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T element)
    {
        return source.Concat(Enumerable.Repeat(element, 1));
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source)
    {
        return source ?? Enumerable.Empty<T>();
    }
}
