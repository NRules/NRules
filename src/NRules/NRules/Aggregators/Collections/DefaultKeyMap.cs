using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NRules.Aggregators.Collections;

/// <summary>
/// Map (dictionary) that supports mapping using a default value for the key type 
/// (which is <c>null</c> for reference types).
/// </summary>
internal class DefaultKeyMap<TKey, TValue>
{
    private readonly Dictionary<TKey, TValue> _map = new();
    private readonly TKey? _defaultKey = default;
    private TValue? _defaultValue;
    private bool _hasDefault;

    public int Count => _map.Count + (_hasDefault ? 1 : 0);
    public int KeyCount => _map.Keys.Count + (_hasDefault ? 1 : 0);

    public bool ContainsKey(TKey? key)
    {
        if (Equals(key, _defaultKey))
        {
            return _hasDefault;
        }
        return _map.ContainsKey(key!);
    }

    public void Add(TKey? key, TValue value)
    {
        if (Equals(key, _defaultKey))
        {
            if (_hasDefault)
                throw new ArgumentException("An item with the default key has already been added");
            
            _hasDefault = true;
            _defaultValue = value;
        }
        else
        {
           _map.Add(key!, value);
        }
    }

    public bool Remove(TKey? key)
    {
        if (Equals(key, _defaultKey))
        {
            if (_hasDefault)
            {
                _defaultValue = default;
                _hasDefault = false;
                return true;
            }
            return false;
        }
        return _map.Remove(key!);
    }

    public bool TryGetValue(TKey? key, [NotNullWhen(returnValue:true)]out TValue? value)
    {
        if (Equals(key, _defaultKey))
        {
            value = _defaultValue;
            return _hasDefault;
        }
        return _map.TryGetValue(key!, out value);
    }

    public TValue this[TKey? key]
    {
        get
        {
            if (Equals(key, _defaultKey))
            {
                if (!_hasDefault)
                    throw new KeyNotFoundException("Default key was not found");
                return _defaultValue!;
            }
            return _map[key!];
        }
        set
        {
            if (Equals(key, _defaultKey))
            {
                _hasDefault = true;
                _defaultValue = value;
            }
            else
            {
                _map[key!] = value!;
            }
        }
    }

    public IEnumerable<TKey?> Keys
    {
        get
        {
            if (_hasDefault) yield return _defaultKey;
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
            if (_hasDefault) yield return _defaultValue!;
            foreach (var item in _map)
            {
                yield return item.Value;
            }
        }
    }
}