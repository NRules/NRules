using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NRules.RuleModel.Builders
{
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
            var expectedResultType = typeof(IEnumerable<>).MakeGenericType(sourceType);
            if (!expectedResultType.GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo()))
            {
                throw new ArgumentException(
                    $"Collect result must be a collection of source elements. ElementType={sourceType}, ResultType={resultType}");
            }

            var keySelectorsAscending = element.Expressions.Find(AggregateElement.KeySelectorAscendingName);
            var keySelectorsDescending = element.Expressions.Find(AggregateElement.KeySelectorDescendingName);

            foreach (var sortKeySelector in keySelectorsAscending.Concat(keySelectorsDescending).Select(x => x.Expression))
            {
                if (sortKeySelector.Parameters.Count == 0)
                {
                    throw new ArgumentException(
                        $"Sort key selector must have at least one parameter. KeySelector={sortKeySelector}");
                }
                if (sortKeySelector.Parameters[0].Type != sourceType)
                {
                    throw new ArgumentException(
                        "Sort key selector must have a parameter type that matches the aggregate source. " +
                        $"KeySelector={sortKeySelector}, ExpectedType={sourceType}, ActualType={sortKeySelector.Parameters[0].Type}");
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
                    $"GroupBy key selector must have at least one parameter. KeySelector={keySelector}");
            }
            if (keySelector.Parameters[0].Type != sourceType)
            {
                throw new ArgumentException(
                    "GroupBy key selector must have a parameter type that matches the aggregate source. " +
                    $"KeySelector={keySelector}, ExpectedType={sourceType}, ActualType={keySelector.Parameters[0].Type}");
            }

            var elementSelector = element.Expressions[AggregateElement.ElementSelectorName].Expression;
            if (elementSelector.Parameters.Count == 0)
            {
                throw new ArgumentException(
                    $"GroupBy element selector must have at least one parameter. ElementSelector={elementSelector}");
            }
            if (elementSelector.Parameters[0].Type != sourceType)
            {
                throw new ArgumentException(
                    "GroupBy element selector must have a parameter type that matches the aggregate source. " +
                    $"ElementSelector={elementSelector}, ExpectedType={sourceType}, ActualType={elementSelector.Parameters[0].Type}");
            }

            var groupType = typeof(IGrouping<,>).MakeGenericType(keySelector.ReturnType, elementSelector.ReturnType);
            if (!resultType.GetTypeInfo().IsAssignableFrom(groupType.GetTypeInfo()))
            {
                throw new ArgumentException(
                    "GroupBy key/element selectors must produce a grouping assignable to the aggregation result. " +
                    $"ElementSelector={elementSelector}, ResultType={resultType}, GroupingType={groupType}");
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
                    $"Projection selector must have at least one parameter. Selector={selector}");
            }
            if (selector.Parameters[0].Type != sourceType)
            {
                throw new ArgumentException(
                    "Projection selector must have its first parameter type that matches the aggregate source. " +
                    $"Selector={selector}, ExpectedType={sourceType}, ActualType={selector.Parameters[0].Type}");
            }
            if (!resultType.GetTypeInfo().IsAssignableFrom(selector.ReturnType.GetTypeInfo()))
            {
                throw new ArgumentException(
                    "Projection selector must produce a value assignable to the aggregation result. " +
                    $"Selector={selector}, ResultType={resultType}, SelectorReturnType={selector.ReturnType}");
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
                    $"Flattening selector must have a single parameter. Selector={selector}");
            }
            if (selector.Parameters[0].Type != sourceType)
            {
                throw new ArgumentException(
                    "Flattening selector must have a parameter type that matches the aggregate source. " +
                    $"Selector={selector}, ExpectedType={sourceType}, ActualType={selector.Parameters[0].Type}");
            }
            var resultCollectionType = typeof(IEnumerable<>).MakeGenericType(resultType);
            if (!resultCollectionType.GetTypeInfo().IsAssignableFrom(selector.ReturnType.GetTypeInfo()))
            {
                throw new ArgumentException(
                    "Flattening selector must produce a collection of values that are assignable to the aggregation result. " +
                    $"Selector={selector}, ResultType={resultType}, SelectorReturnType={selector.ReturnType}");
            }
        }

        public static void ValidateBinding(BindingElement element)
        {
            var resultType = element.ResultType;
            var expressionReturnType = element.Expression.ReturnType;
            if (!resultType.GetTypeInfo().IsAssignableFrom(expressionReturnType.GetTypeInfo()))
            {
                throw new ArgumentException($"Binding expression not assignable to result type. ResultType={resultType}, ExpressionResult={expressionReturnType}");
            }
        }
    }
}