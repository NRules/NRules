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

        public ILeftHandSideExpression Match<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions)
        {
            var symbol = alias.ToParameterExpression();
            return Match(symbol, conditions);
        }

        public ILeftHandSideExpression Match<T>(Expression<Func<T, bool>> condition, params Expression<Func<T, bool>>[] conditions)
        {
            var symbol = Expression.Parameter(typeof(T));
            return Match(symbol, Enumerable.Repeat(condition, 1).Union(conditions));
        }

        public ILeftHandSideExpression Match<T>()
        {
            var symbol = Expression.Parameter(typeof(T));
            return Match(symbol, new Expression<Func<T, bool>>[] { });
        }

        private ILeftHandSideExpression Match<T>(ParameterExpression symbol, IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            var patternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ICollectPatternExpression<IEnumerable<T>> Collect<T>(Expression<Func<IEnumerable<T>>> alias, params Expression<Func<T, bool>>[] conditions)
        {
            var symbol = alias.ToParameterExpression();

            var collectionPatternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);

            var aggregateBuilder = collectionPatternBuilder.Aggregate();
            aggregateBuilder.CollectionOf(typeof (T));

            var itemPatternBuilder = aggregateBuilder.Pattern(typeof (T));
            itemPatternBuilder.DslConditions(_groupBuilder.Declarations, conditions);

            return new CollectPatternExpression<IEnumerable<T>>(_builder, _groupBuilder, collectionPatternBuilder);
        }

        public ILeftHandSideExpression Exists<T>(params Expression<Func<T, bool>>[] conditions)
        {
            var existsBuilder = _groupBuilder.Exists();

            var patternBuilder = existsBuilder.Pattern(typeof (T));
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression Not<T>(params Expression<Func<T, bool>>[] conditions)
        {
            var notBuilder = _groupBuilder.Not();

            var patternBuilder = notBuilder.Pattern(typeof(T));
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression All<T>(Expression<Func<T, bool>> condition)
        {
            return All(x => true, new [] {condition});
        }

        public ILeftHandSideExpression All<T>(Expression<Func<T, bool>> baseCondition, params Expression<Func<T, bool>>[] conditions)
        {
            return All(baseCondition, conditions.AsEnumerable());
        }

        private ILeftHandSideExpression All<T>(Expression<Func<T, bool>> baseCondition, IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            var forallBuilder = _groupBuilder.ForAll();
            
            var basePatternBuilder = forallBuilder.BasePattern(typeof(T));
            basePatternBuilder.DslCondition(_groupBuilder.Declarations, baseCondition);

            var patternBuilder = forallBuilder.Pattern(typeof(T));
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