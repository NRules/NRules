using System;
using System.Linq.Expressions;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator factory for projection aggregator.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TElement">Type of projected element.</typeparam>
    internal class ProjectionAggregatorFactory<TSource, TElement> : IAggregatorFactory, IEquatable<ProjectionAggregatorFactory<TSource, TElement>>
    {
        private readonly Expression<Func<TSource, TElement>> _selectorExpression;
        private readonly Func<TSource, TElement> _selector;

        public ProjectionAggregatorFactory(Expression<Func<TSource, TElement>> selectorExpression)
        {
            _selectorExpression = selectorExpression;
            _selector = selectorExpression.Compile();
        }

        public IAggregator Create()
        {
            return new ProjectionAggregator<TSource, TElement>(_selector);
        }

        public bool Equals(ProjectionAggregatorFactory<TSource, TElement> other)
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
            return Equals((ProjectionAggregatorFactory<TSource, TElement>)obj);
        }

        public override int GetHashCode()
        {
            return _selectorExpression.GetHashCode();
        }
    }
}