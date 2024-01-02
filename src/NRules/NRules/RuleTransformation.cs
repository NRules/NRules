using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules;

internal class RuleTransformation : RuleElementVisitor<Context>
{
    public IRuleDefinition Transform(IRuleDefinition rule)
    {
        return Visit(Context.Empty, rule);
    }
    
    protected override RuleElement VisitAnd(Context context, AndElement element)
    {
        var childElements = Visit(context, element.ChildElements, Visit);
        
        if (CollapseSingleGroup(childElements, out var collapseResult)) return collapseResult;
        if (SplitOrGroup(childElements, out var splitResult)) return splitResult;
        
        if (!ReferenceEquals(childElements, element.ChildElements))
            return Element.AndGroup(childElements);
        return element;
    }

    protected override RuleElement VisitOr(Context context, OrElement element)
    {
        var childElements = Visit(context, element.ChildElements, Visit);
        
        if (CollapseSingleGroup(childElements, out var collapseResult)) return collapseResult;
        if (MergeOrGroups(childElements, out var mergeResult)) return mergeResult;

        if (!ReferenceEquals(childElements, element.ChildElements))
            return Element.OrGroup(childElements);
        return element;
    }

    protected override RuleElement VisitForAll(Context context, ForAllElement element)
    {
        var basePattern = VisitPattern(context, element.BasePattern);
        var patterns = Visit(context, element.Patterns, VisitPattern);

        //forall -> not(base and not(patterns))

        Declaration baseDeclaration = basePattern.Declaration;
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

        return result;
    }

    private bool CollapseSingleGroup(IReadOnlyCollection<RuleElement> childElements, out RuleElement result)
    {
        if (childElements.Count == 1 &&
            childElements.Single() is GroupElement ge)
        {
            result = ge;
            return true;
        }

        result = null;
        return false;
    }

    private bool SplitOrGroup(IReadOnlyCollection<RuleElement> childElements, out RuleElement result)
    {
        if (!childElements.OfType<OrElement>().Any())
        {
            result = null;
            return false;
        }

        var groups = new List<List<RuleElement>>();
        groups.Add(new List<RuleElement>());
        ExpandOrElements(groups, childElements.ToList(), 0);

        var andElements = groups.Select(Element.AndGroup).ToList();
        result = Element.OrGroup(andElements);
        return true;
    }

    private bool MergeOrGroups(IReadOnlyCollection<RuleElement> childElements, out RuleElement result)
    {
        if (!childElements.OfType<OrElement>().Any())
        {
            result = null;
            return false;
        }
        
        var newChildElements = new List<RuleElement>();
        foreach (var childElement in childElements)
        {
            if (childElement is OrElement childOrElement)
            {
                newChildElements.AddRange(childOrElement.ChildElements);
            }
            else
            {
                newChildElements.Add(childElement);
            }

        }
        result = Element.OrGroup(newChildElements);
        return true;
    }

    private void ExpandOrElements(List<List<RuleElement>> groups, List<RuleElement> childElements, int index)
    {
        if (index == childElements.Count) return;

        var currentElement = childElements[index];
        var orElement = currentElement as OrElement;

        var count = groups.Count;
        for (int i = 0; i < count; i++)
        {
            if (orElement != null)
            {
                int offset = groups.Count;
                var orElementChildren = orElement.ChildElements.ToList();
                var firstChild = orElementChildren.First();
                var restChildren = orElementChildren.Skip(1).ToList();
                for (int j = 0; j < restChildren.Count; j++)
                {
                    groups.Add(new List<RuleElement>(groups[i]));
                    groups[offset + j].Add(restChildren[j]);
                }
                groups[i].Add(firstChild);
            }
            else
            {
                groups[i].Add(currentElement);
            }
        }

        ExpandOrElements(groups, childElements, index + 1);
    }
}