using System;
using System.Collections.Generic;
using NRules.RuleModel;
using System.Linq.Expressions;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for collection aggregator.
    /// </summary>
    internal class CollectionAggregatorFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IDictionary<string, IAggregateExpression> compiledExpressions)
        {
            var sourceType = element.Source.ValueType;
            var aggregatorType = typeof(CollectionAggregator<>).MakeGenericType(sourceType);
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(aggregatorType));
            _factory = factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }
}
