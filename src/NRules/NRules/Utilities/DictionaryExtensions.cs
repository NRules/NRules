using System;
using System.Collections.Generic;

namespace NRules.Utilities;

internal static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValueFunc) =>
        dictionary.TryGetValue(key, out TValue value)
            ? value
            : dictionary[key] = createValueFunc(key);
}
