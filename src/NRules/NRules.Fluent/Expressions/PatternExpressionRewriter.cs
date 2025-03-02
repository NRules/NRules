using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions;

internal class PatternExpressionRewriter(Declaration patternDeclaration, ISymbolLookup symbolLookup)
    : ExpressionRewriter(symbolLookup)
{
    private ParameterExpression? _originalParameter;

    protected override void InitParameters(LambdaExpression expression)
    {
        _originalParameter = expression.Parameters.Single();
        var normalizedParameter = patternDeclaration.ToParameterExpression();
        Parameters.Add(normalizedParameter);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (node == _originalParameter)
        {
            return Parameters[0];
        }
        return base.VisitParameter(node);
    }
}