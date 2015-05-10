using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class AggregateNode : BetaNode
    {
        private readonly IAggregateFactory _aggregateFactory;

        public IAggregateFactory AggregateFactory { get { return _aggregateFactory; } }

        public AggregateNode(ITupleSource leftSource, IObjectSource rightSource, IAggregateFactory aggregateFactory)
            : base(leftSource, rightSource)
        {
            _aggregateFactory = aggregateFactory;
        }

        public override void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            IAggregate aggregate = Aggregate(context, tuple);
            var matchingFacts = MatchingFacts(context, tuple);
            foreach (var matchingFact in matchingFacts)
            {
                var results = aggregate.Add(matchingFact.Object);
                HandleAggregateResult(context, results, tuple, aggregate);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            //Do nothing
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            IAggregate aggregate = Aggregate(context, tuple);
            foreach (var result in aggregate.Results)
            {
                Fact aggregateFact = ToAggregateFact(context, result);
                PropagateAggregateRetract(context, tuple, aggregateFact);
            }
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregate aggregate = Aggregate(context, tuple);
                var results = aggregate.Add(fact.Object);
                HandleAggregateResult(context, results, tuple, aggregate);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregate aggregate = Aggregate(context, tuple);
                var results = aggregate.Modify(fact.Object);
                HandleAggregateResult(context, results, tuple, aggregate);
            }
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregate aggregate = Aggregate(context, tuple);
                var results = aggregate.Remove(fact.Object);
                HandleAggregateResult(context, results, tuple, aggregate);
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitAggregateNode(context, this);
        }

        private void HandleAggregateResult(IExecutionContext context, IEnumerable<AggregationResult> results, Tuple leftTuple, IAggregate aggregate)
        {
            foreach (var item in results)
            {
                Fact aggregateFact = ToAggregateFact(context, item.Result);
                switch (item.Action)
                {
                    case AggregationAction.Added:
                        PropagateAggregateAssert(context, leftTuple, aggregateFact);
                        break;
                    case AggregationAction.Modified:
                        PropagateAggregateUpdate(context, leftTuple, aggregateFact);
                        break;
                    case AggregationAction.Removed:
                        PropagateAggregateRetract(context, leftTuple, aggregateFact);
                        break;
                }
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

        private IAggregate Aggregate(IExecutionContext context, Tuple tuple)
        {
            var aggregate = tuple.GetState<IAggregate>(this);
            if (aggregate == null)
            {
                aggregate = _aggregateFactory.Create();
                tuple.SetState(this, aggregate);
                var results = aggregate.Initial();
                HandleAggregateResult(context, results, tuple, aggregate);
            }
            return aggregate;
        }

        private Fact ToAggregateFact(IExecutionContext context, object result)
        {
            if (result == null) return null;
            Fact fact = context.WorkingMemory.GetFact(result);
            if (fact == null)
            {
                fact = new Fact(result);
                context.WorkingMemory.SetFact(fact);
            }
            return fact;
        }
    }
}