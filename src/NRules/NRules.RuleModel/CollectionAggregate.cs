using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Aggregate that folds matching facts into a collection.
    /// </summary>
    /// <typeparam name="T">Type of facts to collect.</typeparam>
    internal class CollectionAggregate<T> : IAggregate
    {
        private readonly List<T> _items = new List<T>();

        public AggregationResults Initial()
        {
            return AggregationResults.Added;
        }

        public AggregationResults Add(object fact)
        {
            _items.Add((T) fact);
            return AggregationResults.Modified;
        }

        public AggregationResults Modify(object fact)
        {
            return AggregationResults.None;
        }

        public AggregationResults Remove(object fact)
        {
            _items.Remove((T) fact);
            return AggregationResults.Modified;
        }

        public object Result
        {
            get { return _items; }
        }
    }
}