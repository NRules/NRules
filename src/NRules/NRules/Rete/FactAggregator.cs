using System;
using System.Collections.Generic;
using NRules.Aggregators;

namespace NRules.Rete
{
    internal interface IFactAggregator
    {
        void Add(IExecutionContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts);
        void Modify(IExecutionContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts);
        void Modify(IExecutionContext context, Aggregation aggregation, Tuple tuple);
        void Remove(IExecutionContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts);
        void Remove(IExecutionContext context, Aggregation aggregation, Tuple tuple);
    }

    internal class FactAggregator : IFactAggregator
    {
        private readonly IAggregator _aggregator;
        private readonly Dictionary<object, Fact> _aggregateFactMap = new Dictionary<object, Fact>();

        public FactAggregator(IAggregator aggregator)
        {
            _aggregator = aggregator;
        }

        public void Add(IExecutionContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
        {
            IEnumerable<AggregationResult> results;
            try
            {
                results = _aggregator.Add(tuple, facts);
            }
            catch (AggregateExpressionException e)
            {
                context.EventAggregator.RaiseAggregateFailed(context.Session, e.InnerException, e.Expression, e.Tuple, e.Fact);
                throw new RuleExpressionEvaluationException("Failed to evaluate aggregate expression", e.Expression.ToString(), e.InnerException);
            }
            AddAggregationResult(aggregation, tuple, results);
        }

        public void Modify(IExecutionContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
        {
            IEnumerable<AggregationResult> results;
            try
            {
                results = _aggregator.Modify(tuple, facts);
            }
            catch (AggregateExpressionException e)
            {
                context.EventAggregator.RaiseAggregateFailed(context.Session, e.InnerException, e.Expression, e.Tuple, e.Fact);
                throw new RuleExpressionEvaluationException("Failed to evaluate aggregate expression", e.Expression.ToString(), e.InnerException);
            }
            AddAggregationResult(aggregation, tuple, results);
        }

        public void Modify(IExecutionContext context, Aggregation aggregation, Tuple tuple)
        {
            foreach (var aggregate in _aggregator.Aggregates)
            {
                var aggregateFact = GetAggregateFact(aggregate);
                aggregation.Modify(tuple, aggregateFact);
            }
        }

        public void Remove(IExecutionContext context, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
        {
            IEnumerable<AggregationResult> results;
            try
            {
                results = _aggregator.Remove(tuple, facts);
            }
            catch (AggregateExpressionException e)
            {
                context.EventAggregator.RaiseAggregateFailed(context.Session, e.InnerException, e.Expression, e.Tuple, e.Fact);
                throw new RuleExpressionEvaluationException("Failed to evaluate aggregate expression", e.Expression.ToString(), e.InnerException);
            }
            AddAggregationResult(aggregation, tuple, results);
        }

        public void Remove(IExecutionContext context, Aggregation aggregation, Tuple tuple)
        {
            foreach (var aggregate in _aggregator.Aggregates)
            {
                var aggregateFact = RemoveAggregateFact(aggregate);
                aggregation.Remove(tuple, aggregateFact);
            }
        }

        private void AddAggregationResult(Aggregation aggregation, Tuple tuple, IEnumerable<AggregationResult> results)
        {
            foreach (var result in results)
            {
                switch (result.Action)
                {
                    case AggregationAction.Added:
                        var addedFact = CreateAggregateFact(result.Aggregate);
                        aggregation.Add(tuple, addedFact);
                        break;
                    case AggregationAction.Modified:
                        var modifiedFact = GetAggregateFact(result.Aggregate, result.Previous);
                        aggregation.Modify(tuple, modifiedFact);
                        break;
                    case AggregationAction.Removed:
                        var removedFact = RemoveAggregateFact(result.Aggregate);
                        aggregation.Remove(tuple, removedFact);
                        break;
                }
            }
        }

        private Fact CreateAggregateFact(object aggregate)
        {
            Fact fact = new Fact(aggregate);
            _aggregateFactMap.Add(aggregate, fact);
            return fact;
        }

        private Fact GetAggregateFact(object aggregate, object previous = null)
        {
            if (!_aggregateFactMap.TryGetValue(previous ?? aggregate, out var fact))
            {
                throw new InvalidOperationException(
                    $"Fact for aggregate object does not exist. AggregatorTye={_aggregator.GetType()}, FactType={aggregate.GetType()}");
            }

            if (!ReferenceEquals(fact.RawObject, aggregate))
            {
                _aggregateFactMap.Remove(fact.RawObject);
                fact.RawObject = aggregate;
                _aggregateFactMap.Add(fact.RawObject, fact);
            }
            return fact;
        }

        private Fact RemoveAggregateFact(object aggregate)
        {
            if (!_aggregateFactMap.TryGetValue(aggregate, out var fact))
            {
                throw new InvalidOperationException(
                    $"Fact for aggregate object does not exist. AggregatorTye={_aggregator.GetType()}, FactType={aggregate.GetType()}");
            }

            _aggregateFactMap.Remove(fact.RawObject);
            fact.RawObject = aggregate;
            return fact;
        }
    }
}