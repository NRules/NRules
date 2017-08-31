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

        public IFilterExpression OnChange(Expression<Func<object>> keySelector)
        {
            var filters = _builder.Filters();
            var expression = keySelector.DslExpression(filters.Declarations);
            filters.Filter(FilterType.KeyChange, expression);
            return this;
        }

        public IFilterExpression Where(Expression<Func<bool>> keySelector)
        {
            var filters = _builder.Filters();
            var expression = keySelector.DslExpression(filters.Declarations);
            filters.Filter(FilterType.Predicate, expression);
            return this;
        }
    }
}