using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions;

internal class LeftHandSideExpression(GroupBuilder builder, SymbolStack symbolStack) : ILeftHandSideExpression
{
    private PatternBuilder? _currentPatternBuilder;

    public ILeftHandSideExpression Match<TFact>(Expression<Func<TFact>> alias, params Expression<Func<TFact, bool>>[] conditions)
        where TFact : notnull
    {
        var symbol = alias.ToParameterExpression();
        var patternBuilder = builder.Pattern(symbol.Type, symbol.Name);
        patternBuilder.DslConditions(symbolStack.Scope, conditions);
        symbolStack.Scope.Add(patternBuilder.Declaration);
        _currentPatternBuilder = patternBuilder;
        return this;
    }

    public ILeftHandSideExpression Match<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        where TFact : notnull
    {
        var symbol = Expression.Parameter(typeof (TFact));
        var patternBuilder = builder.Pattern(symbol.Type, symbol.Name);
        patternBuilder.DslConditions(symbolStack.Scope, conditions);
        symbolStack.Scope.Add(patternBuilder.Declaration);
        _currentPatternBuilder = patternBuilder;
        return this;
    }

    public ILeftHandSideExpression Exists<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        where TFact : notnull
    {
        var existsBuilder = builder.Exists();
        var patternBuilder = existsBuilder.Pattern(typeof(TFact));
        patternBuilder.DslConditions(symbolStack.Scope, conditions);
        return this;
    }

    public ILeftHandSideExpression Not<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        where TFact : notnull
    {
        var notBuilder = builder.Not();
        var patternBuilder = notBuilder.Pattern(typeof(TFact));
        patternBuilder.DslConditions(symbolStack.Scope, conditions);
        return this;
    }

    public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> condition)
        where TFact : notnull
    {
        return ForAll(x => true, condition);
    }

    public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions)
        where TFact : notnull
    {
        return ForAll(baseCondition, conditions);
    }

    private ILeftHandSideExpression ForAll<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions)
        where TFact : notnull
    {
        var forallBuilder = builder.ForAll();

        var basePatternBuilder = forallBuilder.BasePattern(typeof(TFact));
        basePatternBuilder.DslConditions(symbolStack.Scope, baseCondition);

        var patternBuilder = forallBuilder.Pattern(typeof(TFact));
        patternBuilder.DslConditions(symbolStack.Scope, conditions);
        return this;
    }

    public ILeftHandSideExpression Query<TResult>(Expression<Func<TResult>> alias, Func<IQuery, IQuery<TResult>> queryAction)
        where TResult : notnull
    {
        var symbol = alias.ToParameterExpression();
        var queryBuilder = new QueryExpression(symbol, symbolStack, builder);
        queryAction(queryBuilder);
        _currentPatternBuilder = queryBuilder.Build();
        return this;
    }

    public ILeftHandSideExpression And(Action<ILeftHandSideExpression> builderAction)
    {
        var expressionBuilder = new LeftHandSideExpression(builder.Group(GroupType.And), symbolStack);
        builderAction(expressionBuilder);
        return this;
    }

    public ILeftHandSideExpression Or(Action<ILeftHandSideExpression> builderAction)
    {
        var expressionBuilder = new LeftHandSideExpression(builder.Group(GroupType.Or), symbolStack);
        builderAction(expressionBuilder);
        return this;
    }

    public ILeftHandSideExpression Let<TResult>(Expression<Func<TResult>> alias, Expression<Func<TResult>> expression)
    {
        var symbol = alias.ToParameterExpression();
        var patternBuilder = builder.Pattern(symbol.Type, symbol.Name);
        var bindingBuilder = patternBuilder.Binding();
        bindingBuilder.DslBindingExpression(symbolStack.Scope, expression);
        symbolStack.Scope.Add(patternBuilder.Declaration);
        _currentPatternBuilder = patternBuilder;
        return this;
    }

    public ILeftHandSideExpression Having(params Expression<Func<bool>>[] conditions)
    {
        if (_currentPatternBuilder == null)
        {
            throw new ArgumentException("HAVING clause can only be used on existing rule patterns");
        }
        _currentPatternBuilder.DslConditions(symbolStack.Scope, conditions);
        return this;
    }
}
