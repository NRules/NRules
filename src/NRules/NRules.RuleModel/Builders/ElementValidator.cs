using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel.Builders;

internal static class ElementValidator
{
    public static void ValidateUniqueDeclarations(params RuleElement[] elements)
    {
        ValidateUniqueDeclarations(elements.AsEnumerable());
    }

    public static void ValidateUniqueDeclarations(IEnumerable<RuleElement> elements)
    {
        var duplicates = elements.SelectMany(x => x.Exports)
            .GroupBy(x => x.Name)
            .Where(x => x.Count() > 1)
            .ToArray();
        if (duplicates.Any())
        {
            var declarations = string.Join(",", duplicates.Select(x => x.Key));
            throw new InvalidOperationException($"Duplicate declarations. Declaration={declarations}");
        }
    }

    public static void ValidateRuleDefinition(RuleDefinition definition)
    {
        var exports = definition.LeftHandSide.Exports
            .Concat(definition.DependencyGroup.Exports).ToArray();

        var undefinedLhs = definition.LeftHandSide.Imports
            .Except(exports).ToArray();
        if (undefinedLhs.Any())
        {
            var variables = string.Join(",", undefinedLhs.Select(x => x.Name));
            throw new InvalidOperationException($"Undefined variables in rule match conditions. Variables={variables}");
        }

        var undefinedFilter = definition.FilterGroup.Imports
            .Except(exports).ToArray();
        if (undefinedFilter.Any())
        {
            var variables = string.Join(",", undefinedFilter.Select(x => x.Name));
            throw new InvalidOperationException($"Undefined variables in rule filter. Variables={variables}");
        }

        var undefinedRhs = definition.RightHandSide.Imports
            .Except(exports).ToArray();
        if (undefinedRhs.Any())
        {
            var variables = string.Join(",", undefinedRhs.Select(x => x.Name));
            throw new InvalidOperationException($"Undefined variables in rule actions. Variables={variables}");
        }

        var lhsDependencyRefs = definition.LeftHandSide.Imports
            .Intersect(definition.DependencyGroup.Exports).ToArray();
        if (lhsDependencyRefs.Any())
        {
            var variables = string.Join(",", lhsDependencyRefs.Select(x => x.Name));
            throw new InvalidOperationException($"Rule match conditions cannot reference injected dependencies. Variables={variables}");
        }
    }

    public static void ValidateAggregate(AggregateElement element)
    {
        switch (element.Name)
        {
            case AggregateElement.CollectName:
                ValidateCollectAggregate(element);
                break;
            case AggregateElement.GroupByName:
                ValidateGroupByAggregate(element);
                break;
            case AggregateElement.ProjectName:
                ValidateProjectAggregate(element);
                break;
            case AggregateElement.FlattenName:
                ValidateFlattenAggregate(element);
                break;
        }
    }

