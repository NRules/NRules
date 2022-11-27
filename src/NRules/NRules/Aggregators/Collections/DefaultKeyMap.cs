using System;
using System.Collections.Generic;

namespace NRules.Aggregators.Collections;

/// <summary>
/// Map (dictionary) that supports mapping using a default value for the key type 
/// (which is <c>null</c> for reference types).
/// </summary>
internal class DefaultKeyMap<TKey, TValue>
{
    private static readonly TKey DefaultKey = default(TKey)!;

    private readonly Dictionary<TKey, TValue> _map = new();
    private KeyValuePair<TKey, TValue>? _defaultPair;

    public int Count => _map.Count + (_defaultPair.HasValue ? 1 : 0);

    public bool ContainsKey(TKey key)
    {
        if (Equals(key, DefaultKey))
        {
            return _defaultPair.HasValue;
        }
        return _map.ContainsKey(key!);
    }

    public void Add(TKey key, TValue value)
    {
        if (Equals(key, DefaultKey))
        {
            if (_defaultPair.HasValue)
                throw new ArgumentException("An item with the default key has already been added");
            SetDefaultValue(value);
        }
        else
        {
            _map.Add(key, value);
        }
    }

    public bool Remove(TKey key)
    {
        if (Equals(key, DefaultKey))
        {
            if (_defaultPair.HasValue)
            {
                _defaultPair = null;
                return true;
            }

            return false;
        }
        return _map.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (Equals(key, DefaultKey))
        {
            value = _defaultPair.HasValue
                ? _defaultPair.Value.Value
                : default!;
            return _defaultPair.HasValue;
        }
        return _map.TryGetValue(key, out value);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (Equals(key, DefaultKey))
            {
                if (!_defaultPair.HasValue)
                    throw new KeyNotFoundException("Default key was not found");

                return _defaultPair.Value.Value;
            }
            return _map[key];
        }
        set
        {
            if (Equals(key, DefaultKey))
            {
                SetDefaultValue(value);
            }
            else
            {
                _map[key] = value;
            }
        }
    }

    public IEnumerable<TKey> Keys
    {
        get
        {
            if (_defaultPair.HasValue)
                yield return DefaultKey;

            foreach (var item in _map)
            {
                yield return item.Key;
            }
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            if (_defaultPair.HasValue)
                yield return _defaultPair.Value.Value;

            foreach (var item in _map)
            {
                yield return item.Value;
            }
        }
    }

    private void SetDefaultValue(TValue value) => _defaultPair = new KeyValuePair<TKey, TValue>(DefaultKey, value);
}