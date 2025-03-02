using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions;

internal class FilterExpression(FilterGroupBuilder builder, SymbolStack symbolStack) : IFilterExpression
{
    public IFilterExpression OnChange(params Expression<Func<object>>[] keySelectors)
    {
        foreach (var keySelector in keySelectors)
        {
            var expression = keySelector.DslExpression(symbolStack.Scope);
            builder.Filter(FilterType.KeyChange, expression);
        }
        return this;
    }

    public IFilterExpression Where(params Expression<Func<bool>>[] predicates)
    {
        foreach (var predicate in predicates)
        {
            var expression = predicate.DslExpression(symbolStack.Scope);
            builder.Filter(FilterType.Predicate, expression);
        }
        return this;
    }
}