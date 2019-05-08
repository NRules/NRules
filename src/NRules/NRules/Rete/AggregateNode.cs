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
        public ExpressionCollection Expressions { get; }

        public AggregateNode(ITupleSource leftSource, IObjectSource rightSource, string name, ExpressionCollection expressions, IAggregatorFactory aggregatorFactory, bool isSubnetJoin)
            : base(leftSource, rightSource)
        {
            Name = name;
            Expressions = expressions;
            _aggregatorFactory = aggregatorFactory;
            _isSubnetJoin = isSubnetJoin;
        }

        public override void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
        {
            var aggregationContext = new AggregationContext(context.Session, context.EventAggregator, NodeInfo);
            var joinedSets = JoinedSets(context, tuples);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                var matchingFacts = set.Facts;
                IFactAggregator aggregator = CreateFactAggregator(set.Tuple);
                AddToAggregate(aggregationContext, aggregator, aggregation, set.Tuple, matchingFacts);
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
        {
            var aggregationContext = new AggregationContext(context.Session, context.EventAggregator, NodeInfo);
            var joinedSets = JoinedSets(context, tuples);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                IFactAggregator aggregator = GetFactAggregator(set.Tuple);
                if (aggregator != null)
                {
                    if (_isSubnetJoin && set.Facts.Count > 0)
                    {
                        //Update already propagated from the right
                        continue;
                    }
                    var matchingFacts = set.Facts;
                    UpdateInAggregate(aggregationContext, aggregator, aggregation, set.Tuple, matchingFacts);
                }
                else
                {
                    var matchingFacts = set.Facts;
                    aggregator = CreateFactAggregator(set.Tuple);
                    AddToAggregate(aggregationContext, aggregator, aggregation, set.Tuple, matchingFacts);
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
        {
            var aggregation = new Aggregation();
            foreach (var tuple in tuples)
            {
                IFactAggregator aggregator = RemoveFactAggregator(tuple);
                if (aggregator != null)
                {
                    aggregation.Remove(tuple, aggregator.AggregateFacts);
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateAssert(IExecutionContext context, List<Fact> facts)
        {
            var aggregationContext = new AggregationContext(context.Session, context.EventAggregator, NodeInfo);
            var joinedSets = JoinedSets(context, facts);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                if (set.Facts.Count == 0) continue;
                var matchingFacts = set.Facts;
                if (matchingFacts.Count > 0)
                {
                    IFactAggregator aggregator = GetFactAggregator(set.Tuple);
                    if (aggregator == null)
                    {
                        aggregator = CreateFactAggregator(set.Tuple);

                        var originalSet = JoinedSet(context, set.Tuple);
                        var matchingOriginalFacts = originalSet.Facts;
                        AddToAggregate(aggregationContext, aggregator, aggregation, originalSet.Tuple, matchingOriginalFacts);
                    }

                    AddToAggregate(aggregationContext, aggregator, aggregation, set.Tuple, matchingFacts);
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateUpdate(IExecutionContext context, List<Fact> facts)
        {
            var aggregationContext = new AggregationContext(context.Session, context.EventAggregator, NodeInfo);
            var joinedSets = JoinedSets(context, facts);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                if (set.Facts.Count == 0) continue;
                var matchingFacts = set.Facts;
                if (matchingFacts.Count > 0)
                {
                    IFactAggregator aggregator = GetFactAggregator(set.Tuple);
                    if (aggregator != null)
                    {
                        UpdateInAggregate(aggregationContext, aggregator, aggregation, set.Tuple, matchingFacts);
                    }
                    else
                    {
                        var fullSet = JoinedSet(context, set.Tuple);
                        var allMatchingFacts = fullSet.Facts;
                        aggregator = CreateFactAggregator(fullSet.Tuple);
                        AddToAggregate(aggregationContext, aggregator, aggregation, fullSet.Tuple, allMatchingFacts);
                    }
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateRetract(IExecutionContext context, List<Fact> facts)
        {
            var aggregationContext = new AggregationContext(context.Session, context.EventAggregator, NodeInfo);
            var joinedSets = JoinedSets(context, facts);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                if (set.Facts.Count == 0) continue;
                var matchingFacts = set.Facts;
                if (matchingFacts.Count > 0)
                {
                    IFactAggregator aggregator = GetFactAggregator(set.Tuple);
                    if (aggregator != null)
                    {
                        RetractFromAggregate(aggregationContext, aggregator, aggregation, set.Tuple, set.Facts);
                    }
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitAggregateNode(context, this);
        }

        private void AddToAggregate(AggregationContext context, IFactAggregator aggregator, Aggregation aggregation, Tuple tuple, IList<Fact> facts)
        {
            try
            {
                aggregator.Add(context, aggregation, tuple, facts);
            }
            catch (ExpressionEvaluationException e)
            {
                if (!e.IsHandled)
                {
                    throw new RuleLhsExpressionEvaluationException("Failed to evaluate aggregate expression",
                        e.Expression.ToString(), e.InnerException);
                }
                ResetAggregator(aggregation, tuple, aggregator);
            }
        }

        private void UpdateInAggregate(AggregationContext context, IFactAggregator aggregator, Aggregation aggregation, Tuple tuple, IList<Fact> facts)
        {
            try
            {
                aggregator.Modify(context, aggregation, tuple, facts);
            }
            catch (ExpressionEvaluationException e)
            {
                if (!e.IsHandled)
                {
                    throw new RuleLhsExpressionEvaluationException("Failed to evaluate aggregate expression",
                        e.Expression.ToString(), e.InnerException);
                }
                ResetAggregator(aggregation, tuple, aggregator);
            }
        }

        private void RetractFromAggregate(AggregationContext context, IFactAggregator aggregator, Aggregation aggregation, Tuple tuple, List<Fact> facts)
        {
            try
            {
                aggregator.Remove(context, aggregation, tuple, facts);
            }
            catch (ExpressionEvaluationException e)
            {
                if (!e.IsHandled)
                {
                    throw new RuleLhsExpressionEvaluationException("Failed to evaluate aggregate expression",
                        e.Expression.ToString(), e.InnerException);
                }
                ResetAggregator(aggregation, tuple, aggregator);
            }
        }

        private void ResetAggregator(Aggregation aggregation, Tuple tuple, IFactAggregator aggregator)
        {
            tuple.RemoveState<IFactAggregator>(this);
            aggregation.Remove(tuple, aggregator.AggregateFacts);
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
    }
}