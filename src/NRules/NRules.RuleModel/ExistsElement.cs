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

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitExists(context, this);
        }
    }
}