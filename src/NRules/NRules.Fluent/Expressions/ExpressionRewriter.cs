using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions;

internal class ExpressionRewriter : ExpressionVisitor
{
    private readonly List<ParameterExpression> _parameters = new();

    public ExpressionRewriter(IEnumerable<Declaration> declarations)
    {
        Declarations = declarations.ToDictionary(d => d.Name);
    }

    private IDictionary<string, Declaration> Declarations { get; }

    protected IReadOnlyCollection<ParameterExpression> Parameters => _parameters;

    protected void AddParameter(ParameterExpression parameter)
    {
        _parameters.Add(parameter);
    }

    public LambdaExpression Rewrite(LambdaExpression expression)
    {
        _parameters.Clear();
        InitParameters(expression);
        Expression body = Visit(expression.Body);
        return Expression.Lambda(body, expression.TailCall, _parameters);
    }

    protected virtual void InitParameters(LambdaExpression expression)
    {
        _parameters.Clear();
        _parameters.AddRange(expression.Parameters);
    }

    protected override Expression VisitMember(MemberExpression member)
    {
        if (Declarations.TryGetValue(member.Member.Name, out var declaration))
        {
            ParameterExpression parameter = _parameters.FirstOrDefault(p => p.Name == declaration.Name);
            if (parameter == null)
            {
                parameter = declaration.ToParameterExpression();
                _parameters.Add(parameter);
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