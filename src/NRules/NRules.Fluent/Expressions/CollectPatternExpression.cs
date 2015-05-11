using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class CollectPatternExpression<TFact> : LeftHandSideExpression, ICollectPatternExpression<TFact>
    {
        private readonly PatternBuilder _patternBuilder;

        public CollectPatternExpression(RuleBuilder builder, GroupBuilder groupBuilder, PatternBuilder patternBuilder) 
            : base(builder, groupBuilder)
        {
            _patternBuilder = patternBuilder;
        }

        public ILeftHandSideExpression Where(params Expression<Func<IEnumerable<TFact>, bool>>[] conditions)
        {
            _patternBuilder.DslConditions(_patternBuilder.Declarations, conditions);
            return this;
        }
    }
}