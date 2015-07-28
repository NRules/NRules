using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class AggregateNode : BetaNode
    {
        private readonly string _name;
        private readonly ExpressionMap _expressionMap;
        private readonly IAggregatorFactory _aggregatorFactory;

        public string Name { get { return _name; } }

        public ExpressionMap ExpressionMap
        {
            get { return _expressionMap; }
        }

        public AggregateNode(ITupleSource leftSource, IObjectSource rightSource, string name, ExpressionMap expressionMap, IAggregatorFactory aggregatorFactory)
            : base(leftSource, rightSource)
        {
            _name = name;
            _expressionMap = expressionMap;
            _aggregatorFactory = aggregatorFactory;
        }

        public override void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            IAggregator aggregator = Aggregator(context, tuple);
            var matchingFacts = MatchingFacts(context, tuple);
            foreach (var matchingFact in matchingFacts)
            {
                var results = aggregator.Add(matchingFact.Object);
                HandleAggregationResult(context, results, tuple);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            //Do nothing
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            IAggregator aggregator = Aggregator(context, tuple);
            foreach (var aggregate in aggregator.Aggregates)
            {
                Fact aggregateFact = ToAggregateFact(context, aggregate);
                PropagateAggregateRetract(context, tuple, aggregateFact);
            }
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregator aggregator = Aggregator(context, tuple);
                var results = aggregator.Add(fact.Object);
                HandleAggregationResult(context, results, tuple);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregator aggregator = Aggregator(context, tuple);
                var results = aggregator.Modify(fact.Object);
                HandleAggregationResult(context, results, tuple);
            }
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            var tuples = MatchingTuples(context, fact);
            foreach (var tuple in tuples)
            {
                IAggregator aggregator = Aggregator(context, tuple);
                var results = aggregator.Remove(fact.Object);
                HandleAggregationResult(context, results, tuple);
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitAggregateNode(context, this);
        }

        private void HandleAggregationResult(IExecutionContext context, IEnumerable<AggregationResult> results, Tuple leftTuple)
        {
            foreach (var result in results)
            {
                Fact aggregateFact = ToAggregateFact(context, result.Aggregate);
                switch (result.Action)
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
                context.WorkingMemory.RemoveInternalFact(this, aggregateFact);
            }
        }

        private IAggregator Aggregator(IExecutionContext context, Tuple tuple)
        {
            var aggregator = tuple.GetState<IAggregator>(this);
            if (aggregator == null)
            {
                aggregator = _aggregatorFactory.Create();
                tuple.SetState(this, aggregator);
                var results = aggregator.Initial();
                HandleAggregationResult(context, results, tuple);
            }
            return aggregator;
        }

        private Fact ToAggregateFact(IExecutionContext context, object aggregate)
        {
            if (aggregate == null) return null;
            Fact fact = context.WorkingMemory.GetInternalFact(this, aggregate);
            if (fact == null)
            {
                fact = new Fact(aggregate);
                context.WorkingMemory.SetInternalFact(this, fact);
            }
            return fact;
        }
    }
}