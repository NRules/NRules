using System;
using System.Collections.Generic;
using System.Linq;

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
            var values = new HashSet<TResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact;
                var value = _selector(source);
                _sourceToValue[source] = value;
                values.Add(value);
            }
            return values.Select(value => AggregationResult.Added(value));
        }

        public IEnumerable<AggregationResult> Modify(IEnumerable<object> facts)
        {
            var valuesToModify = new HashSet<TResult>();
            var valuesToRemove = new HashSet<TResult>();
            var valuesToInsert = new HashSet<TResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact;
                var value = _selector(source);
                var oldValue = (TResult)_sourceToValue[source];
                _sourceToValue[source] = value;

                if (Equals(oldValue, value))
                    valuesToModify.Add(value);
                else
                {
                    valuesToRemove.Add(oldValue);
                    valuesToInsert.Add(value);
                }
            }
            var results = valuesToModify.Select(x=>AggregationResult.Modified(x)).ToList();
            results.AddRange(valuesToRemove.Select(x => AggregationResult.Removed(x)));
            results.AddRange(valuesToInsert.Select(x => AggregationResult.Added(x)));
            return results;
        }

        public IEnumerable<AggregationResult> Remove(IEnumerable<object> facts)
        {
            var oldValues = new List<object>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact;
                var oldValue = _sourceToValue[source];
                _sourceToValue.Remove(source);
                oldValues.Add(oldValue);
            }
            return oldValues.Select(AggregationResult.Removed);
        }

        public IEnumerable<object> Aggregates { get { return _sourceToValue.Values; } }
    }
}