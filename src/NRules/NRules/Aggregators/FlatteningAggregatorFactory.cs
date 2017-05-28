using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;
using System.Reflection;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for flattening aggregator.
    /// </summary>
    internal class FlatteningAggregatorFactory : IAggregatorFactory
    {
        private readonly Func<IAggregator> _factory;

        public FlatteningAggregatorFactory(AggregateElement element)
        {
            var selector = element.ExpressionMap["Selector"];
            var sourceType = element.Source.ValueType;
            //Flatten slector is X -> IEnumerable<Y>
            var resultType = element.ResultType;
            Type aggregatorType = typeof(FlatteningAggregator<,>).MakeGenericType(sourceType, resultType);

            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, selector.Expression));
            _factory = factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }
}