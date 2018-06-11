using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class LeftHandSideExpression : ILeftHandSideExpression
    {
        private readonly RuleBuilder _builder;
        private readonly GroupBuilder _groupBuilder;

        public LeftHandSideExpression(RuleBuilder builder)
        {
            _builder = builder;
            _groupBuilder = builder.LeftHandSide();
        }

        public LeftHandSideExpression(RuleBuilder builder, GroupBuilder groupBuilder)
        {
            _builder = builder;
            _groupBuilder = groupBuilder;
        }

        public ILeftHandSideExpression Match<TFact>(Expression<Func<TFact>> alias, params Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = alias.ToParameterExpression();
            var patternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression Match<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = Expression.Parameter(typeof (TFact));
            var patternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression Exists<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            var existsBuilder = _groupBuilder.Exists();
            var patternBuilder = existsBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression Not<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            var notBuilder = _groupBuilder.Not();
            var patternBuilder = notBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> condition)
        {
            return ForAll(x => true, condition);
        }

        public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions)
        {
            return ForAll(baseCondition, conditions);
        }

        private ILeftHandSideExpression ForAll<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions)
        {
            var forallBuilder = _groupBuilder.ForAll();

            var basePatternBuilder = forallBuilder.BasePattern(typeof(TFact));
            basePatternBuilder.DslConditions(_groupBuilder.Declarations, baseCondition);

            var patternBuilder = forallBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression Query<TResult>(Expression<Func<TResult>> alias, Func<IQuery, IQuery<TResult>> queryAction)
        {
            var symbol = alias.ToParameterExpression();
            var queryBuilder = new QueryExpression(_groupBuilder);
            queryAction(queryBuilder);
            queryBuilder.Build(symbol.Name);
            return this;
        }

        public ILeftHandSideExpression And(Action<ILeftHandSideExpression> builderAction)
        {
            var expressionBuilder = new LeftHandSideExpression(_builder, _groupBuilder.Group(GroupType.And));
            builderAction(expressionBuilder);
            return this;
        }

        public ILeftHandSideExpression Or(Action<ILeftHandSideExpression> builderAction)
        {
            var expressionBuilder = new LeftHandSideExpression(_builder, _groupBuilder.Group(GroupType.Or));
            builderAction(expressionBuilder);
            return this;
        }

        public ILeftHandSideExpression Let<TResult>(Expression<Func<TResult>> alias, Expression<Func<TResult>> expression)
        {
            var symbol = alias.ToParameterExpression();
            var patternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);
            var bindingBuilder = patternBuilder.Binding();
            bindingBuilder.DslBindingExpression(_groupBuilder.Declarations, expression);
            return this;
        }

        public ILeftHandSideExpression Having(params Expression<Func<bool>>[] conditions)
        {
            var patternBuilder = _groupBuilder.NestedBuilders.OfType<PatternBuilder>().LastOrDefault();
            if (patternBuilder == null)
            {
                throw new ArgumentException("HAVING clause can only be used on existing rule patterns");
            }
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }
    }
}
