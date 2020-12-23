using System.Diagnostics;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Pattern condition element.
    /// </summary>
    [DebuggerDisplay("{Expression.ToString()}")]
    public class ConditionElement : NamedExpressionElement
    {
        internal ConditionElement(LambdaExpression expression)
            : base(PatternElement.ConditionName, expression)
        {
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitCondition(context, this);
        }
    }
}