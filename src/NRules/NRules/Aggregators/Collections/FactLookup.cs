using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Aggregators.Collections
{
    internal class FactLookup<TKey, TElement> : IKeyedLookup<TKey, TElement>
    {
        private readonly DefaultKeyMap<TKey, FactGrouping<TKey, TElement>> _groups = new();

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() => 
            _groups.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(TKey key) => _groups.ContainsKey(key);

        public int Count => _groups.KeyCount;

        IEnumerable<TElement> ILookup<TKey, TElement>.this[TKey key] =>
            _groups.TryGetValue(key, out var grouping) ? grouping : Array.Empty<TElement>();

        public FactGrouping<TKey, TElement> this[TKey key] => _groups[key];

        public bool TryGetValue(TKey key, out FactGrouping<TKey, TElement> value) =>
            _groups.TryGetValue(key, out value);

        public void Add(TKey key, FactGrouping<TKey, TElement> value) =>
            _groups.Add(key, value);

        public bool Remove(TKey key) =>
            _groups.Remove(key);

        public IEnumerable<IFact> Facts =>
            _groups.Values.SelectMany(x => x.Facts);

        public IEnumerable<TKey> Keys => _groups.Keys;
    }
}