namespace NRules.RuleModel
{
    /// <summary>
    /// Negative existential quantifier.
    /// </summary>
    public class NotElement : RuleLeftElement
    {
        /// <summary>
        /// Fact source of the quantifier.
        /// </summary>
        public RuleLeftElement Source { get; private set; }

        internal NotElement(RuleLeftElement source)
        {
            Source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitNot(context, this);
        }
    }
}