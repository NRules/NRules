using System.Collections;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    internal class FactCollection<TElement> : IEnumerable<TElement>
    {
        private readonly Dictionary<IFact, NodePair> _nodeLookup = new Dictionary<IFact, NodePair>();
        private readonly LinkedList<TElement> _elements = new LinkedList<TElement>();
        private readonly LinkedList<IFact> _facts = new LinkedList<IFact>();

        public void Add(IFact fact, TElement element)
        {
            var nodePair = new NodePair(fact, element);
            _facts.AddLast(nodePair.FactNode);
            _elements.AddLast(nodePair.ElementNode);
            _nodeLookup[fact] = nodePair;
        }

        public void Modify(IFact fact, TElement element)
        {
            var nodePair = _nodeLookup[fact];
            if (!ReferenceEquals(nodePair.ElementNode.Value, element))
            {
                nodePair.ElementNode.Value = element;
            }
        }

        public void Remove(IFact fact, TElement element)
        {
            var nodePair = _nodeLookup[fact];
            _facts.Remove(nodePair.FactNode);
            _elements.Remove(nodePair.ElementNode);
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

        public IEnumerable<IFact> Facts => _facts;

        private readonly struct NodePair
        {
            public NodePair(IFact fact, TElement element)
            {
                FactNode = new LinkedListNode<IFact>(fact);
                ElementNode = new LinkedListNode<TElement>(element);
            }

            public LinkedListNode<IFact> FactNode { get; }
            public LinkedListNode<TElement> ElementNode { get; }
        }
    }
}
