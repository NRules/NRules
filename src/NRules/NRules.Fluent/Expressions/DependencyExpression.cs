using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions;

internal class DependencyExpression(DependencyGroupBuilder builder, SymbolStack symbolStack) : IDependencyExpression
{
    public IDependencyExpression Resolve<TDependency>(Expression<Func<TDependency>> alias)
        where TDependency : notnull
    {
        var symbol = alias.ToParameterExpression();
        var declaration = builder.Dependency(symbol.Type, symbol.Name);
        symbolStack.Scope.Add(declaration);
        return this;
    }
}