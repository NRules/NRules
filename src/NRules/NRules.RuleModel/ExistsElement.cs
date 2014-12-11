namespace NRules.RuleModel
{
    /// <summary>
    /// Existential quantifier.
    /// </summary>
    public class ExistsElement : RuleLeftElement
    {
        /// <summary>
        /// Fact source of the quantifier.
        /// </summary>
        public RuleLeftElement Source { get; private set; }

        internal ExistsElement(RuleLeftElement source)
        {
            Source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitExists(context, this);
        }
    }
}