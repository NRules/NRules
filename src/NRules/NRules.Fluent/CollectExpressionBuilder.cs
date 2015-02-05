using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.Fluent.Expressions;
using NRules.RuleModel.Builders;

namespace NRules.Fluent
{
    internal class CollectExpressionBuilder<TCollection> : ExpressionBuilderDecorator, ICollectPattern<TCollection>
    {
        private readonly PatternBuilder _patternBuilder;

        public CollectExpressionBuilder(ExpressionBuilder builder, PatternBuilder patternBuilder) : base(builder)
        {
            _patternBuilder = patternBuilder;
        }

        public ILeftHandSide Where(params Expression<Func<TCollection, bool>>[] conditions)
        {
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(_patternBuilder.Declaration, _patternBuilder.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                _patternBuilder.Condition(rewrittenCondition);
            }
            return Builder;
        }
    }
}