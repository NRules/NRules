using System;
using System.Linq.Expressions;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for projection aggregator.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TResult">Type of result element.</typeparam>
    internal class ProjectionAggregatorFactory<TSource, TResult> : IAggregatorFactory
    {
        private readonly Func<TSource, TResult> _selector;

        public ProjectionAggregatorFactory(Expression<Func<TSource, TResult>> selector)
        {
            _selector = selector.Compile();
        }

        public IAggregator Create()
        {
            return new ProjectionAggregator<TSource, TResult>(_selector);
        }
    }
}