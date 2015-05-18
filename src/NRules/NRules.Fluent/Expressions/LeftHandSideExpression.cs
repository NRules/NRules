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
        private IContinuationExpression _continuationExpression;

        public LeftHandSideExpression(RuleBuilder builder)
        {
            _builder = builder;
            _groupBuilder = Builder.LeftHandSide();
        }

        public LeftHandSideExpression(RuleBuilder builder, GroupBuilder groupBuilder)
        {
            _builder = builder;
            _groupBuilder = groupBuilder;
        }

        public RuleBuilder Builder
        {
            get { return _builder; }
        }

        public GroupBuilder GroupBuilder
        {
            get { return _groupBuilder; }
        }

        public ILeftHandSideExpression Match<TFact>(Expression<Func<TFact>> alias, params Expression<Func<TFact, bool>>[] conditions)
        {
            CompleteContinuation();
            var symbol = alias.ToParameterExpression();
            var patternBuilder = GroupBuilder.Pattern(symbol.Type, symbol.Name);
            patternBuilder.DslConditions(GroupBuilder.Declarations, conditions);
            return this;
        }

        public IContinuationExpression<TFact> Match<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            CompleteContinuation();
            var matchContinuation = new ContinuationExpression<TFact>(this);
            matchContinuation.Match(conditions);
            RegisterContinuation(matchContinuation);
            return new ContinuationExpression<TFact>(this, matchContinuation);
        }

        public IConditionExpression<IEnumerable<TFact>> Collect<TFact>(Expression<Func<IEnumerable<TFact>>> alias, params Expression<Func<TFact, bool>>[] conditions)
        {
            CompleteContinuation();
            return Match(conditions).Collect(alias);
        }

        public ILeftHandSideExpression Exists<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            CompleteContinuation();
            var existsBuilder = GroupBuilder.Exists();

            var patternBuilder = existsBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(GroupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression Not<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            CompleteContinuation();
            var notBuilder = GroupBuilder.Not();

            var patternBuilder = notBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(GroupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> condition)
        {
            CompleteContinuation();
            return All(x => true, new[] { condition });
        }

        public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions)
        {
            CompleteContinuation();
            return All(baseCondition, conditions.AsEnumerable());
        }

        private ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> baseCondition, IEnumerable<Expression<Func<TFact, bool>>> conditions)
        {
            var forallBuilder = GroupBuilder.ForAll();

            var basePatternBuilder = forallBuilder.BasePattern(typeof(TFact));
            basePatternBuilder.DslCondition(GroupBuilder.Declarations, baseCondition);

            var patternBuilder = forallBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(GroupBuilder.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression And(Action<ILeftHandSideExpression> builderAction)
        {
            CompleteContinuation();
            var expressionBuilder = new LeftHandSideExpression(Builder, GroupBuilder.Group(GroupType.And));
            builderAction(expressionBuilder);
            return this;
        }

        public ILeftHandSideExpression Or(Action<ILeftHandSideExpression> builderAction)
        {
            CompleteContinuation();
            var expressionBuilder = new LeftHandSideExpression(Builder, GroupBuilder.Group(GroupType.Or));
            builderAction(expressionBuilder);
            return this;
        }

        public void RegisterContinuation(IContinuationExpression continuationExpression)
        {
            _continuationExpression = continuationExpression;
        }

        public void CompleteContinuation()
        {
            if (_continuationExpression != null)
            {
                _continuationExpression.Complete(GroupBuilder);
                _continuationExpression = null;
            }
        }
    }
}