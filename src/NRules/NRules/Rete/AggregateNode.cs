using System.Collections.Generic;
using NRules.Aggregators;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class AggregateNode : BinaryBetaNode
    {
        private readonly IAggregatorFactory _aggregatorFactory;
        private readonly bool _isSubnetJoin;

        public string Name { get; }
        public ExpressionMap ExpressionMap { get; }

        public AggregateNode(ITupleSource leftSource, IObjectSource rightSource, string name, ExpressionMap expressionMap, IAggregatorFactory aggregatorFactory, bool isSubnetJoin)
            : base(leftSource, rightSource)
        {
            Name = name;
            ExpressionMap = expressionMap;
            _aggregatorFactory = aggregatorFactory;
            _isSubnetJoin = isSubnetJoin;
        }

        public override void PropagateAssert(IExecutionContext context, IList<Tuple> tuples)
        {
            var joinedSets = JoinedSets(context, tuples);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                var matchingFacts = new List<Fact>();
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        matchingFacts.Add(fact);
                }
                IFactAggregator aggregator = CreateFactAggregator(set.Tuple);
                aggregator.Add(context, aggregation, set.Tuple, matchingFacts);
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples)
        {
            var joinedSets = JoinedSets(context, tuples);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                if (_isSubnetJoin && HasRightFacts(context, set))
                {
                    //Update already propagated from the right
                    continue;
                }

                IFactAggregator aggregator = GetFactAggregator(set.Tuple);
                aggregator.Modify(context, aggregation, set.Tuple);
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
        {
            var aggregation = new Aggregation();
            foreach (var tuple in tuples)
            {
                IFactAggregator aggregator = RemoveFactAggregator(tuple);
                aggregator.Remove(context, aggregation, tuple);
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateAssert(IExecutionContext context, IList<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                if (set.Facts.Count == 0) continue;
                var matchingFacts = new List<Fact>();
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        matchingFacts.Add(fact);
                }
                if (matchingFacts.Count > 0)
                {
                    IFactAggregator aggregator = GetFactAggregator(set.Tuple);
                    aggregator.Add(context, aggregation, set.Tuple, matchingFacts);
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateUpdate(IExecutionContext context, IList<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                if (set.Facts.Count == 0) continue;
                var matchingFacts = new List<Fact>();
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        matchingFacts.Add(fact);
                }
                if (matchingFacts.Count > 0)
                {
                    IFactAggregator aggregator = GetFactAggregator(set.Tuple);
                    aggregator.Modify(context, aggregation, set.Tuple, matchingFacts);
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateRetract(IExecutionContext context, IList<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                if (set.Facts.Count == 0) continue;
                var matchingFacts = new List<Fact>();
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        matchingFacts.Add(fact);
                }
                if (matchingFacts.Count > 0)
                {
                    IFactAggregator aggregator = GetFactAggregator(set.Tuple);
                    aggregator.Remove(context, aggregation, set.Tuple, set.Facts);
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitAggregateNode(context, this);
        }

        private void PropagateAggregation(IExecutionContext context, Aggregation aggregation)
        {
            foreach (var aggregateList in aggregation.AggregateLists)
            {
                if (aggregateList.Count == 0) continue;

                switch (aggregateList.Action)
                {
                    case AggregationAction.Added:
                        MemoryNode.PropagateAssert(context, aggregateList);
                        break;
                    case AggregationAction.Modified:
                        MemoryNode.PropagateUpdate(context, aggregateList);
                        break;
                    case AggregationAction.Removed:
                        MemoryNode.PropagateRetract(context, aggregateList);
                        break;
                }
            }
        }

        private IFactAggregator CreateFactAggregator(Tuple tuple)
        {
            var aggregator = _aggregatorFactory.Create();
            var factAggregator = new FactAggregator(aggregator);
            tuple.SetState(this, factAggregator);
            return factAggregator;
        }

        private IFactAggregator GetFactAggregator(Tuple tuple)
        {
            var factAggregator = tuple.GetState<IFactAggregator>(this);
            return factAggregator;
        }

        private IFactAggregator RemoveFactAggregator(Tuple tuple)
        {
            var factAggregator = tuple.RemoveState<IFactAggregator>(this);
            return factAggregator;
        }

        private bool HasRightFacts(IExecutionContext context, TupleFactSet set)
        {
            foreach (var fact in set.Facts)
            {
                if (MatchesConditions(context, set.Tuple, fact))
                {
                    return true;
                }
            }
            return false;
        }
    }
}