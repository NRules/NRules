using System;
using System.Linq.Expressions;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregate factory for group by aggregator.
    /// </summary>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TFact">Type of facts to group.</typeparam>
    internal class GroupByAggregatorFactory<TKey, TFact> : IAggregatorFactory, IEquatable<GroupByAggregatorFactory<TKey, TFact>>
    {
        private readonly Expression<Func<TFact, TKey>> _keySelectorExpression;
        private readonly Func<TFact, TKey> _keySelector;

        public GroupByAggregatorFactory(Expression<Func<TFact, TKey>> keySelectorExpression)
        {
            _keySelectorExpression = keySelectorExpression;
            _keySelector = keySelectorExpression.Compile();
        }

        public IAggregator Create()
        {
            return new GroupByAggregator<TKey, TFact>(_keySelector);
        }

        public bool Equals(GroupByAggregatorFactory<TKey, TFact> other)
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
            return Equals((GroupByAggregatorFactory<TKey, TFact>)obj);
        }

        public override int GetHashCode()
        {
            return _keySelectorExpression.GetHashCode();
        }
    }
}