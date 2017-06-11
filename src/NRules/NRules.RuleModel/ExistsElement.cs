using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Existential quantifier.
    /// </summary>
    public class ExistsElement : RuleLeftElement
    {
        /// <summary>
        /// Fact source of the existential element.
        /// </summary>
        public RuleLeftElement Source { get; }

        internal ExistsElement(IEnumerable<Declaration> declarations, RuleLeftElement source)
            : base(declarations)
        {
            Source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitExists(context, this);
        }
    }
}