using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class AggregateNode : BetaNode
    {
        private readonly string _name;
        private readonly ExpressionMap _expressionMap;
        private readonly IAggregatorFactory _aggregatorFactory;
        private readonly bool _isSubnetJoin;

        public string Name { get { return _name; } }

        public ExpressionMap ExpressionMap
        {
            get { return _expressionMap; }
        }

        public AggregateNode(ITupleSource leftSource, IObjectSource rightSource, string name, ExpressionMap expressionMap, IAggregatorFactory aggregatorFactory, bool isSubnetJoin)
            : base(leftSource, rightSource)
        {
            _name = name;
            _expressionMap = expressionMap;
            _aggregatorFactory = aggregatorFactory;
            _isSubnetJoin = isSubnetJoin;
        }

        public override void PropagateAssert(IExecutionContext context, IList<Tuple> tuples)
        {
            var joinedSets = JoinedSets(context, tuples);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                var factObjects = new List<object>();
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        factObjects.Add(fact.Object);
                }
                IAggregator aggregator = CreateAggregator(set.Tuple);
                var results = aggregator.Add(factObjects);
                aggregation.Add(set.Tuple, results);
            }
            PropagateAggregation(context, aggregation);
        }

        public override void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples)
        {
            if (_isSubnetJoin) return;

            var toUpdate = new TupleFactList();
            foreach (var tuple in tuples)
            {
                IAggregator aggregator = GetAggregator(tuple);
                foreach (var aggregate in aggregator.Aggregates)
                {
                    Fact aggregateFact = ToAggregateFact(context, aggregate);
                    toUpdate.Add(tuple, aggregateFact);
                }
            }
            MemoryNode.PropagateUpdate(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
        {
            var toRetract = new TupleFactList();
            foreach (var tuple in tuples)
            {
                IAggregator aggregator = GetAggregator(tuple);
                foreach (var aggregate in aggregator.Aggregates)
                {
                    Fact aggregateFact = ToAggregateFact(context, aggregate);
                    toRetract.Add(tuple, aggregateFact);
                }
            }
            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void PropagateAssert(IExecutionContext context, IList<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var aggregation = new Aggregation();
            foreach (var set in joinedSets)
            {
                if (set.Facts.Count == 0) continue;
                var factObjects = new List<object>();
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        factObjects.Add(fact.Object);
                }
                if (factObjects.Count > 0)
                {
                    IAggregator aggregator = GetAggregator(set.Tuple);
                    var results = aggregator.Add(factObjects);
                    aggregation.Add(set.Tuple, results);
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
                var factObjects = new List<object>();
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        factObjects.Add(fact.Object);
                }
                if (factObjects.Count > 0)
                {
                    IAggregator aggregator = GetAggregator(set.Tuple);
                    var results = aggregator.Modify(factObjects);
                    aggregation.Add(set.Tuple, results);
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
                var factObjects = new List<object>();
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        factObjects.Add(fact.Object);
                }
                if (factObjects.Count > 0)
                {
                    IAggregator aggregator = GetAggregator(set.Tuple);
                    var results = aggregator.Remove(factObjects);
                    aggregation.Add(set.Tuple, results);
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
            PropagateAggregateRetracts(context, aggregation);
            PropagateAggregateAsserts(context, aggregation);
            PropagateAggregateUpdates(context, aggregation);
        }

        private void PropagateAggregateAsserts(IExecutionContext context, Aggregation aggregation)
        {
            var asserts = new TupleFactList();
            foreach (var assert in aggregation.Asserts)
            {
                var fact = ToAggregateFact(context, assert.ResultObject);
                asserts.Add(assert.Tuple, fact);
            }
            if (asserts.Count > 0)
            {
                MemoryNode.PropagateAssert(context, asserts);
            }
        }

        private void PropagateAggregateUpdates(IExecutionContext context, Aggregation aggregation)
        {
            var updates = new TupleFactList();
            foreach (var update in aggregation.Updates)
            {
                var fact = ToAggregateFact(context, update.ResultObject);
                updates.Add(update.Tuple, fact);
            }
            if (updates.Count > 0)
            {
                MemoryNode.PropagateUpdate(context, updates);
            }
        }

        private void PropagateAggregateRetracts(IExecutionContext context, Aggregation aggregation)
        {
            var retracts = new TupleFactList();
            foreach (var retract in aggregation.Retracts)
            {
                var fact = ToAggregateFact(context, retract.ResultObject);
                retracts.Add(retract.Tuple, fact);
            }
            if (retracts.Count > 0)
            {
                MemoryNode.PropagateRetract(context, retracts);
                var enumerator = retracts.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    context.WorkingMemory.RemoveInternalFact(this, enumerator.CurrentFact);
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

        private Fact ToAggregateFact(IExecutionContext context, object aggregate)
        {
            Fact fact = context.WorkingMemory.GetInternalFact(this, aggregate);
            if (fact == null)
            {
                fact = new Fact(aggregate);
                context.WorkingMemory.AddInternalFact(this, fact);
            }
            else if (!ReferenceEquals(fact.RawObject, aggregate))
            {
                fact.RawObject = aggregate;
                context.WorkingMemory.UpdateInternalFact(this, fact);
            }
            return fact;
        }
    }
}