using System;
using System.Collections.Generic;

namespace NRules.Utilities;

internal static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
    {
        var destination = new Dictionary<TKey, TValue>();
        source.CloneInto(destination);
        return destination;
    }

    public static void DeepCloneInto<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IDictionary<TKey, TValue> destination)
        where TValue : ICanDeepClone<TValue>
    {
        source.CloneInto(destination, x => x.DeepClone());
    }

    public static void CloneInto<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IDictionary<TKey, TValue> destination)
    {
        source.CloneInto(destination, x => x);
    }

    public static void CloneInto<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IDictionary<TKey, TValue> destination, Func<TValue, TValue> cloneValueFunc)
    {
        foreach (var pair in source)
            destination.Add(pair.Key, cloneValueFunc(pair.Value));
    }
}
