using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<AggregationResult> Add(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var value = _selector.Invoke(context, tuple, fact);
                _sourceToValue[fact] = value;
                results.Add(AggregationResult.Added(value, Enumerable.Repeat(fact, 1)));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Modify(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var value = _selector.Invoke(context, tuple, fact);
                var oldValue = (TResult)_sourceToValue[fact];
                _sourceToValue[fact] = value;
                results.Add(AggregationResult.Modified(value, oldValue, Enumerable.Repeat(fact, 1)));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Remove(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
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
    }
}