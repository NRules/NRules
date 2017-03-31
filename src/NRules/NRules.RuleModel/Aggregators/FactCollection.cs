using System.Collections;
using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    internal class FactCollection<TFact> : IEnumerable<TFact>
    {
        private readonly Dictionary<object, LinkedListNode<TFact>> _nodeLookup = new Dictionary<object, LinkedListNode<TFact>>();
        private readonly LinkedList<TFact> _elements = new LinkedList<TFact>();

        public void Add(TFact fact)
        {
            var node = new LinkedListNode<TFact>(fact);
            _elements.AddLast(node);
            _nodeLookup[fact] = node;
        }

        public void Modify(TFact fact)
        {
            var node = _nodeLookup[fact];
            if (!ReferenceEquals(node.Value, fact))
            {
                node.Value = fact;
            }
        }

        public void Remove(TFact fact)
        {
            var node = _nodeLookup[fact];
            _elements.Remove(node);
            _nodeLookup.Remove(fact);
        }

        public int Count { get { return _elements.Count; } }

        public IEnumerator<TFact> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
