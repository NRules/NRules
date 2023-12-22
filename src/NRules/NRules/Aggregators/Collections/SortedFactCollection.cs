using System;
using System.Collections;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators.Collections;

internal class SortedFactCollection<TElement, TKey> : IEnumerable<TElement>
{
    private readonly SortedDictionary<KeyContainer, LinkedList<IFact>> _items;
    private readonly Dictionary<IFact, SortedFactData> _dataMap;

    public SortedFactCollection(IComparer<TKey> comparer)
    {
        _dataMap = new Dictionary<IFact, SortedFactData>();
        _items = new SortedDictionary<KeyContainer, LinkedList<IFact>>(new KeyComparer(comparer));
    }

    public void AddFact(TKey key, IFact fact)
    {
        var container = new KeyContainer(key);
        if (!_items.TryGetValue(container, out var list))
        {
            list = new LinkedList<IFact>();
            _items.Add(container, list);
        }

        var linkedListNode = list.AddLast(fact);
        _dataMap[fact] = new SortedFactData(key, linkedListNode);
    }

    public void RemoveFact(IFact fact)
    {
        var data = _dataMap[fact];
        _dataMap.Remove(fact);

        var container = new KeyContainer(data.Key);
        var list = _items[container];
        list.Remove(data.LinkedListNode);

        if (list.Count == 0)
        {
            _items.Remove(container);
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

    private class SortedFactData
    {
        internal SortedFactData(TKey key, LinkedListNode<IFact> linkedListNode)
        {
            Key = key;
            LinkedListNode = linkedListNode;
        }

        public TKey Key { get; }
        public LinkedListNode<IFact> LinkedListNode { get; }
    }

    private readonly struct KeyContainer : IEquatable<KeyContainer>
    {
        public KeyContainer(TKey key)
        {
            Key = key;
        }

        public TKey Key { get; }

        public bool Equals(KeyContainer other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key);
        }

        public override bool Equals(object obj)
        {
            return obj is KeyContainer container && Equals(container);
        }

        public override int GetHashCode()
        {
            return 990326508 + EqualityComparer<TKey>.Default.GetHashCode(Key);
        }
    }

    private class KeyComparer : IComparer<KeyContainer>
    {
        private readonly IComparer<TKey> _keyComparer;

        public KeyComparer(IComparer<TKey> keyComparer)
        {
            _keyComparer = keyComparer;
        }

        public int Compare(KeyContainer x, KeyContainer y)
        {
            return _keyComparer.Compare(x.Key, y.Key);
        }
    }
}