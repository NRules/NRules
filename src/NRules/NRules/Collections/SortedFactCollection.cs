using System.Collections;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    internal class SortedFactCollection<TElement, TKey> : IEnumerable<TElement>
    {
        private readonly SortedDictionary<TKey, List<IFact>> _items;
        private readonly Dictionary<IFact, TKey> _keyMap;

        public SortedFactCollection(IComparer<TKey> comparer)
        {
            _keyMap = new Dictionary<IFact, TKey>();
            _items = new SortedDictionary<TKey, List<IFact>>(comparer);
        }

        public void AddFact(TKey key, IFact fact)
        {
            _keyMap[fact] = key;

            if (!_items.TryGetValue(key, out var list))
            {
                list = new List<IFact>();
                _items.Add(key, list);
            }

            list.Add(fact);
        }

        public void RemoveFact(IFact fact)
        {
            var key = _keyMap[fact];
            _keyMap.Remove(fact);

            var list = _items[key];
            list.Remove(fact);

            if (list.Count == 0)
            {
                _items.Remove(key);
            }
        }

        public IEnumerable<IFact> GetFactEnumerable()
        {
            foreach (var list in _items.Values)
            {
                foreach (var fact in list)
                {
                    yield return fact;
                }
            }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            foreach (var list in _items.Values)
            {
                foreach (var fact in list)
                {
                    yield return (TElement)fact.Value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}