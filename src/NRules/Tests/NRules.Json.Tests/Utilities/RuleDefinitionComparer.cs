using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Json.Tests.Utilities;

public static class RuleDefinitionComparer
{
    public static bool AreEqual(IRuleDefinition? x, IRuleDefinition? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;

        if (x.Name != y.Name) return false;
        if (x.Description != y.Description) return false;
        if (x.Priority != y.Priority) return false;
        if (x.Repeatability != y.Repeatability) return false;

        if (!AreEqual(x.DependencyGroup, y.DependencyGroup)) return false;
        if (!AreEqual(x.LeftHandSide, y.LeftHandSide)) return false;
        if (!AreEqual(x.FilterGroup, y.FilterGroup)) return false;
        if (!AreEqual(x.RightHandSide, y.RightHandSide)) return false;

        return true;
    }

    private static bool AreEqual(RuleElement? x, RuleElement? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        if (x.ElementType != y.ElementType) return false;

        switch (x.ElementType)
        {
            case ElementType.Action:
            {
                var a = (ActionElement) x;
                var b = (ActionElement) y;
                return a.ActionTrigger == b.ActionTrigger &&
                       ExpressionComparer.AreEqual(a.Expression, b.Expression);
            }
            case ElementType.ActionGroup:
            {
                var a = (ActionGroupElement) x;
                var b = (ActionGroupElement) y;
                return AreEqual(a.Actions, b.Actions);
            }
            case ElementType.Aggregate:
            {
                var a = (AggregateElement) x;
                var b = (AggregateElement) y;
                return a.Name == b.Name &&
                       AreEqual(a.Expressions, b.Expressions) &&
                       a.CustomFactoryType == b.CustomFactoryType &&
                       AreEqual(a.Source, b.Source);
            }
            case ElementType.And:
            case ElementType.Or:
            {
                var a = (GroupElement) x;
                var b = (GroupElement) y;
                return AreEqual(a.ChildElements, b.ChildElements);
            }
            case ElementType.Binding:
            {
                var a = (BindingElement) x;
                var b = (BindingElement) y;
                return ExpressionComparer.AreEqual(a.Expression, b.Expression);
            }
            case ElementType.Dependency:
            {
                var a = (DependencyElement) x;
                var b = (DependencyElement) y;
                return a.ServiceType == b.ServiceType &&
                       AreEqual(a.Declaration, b.Declaration);
            }
            case ElementType.DependencyGroup:
            {
                var a = (DependencyGroupElement) x;
                var b = (DependencyGroupElement) y;
                return AreEqual(a.Dependencies, b.Dependencies);
            }
            case ElementType.Exists:
            {
                var a = (ExistsElement) x;
                var b = (ExistsElement) y;
                return AreEqual(a.Source, b.Source);
            }
            case ElementType.Filter:
            {
                var a = (FilterElement) x;
                var b = (FilterElement) y;
                return a.FilterType == b.FilterType &&
                       ExpressionComparer.AreEqual(a.Expression, b.Expression);
            }
            case ElementType.FilterGroup:
            {
                var a = (FilterGroupElement) x;
                var b = (FilterGroupElement) y;
                return AreEqual(a.Filters, b.Filters);
            }
            case ElementType.ForAll:
            {
                var a = (ForAllElement) x;
                var b = (ForAllElement) y;
                return AreEqual(a.BasePattern, b.BasePattern) &&
                       AreEqual(a.Patterns, b.Patterns);
            }
            case ElementType.Not:
            {
                var a = (NotElement) x;
                var b = (NotElement) y;
                return AreEqual(a.Source, b.Source);
            }
            case ElementType.Pattern:
            {
                var a = (PatternElement) x;
                var b = (PatternElement) y;
                return AreEqual(a.Expressions, b.Expressions) &&
                       AreEqual(a.Declaration, b.Declaration) &&
                       AreEqual(a.Source, b.Source);
            }
            case ElementType.NamedExpression:
            {
                var a = (NamedExpressionElement) x;
                var b = (NamedExpressionElement) y;
                return a.Name == b.Name &&
                       ExpressionComparer.AreEqual(a.Expression, b.Expression);
            }
            default:
                throw new ArgumentOutOfRangeException($"ElementType={x.ElementType}");
        }
    }

    private static bool AreEqual(IReadOnlyCollection<RuleElement> x, IReadOnlyCollection<RuleElement> y)
    {
        return x.Count == y.Count
               && x.Zip(y, (first, second) => new {X = first, Y = second})
                   .All(o => AreEqual(o.X, o.Y));
    }

    private static bool AreEqual(Declaration? x, Declaration? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;

        if (x.Name != y.Name) return false;
        if (x.Type != y.Type) return false;

        return true;
    }
}
