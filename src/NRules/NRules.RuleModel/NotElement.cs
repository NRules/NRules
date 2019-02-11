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

        internal NotElement(RuleLeftElement source)
        {
            Source = source;

            AddImports(source);
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitNot(context, this);
        }
    }
}