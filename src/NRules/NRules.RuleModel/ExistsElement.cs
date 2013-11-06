using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element based on existential quantifier.
    /// </summary>
    public class ExistsElement : GroupElement
    {
        internal ExistsElement(IEnumerable<RuleLeftElement> childElements) : base(childElements)
        {
        }

        internal override void Accept(RuleElementVisitor visitor)
        {
            visitor.VisitExists(this);
        }
    }
}