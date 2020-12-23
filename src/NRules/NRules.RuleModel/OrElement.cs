using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element based on the logical OR condition.
    /// </summary>
    public class OrElement : GroupElement
    {
        internal OrElement(IEnumerable<RuleElement> childElements)
            : base(childElements)
        {
        }

        /// <inheritdoc cref="RuleElement.ElementType"/>
        public override ElementType ElementType => ElementType.Or;

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitOr(context, this);
        }
    }
}