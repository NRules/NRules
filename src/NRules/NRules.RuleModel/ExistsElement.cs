using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Existential quantifier.
    /// </summary>
    public class ExistsElement : RuleLeftElement
    {
        private readonly RuleLeftElement _source;

        /// <summary>
        /// Fact source of the existential element.
        /// </summary>
        public RuleLeftElement Source
        {
            get { return _source; }
        }

        internal ExistsElement(IEnumerable<Declaration> declarations, RuleLeftElement source)
            : base(declarations)
        {
            _source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitExists(context, this);
        }
    }
}