using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rules;

namespace NRules.Core.Rete
{
    internal class AggregateNode : BetaNode
    {
        private readonly Dictionary<Tuple, IAggregate> _aggregates = new Dictionary<Tuple, IAggregate>();
        private readonly Dictionary<IAggregate, Fact> _aggregatedFacts = new Dictionary<IAggregate, Fact>();
        private readonly Func<IAggregate> _aggregateFactory;

        public AggregateNode(ITupleMemory leftSource, IObjectMemory rightSource, Type aggregateType)
            : base(leftSource, rightSource)
        {
            _aggregateFactory = () => (IAggregate) Activator.CreateInstance(aggregateType);
        }

        protected override void PropagateMatchedAssert(Tuple leftTuple, Fact rightFact)
        {
            IAggregate aggregate = GetAggregate(leftTuple);
            var result = aggregate.Add(rightFact.Object);
            HandleAggregateResult(result, leftTuple, aggregate);
        }

        protected override void PropagateMatchedUpdate(Tuple leftTuple, Fact rightFact)
        {
            IAggregate aggregate = GetAggregate(leftTuple);
            var result = aggregate.Modify(rightFact.Object);
            HandleAggregateResult(result, leftTuple, aggregate);
        }

        protected override void PropagateMatchedRetract(Tuple leftTuple, Fact rightFact)
        {
            IAggregate aggregate = GetAggregate(leftTuple);
            var result = aggregate.Remove(rightFact.Object);
            HandleAggregateResult(result, leftTuple, aggregate);
        }

        private void HandleAggregateResult(AggregationResults result, Tuple leftTuple, IAggregate aggregate)
        {
            switch (result)
            {
                case AggregationResults.Added:
                    PropagateAggregateAssert(leftTuple, aggregate);
                    break;
                case AggregationResults.Modified:
                    PropagateAggregateUpdate(leftTuple, aggregate);
                    break;
                case AggregationResults.Removed:
                    PropagateAggregateRetract(leftTuple, aggregate);
                    break;
            }
        }

        private void PropagateAggregateAssert(Tuple leftTuple, IAggregate aggregate)
        {
            //todo: register in session fact map
            var fact = new Fact(aggregate.Result);
            _aggregatedFacts[aggregate] = fact;
            var newTuple = CreateTuple(leftTuple, fact);
            Memory.PropagateAssert(newTuple);
        }

        private void PropagateAggregateUpdate(Tuple leftTuple, IAggregate aggregate)
        {
            Fact fact = _aggregatedFacts[aggregate];
            var childTuples = leftTuple.ChildTuples.Where(t => t.RightFact == fact).ToList();
            foreach (var childTuple in childTuples)
            {
                Memory.PropagateUpdate(childTuple);
            }
        }

        private void PropagateAggregateRetract(Tuple leftTuple, IAggregate aggregate)
        {
            Fact fact = _aggregatedFacts[aggregate];
            _aggregatedFacts.Remove(aggregate);
            var childTuples = leftTuple.ChildTuples.Where(t => t.RightFact == fact).ToList();
            foreach (var childTuple in childTuples)
            {
                Memory.PropagateRetract(childTuple);
            }
        }

        private IAggregate GetAggregate(Tuple tuple)
        {
            IAggregate aggregate;
            if (!_aggregates.TryGetValue(tuple, out aggregate))
            {
                aggregate = _aggregateFactory();
                _aggregates[tuple] = aggregate;
            }
            return aggregate;
        }
    }
}