namespace NRules.RuleModel
{
    /// <summary>
    /// Base class for rule elements that implement quantification operators.
    /// </summary>
    public abstract class QuantifierElement : RuleLeftElement
    {
        /// <summary>
        /// Fact source of the quantifier.
        /// </summary>
        public PatternElement Source { get; private set; }

        internal QuantifierElement(PatternElement source)
        {
            Source = source;
        }
    }
}