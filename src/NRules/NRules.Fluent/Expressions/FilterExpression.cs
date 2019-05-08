using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class FilterExpression : IFilterExpression
    {
        private readonly FilterGroupBuilder _builder;
        private readonly SymbolStack _symbolStack;

        public FilterExpression(FilterGroupBuilder builder, SymbolStack symbolStack)
        {
            _builder = builder;
            _symbolStack = symbolStack;
        }

        public IFilterExpression OnChange(params Expression<Func<object>>[] keySelectors)
        {
            foreach (var keySelector in keySelectors)
            {
                var expression = keySelector.DslExpression(_symbolStack.Scope.Declarations);
                _builder.Filter(FilterType.KeyChange, expression);
            }
            return this;
        }

        public IFilterExpression Where(params Expression<Func<bool>>[] predicates)
        {
            foreach (var predicate in predicates)
            {
                var expression = predicate.DslExpression(_symbolStack.Scope.Declarations);
                _builder.Filter(FilterType.Predicate, expression);
            }
            return this;
        }
    }
}