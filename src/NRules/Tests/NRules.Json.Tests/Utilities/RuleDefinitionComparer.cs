using System;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Json.Tests.Utilities
{
    public static class RuleDefinitionComparer
    {
        public static bool AreEqual(IRuleDefinition x, IRuleDefinition y)
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

        private static bool AreEqual(RuleElement x, RuleElement y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            if (x.ElementType != y.ElementType) return false;

            if (x.ElementType == ElementType.Action)
            {
                var a = (ActionElement) x;
                var b = (ActionElement) y;
                return a.ActionTrigger == b.ActionTrigger &&
                       ExpressionComparer.AreEqual(a.Expression, b.Expression);
            }
            else if (x.ElementType == ElementType.ActionGroup)
            {
                var a = (ActionGroupElement) x;
                var b = (ActionGroupElement) y;
                return AreEqual(a.Actions, b.Actions);
            }
            else if (x.ElementType == ElementType.Aggregate)
            {
                var a = (AggregateElement) x;
                var b = (AggregateElement) y;
                return a.Name == b.Name &&
                       AreEqual(a.Expressions, b.Expressions) &&
                       a.CustomFactoryType == b.CustomFactoryType &&
                       AreEqual(a.Source, b.Source);
            }
            else if (x.ElementType == ElementType.And ||
                     x.ElementType == ElementType.Or)
            {
                var a = (GroupElement) x;
                var b = (GroupElement) y;
                return AreEqual(a.ChildElements, b.ChildElements);
            }
            else if (x.ElementType == ElementType.Binding)
            {
                var a = (BindingElement) x;
                var b = (BindingElement) y;
                return ExpressionComparer.AreEqual(a.Expression, b.Expression);
            }
            else if (x.ElementType == ElementType.Dependency)
            {
                var a = (DependencyElement) x;
                var b = (DependencyElement) y;
                return a.ServiceType == b.ServiceType &&
                       AreEqual(a.Declaration, b.Declaration);
            }
            else if (x.ElementType == ElementType.DependencyGroup)
            {
                var a = (DependencyGroupElement) x;
                var b = (DependencyGroupElement) y;
                return AreEqual(a.Dependencies, b.Dependencies);
            }
            else if (x.ElementType == ElementType.Exists)
            {
                var a = (ExistsElement) x;
                var b = (ExistsElement) y;
                return AreEqual(a.Source, b.Source);
            }
            else if (x.ElementType == ElementType.Filter)
            {
                var a = (FilterElement) x;
                var b = (FilterElement) y;
                return a.FilterType == b.FilterType &&
                       ExpressionComparer.AreEqual(a.Expression, b.Expression);
            }
            else if (x.ElementType == ElementType.FilterGroup)
            {
                var a = (FilterGroupElement) x;
                var b = (FilterGroupElement) y;
                return AreEqual(a.Filters, b.Filters);
            }
            else if (x.ElementType == ElementType.ForAll)
            {
                var a = (ForAllElement) x;
                var b = (ForAllElement) y;
                return AreEqual(a.BasePattern, b.BasePattern) &&
                       AreEqual(a.Patterns, b.Patterns);
            }
            else if (x.ElementType == ElementType.Not)
            {
                var a = (NotElement) x;
                var b = (NotElement) y;
                return AreEqual(a.Source, b.Source);
            }
            else if (x.ElementType == ElementType.Pattern)
            {
                var a = (PatternElement) x;
                var b = (PatternElement) y;
                return AreEqual(a.Expressions, b.Expressions) &&
                       AreEqual(a.Declaration, b.Declaration) &&
                       AreEqual(a.Source, b.Source);
            }
            else if (x.ElementType == ElementType.NamedExpression)
            {
                var a = (NamedExpressionElement) x;
                var b = (NamedExpressionElement) y;
                return a.Name == b.Name &&
                       ExpressionComparer.AreEqual(a.Expression, b.Expression);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"ElementType={x.ElementType}");
            }
        }

        private static bool AreEqual(IEnumerable<RuleElement> x, IEnumerable<RuleElement> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;

            using var enumerator1 = x.GetEnumerator();
            using var enumerator2 = y.GetEnumerator();

            while (true)
            {
                bool hasNext1 = enumerator1.MoveNext();
                bool hasNext2 = enumerator2.MoveNext();

                if (hasNext1 && hasNext2)
                {
                    if (!AreEqual(enumerator1.Current, enumerator2.Current))
                        return false;
                }
                else if (hasNext1 || hasNext2)
                {
                    return false;
                }
                else
                {
                    break;
                }
            }

            return true;
        }

        private static bool AreEqual(Declaration x, Declaration y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;

            if (x.Name != y.Name) return false;
            if (x.Type != y.Type) return false;

            return true;
        }
    }
}
