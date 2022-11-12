using System.Collections;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Aggregators.Collections;

internal class SortedFactCollection<TElement, TKey> : IEnumerable<TElement?>
{
    private readonly SortedDictionary<TKey, LinkedList<IFact>> _items;
    private readonly Dictionary<IFact, SortedFactData> _dataMap;

    public SortedFactCollection(IComparer<TKey> comparer)
    {
        _dataMap = new Dictionary<IFact, SortedFactData>();
        _items = new SortedDictionary<TKey, LinkedList<IFact>>(comparer);
    }

    public void AddFact(TKey key, IFact fact)
    {
        var list = _items.GetOrAdd(key, _ => new());
        var linkedListNode = list.AddLast(fact);
        _dataMap[fact] = new SortedFactData(key, linkedListNode);
    }

    public void RemoveFact(IFact fact)
    {
        var data = _dataMap.RemoveAndGetValue(fact);
        var list = _items[data.Key];
        list.Remove(data.LinkedListNode);
        if (list.Count == 0)
        {
            _items.Remove(data.Key);
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

    public IEnumerator<TElement?> GetEnumerator()
    {
        foreach (var list in _items.Values)
        {
            foreach (var fact in list)
            {
                yield return (TElement?)fact.Value;
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
}