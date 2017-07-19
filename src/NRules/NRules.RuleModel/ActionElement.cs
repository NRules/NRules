using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Action executed by the engine when the rule fires.
    /// </summary>
    [DebuggerDisplay("{Expression.ToString()}")]
    public class ActionElement : ExpressionElement
    {
        internal ActionElement(IEnumerable<Declaration> declarations, IEnumerable<Declaration> references, LambdaExpression expression)
            : base(declarations, references, expression)
        {
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAction(context, this);
        }
    }
}