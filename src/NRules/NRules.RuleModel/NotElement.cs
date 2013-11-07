using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element based on logical NOT condition.
    /// </summary>
    public class NotElement : GroupElement
    {
        internal NotElement(IEnumerable<RuleLeftElement> childElements) : base(childElements)
        {
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitNot(context, this);
        }
    }
}