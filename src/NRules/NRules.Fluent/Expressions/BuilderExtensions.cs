using System;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions;

internal static class BuilderExtensions
{
    public static void DslConditions<TFact>(this PatternBuilder builder, ISymbolLookup symbolLookup, params Expression<Func<TFact, bool>>[] conditions)
    {
        var rewriter = new PatternExpressionRewriter(builder.Declaration, symbolLookup);
        foreach (var condition in conditions)
        {
            var rewrittenCondition = rewriter.Rewrite(condition);
            builder.Condition(rewrittenCondition);
        }
    }

    public static void DslConditions(this PatternBuilder builder, ISymbolLookup symbolLookup, params Expression<Func<bool>>[] conditions)
    {
        var rewriter = new ExpressionRewriter(symbolLookup);
        foreach (var condition in conditions)
        {
            var rewrittenCondition = rewriter.Rewrite(condition);
            builder.Condition(rewrittenCondition);
        }
    }

    public static void DslBindingExpression(this BindingBuilder builder, ISymbolLookup symbolLookup, LambdaExpression expression)
    {
        var rewriter = new ExpressionRewriter(symbolLookup);
        var rewrittenExpression = rewriter.Rewrite(expression);
        builder.BindingExpression(rewrittenExpression);
    }

    public static void DslAction(this ActionGroupBuilder builder, ISymbolLookup symbolLookup, Expression<Action<IContext>> action, ActionTrigger actionTrigger)
    {
        var rewriter = new ExpressionRewriter(symbolLookup);
        var rewrittenAction = rewriter.Rewrite(action);
        builder.Action(rewrittenAction, actionTrigger);
    }

    public static LambdaExpression DslPatternExpression(this PatternBuilder builder, ISymbolLookup symbolLookup, LambdaExpression expression)
    {
        var rewriter = new PatternExpressionRewriter(builder.Declaration, symbolLookup);
        var rewrittenExpression = rewriter.Rewrite(expression);
        return rewrittenExpression;
    }

    public static LambdaExpression DslExpression(this LambdaExpression expression, ISymbolLookup symbolLookup)
    {
        var rewriter = new ExpressionRewriter(symbolLookup);
        var rewrittenExpression = rewriter.Rewrite(expression);
        return rewrittenExpression;
    }
}
