using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for projection aggregator.
    /// </summary>
    internal class ProjectionAggregatorFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IDictionary<string, IAggregateExpression> compiledExpressions)
        {
            var selector = element.ExpressionMap["Selector"];
            var sourceType = element.Source.ValueType;
            var resultType = selector.Expression.ReturnType;
            Type aggregatorType = typeof(ProjectionAggregator<,>).MakeGenericType(sourceType, resultType);

            var compiledSelector = compiledExpressions["Selector"];
            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(compiledSelector)));
            _factory = factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }
}