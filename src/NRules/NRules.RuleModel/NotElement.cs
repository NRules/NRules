using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Negative existential quantifier.
    /// </summary>
    public class NotElement : RuleLeftElement
    {
        /// <summary>
        /// Fact source of the not element.
        /// </summary>
        public RuleLeftElement Source { get; }

        internal NotElement(IEnumerable<Declaration> declarations, RuleLeftElement source)
            : base(declarations)
        {
            Source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitNot(context, this);
        }
    }
}