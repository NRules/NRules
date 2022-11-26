using System;
using System.Collections.Generic;

namespace NRules.Aggregators.Collections;

/// <summary>
/// Map (dictionary) that supports mapping using a default value for the key type 
/// (which is <c>null</c> for reference types).
/// </summary>
internal class DefaultKeyMap<TKey, TValue>
{
    private readonly Dictionary<TKey, TValue> _map = new();
    private KeyValuePair<TKey, TValue>? _defaultPair;

    public int Count => _map.Count + (_defaultPair.HasValue ? 1 : 0);

    public bool ContainsKey(TKey key)
    {
        return Equals(key, default(TKey)) ? _defaultPair.HasValue : _map.ContainsKey(key);
    }

    public void Add(TKey key, TValue value)
    {
        if (Equals(key, default(TKey)))
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
        if (Equals(key, default(TKey)))
        {
            if (!_defaultPair.HasValue)
                return false;

            _defaultPair = null;
            return true;
        }
        return _map.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (Equals(key, default(TKey)))
        {
            value = _defaultPair.HasValue
                ? GetDefaultValueUnsafe()
                : default!;

            return _defaultPair.HasValue;
        }
        return _map.TryGetValue(key, out value);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (Equals(key, default(TKey)))
            {
                if (!_defaultPair.HasValue)
                    throw new KeyNotFoundException("Default key was not found");

                return GetDefaultValueUnsafe();
            }
            return _map[key];
        }
        set
        {
            if (Equals(key, default(TKey)))
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
                yield return default(TKey)!; // WARNING: Key can be null for reference types

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
                yield return GetDefaultValueUnsafe();

            foreach (var item in _map)
            {
                yield return item.Value;
            }
        }
    }

    private void SetDefaultValue(TValue value) => _defaultPair = new KeyValuePair<TKey, TValue>(default(TKey)!, value); // WARNING: Key can be null for reference types

    private TValue GetDefaultValueUnsafe() => _defaultPair!.Value.Value;

}