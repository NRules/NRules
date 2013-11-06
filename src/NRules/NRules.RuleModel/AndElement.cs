using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element based on logical AND condition.
    /// </summary>
    public class AndElement : GroupElement
    {
        internal AndElement(IEnumerable<RuleLeftElement> childElements) : base(childElements)
        {
        }

        internal override void Accept(RuleElementVisitor visitor)
        {
            visitor.VisitAnd(this);
        }
    }
}