using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Pattern condition element.
    /// </summary>
    [DebuggerDisplay("{Expression.ToString()}")]
    public class ConditionElement : ExpressionElement
    {
        internal ConditionElement(IEnumerable<Declaration> declarations, IEnumerable<Declaration> references, LambdaExpression expression)
            : base(declarations, references, expression)
        {
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitCondition(context, this);
        }
    }
}