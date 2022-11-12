﻿using System.Collections;
using NRules.Utilities;

namespace NRules.Collections;

internal class OrderedHashSet<TValue> : IEnumerable<TValue>
{
    private readonly Dictionary<TValue, LinkedListNode<TValue>> _dictionary;
    private readonly LinkedList<TValue> _linkedList;

    public OrderedHashSet()
        : this(EqualityComparer<TValue>.Default)
    {
    }

    public OrderedHashSet(IEqualityComparer<TValue> comparer)
    {
        _dictionary = new Dictionary<TValue, LinkedListNode<TValue>>(comparer);
        _linkedList = new LinkedList<TValue>();
    }

    public int Count => _dictionary.Count;

    public void Clear()
    {
        _linkedList.Clear();
        _dictionary.Clear();
    }

    public bool Remove(TValue item)
    {
        var node = _dictionary.TryRemoveAndGetValue(item);
        if (node is null)
            return false;
        _linkedList.Remove(node);
        return true;
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        return _linkedList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(TValue item)
    {
        return _dictionary.ContainsKey(item);
    }

    public bool Add(TValue item)
    {
        if (_dictionary.ContainsKey(item))
            return false;
        var node = _linkedList.AddLast(item);
        _dictionary.Add(item, node);
        return true;
    }
}