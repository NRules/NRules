using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.Fluent.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent
{
    internal class ExpressionBuilder : ILeftHandSide, IRightHandSide
    {
        private readonly RuleBuilder _builder;
        private readonly GroupBuilderChain _groupBuilders;

        public ExpressionBuilder(RuleBuilder builder)
        {
            _builder = builder;
            _groupBuilders = new GroupBuilderChain(_builder.LeftHandSide());
        }

        public ILeftHandSide Match<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions)
        {
            var patternSymbol = ExtractSymbol(alias);
            return Match(patternSymbol, conditions);
        }

        public ILeftHandSide Match<T>(Expression<Func<T, bool>> condition, params Expression<Func<T, bool>>[] conditions)
        {
            var patternSymbol = Expression.Parameter(typeof (T));
            return Match(patternSymbol, Enumerable.Repeat(condition, 1).Union(conditions));
        }

        public ILeftHandSide Match<T>()
        {
            var patternSymbol = Expression.Parameter(typeof(T));
            return Match(patternSymbol, new Expression<Func<T, bool>>[] {});
        }

        private ILeftHandSide Match<T>(ParameterExpression symbol, IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            var groupBuilder = _groupBuilders.Current;
            var patternBuilder = groupBuilder.Pattern(symbol.Type, symbol.Name);
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, groupBuilder.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ICollectPattern<IEnumerable<T>> Collect<T>(Expression<Func<IEnumerable<T>>> alias, params Expression<Func<T, bool>>[] itemConditions)
        {
            var collectionSymbol = ExtractSymbol(alias);
            var groupBuilder = _groupBuilders.Current;

            var outerPatternBuilder = groupBuilder.Pattern(collectionSymbol.Type, collectionSymbol.Name);

            var aggregateBuilder = outerPatternBuilder.Aggregate();
            aggregateBuilder.CollectionOf(typeof (T));

            var patternBuilder = aggregateBuilder.Pattern(typeof (T));
            foreach (var condition in itemConditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, groupBuilder.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return new CollectExpressionBuilder<IEnumerable<T>>(this, outerPatternBuilder);
        }

        public ILeftHandSide Exists<T>(params Expression<Func<T, bool>>[] conditions)
        {
            var groupBuilder = _groupBuilders.Current;
            var existsBuilder = groupBuilder.Exists();

            var patternBuilder = existsBuilder.Pattern(typeof (T));
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, groupBuilder.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide Not<T>(params Expression<Func<T, bool>>[] conditions)
        {
            var groupBuilder = _groupBuilders.Current;
            var notBuilder = groupBuilder.Not();

            var patternBuilder = notBuilder.Pattern(typeof(T));
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, groupBuilder.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide All<T>(Expression<Func<T, bool>> condition)
        {
            return All(x => true, new [] {condition});
        }

        public ILeftHandSide And(Action<ILeftHandSide> builder)
        {
            using (_groupBuilders.BeginGroup(GroupType.And))
            {
                builder(this);
            }
            return this;
        }

        public ILeftHandSide Or(Action<ILeftHandSide> builder)
        {
            using (_groupBuilders.BeginGroup(GroupType.Or))
            {
                builder(this);
            }
            return this;
        }

        public ILeftHandSide All<T>(Expression<Func<T, bool>> baseCondition, params Expression<Func<T, bool>>[] conditions)
        {
            return ForAll(baseCondition, conditions);
        }

        public IRightHandSide Do(Expression<Action<IContext>> action)
        {
            var rightHandSide = _builder.RightHandSide();

            var rewriter = new ActionRewriter(rightHandSide.Declarations);
            var rewrittenAction = rewriter.Rewrite(action);
            rightHandSide.Action(rewrittenAction);

            return this;
        }

        private ILeftHandSide ForAll<T>(Expression<Func<T, bool>> baseCondition, IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            var leftHandSide = _builder.LeftHandSide();

            var forallBuilder = leftHandSide.ForAll();
            var basePatternBuilder = forallBuilder.BasePattern(typeof(T));
            {
                var rewriter = new ConditionRewriter(basePatternBuilder.Declaration, leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(baseCondition);
                basePatternBuilder.Condition(rewrittenCondition);
            }
            var patternBuilder = forallBuilder.Pattern(typeof(T));
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        private static ParameterExpression ExtractSymbol<T>(Expression<Func<T>> alias)
        {
            if (alias == null)
            {
                throw new ArgumentNullException("alias", "Pattern alias is null");
            }
            var fieldMember = alias.Body as MemberExpression;
            if (fieldMember == null)
            {
                throw new ArgumentException(
                    string.Format("Invalid pattern alias expression. Expected={0}, Actual={1}",
                        typeof(MemberExpression), alias.Body.GetType()));
            }
            return Expression.Parameter(fieldMember.Type, fieldMember.Member.Name);
        }
    }
}