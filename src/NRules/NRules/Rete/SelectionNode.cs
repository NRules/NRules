using System;
using NRules.RuleModel;

namespace NRules.Rete;

internal class SelectionNode : AlphaNode
{
    private readonly ILhsFactExpression<bool> _compiledExpression;

    public ExpressionElement ExpressionElement { get; }

    public SelectionNode(int id, Type outputType, ExpressionElement expressionElement, ILhsFactExpression<bool> compiledExpression)
        : base(id, outputType)
    {
        ExpressionElement = expressionElement;
        _compiledExpression = compiledExpression;
    }

    protected override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
    {
        try
        {
            return _compiledExpression.Invoke(context, NodeInfo, fact);
        }
        catch (ExpressionEvaluationException e)
        {
            if (!e.IsHandled)
            {
                throw new RuleLhsExpressionEvaluationException("Failed to evaluate condition", e.Expression.ToString(), e.InnerException);
            }

            return false;
        }
    }

    public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.VisitSelectionNode(context, this);
    }
}