using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions;

internal class ExpressionRewriter(ISymbolLookup symbolLookup) : ExpressionVisitor
{
    private ISymbolLookup SymbolLookup { get; } = symbolLookup;
    protected List<ParameterExpression> Parameters { get; } = new();

    public LambdaExpression Rewrite(LambdaExpression expression)
    {
        Parameters.Clear();
        InitParameters(expression);
        Expression body = Visit(expression.Body)!;
        return Expression.Lambda(body, expression.TailCall, Parameters);
    }

    protected virtual void InitParameters(LambdaExpression expression)
    {
        Parameters.Clear();
        Parameters.AddRange(expression.Parameters);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        switch (node)
        {
            case { Expression: ConstantExpression}:
                if (SymbolLookup.TryGetValue(node.Member.Name, out var declaration1))
                    return ReplaceWithParameter(declaration1);
                break;
            case { Expression: MemberExpression, Member: PropertyInfo pi }:
                if (SymbolLookup.TryGetValue(node.Member.Name, out var declaration2) &&
                    declaration2.Type.IsAssignableFrom(pi.PropertyType))
                    return ReplaceWithParameter(declaration2);
                break;
            case { Expression: MemberExpression, Member: FieldInfo fi }:
                if (SymbolLookup.TryGetValue(node.Member.Name, out var declaration3) &&
                    declaration3.Type.IsAssignableFrom(fi.FieldType))
                    return ReplaceWithParameter(declaration3);
                break;
        }
        
        return base.VisitMember(node);
    }

    private Expression ReplaceWithParameter(Declaration declaration)
    {
        ParameterExpression? parameter = Parameters.FirstOrDefault(p => p.Name == declaration.Name);
        if (parameter == null)
        {
            parameter = declaration.ToParameterExpression();
            Parameters.Add(parameter);
        }
        else if (parameter.Type != declaration.Type)
        {
            throw new ArgumentException(
                $"Expression parameter type mismatch. Name={declaration.Name}, ExpectedType={declaration.Type}, FoundType={parameter.Type}");
        }
        
        return parameter;
    }
}