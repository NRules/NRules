using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Utilities;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T element)
    {
        return source.Concat(Enumerable.Repeat(element, 1));
    }

    public static TCollection DeepClone<TValue, TCollection>(this IEnumerable<TValue> source, TCollection destination)
        where TValue : ICanDeepClone<TValue>
        where TCollection : ICollection<TValue>
    {
        source.DeepCloneInto(destination);
        return destination;
    }

    public static TCollection Clone<TValue, TCollection>(this IEnumerable<TValue> source, TCollection destination)
        where TCollection : ICollection<TValue>
    {
        source.CloneInto(destination);
        return destination;
    }

    public static void DeepCloneInto<TValue>(this IEnumerable<TValue> source, ICollection<TValue> destination)
        where TValue : ICanDeepClone<TValue>
    {
        source.CloneInto(destination, x => x.DeepClone());
    }

    public static void CloneInto<TValue>(this IEnumerable<TValue> source, ICollection<TValue> destination)
    {
        source.CloneInto(destination, x => x);
    }

    public static void CloneInto<TValue>(this IEnumerable<TValue> source, ICollection<TValue> destination, Func<TValue, TValue> cloneValueFunc)
    {
        foreach (var value in source)
            destination.Add(cloneValueFunc(value));
    }

}
