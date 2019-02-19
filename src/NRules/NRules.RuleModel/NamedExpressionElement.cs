using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Expression with a name used by an aggregator.
    /// </summary>
    public class NamedExpressionElement : ExpressionElement
    {
        internal NamedExpressionElement(string name, LambdaExpression lambdaExpression)
            : base(lambdaExpression)
        {
            Name = name;
        }

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