using System;
using NRules.RuleModel;
using System.Linq.Expressions;

namespace NRules.Aggregators
{
    internal class CollectionAggregatorFactory : IAggregatorFactory
    {
        private readonly Func<IAggregator> _factory;

        public CollectionAggregatorFactory(AggregateElement element)
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
