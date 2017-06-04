using System;
using System.Collections.Generic;
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
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IDictionary<string, IAggregateExpression> compiledExpressions)
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