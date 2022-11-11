using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders;

public class RuleTransformation : RuleElementVisitor<RuleTransformation.Context>
{
    public class Context
    {
        private readonly Stack<RuleElement> _elements = new();

        internal bool IsModified { get; set; }

        internal RuleElement Pop()
        {
            return _elements.Pop();
        }

        internal void Push(RuleElement element)
        {
            _elements.Push(element);
        }
    }

    public IRuleDefinition Transform(IRuleDefinition rule)
    {
        var lhsContext = new Context();
        var lhs = Transform<GroupElement>(lhsContext, rule.LeftHandSide);

        var rhsContext = new Context();
        var rhs = Transform<ActionGroupElement>(rhsContext, rule.RightHandSide);

        if (lhsContext.IsModified || rhsContext.IsModified)
        {
            var transformedRule = Element.RuleDefinition(rule.Name, rule.Description, rule.Priority,
                rule.Repeatability, rule.Tags, rule.Properties, rule.DependencyGroup, lhs ?? throw new InvalidOperationException("Left hand side is null"),
                rule.FilterGroup, rhs ?? throw new InvalidOperationException("Right hand side is null"));
            return transformedRule;
        }
        return rule;
    }

    private void Result(Context context, RuleElement element)
    {
        context.Pop();
        context.Push(element);
        context.IsModified = true;
    }

    private T Transform<T>(Context context, RuleElement element) where T : RuleElement
    {
        var savedIsModified = context.IsModified;
        context.IsModified = false;

        context.Push(element);
        Visit(context, element);
        context.IsModified |= savedIsModified;

        return (T)context.Pop();
    }

    protected internal override void VisitPattern(Context context, PatternElement element)
    {
        var source = element.Source is null ? null : Transform<RuleElement>(context, element.Source);
        if (context.IsModified)
        {
            var newElement = Element.Pattern(element.Declaration, element.Expressions, source);
            Result(context, newElement);
        }
    }

    protected internal override void VisitBinding(Context context, BindingElement element)
    {
    }

    protected internal override void VisitAggregate(Context context, AggregateElement element)
    {
        var source = element.Source is null ? null : Transform<PatternElement>(context, element.Source);
        if (context.IsModified)
        {
            var newElement = Element.Aggregate(element.ResultType, element.Name, element.Expressions, source!, element.CustomFactoryType);
            Result(context, newElement);
        }
    }

    protected internal override void VisitActionGroup(Context context, ActionGroupElement element)
    {
        var actions = element.Actions.Select(x => Transform<ActionElement>(context, x)).ToList();
        if (context.IsModified)
        {
            var newElement = Element.ActionGroup(actions);
            Result(context, newElement);
        }
    }

    protected internal override void VisitAction(Context context, ActionElement element)
    {
    }

    protected internal override void VisitAnd(Context context, AndElement element)
    {
        var childElements = element.ChildElements.Select(x => Transform<RuleElement>(context, x)).ToList();
        if (CollapseSingleGroup(context, childElements))
            return;
        if (SplitOrGroup(context, childElements))
            return;
        if (context.IsModified)
        {
            var newElement = Element.AndGroup(childElements);
            Result(context, newElement);
        }
    }

    protected internal override void VisitOr(Context context, OrElement element)
    {
        var childElements = element.ChildElements.Select(x => Transform<RuleElement>(context, x)).ToList();
        if (CollapseSingleGroup(context, childElements))
            return;
        if (MergeOrGroups(context, childElements))
            return;
        if (context.IsModified)
        {
            var newElement = Element.OrGroup(childElements);
            Result(context, newElement);
        }
    }

    protected internal override void VisitNot(Context context, NotElement element)
    {
        var source = Transform<RuleElement>(context, element.Source)!;
        if (context.IsModified)
        {
            var newElement = Element.Not(source);
            Result(context, newElement);
        }
    }

