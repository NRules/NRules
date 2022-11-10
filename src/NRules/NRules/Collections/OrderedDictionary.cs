using System.Collections.Generic;
using NRules.Utilities;

namespace NRules.Collections;

internal class OrderedDictionary<TKey, TValue>
{
    private readonly IDictionary<TKey, LinkedListNode<TValue>> _dictionary;
    private readonly LinkedList<TValue> _linkedList = new();

    public OrderedDictionary()
    {
        _dictionary = new Dictionary<TKey, LinkedListNode<TValue>>(EqualityComparer<TKey>.Default);
    }

    public IEnumerable<TValue> Values => _linkedList;

    public void Add(TKey key, TValue value)
    {
        var node = _linkedList.AddLast(value);
        _dictionary.Add(key, node);
    }

    public void Remove(TKey key)
    {
        var node = _dictionary.TryRemoveAndGetValue(key);
        if (node is not null)
            _linkedList.Remove(node);
    }

    public bool TryGetValue(TKey key, out TValue? value)
    {
        value = default;
        if (_dictionary.TryGetValue(key, out var node))
        {
            value = node.Value;
            return true;
        }

        return false;
    }
}