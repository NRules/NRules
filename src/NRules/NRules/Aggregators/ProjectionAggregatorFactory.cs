using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for projection aggregator.
    /// </summary>
    internal class ProjectionAggregatorFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IEnumerable<IAggregateExpression> compiledExpressions)
        {
            var selector = element.Expressions[AggregateElement.SelectorName];
            var sourceType = element.Source.ValueType;
            var resultType = selector.Expression.ReturnType;
            Type aggregatorType = typeof(ProjectionAggregator<,>).MakeGenericType(sourceType, resultType);

            var compiledSelector = compiledExpressions.FindSingle(AggregateElement.SelectorName);
            var ctor = aggregatorType.GetConstructors().Single();
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