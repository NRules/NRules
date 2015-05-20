using System;
using System.Linq.Expressions;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator factory for group by aggregator.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements to group.</typeparam>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TValue">Type of values to group.</typeparam>
    internal class GroupByAggregatorFactory<TSource, TKey, TValue> : IAggregatorFactory, IEquatable<GroupByAggregatorFactory<TSource, TKey, TValue>>
    {
        private readonly Expression<Func<TSource, TKey>> _keySelectorExpression;
        private readonly Expression<Func<TSource, TValue>> _valueSelectorExpression;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TValue> _valueSelector;

        public GroupByAggregatorFactory(Expression<Func<TSource, TKey>> keySelectorExpression, Expression<Func<TSource, TValue>> valueSelectorExpression)
        {
            _keySelectorExpression = keySelectorExpression;
            _valueSelectorExpression = valueSelectorExpression;
            _keySelector = keySelectorExpression.Compile();
            _valueSelector = valueSelectorExpression.Compile();
        }

        public IAggregator Create()
        {
            return new GroupByAggregator<TSource, TKey, TValue>(_keySelector, _valueSelector);
        }

        public bool Equals(GroupByAggregatorFactory<TSource, TKey, TValue> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _keySelectorExpression.Equals(other._keySelectorExpression) && _valueSelectorExpression.Equals(other._valueSelectorExpression);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GroupByAggregatorFactory<TSource, TKey, TValue>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_keySelectorExpression.GetHashCode() * 397) ^ _valueSelectorExpression.GetHashCode();
            }
        }
    }
}