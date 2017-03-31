using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Map (dictionary) that supports mapping using a default value for the key type 
    /// (which is <c>null</c> for reference types).
    /// </summary>
    internal class DefaultKeyMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _map = new Dictionary<TKey, TValue>();
        private readonly TKey _defaultKey = default(TKey);
        private TValue _defaultValue;
        private bool _hasDefault = false;

        public int Count
        {
            get { return _map.Count + (_hasDefault ? 1 : 0); }
        }

        public bool ContainsKey(TKey key)
        {
            if (Equals(key, _defaultKey))
            {
                return _hasDefault;
            }
            return _map.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (Equals(key, _defaultKey))
            {
                if (_hasDefault)
                {
                    _defaultValue = default(TValue);
                    _hasDefault = false;
                    return true;
                }
                return false;
            }
            return _map.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (Equals(key, _defaultKey))
            {
                value = _defaultValue;
                return _hasDefault;
            }
            return _map.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (Equals(key, _defaultKey))
                {
                    if (!_hasDefault)
                        throw new KeyNotFoundException("Default key was not found");
                    return _defaultValue;
                }
                return _map[key];
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
                    _map[key] = value;
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                if (_hasDefault) yield return _defaultValue;
                foreach (var item in _map)
                {
                    yield return item.Value;
                }
            }
        } 
    }
}