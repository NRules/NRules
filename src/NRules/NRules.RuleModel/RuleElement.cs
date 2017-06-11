using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Base class for rule elements.
    /// </summary>
    public abstract class RuleElement
    {
        private readonly List<Declaration> _declarations;

        /// <summary>
        /// Declarations visible from this rule element.
        /// </summary>
        public IEnumerable<Declaration> Declarations => _declarations;

        internal RuleElement(IEnumerable<Declaration> declarations)
        {
            _declarations = new List<Declaration>(declarations);
        }

        internal abstract void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor);
    }
}