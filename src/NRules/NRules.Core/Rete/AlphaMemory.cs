using System;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class AlphaMemory : IObjectSink, IObjectMemory
    {
        private readonly List<Fact> _facts = new List<Fact>();
        private readonly List<IObjectSink> _sinks = new List<IObjectSink>();

        public Type FactType { get; private set; }

        public AlphaMemory(Type factType)
        {
            FactType = factType;
        }

        public void PropagateAssert(Fact fact)
        {
            _facts.Add(fact);
            _sinks.ForEach(s => s.PropagateAssert(fact));
        }

        public void PropagateUpdate(Fact fact)
        {
            if (_facts.Contains(fact))
            {
                _sinks.ForEach(s => s.PropagateUpdate(fact));
            }
            else
            {
                PropagateAssert(fact);
            }
        }

        public void PropagateRetract(Fact fact)
        {
            _facts.Remove(fact);
            _sinks.ForEach(s => s.PropagateRetract(fact));
        }

        public IEnumerable<Fact> GetFacts()
        {
            return _facts;
        }

        public void Attach(IObjectSink sink)
        {
            _sinks.Add(sink);
        }
    }
}