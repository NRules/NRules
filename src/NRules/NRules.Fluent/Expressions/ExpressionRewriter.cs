using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions;

internal class ExpressionRewriter : ExpressionVisitor
{
    protected List<ParameterExpression> Parameters { get; }
    public ISymbolLookup SymbolLookup { get; }

    public ExpressionRewriter(ISymbolLookup symbolLookup)
    {
        Parameters = new List<ParameterExpression>();
        SymbolLookup = symbolLookup;
    }

    public LambdaExpression Rewrite(LambdaExpression expression)
    {
        Parameters.Clear();
        InitParameters(expression);
        Expression body = Visit(expression.Body);
        return Expression.Lambda(body, expression.TailCall, Parameters);
    }

    protected virtual void InitParameters(LambdaExpression expression)
    {
        Parameters.Clear();
        Parameters.AddRange(expression.Parameters);
    }

    protected override Expression VisitMember(MemberExpression member)
    {
        if (SymbolLookup.TryGetValue(member.Member.Name, out var declaration))
        {
            ParameterExpression parameter = Parameters.FirstOrDefault(p => p.Name == declaration.Name);
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

        return base.VisitMember(member);
    }
}