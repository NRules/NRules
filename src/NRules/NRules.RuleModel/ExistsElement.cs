namespace NRules.RuleModel
{
    /// <summary>
    /// Existential quantifier.
    /// </summary>
    public class ExistsElement : RuleElement
    {
        /// <inheritdoc cref="RuleElement.ElementType"/>
        public override ElementType ElementType => ElementType.Exists;

        /// <summary>
        /// Fact source of the existential element.
        /// </summary>
        public RuleElement Source { get; }

        internal ExistsElement(RuleElement source)
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