using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Core.Expressions;
using NRules.Dsl;
using NRules.Rule;
using NRules.Rule.Builders;

namespace NRules.Core
{
    internal class Definition : IDefinition, ILeftHandSide, IRightHandSide
    {
        private readonly RuleBuilder _builder;

        public Definition(RuleBuilder builder, RuleMetadata metadata)
        {
            _builder = builder;

            if (metadata.Priority.HasValue)
            {
                _builder.Priority(metadata.Priority.Value);
            }
        }

        public ILeftHandSide If<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions)
        {
            var patternSymbol = alias.ExtractSymbol();
            var leftHandSide = _builder.LeftHandSide();
            IEnumerable<Declaration> declarations = leftHandSide.Declarations.ToList();
            
            Declaration declaration = leftHandSide.Declare(patternSymbol.Name, patternSymbol.Type);
            var patternBuilder = leftHandSide.Pattern(declaration);
            foreach (var condition in conditions)
            {
                var rewriter = new ConditionRewriter(declarations);
                var rewrittenCondition = rewriter.Rewrite(declaration, condition);
                patternBuilder.Condition(rewrittenCondition);
            }
            return this;
        }

        public ILeftHandSide Collect<T>(Expression<Func<IEnumerable<T>>> alias, Expression<Func<T, bool>> itemCondition)
        {
            var collectionSymbol = alias.ExtractSymbol();
            var leftHandSide = _builder.LeftHandSide();
            IEnumerable<Declaration> declarations = leftHandSide.Declarations.ToList();

            Declaration declaration = leftHandSide.Declare(collectionSymbol.Name, collectionSymbol.Type);
            var aggregateBuilder = leftHandSide.Aggregate(declaration);
            aggregateBuilder.CollectionOf(typeof(T));

            var patternDeclaration = aggregateBuilder.Declare(typeof (T));
            var patternBuilder = aggregateBuilder.SourcePattern(patternDeclaration);
            var rewriter = new ConditionRewriter(declarations);
            var rewrittenCondition = rewriter.Rewrite(patternDeclaration, itemCondition);
            patternBuilder.Condition(rewrittenCondition);

            return this;
        }

        public ILeftHandSide Exists<T>(Expression<Func<T, bool>> condition)
        {
            var leftHandSide = _builder.LeftHandSide();
            IEnumerable<Declaration> declarations = leftHandSide.Declarations.ToList();

            var existsBuilder = leftHandSide.Group(GroupType.Exists);

            var patternDeclaration = existsBuilder.Declare(typeof (T));
            var patternBuilder = existsBuilder.Pattern(patternDeclaration);
            var rewriter = new ConditionRewriter(declarations);
            var rewrittenCondition = rewriter.Rewrite(patternDeclaration, condition);
            patternBuilder.Condition(rewrittenCondition);

            return this;
        }

        public IRightHandSide Do(Expression<Action> action)
        {
            var leftHandSide = _builder.LeftHandSide();
            IEnumerable<Declaration> declarations = leftHandSide.Declarations.ToList();

            var rewriter = new ActionRewriter(declarations);
            var rewrittenAction = rewriter.Rewrite(action);
            _builder.Action(rewrittenAction);

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