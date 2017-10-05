using System.Collections;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    internal class FactCollection<TElement> : IEnumerable<TElement>
    {
        private readonly Dictionary<IFact, LinkedListNode<TElement>> _nodeLookup = new Dictionary<IFact, LinkedListNode<TElement>>();
        private readonly LinkedList<TElement> _elements = new LinkedList<TElement>();

        public void Add(IFact fact, TElement element)
        {
            var node = new LinkedListNode<TElement>(element);
            _elements.AddLast(node);
            _nodeLookup[fact] = node;
        }

        public void Modify(IFact fact, TElement element)
        {
            var node = _nodeLookup[fact];
            if (!ReferenceEquals(node.Value, element))
            {
                node.Value = element;
            }
        }

        public void Remove(IFact fact, TElement element)
        {
            var node = _nodeLookup[fact];
            _elements.Remove(node);
            _nodeLookup.Remove(fact);
        }

        public int Count => _elements.Count;

        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
