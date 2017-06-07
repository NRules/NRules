using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal static class BuilderExtensions
    {
        public static void DslConditions<TFact>(this PatternBuilder builder, IEnumerable<Declaration> declarations, params Expression<Func<TFact, bool>>[] conditions)
        {
            var rewriter = new PatternExpressionRewriter(builder.Declaration, declarations.ToArray());
            foreach (var condition in conditions)
            {
                var rewrittenCondition = rewriter.Rewrite(condition);
                builder.Condition(rewrittenCondition);
            }
        }

        public static void DslAction(this ActionGroupBuilder builder, IEnumerable<Declaration> declarations, Expression<Action<IContext>> action)
        {
            var rewriter = new ExpressionRewriter(declarations);
            var rewrittenAction = rewriter.Rewrite(action);
            builder.Action(rewrittenAction);
        }

        public static LambdaExpression DslPatternExpression(this PatternBuilder builder, IEnumerable<Declaration> declarations, LambdaExpression expression)
        {
            var rewriter = new PatternExpressionRewriter(builder.Declaration, declarations);
            var rewrittenExpression = rewriter.Rewrite(expression);
            return rewrittenExpression;
        }
    }
}
