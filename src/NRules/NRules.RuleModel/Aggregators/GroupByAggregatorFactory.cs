using System;
using System.Linq.Expressions;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregate factory for group by aggregator.
    /// </summary>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TValue">Type of values to group.</typeparam>
    /// <typeparam name="TFact">Type of facts to group.</typeparam>
    internal class GroupByAggregatorFactory<TKey, TValue, TFact> : IAggregatorFactory, IEquatable<GroupByAggregatorFactory<TKey, TValue, TFact>>
    {
        private readonly Expression<Func<TFact, TKey>> _keySelectorExpression;
        private readonly Expression<Func<TFact, TValue>> _valueSelectorExpression;
        private readonly Func<TFact, TKey> _keySelector;
        private readonly Func<TFact, TValue> _valueSelector;

        public GroupByAggregatorFactory(Expression<Func<TFact, TKey>> keySelectorExpression, Expression<Func<TFact, TValue>> valueSelectorExpression)
        {
            _keySelectorExpression = keySelectorExpression;
            _valueSelectorExpression = valueSelectorExpression;
            _keySelector = keySelectorExpression.Compile();
            _valueSelector = valueSelectorExpression.Compile();
        }

        public IAggregator Create()
        {
            return new GroupByAggregator<TKey, TValue, TFact>(_keySelector, _valueSelector);
        }

        public bool Equals(GroupByAggregatorFactory<TKey, TValue, TFact> other)
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
            return Equals((GroupByAggregatorFactory<TKey, TValue, TFact>)obj);
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