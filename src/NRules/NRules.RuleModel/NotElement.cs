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

        internal override void Accept(RuleElementVisitor visitor)
        {
            visitor.VisitNot(this);
        }
    }
}