using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Dsl;
using NRules.Inline.Expressions;
using NRules.Rule;
using NRules.Rule.Builders;

namespace NRules.Inline
{
    internal class Definition : IDefinition, ILeftHandSide, IRightHandSide
    {
        private readonly RuleBuilder _builder;

        public Definition(RuleBuilder builder, IRule instance)
        {
            _builder = builder;

            instance.ApplyAttribute<RulePriorityAttribute>(a => _builder.Priority(a.Priority));
        }

        public ILeftHandSide If<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions)
        {
            var patternSymbol = alias.ExtractSymbol();
            var leftHandSide = _builder.LeftHandSide();
            
            var patternBuilder = leftHandSide.Pattern(patternSymbol.Type, patternSymbol.Name);
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(patternBuilder.Declaration, condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide Collect<T>(Expression<Func<IEnumerable<T>>> alias, params Expression<Func<T, bool>>[] itemConditions)
        {
            var collectionSymbol = alias.ExtractSymbol();
            var leftHandSide = _builder.LeftHandSide();

            var outerPatternBuilder = leftHandSide.Pattern(collectionSymbol.Type, collectionSymbol.Name);

            var aggregateBuilder = outerPatternBuilder.SourceAggregate();
            aggregateBuilder.CollectionOf(typeof(T));

            var patternBuilder = aggregateBuilder.SourcePattern(typeof(T));
            foreach (var condition in itemConditions)
            {
                var rewriter = new ConditionRewriter(leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(patternBuilder.Declaration, condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide Exists<T>(params Expression<Func<T, bool>>[] conditions)
        {
            var leftHandSide = _builder.LeftHandSide();

            var existsBuilder = leftHandSide.Group(GroupType.Exists);

            var patternBuilder = existsBuilder.Pattern(typeof(T));
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(leftHandSide.Declarations);
                var rewrittenCondition = rewriter.Rewrite(patternBuilder.Declaration, condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public IRightHandSide Do(Expression<Action<IContext>> action)
        {
            var rightHandSide = _builder.RightHandSide();

            var rewriter = new ActionRewriter(rightHandSide.Declarations);
            var rewrittenAction = rewriter.Rewrite(action);
            rightHandSide.Action(rewrittenAction);

            return this;
        }

        public ILeftHandSide When()
        {
            return this;
        }

        public IRightHandSide Then()
        {
            return this;
        }
    }
}