    protected internal override void VisitExists(Context context, ExistsElement element)
    {
        var source = Transform<RuleElement>(context, element.Source)!;
        if (context.IsModified)
        {
            var newElement = Element.Exists(source);
            Result(context, newElement);
        }
    }

    protected internal override void VisitForAll(Context context, ForAllElement element)
    {
        var basePattern = Transform<PatternElement>(context, element.BasePattern)!;
        var patterns = element.Patterns.Select(x => Transform<PatternElement>(context, x)!).ToList();

        //forall -> not(base and not(patterns))

        var baseDeclaration = basePattern.Declaration;
        var baseParameter = baseDeclaration.ToParameterExpression();

        var negatedPatterns = new List<RuleElement>();
        foreach (var pattern in patterns)
        {
            var parameter = pattern.Declaration.ToParameterExpression();

            var expressions = new List<NamedExpressionElement>
            {
                Element.Condition(
                    Expression.Lambda(
                        Expression.ReferenceEqual(baseParameter, parameter),
                        baseParameter, parameter))
            };
            expressions.AddRange(pattern.Expressions);

            negatedPatterns.Add(
                Element.Not(
                    Element.Pattern(pattern.Declaration, expressions, pattern.Source)
                ));
        }

        var result = Element.Not(
            Element.AndGroup(
                new RuleElement[] { basePattern }.Concat(negatedPatterns)));
        Result(context, result);
    }

    private bool CollapseSingleGroup(Context context, ICollection<RuleElement> childElements)
    {
        if (childElements.Count != 1)
            return false;

        if (childElements.First() is not GroupElement e)
            return false;

        Result(context, e);
        return true;
    }

    private bool SplitOrGroup(Context context, IList<RuleElement> childElements)
    {
        if (!childElements.OfType<OrElement>().Any())
            return false;

        var groups = new List<ICollection<RuleElement>> { new List<RuleElement>() };
        ExpandOrElements(groups, childElements, 0);

        var andElements = groups.Select(Element.AndGroup);
        var orElement = Element.OrGroup(andElements);
        Result(context, orElement);
        return true;
    }

    private bool MergeOrGroups(Context context, IList<RuleElement> childElements)
    {
        if (!childElements.OfType<OrElement>().Any())
            return false;

        var orElement = Element.OrGroup(Flatten(childElements));
        Result(context, orElement);
        return true;

        static IEnumerable<RuleElement> Flatten(IEnumerable<RuleElement> elements)
        {
            foreach (var childElement in elements)
            {
                switch (childElement)
                {
                    case OrElement childOrElement:
                        {
                            foreach (var c in Flatten(childOrElement.ChildElements))
                            {
                                yield return c;
                            }

                            break;
                        }
                    default:
                        yield return childElement;
                        break;
                }
            }
        }
    }

    private static void ExpandOrElements(IList<ICollection<RuleElement>> groups, IList<RuleElement> childElements, int index)
    {
        while (true)
        {
            if (index == childElements.Count)
                return;

            var count = groups.Count;

            var currentElement = childElements[index++];


            switch (currentElement)
            {
                case OrElement orElement:
                    AddGroupElement(groups, count, orElement);
                    break;
                case { } ruleElement:
                    AddSingleElement(groups, count, ruleElement);
                    break;
            }
        }

        static void AddGroupElement(IList<ICollection<RuleElement>> groups, int count, GroupElement groupElement)
        {
            for (var i = 0; i < count; i++)
            {
                var offset = groups.Count;

                var firstChild = groupElement.ChildElements.First();
                var restChildren = groupElement.ChildElements.Skip(1).ToArray();

                for (var j = 0; j < restChildren.Length; j++)
                {
                    groups.Add(groups[i].ToList());
                    groups[offset + j].Add(restChildren[j]);
                }

                groups[i].Add(firstChild);
            }
        }

        static void AddSingleElement(IList<ICollection<RuleElement>> groups, int count, RuleElement currentElement)
        {
            for (var i = 0; i < count; i++)
            {
                groups[i].Add(currentElement);
            }
        }
    }
}