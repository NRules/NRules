namespace NRules.RuleModel
{
    /// <summary>
    /// Negative existential quantifier.
    /// </summary>
    public class NotElement : RuleElement
    {
        /// <inheritdoc cref="RuleElement.ElementType"/>
        public override ElementType ElementType => ElementType.Not;

        /// <summary>
        /// Fact source of the not element.
        /// </summary>
        public RuleElement Source { get; }

        internal NotElement(RuleElement source)
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