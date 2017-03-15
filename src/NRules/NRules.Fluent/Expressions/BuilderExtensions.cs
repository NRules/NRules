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
        public static void DslCondition<TFact>(this PatternBuilder builder, IEnumerable<Declaration> declarations, Expression<Func<TFact, bool>> condition)
        {
            builder.DslConditions(builder.Declarations, Enumerable.Repeat(condition, 1));
        }
        
        public static void DslConditions<TFact>(this PatternBuilder builder, IEnumerable<Declaration> declarations, IEnumerable<Expression<Func<TFact, bool>>> conditions)
        {
            var availableDeclarations = declarations.ToArray();
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(builder.Declaration, availableDeclarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                builder.Condition(rewrittenCondition);
            }
        }

        public static void DslAction(this ActionGroupBuilder builder, IEnumerable<Declaration> declarations, Expression<Action<IContext>> action)
        {
            var rewriter = new ActionRewriter(declarations);
            var rewrittenAction = rewriter.Rewrite(action);
            builder.Action(rewrittenAction);
        }
    }
}
