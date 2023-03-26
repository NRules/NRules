using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Aggregators;

/// <summary>
/// Aggregator factory for collection aggregator.
/// Depending on supplied expressions this will create an ordered or an unordered fact aggregator.
/// </summary>
internal class CollectionAggregatorFactory : IAggregatorFactory
{
    private Func<IAggregator> _factory;

    public void Compile(AggregateElement element, IReadOnlyCollection<IAggregateExpression> compiledExpressions)
    {
        var sourceType = element.Source.ValueType;

        var sortConditions = compiledExpressions.Where(x => Equals(x.Name, AggregateElement.KeySelectorAscendingName) || Equals(x.Name, AggregateElement.KeySelectorDescendingName))
            .Select(x => new SortCondition(x.Name, GetSortDirection(x.Name), x)).ToArray();
        var groupConditions = compiledExpressions.Where(x => Equals(x.Name, AggregateElement.KeySelectorName)).ToArray();

        switch (element.Name)
        {
            case AggregateElement.CollectName when sortConditions.Length == 0 && groupConditions.Length == 0:
                _factory = CreateCollectAggregator(sourceType);
                break;
            case AggregateElement.CollectName when sortConditions.Length == 1 && groupConditions.Length == 0:
                var expression = element.Expressions[sortConditions[0].Name].Expression;
                _factory = CreateSingleKeySortedAggregatorFactory(sourceType, sortConditions[0], expression);
                break;
            case AggregateElement.CollectName when sortConditions.Length > 1 && groupConditions.Length == 0:
                _factory = CreateMultiKeySortedAggregatorFactory(sourceType, sortConditions);
                break;
            case AggregateElement.CollectName when sortConditions.Length == 0 && groupConditions.Length == 1:
                _factory = CreateLookupAggregator(sourceType, compiledExpressions, element.Expressions);
                break;
            default:
                throw new ArgumentException("Unsupported collection aggregator. " +
                    $"Name={element.Name}, KeySelectorCount={groupConditions.Length}, SortSelectorCount={sortConditions.Length}");
        }
    }

    public IAggregator Create()
    {
        return _factory();
    }

    private Func<IAggregator> CreateCollectAggregator(Type sourceType)
    {
        var aggregatorType = typeof(CollectionAggregator<>).MakeGenericType(sourceType);

        var factoryExpression = Expression.Lambda<Func<IAggregator>>(
            Expression.New(aggregatorType));

        return factoryExpression.Compile();
    }

    private static Func<IAggregator> CreateSingleKeySortedAggregatorFactory(Type sourceType, SortCondition sortCondition, LambdaExpression expression)
    {
        var keyType = expression.ReturnType;
        var aggregatorType = typeof(SortedAggregator<,>).MakeGenericType(sourceType, keyType);

        var ctor = aggregatorType.GetConstructors().Single();
        var factoryExpression = Expression.Lambda<Func<IAggregator>>(
            Expression.New(ctor, 
                Expression.Constant(sortCondition.Expression),
                Expression.Constant(sortCondition.Direction)));

        return factoryExpression.Compile();
    }

    private static Func<IAggregator> CreateMultiKeySortedAggregatorFactory(Type sourceType, SortCondition[] sortConditions)
    {
        var aggregatorType = typeof(MultiKeySortedAggregator<>).MakeGenericType(sourceType);

        var ctor = aggregatorType.GetConstructors().Single();
        var factoryExpression = Expression.Lambda<Func<IAggregator>>(
            Expression.New(ctor, Expression.Constant(sortConditions)));

        return factoryExpression.Compile();
    }

    private static SortDirection GetSortDirection(string keySelectorName)
    {
        return keySelectorName == AggregateElement.KeySelectorAscendingName 
            ? SortDirection.Ascending : SortDirection.Descending;
    }

    private Func<IAggregator> CreateLookupAggregator(Type sourceType, IReadOnlyCollection<IAggregateExpression> compiledExpressions, ExpressionCollection elementExpressions)
    {
        var keySelector = compiledExpressions.SingleOrDefault(x => x.Name == AggregateElement.KeySelectorName);
        var elementSelector = compiledExpressions.SingleOrDefault(x => x.Name == AggregateElement.ElementSelectorName);

        if (keySelector == null)
            throw new ArgumentNullException($"Lookup key selector not provided");
        if (elementSelector == null)
            throw new ArgumentNullException($"Lookup element selector not provided");

        var keySelectorExpression = elementExpressions[keySelector.Name];
        var elementSelectorExpression = elementExpressions[elementSelector.Name];

        var aggregatorType = typeof(LookupAggregator<,,>).MakeGenericType(sourceType,
            keySelectorExpression.Expression.ReturnType, 
            elementSelectorExpression.Expression.ReturnType);

        var ctor = aggregatorType.GetConstructors().Single();
        var factoryExpression = Expression.Lambda<Func<IAggregator>>(
            Expression.New(ctor, Expression.Constant(keySelector), Expression.Constant(elementSelector)));

        return factoryExpression.Compile();
    }
}