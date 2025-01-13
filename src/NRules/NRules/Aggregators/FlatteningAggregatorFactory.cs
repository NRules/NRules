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
    private Func<IFactIdentityComparer, IAggregator>? _factory;

    [MemberNotNull(nameof(_factory))]
    public void Compile(AggregateElement element, IReadOnlyCollection<IAggregateExpression> compiledExpressions)
    {
        var sourceType = element.Source.ValueType;
        //Flatten selector is Source -> IEnumerable<Result>
        var resultType = element.ResultType;
        Type aggregatorType = typeof(FlatteningAggregator<,>).MakeGenericType(sourceType, resultType);

        var compiledSelector = compiledExpressions.FindSingle(AggregateElement.SelectorName);
        var ctor = aggregatorType.GetConstructors().Single();
        var comparerParameter = Expression.Parameter(typeof(IFactIdentityComparer), "identityComparer");
        var factoryExpression = Expression.Lambda<Func<IFactIdentityComparer, IAggregator>>(
            Expression.New(ctor, comparerParameter, Expression.Constant(compiledSelector)),
            comparerParameter);
        _factory = factoryExpression.Compile();
    }

    public IAggregator Create(AggregationContext context)
    {
        return _factory!(context.FactIdentityComparer);
    }
}