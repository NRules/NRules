using System;
using System.Collections.Generic;
using NRules.Aggregators;
using NRules.Collections;

namespace NRules.Rete
{
    internal interface IFactAggregator
    {
        void Add(AggregationContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts);
        void Modify(AggregationContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts);
        void Remove(AggregationContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts);
        IEnumerable<Fact> AggregateFacts { get; }
    }

    internal class FactAggregator : IFactAggregator
    {
        private readonly IAggregator _aggregator;
        private readonly OrderedDictionary<object, Fact> _aggregateFactMap = new OrderedDictionary<object, Fact>();

        public FactAggregator(IAggregator aggregator)
        {
            _aggregator = aggregator;
        }

        public IEnumerable<Fact> AggregateFacts => _aggregateFactMap.Values;
        
        public void Add(AggregationContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
        {
            var results = _aggregator.Add(context, tuple, facts);
            AddAggregationResult(aggregation, tuple, results);
        }

        public void Modify(AggregationContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
        {
            var results = _aggregator.Modify(context, tuple, facts);
            AddAggregationResult(aggregation, tuple, results);
        }

        public void Remove(AggregationContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
        {
            var results = _aggregator.Remove(context, tuple, facts);
            AddAggregationResult(aggregation, tuple, results);
        }

        private void AddAggregationResult(Aggregation aggregation, Tuple tuple, IEnumerable<AggregationResult> results)
        {
            foreach (var result in results)
            {
                switch (result.Action)
                {
                    case AggregationAction.Added:
                        var addedFact = CreateAggregateFact(result);
                        aggregation.Add(tuple, addedFact);
                        break;
                    case AggregationAction.Modified:
                        var modifiedFact = GetAggregateFact(result);
                        aggregation.Modify(tuple, modifiedFact);
                        break;
                    case AggregationAction.Removed:
                        var removedFact = RemoveAggregateFact(result);
                        aggregation.Remove(tuple, removedFact);
                        break;
                }
            }
        }

        private Fact CreateAggregateFact(AggregationResult result)
        {
            var fact = new SyntheticFact(result.Aggregate);
            fact.Source = new AggregateFactSource(result.Source);
            _aggregateFactMap.Add(result.Aggregate, fact);
            return fact;
        }

        private Fact GetAggregateFact(AggregationResult result)
        {
            if (!_aggregateFactMap.TryGetValue(result.Previous ?? result.Aggregate, out var fact))
            {
                throw new InvalidOperationException(
                    $"Fact for aggregate object does not exist. AggregatorType={_aggregator.GetType()}, FactType={result.Aggregate.GetType()}");
            }

            fact.Source = new AggregateFactSource(result.Source);
            if (!ReferenceEquals(fact.RawObject, result.Aggregate))
            {
                _aggregateFactMap.Remove(fact.RawObject);
                fact.RawObject = result.Aggregate;
                _aggregateFactMap.Add(fact.RawObject, fact);
            }
            return fact;
        }

        private Fact RemoveAggregateFact(AggregationResult result)
        {
            if (!_aggregateFactMap.TryGetValue(result.Aggregate, out var fact))
            {
                throw new InvalidOperationException(
                    $"Fact for aggregate object does not exist. AggregatorType={_aggregator.GetType()}, FactType={result.Aggregate.GetType()}");
            }

            _aggregateFactMap.Remove(fact.RawObject);
            fact.RawObject = result.Aggregate;
            fact.Source = null;
            return fact;
        }
    }
}