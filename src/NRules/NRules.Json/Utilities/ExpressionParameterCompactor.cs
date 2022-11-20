using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Json.Utilities;

internal class ExpressionParameterCompactor : ExpressionVisitor
{
    private Dictionary<string, ParameterExpression> _parameterMap;

    public LambdaExpression Compact(LambdaExpression lambdaExpression)
    {
        _parameterMap = lambdaExpression.Parameters.ToDictionary(p => p.Name);
        var result = Visit(lambdaExpression);
        return (LambdaExpression) result;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (_parameterMap.TryGetValue(node.Name, out var lambdaParameter))
            return lambdaParameter;

        return base.VisitParameter(node);
    }
}
