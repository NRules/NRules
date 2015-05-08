using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class DependencyExpression : IDependencyExpression
    {
        private readonly RuleBuilder _builder;

        public DependencyExpression(RuleBuilder builder)
        {
            _builder = builder;
        }

        public IDependencyExpression Resolve<TDependency>(Expression<Func<TDependency>> alias)
        {
            var symbol = alias.ToParameterExpression();
            var dependencies = _builder.Dependencies();
            dependencies.Dependency(symbol.Type, symbol.Name);
            return this;
        }
    }
}