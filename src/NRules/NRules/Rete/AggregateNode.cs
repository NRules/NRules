using System;
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
                IAggregator aggregator = CreateAggregator(set.Tuple);
                var results = aggregator.Add(set.Tuple, matchingFacts);
                AddAggregationResult(context, aggregation, set.Tuple, results);
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

                IAggregator aggregator = GetAggregator(set.Tuple);
                foreach (var aggregate in aggregator.Aggregates)
                {
                    var aggregateFact = GetAggregateFact(context, aggregate);
                    aggregation.AddUpdate(set.Tuple, aggregateFact);
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
        {
            var aggregation = new Aggregation();
            foreach (var tuple in tuples)
            {
                IAggregator aggregator = GetAggregator(tuple);
                foreach (var aggregate in aggregator.Aggregates)
                {
                    var aggregateFact = GetAggregateFact(context, aggregate);
                    aggregation.AddRetract(tuple, aggregateFact);
                }
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
                    IAggregator aggregator = GetAggregator(set.Tuple);
                    var results = aggregator.Add(set.Tuple, matchingFacts);
                    AddAggregationResult(context, aggregation, set.Tuple, results);
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
                    IAggregator aggregator = GetAggregator(set.Tuple);
                    var results = aggregator.Modify(set.Tuple, matchingFacts);
                    AddAggregationResult(context, aggregation, set.Tuple, results);
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
                    IAggregator aggregator = GetAggregator(set.Tuple);
                    var results = aggregator.Remove(set.Tuple, set.Facts);
                    AddAggregationResult(context, aggregation, set.Tuple, results);
                }
            }
            PropagateAggregation(context, aggregation);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitAggregateNode(context, this);
        }

        private void AddAggregationResult(IExecutionContext context, Aggregation aggregation, Tuple tuple, IEnumerable<AggregationResult> results)
        {
            foreach (var result in results)
            {
                switch (result.Action)
                {
                    case AggregationAction.Added:
                        var addedFact = CreateAggregateFact(context, result.Aggregate);
                        aggregation.Add(result.Action, tuple, addedFact);
                        break;
                    case AggregationAction.Modified:
                        var modifiedFact = GetAggregateFact(context, result.Aggregate);
                        aggregation.Add(result.Action, tuple, modifiedFact);
                        break;
                    case AggregationAction.Removed:
                        var retractedFact = GetAggregateFact(context, result.Aggregate);
                        aggregation.Add(result.Action, tuple, retractedFact);
                        break;
                }
            }
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
                        RemoveAggregateFacts(context, aggregateList);
                        break;
                }
            }
        }
        
        private IAggregator GetAggregator(Tuple tuple)
        {
            var aggregator = tuple.GetState<IAggregator>(this);
            return aggregator;
        }

        private IAggregator CreateAggregator(Tuple tuple)
        {
            var aggregator = _aggregatorFactory.Create();
            tuple.SetState(this, aggregator);
            return aggregator;
        }

        private Fact CreateAggregateFact(IExecutionContext context, object aggregate)
        {
            Fact fact = new Fact(aggregate);
            context.WorkingMemory.AddInternalFact(this, fact);
            return fact;
        }

        private Fact GetAggregateFact(IExecutionContext context, object aggregate)
        {
            Fact fact = context.WorkingMemory.GetInternalFact(this, aggregate);
            if (fact == null)
            {
                throw new InvalidOperationException(
                    $"Fact for aggregate object does not exist. Aggregate={Name}, FactType={aggregate.GetType()}");
            }
            if (!ReferenceEquals(fact.RawObject, aggregate))
            {
                fact.RawObject = aggregate;
                context.WorkingMemory.UpdateInternalFact(this, fact);
            }
            return fact;
        }

        private void RemoveAggregateFacts(IExecutionContext context, TupleFactList tupleFactList)
        {
            var enumerator = tupleFactList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                context.WorkingMemory.RemoveInternalFact(this, enumerator.CurrentFact);
            }
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