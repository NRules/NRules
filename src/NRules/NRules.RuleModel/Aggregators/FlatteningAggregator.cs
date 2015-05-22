using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator that projects each matching fact into a collection and creates a new fact for each element in that collection.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TResult">Type of result element.</typeparam>
    internal class FlatteningAggregator<TSource, TResult> : IAggregator
    {
        private readonly Func<TSource, IEnumerable<TResult>> _selector;
        private readonly Dictionary<TSource, IList<TResult>> _sourceToList = new Dictionary<TSource, IList<TResult>>();

        public FlatteningAggregator(Func<TSource, IEnumerable<TResult>> selector)
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
            var list = value.ToList();
            _sourceToList[source] = list;
            
            return list.Select(x => AggregationResult.Added(x)).ToArray();
        }

        public IEnumerable<AggregationResult> Modify(object fact)
        {
            var source = (TSource)fact;
            var value = _selector(source);
            var list = value.ToList();
            var oldList = _sourceToList[source];
            _sourceToList[source] = list;

            return oldList.Select(x => AggregationResult.Removed(x)).Union(
                list.Select(x => AggregationResult.Added(x))).ToArray();
        }

        public IEnumerable<AggregationResult> Remove(object fact)
        {
            var source = (TSource)fact;
            var oldList = _sourceToList[source];
            _sourceToList.Remove(source);
            return oldList.Select(x => AggregationResult.Removed(x)).ToArray();
        }

        public IEnumerable<object> Aggregates { get { return _sourceToList.Values.SelectMany(x => x).Cast<object>(); } }
    }
}