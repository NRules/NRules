using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator that projects matching facts into new elements.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TResult">Type of result element.</typeparam>
    internal class ProjectionAggregator<TSource, TResult> : IAggregator
    {
        private readonly IAggregateExpression _selector;
        private readonly Dictionary<IFact, object> _sourceToValue = new Dictionary<IFact, object>();

        public ProjectionAggregator(IAggregateExpression selector)
        {
            _selector = selector;
        }

        public IEnumerable<AggregationResult> Add(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var value = _selector.Invoke(tuple, fact);
                _sourceToValue[fact] = value;
                results.Add(AggregationResult.Added(value));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Modify(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var value = _selector.Invoke(tuple, fact);
                var oldValue = (TResult)_sourceToValue[fact];
                _sourceToValue[fact] = value;
                results.Add(AggregationResult.Modified(value, oldValue));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Remove(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var oldValue = _sourceToValue[fact];
                _sourceToValue.Remove(fact);
                results.Add(AggregationResult.Removed(oldValue));
            }
            return results;
        }

        public IEnumerable<object> Aggregates => _sourceToValue.Values;
    }
}