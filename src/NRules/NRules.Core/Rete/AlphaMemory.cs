using System;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class AlphaMemory : IObjectSink, IObjectMemory
    {
        private readonly List<Fact> _facts = new List<Fact>();
        private IObjectSink _sink;

        public Type FactType { get; private set; }

        public AlphaMemory(Type factType)
        {
            FactType = factType;
        }

        public void PropagateAssert(Fact fact)
        {
            _facts.Add(fact);
            _sink.PropagateAssert(fact);
        }

        public void PropagateUpdate(Fact fact)
        {
            throw new NotImplementedException();
        }

        public void PropagateRetract(Fact fact)
        {
            _facts.Remove(fact);
            _sink.PropagateRetract(fact);
        }

        public IEnumerable<Fact> GetFacts()
        {
            return _facts;
        }

        public void Attach(IObjectSink sink)
        {
            _sink = sink;
        }
    }
}