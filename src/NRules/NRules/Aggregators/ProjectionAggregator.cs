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
        private readonly Dictionary<TSource, object> _sourceToValue = new Dictionary<TSource, object>();

        public ProjectionAggregator(IAggregateExpression selector)
        {
            _selector = selector;
        }

        public IEnumerable<AggregationResult> Add(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var value = _selector.Invoke(tuple, fact);
                _sourceToValue[source] = value;
                results.Add(AggregationResult.Added(value));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Modify(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var value = _selector.Invoke(tuple, fact);
                var oldValue = (TResult)_sourceToValue[source];
                _sourceToValue[source] = value;
                results.Add(AggregationResult.Modified(value, oldValue));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Remove(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var oldValue = _sourceToValue[source];
                _sourceToValue.Remove(source);
                results.Add(AggregationResult.Removed(oldValue));
            }
            return results;
        }

        public IEnumerable<object> Aggregates => _sourceToValue.Values;
    }
}