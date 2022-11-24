using System;
using System.Collections.Generic;

namespace NRules.Testing;

internal static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createNewValueFunc) =>
        dictionary.TryGetValue(key, out var value)
        ? value
        : dictionary[key] = createNewValueFunc(key);
}
