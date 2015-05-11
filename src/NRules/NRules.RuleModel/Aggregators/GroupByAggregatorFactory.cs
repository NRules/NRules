using System;
using System.Linq.Expressions;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregate factory for group by aggregator.
    /// </summary>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TElement">Type of facts to group.</typeparam>
    internal class GroupByAggregatorFactory<TKey, TElement> : IAggregatorFactory, IEquatable<GroupByAggregatorFactory<TKey, TElement>>
    {
        private readonly Expression<Func<TElement, TKey>> _keySelectorExpression;
        private readonly Func<TElement, TKey> _keySelector;

        public GroupByAggregatorFactory(Expression<Func<TElement, TKey>> keySelectorExpression)
        {
            _keySelectorExpression = keySelectorExpression;
            _keySelector = keySelectorExpression.Compile();
        }

        public IAggregator Create()
        {
            return new GroupByAggregator<TKey, TElement>(_keySelector);
        }

        public bool Equals(GroupByAggregatorFactory<TKey, TElement> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_keySelectorExpression.ToString(), other._keySelectorExpression.ToString());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GroupByAggregatorFactory<TKey, TElement>)obj);
        }

        public override int GetHashCode()
        {
            return _keySelectorExpression.GetHashCode();
        }
    }
}