    public static void ValidateCollectAggregate(AggregateElement element)
    {
        var sourceType = element.Source.ValueType;
        var resultType = element.ResultType;

        var keySelectors = element.Expressions.Find(AggregateElement.KeySelectorName).ToArray();
        if (keySelectors.Length > 1)
        {
            throw new ArgumentException(
                $"Collect aggregator can have no more than one key selector. Count={keySelectors.Length}", nameof(element));
        }

        foreach (var keySelector in keySelectors.Select(x => x.Expression))
        {
            if (keySelector.Parameters.Count == 0)
            {
                throw new ArgumentException(
                    $"Collect key selector must have at least one parameter. KeySelector={keySelector}", nameof(element));
            }
            if (keySelector.Parameters[0].Type != sourceType)
            {
                throw new ArgumentException(
                    "Collect key selector must have a parameter type that matches the aggregate source. " +
                    $"KeySelector={keySelector}, ExpectedType={sourceType}, ActualType={keySelector.Parameters[0].Type}", nameof(element));
            }

        }

        var elementSelectors = element.Expressions.Find(AggregateElement.ElementSelectorName).ToArray();
        if (elementSelectors.Length > 1)
        {
            throw new ArgumentException(
                $"Collect aggregator can have no more than one element selector. Count={elementSelectors.Length}", nameof(element));
        }

        foreach (var elementSelector in elementSelectors.Select(x => x.Expression))
        {
            if (elementSelector.Parameters.Count == 0)
            {
                throw new ArgumentException(
                    $"Collect element selector must have at least one parameter. KeySelector={elementSelector}", nameof(element));
            }
            if (elementSelector.Parameters[0].Type != sourceType)
            {
                throw new ArgumentException(
                    "Collect element selector must have a parameter type that matches the aggregate source. " +
                    $"ElementSelector={elementSelector}, ExpectedType={sourceType}, ActualType={elementSelector.Parameters[0].Type}", nameof(element));
            }

        }

        if (keySelectors.Length > 0)
        {
            var expectedResultType = typeof(ILookup<,>).MakeGenericType(
                keySelectors[0].Expression.ReturnType, elementSelectors[0].Expression.ReturnType);
            if (!expectedResultType.IsAssignableFrom(resultType))
            {
                throw new ArgumentException(
                    $"Collect result with grouping key must be a lookup collection. ExpectedType={expectedResultType}, ActualType={resultType}", nameof(element));
            }
        }
        else
        {
            var expectedResultType = typeof(IEnumerable<>).MakeGenericType(sourceType);
            if (!expectedResultType.IsAssignableFrom(resultType))
            {
                throw new ArgumentException(
                    $"Collect result must be a collection of source elements. ExpectedType={expectedResultType}, ActualType={resultType}", nameof(element));
            }
        }

        var sortKeySelectorsAscending = element.Expressions.Find(AggregateElement.KeySelectorAscendingName);
        var sortKeySelectorsDescending = element.Expressions.Find(AggregateElement.KeySelectorDescendingName);

        foreach (var sortKeySelector in sortKeySelectorsAscending.Concat(sortKeySelectorsDescending).Select(x => x.Expression))
        {
            if (sortKeySelector.Parameters.Count == 0)
            {
                throw new ArgumentException(
                    $"Sort key selector must have at least one parameter. KeySelector={sortKeySelector}", nameof(element));
            }
            if (sortKeySelector.Parameters[0].Type != sourceType)
            {
                throw new ArgumentException(
                    "Sort key selector must have a parameter type that matches the aggregate source. " +
                    $"KeySelector={sortKeySelector}, ExpectedType={sourceType}, ActualType={sortKeySelector.Parameters[0].Type}", nameof(element));
            }
        }
    }

    public static void ValidateGroupByAggregate(AggregateElement element)
    {
        var sourceType = element.Source.ValueType;
        var resultType = element.ResultType;
        var keySelector = element.Expressions[AggregateElement.KeySelectorName].Expression;
        if (keySelector.Parameters.Count == 0)
        {
            throw new ArgumentException(
                $"GroupBy key selector must have at least one parameter. KeySelector={keySelector}", nameof(element));
        }
        if (keySelector.Parameters[0].Type != sourceType)
        {
            throw new ArgumentException(
                "GroupBy key selector must have a parameter type that matches the aggregate source. " +
                $"KeySelector={keySelector}, ExpectedType={sourceType}, ActualType={keySelector.Parameters[0].Type}", nameof(element));
        }

        var elementSelector = element.Expressions[AggregateElement.ElementSelectorName].Expression;
        if (elementSelector.Parameters.Count == 0)
        {
            throw new ArgumentException(
                $"GroupBy element selector must have at least one parameter. ElementSelector={elementSelector}", nameof(element));
        }
        if (elementSelector.Parameters[0].Type != sourceType)
        {
            throw new ArgumentException(
                "GroupBy element selector must have a parameter type that matches the aggregate source. " +
                $"ElementSelector={elementSelector}, ExpectedType={sourceType}, ActualType={elementSelector.Parameters[0].Type}", nameof(element));
        }

        var groupType = typeof(IGrouping<,>).MakeGenericType(keySelector.ReturnType, elementSelector.ReturnType);
        if (!resultType.IsAssignableFrom(groupType))
        {
            throw new ArgumentException(
                "GroupBy key/element selectors must produce a grouping assignable to the aggregation result. " +
                $"ElementSelector={elementSelector}, ResultType={resultType}, GroupingType={groupType}", nameof(element));
        }
    }

