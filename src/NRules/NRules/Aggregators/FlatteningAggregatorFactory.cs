using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Aggregators;

/// <summary>
/// Aggregator factory for flattening aggregator.
/// </summary>
internal class FlatteningAggregatorFactory : IAggregatorFactory
{
    private Func<IAggregator>? _factory;

    [MemberNotNull(nameof(_factory))]
    public void Compile(AggregateElement element, IReadOnlyCollection<IAggregateExpression> compiledExpressions)
    {
        var sourceType = element.Source.ValueType;
        //Flatten selector is Source -> IEnumerable<Result>
        var resultType = element.ResultType;
        Type aggregatorType = typeof(FlatteningAggregator<,>).MakeGenericType(sourceType, resultType);

        var compiledSelector = compiledExpressions.FindSingle(AggregateElement.SelectorName);
        var ctor = aggregatorType.GetConstructors().Single();
        var factoryExpression = Expression.Lambda<Func<IAggregator>>(
            Expression.New(ctor, Expression.Constant(compiledSelector)));
        _factory = factoryExpression.Compile();
    }

    public IAggregator Create()
    {
        return _factory!();
    }
}