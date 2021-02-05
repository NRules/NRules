using System.Collections;
using System.Collections.Generic;

namespace NRules.Collections
{
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
            bool found = _dictionary.TryGetValue(item, out var node);
            if (!found) return false;
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
}