using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Universal quantifier.
    /// </summary>
    public class ForAllElement : RuleLeftElement
    {
        private readonly List<PatternElement> _patterns;
        private readonly PatternElement _basePattern;

        internal ForAllElement(IEnumerable<Declaration> declarations, PatternElement source, IEnumerable<PatternElement> patterns)
            : base(declarations)
        {
            _basePattern = source;
            _patterns = new List<PatternElement>(patterns);
        }

        /// <summary>
        /// Base pattern that determines the universe of facts that the universal quantifier is applied to.
        /// </summary>
        public PatternElement BasePattern
        {
            get { return _basePattern; }
        }

        /// <summary>
        /// Patterns that must all match for the selected facts.
        /// </summary>
        public IEnumerable<PatternElement> Patterns
        {
            get { return _patterns; }
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitForAll(context, this);
        }
    }
}
