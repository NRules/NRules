using System;
using System.Collections.Generic;

namespace NRules.Utilities;

internal static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValueFunc) =>
        dictionary.TryGetValue(key, out TValue value)
            ? value
            : dictionary[key] = createValueFunc(key);

    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default) =>
        dictionary.TryGetValue(key, out TValue value)
            ? value
            : defaultValue;

    public static TValue? TryRemoveAndGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
    {
        if (dictionary.TryGetValue(key, out TValue value))
        {
            dictionary.Remove(key);
            return value;
        }
        else
        {
            return defaultValue;
        }
    }

    public static TValue RemoveAndGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        var value = dictionary[key];
        dictionary.Remove(key);
        return value;
    }
}
