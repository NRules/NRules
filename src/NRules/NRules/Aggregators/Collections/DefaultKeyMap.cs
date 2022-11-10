using System;
using System.Collections.Generic;

namespace NRules.Aggregators.Collections;

/// <summary>
/// Map (dictionary) that supports mapping using a default value for the key type
/// (which is <c>null</c> for reference types).
/// </summary>
internal class DefaultKeyMap<TKey, TValue>
    where TValue : class
{
    private readonly Dictionary<TKey, TValue> _map = new();
    private readonly TKey _defaultKey = default!;
    private TValue? _defaultValue;
    private bool _hasDefault;

    public int Count => _map.Count + (_hasDefault ? 1 : 0);

    public bool ContainsKey(TKey key)
    {
        return !Equals(key, _defaultKey) ? _map.ContainsKey(key) : _hasDefault;
    }

    public void Add(TKey key, TValue value)
    {
        if (!Equals(key, _defaultKey))
        {
            _map.Add(key, value);
            return;
        }

        if (_hasDefault)
        {
            throw new ArgumentException("An item with the default key has already been added");
        }

        _hasDefault = true;
        _defaultValue = value;
    }

    public bool Remove(TKey key)
    {
        if (!Equals(key, _defaultKey))
        {
            return _map.Remove(key);
        }

        if (!_hasDefault)
        {
            return false;
        }

        _defaultValue = default;
        _hasDefault = false;
        return true;
    }

    public bool TryGetValue(TKey key, out TValue? value)
    {
        if (!Equals(key, _defaultKey))
        {
            return _map.TryGetValue(key, out value);
        }

        value = _defaultValue;
        return _hasDefault;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (!Equals(key, _defaultKey))
                return _map[key];
            if (!_hasDefault)
            {
                throw new KeyNotFoundException("Default key was not found");
            }

            return _defaultValue!;
        }
        set
        {
            if (!Equals(key, _defaultKey))
            {
                _map[key] = value;
                return;
            }

            _hasDefault = true;
            _defaultValue = value;
        }
    }

    public IEnumerable<TKey> Keys
    {
        get
        {
            if (_hasDefault)
            {
                yield return _defaultKey;
            }

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
            if (_hasDefault)
            {
                yield return _defaultValue!;
            }

            foreach (var item in _map)
            {
                yield return item.Value;
            }
        }
    }
}