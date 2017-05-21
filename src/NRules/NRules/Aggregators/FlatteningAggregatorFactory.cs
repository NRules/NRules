using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for flattening aggregator.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TResult">Type of result element.</typeparam>
    internal class FlatteningAggregatorFactory<TSource, TResult> : IAggregatorFactory
    {
        private readonly Func<TSource, IEnumerable<TResult>> _selector;

        public FlatteningAggregatorFactory(Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            _selector = selector.Compile();
        }

        public IAggregator Create()
        {
            return new FlatteningAggregator<TSource, TResult>(_selector);
        }
    }
}