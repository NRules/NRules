using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator that projects matching facts into new elements.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TElement">Type of projected element.</typeparam>
    internal class ProjectionAggregator<TSource, TElement> : IAggregator
    {
        private readonly Func<TSource, TElement> _selector;
        private readonly Dictionary<TSource, object> _sourceToValue = new Dictionary<TSource, object>();

        public ProjectionAggregator(Func<TSource, TElement> selector)
        {
            _selector = selector;
        }

        public IEnumerable<AggregationResult> Initial()
        {
            return AggregationResult.Empty;
        }

        public IEnumerable<AggregationResult> Add(object fact)
        {
            var source = (TSource) fact;
            var value = _selector(source);
            _sourceToValue[source] = value;
            return new[] { AggregationResult.Added(value) };
        }

        public IEnumerable<AggregationResult> Modify(object fact)
        {
            var source = (TSource)fact;
            var value = _selector(source);
            var oldValue = _sourceToValue[source];

            if (ReferenceEquals(oldValue, value))
                return new[] { AggregationResult.Modified(value) };

            return new[] {AggregationResult.Removed(oldValue), AggregationResult.Added(value)};
        }

        public IEnumerable<AggregationResult> Remove(object fact)
        {
            var source = (TSource)fact;
            var oldValue = _sourceToValue[source];
            _sourceToValue.Remove(source);
            return new[] {AggregationResult.Removed(oldValue)};
        }

        public IEnumerable<object> Aggregates { get { return _sourceToValue.Values; } }
    }
}