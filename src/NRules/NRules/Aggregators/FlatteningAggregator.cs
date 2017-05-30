using System;
using System.Collections.Generic;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Aggregators
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

        public IEnumerable<AggregationResult> Add(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var value = _selector(source);
                var list = new List<TResult>(value);
                _sourceToList[source] = list;
                foreach (var item in list)
                {
                    results.Add(AggregationResult.Added(item));
                }
            }
            return results;
        }

        public IEnumerable<AggregationResult> Modify(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var value = _selector(source);
                var list = new List<TResult>(value);
                var oldList = _sourceToList[source];
                _sourceToList[source] = list;
                foreach (var item in oldList)
                {
                    results.Add(AggregationResult.Removed(item));
                }
                foreach (var item in list)
                {
                    results.Add(AggregationResult.Added(item));
                }
            }
            return results;
        }

        public IEnumerable<AggregationResult> Remove(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var oldList = _sourceToList[source];
                _sourceToList.Remove(source);
                foreach (var item in oldList)
                {
                    results.Add(AggregationResult.Removed(item));
                }
            }
            return results;
        }

        public IEnumerable<object> Aggregates
        {
            get
            {
                foreach (var itemList in _sourceToList)
                {
                    foreach (var item in itemList.Value)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}