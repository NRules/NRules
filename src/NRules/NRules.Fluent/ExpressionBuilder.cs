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

        public ExpressionBuilder(RuleBuilder builder)
        {
            _builder = builder;
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
            var leftHandSide = _builder.LeftHandSide();

            var patternBuilder = leftHandSide.Pattern(symbol.Type, symbol.Name);
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide Collect<T>(Expression<Func<IEnumerable<T>>> alias, params Expression<Func<T, bool>>[] itemConditions)
        {
            var collectionSymbol = ExtractSymbol(alias);
            var leftHandSide = _builder.LeftHandSide();

            var outerPatternBuilder = leftHandSide.Pattern(collectionSymbol.Type, collectionSymbol.Name);

            var aggregateBuilder = outerPatternBuilder.SourceAggregate();
            aggregateBuilder.CollectionOf(typeof (T));

            var patternBuilder = aggregateBuilder.SourcePattern(typeof (T));
            foreach (var condition in itemConditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide Exists<T>(params Expression<Func<T, bool>>[] conditions)
        {
            var leftHandSide = _builder.LeftHandSide();

            var existsBuilder = leftHandSide.Quantifier(QuantifierType.Exists);

            var patternBuilder = existsBuilder.SourcePattern(typeof (T));
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide Not<T>(params Expression<Func<T, bool>>[] conditions)
        {
            var leftHandSide = _builder.LeftHandSide();

            var notBuilder = leftHandSide.Quantifier(QuantifierType.Not);

            var patternBuilder = notBuilder.SourcePattern(typeof(T));
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(patternBuilder.Declaration, leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide All<T>(Expression<Func<T, bool>> condition, params Expression<Func<T, bool>>[] conditions)
        {
            return All(Enumerable.Repeat(condition, 1).Union(conditions));
        }

        public IRightHandSide Do(Expression<Action<IContext>> action)
        {
            var rightHandSide = _builder.RightHandSide();

            var rewriter = new ActionRewriter(rightHandSide.Declarations);
            var rewrittenAction = rewriter.Rewrite(action);
            rightHandSide.Action(rewrittenAction);

            return this;
        }

        private ILeftHandSide All<T>(IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            var leftHandSide = _builder.LeftHandSide();

            var forallBuilder = leftHandSide.Quantifier(QuantifierType.ForAll);

            var patternBuilder = forallBuilder.SourcePattern(typeof(T));
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