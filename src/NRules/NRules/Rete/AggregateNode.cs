using System;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class AggregateNode : BetaNode
    {
        private readonly Func<IAggregate> _aggregateFactory;

        public Type AggregateType { get; private set; }

        public AggregateNode(ITupleSource leftSource, IObjectSource rightSource, Type aggregateType)
            : base(leftSource, rightSource)
        {
            AggregateType = aggregateType;
            _aggregateFactory = () => (IAggregate) Activator.CreateInstance(aggregateType);
        }

        public override void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(tuple);
            var matchingFacts = MatchingFacts(context, tuple);
            foreach (var matchingFact in matchingFacts)
            {
                var result = aggregate.Add(matchingFact.Object);
                HandleAggregateResult(context, result, tuple, aggregate);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(tuple);
            Fact aggregateFact = context.WorkingMemory.GetFact(aggregate.Result);
            PropagateAggregateUpdate(context, tuple, aggregateFact);
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(tuple);
            Fact aggregateFact = context.WorkingMemory.GetFact(aggregate.Result);
            PropagateAggregateRetract(context, tuple, aggregateFact);
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregate aggregate = GetAggregate(tuple);
                var result = aggregate.Add(fact.Object);
                HandleAggregateResult(context, result, tuple, aggregate);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregate aggregate = GetAggregate(tuple);
                var result = aggregate.Modify(fact.Object);
                HandleAggregateResult(context, result, tuple, aggregate);
            }
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregate aggregate = GetAggregate(tuple);
                var result = aggregate.Remove(fact.Object);
                HandleAggregateResult(context, result, tuple, aggregate);
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitAggregateNode(context, this);
        }

        private void HandleAggregateResult(IExecutionContext context, AggregationResults result, Tuple leftTuple, IAggregate aggregate)
        {
            Fact aggregateFact = GetAggregateFact(context, aggregate);
            switch (result)
            {
                case AggregationResults.Added:
                    PropagateAggregateAssert(context, leftTuple, aggregateFact);
                    break;
                case AggregationResults.Modified:
                    PropagateAggregateUpdate(context, leftTuple, aggregateFact);
                    break;
                case AggregationResults.Removed:
                    PropagateAggregateRetract(context, leftTuple, aggregateFact);
                    break;
            }
        }

        private void PropagateAggregateAssert(IExecutionContext context, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                MemoryNode.PropagateAssert(context, tuple, aggregateFact);
            }
        }

        private void PropagateAggregateUpdate(IExecutionContext context, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                MemoryNode.PropagateUpdate(context, tuple, aggregateFact);
            }
        }

        private void PropagateAggregateRetract(IExecutionContext context, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                MemoryNode.PropagateRetract(context, tuple, aggregateFact);
            }
        }
        
        private IAggregate GetAggregate(Tuple tuple)
        {
            var aggregate = tuple.GetState<IAggregate>();
            if (aggregate == null)
            {
                aggregate = _aggregateFactory();
                tuple.SetState(aggregate);
            }
            return aggregate;
        }

        private Fact GetAggregateFact(IExecutionContext context, IAggregate aggregate)
        {
            if (aggregate.Result == null) return null;
            Fact fact = context.WorkingMemory.GetFact(aggregate.Result);
            if (fact == null)
            {
                fact = new Fact(aggregate.Result);
                context.WorkingMemory.SetFact(fact);
            }
            return fact;
        }
    }
}