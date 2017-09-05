using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class FilterExpression : IFilterExpression
    {
        private readonly RuleBuilder _builder;

        public FilterExpression(RuleBuilder builder)
        {
            _builder = builder;
        }

        public IFilterExpression OnChange(params Expression<Func<object>>[] keySelectors)
        {
            var filters = _builder.Filters();
            foreach (var keySelector in keySelectors)
            {
                var expression = keySelector.DslExpression(filters.Declarations);
                filters.Filter(FilterType.KeyChange, expression);
            }
            return this;
        }

        public IFilterExpression Where(params Expression<Func<bool>>[] predicates)
        {
            var filters = _builder.Filters();
            foreach (var predicate in predicates)
            {
                var expression = predicate.DslExpression(filters.Declarations);
                filters.Filter(FilterType.Predicate, expression);
            }
            return this;
        }
    }
}