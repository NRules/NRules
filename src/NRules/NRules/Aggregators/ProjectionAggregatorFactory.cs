using System;
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
        private readonly Func<IAggregator> _factory;

        public ProjectionAggregatorFactory(AggregateElement element)
        {
            var selector = element.ExpressionMap["Selector"];
            var sourceType = element.Source.ValueType;
            var resultType = selector.Expression.ReturnType;
            Type aggregatorType = typeof(ProjectionAggregator<,>).MakeGenericType(sourceType, resultType);

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