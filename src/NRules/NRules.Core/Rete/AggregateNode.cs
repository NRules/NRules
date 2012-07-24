using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rules;

namespace NRules.Core.Rete
{
    internal class AggregateNode : ITupleSink, IObjectMemory
    {
        private readonly List<Fact> _facts = new List<Fact>();
        private IObjectSink _sink;
        private readonly IAggregate _aggregate;

        public AggregateNode(IAggregate aggregate, ITupleMemory source)
        {
            _aggregate = aggregate;
            _aggregate.ResultAdded += AggregateResultAdded;
            _aggregate.ResultModified += AggregateResultModified;
            _aggregate.ResultRemoved += AggregateResultRemoved;

            source.Attach(this);
        }

        private void AggregateResultAdded(object sender, EventArgs e)
        {
            var fact = new Fact(sender);
            _facts.Add(fact);
            _sink.PropagateAssert(fact);
        }

        private void AggregateResultModified(object sender, EventArgs e)
        {
            var fact = _facts.First(f => f.Object == sender);
            _sink.PropagateUpdate(fact);
        }

        private void AggregateResultRemoved(object sender, EventArgs e)
        {
            var fact = _facts.First(f => f.Object == sender);
            _facts.Remove(fact);
            _sink.PropagateRetract(fact);
        }

        public void PropagateAssert(Tuple tuple)
        {
            _aggregate.Add(tuple.GetFactObjects());
        }

        public void PropagateUpdate(Tuple tuple)
        {
            _aggregate.Modify(tuple.GetFactObjects());
        }

        public void PropagateRetract(Tuple tuple)
        {
            _aggregate.Remove(tuple.GetFactObjects());
        }

        public void Attach(IObjectSink sink)
        {
            _sink = sink;
        }

        public IEnumerable<Fact> GetFacts()
        {
            return _facts;
        }
    }
}