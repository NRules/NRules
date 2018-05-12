using System.Collections.Generic;
using System.Linq;
using NRules.Collections;
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
        private readonly IAggregateExpression _selector;
        private readonly Dictionary<TResult, Counter> _referenceCounter = new Dictionary<TResult, Counter>();
        private readonly Dictionary<IFact, OrderedHashSet<TResult>> _sourceToList = new Dictionary<IFact, OrderedHashSet<TResult>>();

        public FlatteningAggregator(IAggregateExpression selector)
        {
            _selector = selector;
        }

        public IEnumerable<AggregationResult> Add(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var list = new OrderedHashSet<TResult>();
                _sourceToList[fact] = list;
                var value = (IEnumerable<TResult>)_selector.Invoke(context, tuple, fact);
                foreach (var item in value)
                {
                    if (list.Add(item) &&
                        AddRef(item) == 1)
                    {
                        results.Add(AggregationResult.Added(item, Enumerable.Repeat(fact, 1)));
                    }
                }
            }
            return results;
        }

        public IEnumerable<AggregationResult> Modify(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var list = new OrderedHashSet<TResult>();
                var oldList = _sourceToList[fact];
                _sourceToList[fact] = list;
                
                var value = (IEnumerable<TResult>)_selector.Invoke(context, tuple, fact);
                foreach (var item in value)
                {
                    list.Add(item);
                }

                foreach (var item in oldList)
                {
                    if (!list.Contains(item) &&
                        RemoveRef(item) == 0)
                    {
                        results.Add(AggregationResult.Removed(item));
                    }
                }
                foreach (var item in list)
                {
                    if (oldList.Contains(item))
                    {
                        results.Add(AggregationResult.Modified(item, item, Enumerable.Repeat(fact, 1)));
                    }
                    else if (AddRef(item) == 1)
                    {
                        results.Add(AggregationResult.Added(item, Enumerable.Repeat(fact, 1)));
                    }
                }
            }
            return results;
        }

        public IEnumerable<AggregationResult> Remove(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var oldList = _sourceToList[fact];
                _sourceToList.Remove(fact);
                foreach (var item in oldList)
                {
                    if (RemoveRef(item) == 0)
                    {
                        results.Add(AggregationResult.Removed(item));
                    }
                }
            }
            return results;
        }

        private int AddRef(TResult item)
        {
            if (!_referenceCounter.TryGetValue(item, out var counter))
            {
                counter = new Counter();
                _referenceCounter[item] = counter;
            }
            counter.Value++;
            return counter.Value;
        }

        private int RemoveRef(TResult item)
        {
            var counter = _referenceCounter[item];
            counter.Value--;
            return counter.Value;
        }

        private class Counter
        {
            public int Value { get; set; }
        }
    }
}