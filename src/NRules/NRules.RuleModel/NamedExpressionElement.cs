using System.Diagnostics;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Expression with a name used by an aggregator.
    /// </summary>
    [DebuggerDisplay("{Name}={Expression.ToString()}")]
    public class NamedExpressionElement : ExpressionElement
    {
        internal NamedExpressionElement(string name, LambdaExpression lambdaExpression)
            : base(lambdaExpression)
        {
            Name = name;
        }

        /// <inheritdoc cref="RuleElement.ElementType"/>
        public override ElementType ElementType => ElementType.NamedExpression;

        /// <summary>
        /// Expression name.
        /// </summary>
        public string Name { get; }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitNamedExpression(context, this);
        }
    }
}