using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator factory for flattening aggregator.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TResult">Type of result element.</typeparam>
    internal class FlatteningAggregatorFactory<TSource, TResult> : IAggregatorFactory
    {
        private readonly Func<TSource, IEnumerable<TResult>> _selector;

        public FlatteningAggregatorFactory(Func<TSource, IEnumerable<TResult>> selector)
        {
            _selector = selector;
        }

        public IAggregator Create()
        {
            return new FlatteningAggregator<TSource, TResult>(_selector);
        }
    }
}