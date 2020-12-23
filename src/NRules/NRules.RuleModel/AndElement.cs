using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element based on the logical AND condition.
    /// </summary>
    public class AndElement : GroupElement
    {
        internal AndElement(IEnumerable<RuleElement> childElements)
            : base(childElements)
        {
        }

        /// <inheritdoc cref="RuleElement.ElementType"/>
        public override ElementType ElementType => ElementType.And;

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAnd(context, this);
        }
    }
}