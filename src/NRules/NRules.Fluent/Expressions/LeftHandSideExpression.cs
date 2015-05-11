using System;
using System.Collections.Generic;
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
            _groupBuilder = _builder.LeftHandSide();
        }

        public LeftHandSideExpression(RuleBuilder builder, GroupBuilder groupBuilder)
        {
            _builder = builder;
            _groupBuilder = groupBuilder;
        }

        public ILeftHandSideExpression Match<TFact>(Expression<Func<TFact>> alias, params Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = alias.ToParameterExpression();
            return Match(symbol, conditions);
        }

        public ILeftHandSideExpression Match<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = Expression.Parameter(typeof(TFact));
            return Match(symbol, conditions);
        }

        private ILeftHandSideExpression Match<TFact>(ParameterExpression symbol, IEnumerable<Expression<Func<TFact, bool>>> conditions)
        {
            var patternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ICollectPatternExpression<TFact> Collect<TFact>(Expression<Func<IEnumerable<TFact>>> alias, params Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = alias.ToParameterExpression();

            var collectionPatternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);

            var aggregateBuilder = collectionPatternBuilder.Aggregate();
            aggregateBuilder.CollectionOf(typeof (TFact));

            var itemPatternBuilder = aggregateBuilder.Pattern(typeof (TFact));
            itemPatternBuilder.DslConditions(_groupBuilder.Declarations, conditions);

            return new CollectPatternExpression<TFact>(_builder, _groupBuilder, collectionPatternBuilder);
        }

        public IGroupByPatternExpression<TKey, TFact> GroupBy<TKey, TFact>(Expression<Func<IGrouping<TKey, TFact>>> alias, Expression<Func<TFact, TKey>> keySelector, params Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = alias.ToParameterExpression();

            var groupByPatternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);

            var aggregateBuilder = groupByPatternBuilder.Aggregate();
            aggregateBuilder.GroupBy(keySelector);

            var itemPatternBuilder = aggregateBuilder.Pattern(typeof (TFact));
            itemPatternBuilder.DslConditions(_groupBuilder.Declarations, conditions);

            return new GroupByPatternExpression<TKey, TFact>(_builder, _groupBuilder, groupByPatternBuilder);
        }

        public ILeftHandSideExpression Exists<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            var existsBuilder = _groupBuilder.Exists();

            var patternBuilder = existsBuilder.Pattern(typeof (TFact));
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
            return All(x => true, new [] {condition});
        }

        public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions)
        {
            return All(baseCondition, conditions.AsEnumerable());
        }

        private ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> baseCondition, IEnumerable<Expression<Func<TFact, bool>>> conditions)
        {
            var forallBuilder = _groupBuilder.ForAll();
            
            var basePatternBuilder = forallBuilder.BasePattern(typeof(TFact));
            basePatternBuilder.DslCondition(_groupBuilder.Declarations, baseCondition);

            var patternBuilder = forallBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
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
    }
}