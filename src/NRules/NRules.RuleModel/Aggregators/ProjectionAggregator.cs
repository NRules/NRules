using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator that projects matching facts into new elements.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TResult">Type of result element.</typeparam>
    internal class ProjectionAggregator<TSource, TResult> : IAggregator
    {
        private readonly Func<TSource, TResult> _selector;
        private readonly Dictionary<TSource, object> _sourceToValue = new Dictionary<TSource, object>();

        public ProjectionAggregator(Func<TSource, TResult> selector)
        {
            _selector = selector;
        }

        public IEnumerable<AggregationResult> Add(IEnumerable<object> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact;
                var value = _selector(source);
                _sourceToValue[source] = value;
                results.Add(AggregationResult.Added(value));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Modify(IEnumerable<object> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact;
                var value = _selector(source);
                var oldValue = (TResult)_sourceToValue[source];
                _sourceToValue[source] = value;

                if (Equals(oldValue, value))
                {
                    results.Add(AggregationResult.Modified(value));
                }
                else
                {
                    results.Add(AggregationResult.Removed(oldValue));
                    results.Add(AggregationResult.Added(value));
                }
            }
            return results;
        }

        public IEnumerable<AggregationResult> Remove(IEnumerable<object> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact;
                var oldValue = _sourceToValue[source];
                _sourceToValue.Remove(source);
                results.Add(AggregationResult.Removed(oldValue));
            }
            return results;
        }

        public IEnumerable<object> Aggregates { get { return _sourceToValue.Values; } }
    }
}