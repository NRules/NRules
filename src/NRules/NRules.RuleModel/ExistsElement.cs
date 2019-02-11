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

        internal ExistsElement(RuleLeftElement source)
        {
            Source = source;

            AddImports(source);
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitExists(context, this);
        }
    }
}