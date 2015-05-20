using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator factory for flattening aggregator.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TResult">Type of result element.</typeparam>
    internal class FlatteningAggregatorFactory<TSource, TResult> : IAggregatorFactory, IEquatable<FlatteningAggregatorFactory<TSource, TResult>>
    {
        private readonly Expression<Func<TSource, IEnumerable<TResult>>> _selectorExpression;
        private readonly Func<TSource, IEnumerable<TResult>> _selector;

        public FlatteningAggregatorFactory(Expression<Func<TSource, IEnumerable<TResult>>> selectorExpression)
        {
            _selectorExpression = selectorExpression;
            _selector = selectorExpression.Compile();
        }

        public IAggregator Create()
        {
            return new FlatteningAggregator<TSource, TResult>(_selector);
        }

        public bool Equals(FlatteningAggregatorFactory<TSource, TResult> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _selectorExpression.Equals(other._selectorExpression);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FlatteningAggregatorFactory<TSource, TResult>)obj);
        }

        public override int GetHashCode()
        {
            return _selectorExpression.GetHashCode();
        }
    }
}