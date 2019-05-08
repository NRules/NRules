using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class DependencyExpression : IDependencyExpression
    {
        private readonly DependencyGroupBuilder _builder;
        private readonly SymbolStack _symbolStack;

        public DependencyExpression(DependencyGroupBuilder builder, SymbolStack symbolStack)
        {
            _builder = builder;
            _symbolStack = symbolStack;
        }

        public IDependencyExpression Resolve<TDependency>(Expression<Func<TDependency>> alias)
        {
            var symbol = alias.ToParameterExpression();
            var declaration = _builder.Dependency(symbol.Type, symbol.Name);
            _symbolStack.Scope.Add(declaration);
            return this;
        }
    }
}