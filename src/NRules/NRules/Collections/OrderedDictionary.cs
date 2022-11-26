using System.Collections.Generic;

namespace NRules.Collections;

internal class OrderedDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, LinkedListNode<TValue>> _dictionary = new();
    private readonly LinkedList<TValue> _linkedList = new();

    public OrderedDictionary()
    {
    }

    public void Clear()
    {
        _linkedList.Clear();
        _dictionary.Clear();
    }

    public int Count => _dictionary.Count;

    public IEnumerable<TValue> Values => _linkedList;

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    public void Add(TKey key, TValue value)
    {
        LinkedListNode<TValue> node = _linkedList.AddLast(value);
        _dictionary.Add(key, node);
    }

    public bool Remove(TKey key)
    {
        bool found = _dictionary.TryGetValue(key, out var node);
        if (!found)
            return false;
        _dictionary.Remove(key);
        _linkedList.Remove(node);
        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        bool found = _dictionary.TryGetValue(key, out var node);
        if (!found)
        {
            value = default!; // TODO: Use proper value parameter attributes for nullability hints
            return false;
        }
        value = node.Value;
        return true;
    }

    public TValue this[TKey key]
    {
        get
        {
            var node = _dictionary[key];
            return node.Value;
        }
        set
        {
            bool found = _dictionary.TryGetValue(key, out var node);
            if (!found)
            {
                Add(key, value);
            }
            else
            {
                node.Value = value;
            }
        }
    }
}