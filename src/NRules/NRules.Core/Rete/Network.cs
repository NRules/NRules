using System;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal interface INetwork
    {
        IEventSource EventSource { get; }
        void PropagateAssert(object factObject);
        void PropagateUpdate(object factObject);
        void PropagateRetract(object factObject);
    }

    internal class Network : INetwork
    {
        private readonly RootNode _root;
        private readonly IDictionary<object, Fact> _factMap = new Dictionary<object, Fact>();

        public IEventSource EventSource { get; private set; }

        public Network(RootNode root, IEventSource eventSource)
        {
            _root = root;
            EventSource = eventSource;
        }

        public void PropagateAssert(object factObject)
        {
            Fact fact;
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            if (_factMap.TryGetValue(factObject, out fact))
            {
                throw new ArgumentException("Fact for insert already exists", "factObject");
            }
            fact = new Fact(factObject);
            _factMap[factObject] = fact;
            _root.PropagateAssert(fact);
        }

        public void PropagateUpdate(object factObject)
        {
            Fact fact;
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            if (!_factMap.TryGetValue(factObject, out fact))
            {
                throw new ArgumentException("Fact for update does not exist", "factObject");
            }
            _root.PropagateUpdate(fact);
        }

        public void PropagateRetract(object factObject)
        {
            Fact fact;
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            if (!_factMap.TryGetValue(factObject, out fact))
            {
                throw new ArgumentException("Fact for retract does not exist", "factObject");
            }
            _factMap.Remove(factObject);
            _root.PropagateRetract(fact);
        }
    }
}