    public static void ValidateProjectAggregate(AggregateElement element)
    {
        var sourceType = element.Source.ValueType;
        var resultType = element.ResultType;
        var selector = element.Expressions[AggregateElement.SelectorName].Expression;
        if (selector.Parameters.Count == 0)
        {
            throw new ArgumentException(
                $"Projection selector must have at least one parameter. Selector={selector}", nameof(element));
        }
        if (selector.Parameters[0].Type != sourceType)
        {
            throw new ArgumentException(
                "Projection selector must have its first parameter type that matches the aggregate source. " +
                $"Selector={selector}, ExpectedType={sourceType}, ActualType={selector.Parameters[0].Type}", nameof(element));
        }
        if (!resultType.IsAssignableFrom(selector.ReturnType))
        {
            throw new ArgumentException(
                "Projection selector must produce a value assignable to the aggregation result. " +
                $"Selector={selector}, ResultType={resultType}, SelectorReturnType={selector.ReturnType}", nameof(element));
        }
    }

    public static void ValidateFlattenAggregate(AggregateElement element)
    {
        var sourceType = element.Source.ValueType;
        var resultType = element.ResultType;
        var selector = element.Expressions[AggregateElement.SelectorName].Expression;
        if (selector.Parameters.Count != 1)
        {
            throw new ArgumentException(
                $"Flattening selector must have a single parameter. Selector={selector}", nameof(element));
        }
        if (selector.Parameters[0].Type != sourceType)
        {
            throw new ArgumentException(
                "Flattening selector must have a parameter type that matches the aggregate source. " +
                $"Selector={selector}, ExpectedType={sourceType}, ActualType={selector.Parameters[0].Type}", nameof(element));
        }
        var resultCollectionType = typeof(IEnumerable<>).MakeGenericType(resultType);
        if (!resultCollectionType.IsAssignableFrom(selector.ReturnType))
        {
            throw new ArgumentException(
                "Flattening selector must produce a collection of values that are assignable to the aggregation result. " +
                $"Selector={selector}, ResultType={resultType}, SelectorReturnType={selector.ReturnType}", nameof(element));
        }
    }

    public static void ValidatePattern(PatternElement element)
    {
        if (element.Source?.ElementType
            is not null
            and not ElementType.Aggregate
            and not ElementType.Binding)
            throw new ArgumentException($"Invalid source element. ElementType={element.Source.ElementType}", nameof(element));
    }

    public static void ValidateBinding(BindingElement element)
    {
        var resultType = element.ResultType;
        var expressionReturnType = element.Expression.ReturnType;
        if (!resultType.IsAssignableFrom(expressionReturnType))
        {
            throw new ArgumentException($"Binding expression not assignable to result type. ResultType={resultType}, ExpressionResult={expressionReturnType}", nameof(element));
        }
    }

    public static void ValidateGroup(GroupElement element)
    {
        if (!element.ChildElements.Any())
        {
            throw new InvalidOperationException("Group element requires at least one child element");
        }
        foreach (var childElement in element.ChildElements)
        {
            switch (childElement.ElementType)
            {
                case ElementType.Pattern:
                case ElementType.And:
                case ElementType.Or:
                case ElementType.Not:
                case ElementType.Exists:
                case ElementType.ForAll:
                    break;
                default:
                    throw new ArgumentException($"Invalid element in the group. ElementType={childElement.ElementType}", nameof(element));
            }
        }
    }

    public static void ValidateExists(ExistsElement element)
    {
        switch (element.Source.ElementType)
        {
            case ElementType.Pattern:
            case ElementType.And:
            case ElementType.Or:
                break;
            default:
                throw new ArgumentException($"Invalid source element. ElementType={element.Source.ElementType}", nameof(element));
        }
    }

    public static void ValidateNot(NotElement element)
    {
        switch (element.Source.ElementType)
        {
            case ElementType.Pattern:
            case ElementType.And:
            case ElementType.Or:
                break;
            default:
                throw new ArgumentException($"Invalid source element. ElementType={element.Source.ElementType}", nameof(element));
        }
    }

    public static void ValidateForAll(ForAllElement element)
    {
        if (!element.Patterns.Any())
        {
            throw new InvalidOperationException("At least one FORALL pattern must be specified");
        }
    }
}