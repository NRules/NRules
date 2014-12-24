using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Existential quantifier.
    /// </summary>
    public class ExistsElement : RuleLeftElement
    {
        private readonly List<Declaration> _declarations;
        private readonly RuleLeftElement _source;

        /// <summary>
        /// Fact source of the existential element.
        /// </summary>
        public RuleLeftElement Source
        {
            get { return _source; }
        }

        internal ExistsElement(IEnumerable<Declaration> declarations, RuleLeftElement source)
        {
            _declarations = new List<Declaration>(declarations);
            _source = source;
        }

        public override IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitExists(context, this);
        }
    }
}