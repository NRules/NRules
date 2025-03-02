using System.Collections;
using System.Collections.Generic;

namespace NRules.Collections;

internal class OrderedHashSet<TValue>(IEqualityComparer<TValue> comparer) : IEnumerable<TValue>
{
    private readonly Dictionary<TValue, LinkedListNode<TValue>> _dictionary = new(comparer);
    private readonly LinkedList<TValue> _linkedList = new();

    public int Count => _dictionary.Count;

    public void Clear()
    {
        _linkedList.Clear();
        _dictionary.Clear();
    }

    public bool Remove(TValue item)
    {
        if (!_dictionary.TryGetValue(item, out var node)) return false;
        _dictionary.Remove(item);
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
        if (_dictionary.ContainsKey(item)) return false;
        LinkedListNode<TValue> node = _linkedList.AddLast(item);
        _dictionary.Add(item, node);
        return true;
    }
}