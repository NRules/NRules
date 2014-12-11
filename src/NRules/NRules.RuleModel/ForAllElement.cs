using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Universal quantifier.
    /// </summary>
    public class ForAllElement : RuleLeftElement
    {
        private readonly List<PatternElement> _patterns;

        internal ForAllElement(PatternElement source, IEnumerable<PatternElement> patterns) 
        {
            BasePattern = source;
            _patterns = new List<PatternElement>(patterns);
        }

        /// <summary>
        /// Base pattern that determines the universe of facts that the universal quantifier is applied to.
        /// </summary>
        public PatternElement BasePattern { get; private set; }

        /// <summary>
        /// Patterns that must all match for the selected facts.
        /// </summary>
        public IEnumerable<PatternElement> Patterns { get { return _patterns; } }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitForAll(context, this);
        }
    }
}
