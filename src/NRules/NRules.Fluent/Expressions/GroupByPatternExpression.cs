using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class GroupByPatternExpression<TKey, TElement> : LeftHandSideExpression, IGroupByPatternExpression<TKey, TElement>
    {
        private readonly PatternBuilder _patternBuilder;

        public GroupByPatternExpression(RuleBuilder builder, GroupBuilder groupBuilder, PatternBuilder patternBuilder) 
            : base(builder, groupBuilder)
        {
            _patternBuilder = patternBuilder;
        }

        public ILeftHandSideExpression Where(params Expression<Func<IGrouping<TKey, TElement>, bool>>[] conditions)
        {
            _patternBuilder.DslConditions(_patternBuilder.Declarations, conditions);
            return this;
        }
    }
}