﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that defines an expression.
/// </summary>
public abstract class ExpressionElement : RuleElement
{
    private protected ExpressionElement(LambdaExpression expression, IEnumerable<ParameterExpression> parameters)
    {
        Expression = expression;

        var imports = parameters.Select(p => p.ToDeclaration());
        AddImports(imports);
    }

    private protected  ExpressionElement(LambdaExpression expression)
        : this(expression, expression.Parameters)
    {
    }

    /// <summary>
    /// Expression.
    /// </summary>
    public LambdaExpression Expression { get; }
}