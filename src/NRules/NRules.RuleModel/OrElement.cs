using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element based on logical OR condition.
    /// </summary>
    public class OrElement : GroupElement
    {
        internal OrElement(IEnumerable<RuleLeftElement> childElements) : base(childElements)
        {
        }

        internal override void Accept(RuleElementVisitor visitor)
        {
            visitor.VisitOr(this);
        }
    }
}