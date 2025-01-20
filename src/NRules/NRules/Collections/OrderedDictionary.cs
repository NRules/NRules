using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NRules.Collections;

internal class OrderedDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer)
{
    private readonly Dictionary<TKey, LinkedListNode<TValue>> _dictionary = new(comparer);
    private readonly LinkedList<TValue> _linkedList = new();

    public void Clear()
    {
        _linkedList.Clear();
        _dictionary.Clear();
    }

    public int Count => _dictionary.Count;
    public IReadOnlyCollection<TValue> Values => _linkedList;

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
        if (!_dictionary.TryGetValue(key, out var node)) return false;
        _dictionary.Remove(key);
        _linkedList.Remove(node);
        return true;
    }

    public bool TryGetValue(TKey key, [NotNullWhen(returnValue:true)]out TValue? value)
    {
        if (!_dictionary.TryGetValue(key, out var node))
        {
            value = default;
            return false;
        }
        value = node.Value!;
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
            if (!_dictionary.TryGetValue(key, out var node))
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