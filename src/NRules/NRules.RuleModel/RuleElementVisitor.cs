using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NRules.RuleModel.Builders;

namespace NRules.RuleModel;

/// <summary>
/// Empty context for cases where traversal context is not needed.
/// </summary>
public class Context
{
    /// <summary>
    /// Empty context instance.
    /// </summary>
    public static Context Empty { get; } = new();
}

/// <summary>
/// Visitor to traverse or rewrite rule definition (or its part).
/// </summary>
/// <typeparam name="TContext">Traversal context.</typeparam>
public class RuleElementVisitor<TContext>
{
    /// <summary>
    /// Visits a rule definition and all its nodes.
    /// </summary>
    /// <param name="context">Traversal context.</param>
    /// <param name="rule">Rule definition.</param>
    /// <returns>The original or a new rewritten rule definition.</returns>
    public IRuleDefinition Visit(TContext context, IRuleDefinition rule)
    {
        var dependencyGroup = rule.DependencyGroup;
        var newDependencyGroup = VisitDependencyGroup(context, dependencyGroup);
        
        var lhs = rule.LeftHandSide;
        var newLhs = (GroupElement) Visit(context, lhs);

        var filterGroup = rule.FilterGroup;
        var newFilterGroup = VisitFilterGroup(context, filterGroup);
        
        var rhs = rule.RightHandSide;
        var newRhs = VisitActionGroup(context, rhs);
        
        if (!ReferenceEquals(newLhs, lhs) || !ReferenceEquals(newRhs, rhs))
        {
            var transformedRule = Element.RuleDefinition(rule.Name, rule.Description, rule.Priority,
                rule.Repeatability, rule.Tags, rule.Properties, newDependencyGroup, newLhs, newFilterGroup, newRhs);
            return transformedRule;
        }
        return rule;
    }
    
    /// <summary>
    /// Visits a rule element and all its descendant nodes.
    /// </summary>
    /// <param name="context">Traversal context.</param>
    /// <param name="element">Rule element.</param>
    /// <returns>The original or a new rewritten rule element.</returns>
    [return:NotNullIfNotNull("element")]
    public RuleElement? Visit(TContext context, RuleElement? element)
    {
        return element?.Accept(context, this);
    }
    
    /// <summary>
    /// Visits each element in the collection and all their descendant nodes.
    /// If any of the elements is rewritten, a new collection is returned.
    /// </summary>
    /// <param name="context">Traversal context.</param>
    /// <param name="elements">Collection of rule elements to visit.</param>
    /// <param name="visitFunc">Concrete visitor delegate.</param>
    /// <typeparam name="T">Type of rule elements to visit.</typeparam>
    /// <returns>The original or a new rewritten collection of rule elements.</returns>
    public static IReadOnlyList<T> Visit<T>(TContext context, IReadOnlyList<T> elements, Func<TContext, T, T> visitFunc)
        where T : RuleElement
    {
        T[]? newElements = null;
        for (var i = 0; i < elements.Count; i++)
        {
            var element = elements[i];
            var newElement = visitFunc(context, element);
            if (newElements != null)
            {
                newElements[i] = newElement;
            }
            else if (!ReferenceEquals(newElement, element))
            {
                newElements = new T[elements.Count];
                for (var j = 0; j < i; j++)
                {
                    newElements[j] = elements[j];
                }
                newElements[i] = newElement;
            }
        }
        
        return newElements ?? elements;
    }

    protected internal virtual PatternElement VisitPattern(TContext context, PatternElement element)
    {
        var expressions = VisitExpressions(context, element.Expressions);
        var source = Visit(context, element.Source);
        return element.Update(expressions, source);
    }

    protected internal virtual RuleElement VisitBinding(TContext context, BindingElement element)
    {
        return element;
    }

    protected internal virtual RuleElement VisitAggregate(TContext context, AggregateElement element)
    {
        var expressions = VisitExpressions(context, element.Expressions);
        var source = VisitPattern(context, element.Source);
        return element.Update(expressions, source);
    }

    protected internal virtual NamedExpressionElement VisitNamedExpression(TContext context, NamedExpressionElement element)
    {
        return element;
    }

    protected internal virtual RuleElement VisitNot(TContext context, NotElement element)
    {
        var source = Visit(context, element.Source);
        return element.Update(source);
    }

    protected internal virtual RuleElement VisitExists(TContext context, ExistsElement element)
    {
        var source = Visit(context, element.Source);
        return element.Update(source);
    }

    protected internal virtual RuleElement VisitForAll(TContext context, ForAllElement element)
    {
        var basePattern = VisitPattern(context, element.BasePattern);
        var patterns = Visit(context, element.Patterns, VisitPattern);
        return element.Update(basePattern, patterns);
    }
    
    protected internal virtual RuleElement VisitAnd(TContext context, AndElement element)
    {
        var childElements = Visit(context, element.ChildElements, Visit);
        return element.Update(childElements);
    }

    protected internal virtual RuleElement VisitOr(TContext context, OrElement element)
    {
        var childElements = Visit(context, element.ChildElements, Visit);
        return element.Update(childElements);
    }

    protected internal virtual ActionGroupElement VisitActionGroup(TContext context, ActionGroupElement element)
    {
        var actions = Visit(context, element.Actions, VisitAction);
        return element.Update(actions);
    }

    protected internal virtual ActionElement VisitAction(TContext context, ActionElement element)
    {
        return element;
    }

    protected internal virtual DependencyGroupElement VisitDependencyGroup(TContext context, DependencyGroupElement element)
    {
        var dependencies = Visit(context, element.Dependencies, VisitDependency);
        return element.Update(dependencies);
    }
    
    protected internal virtual DependencyElement VisitDependency(TContext context, DependencyElement element)
    {
        return element;
    }

    protected internal virtual FilterGroupElement VisitFilterGroup(TContext context, FilterGroupElement element)
    {
        var filters = Visit(context, element.Filters, VisitFilter);
        return element.Update(filters);
    }

    protected internal virtual FilterElement VisitFilter(TContext context, FilterElement element)
    {
        return element;
    }
    
    private ExpressionCollection VisitExpressions(TContext context, ExpressionCollection expressions)
    {
        var newExpressions = Visit(context, expressions, VisitNamedExpression);
        return expressions.Update(newExpressions);
    }
}