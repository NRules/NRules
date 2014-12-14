namespace NRules.RuleModel
{
    /// <summary>
    /// Negative existential quantifier.
    /// </summary>
    public class NotElement : RuleLeftElement
    {
        private readonly RuleLeftElement _source;

        /// <summary>
        /// Fact source of the not element.
        /// </summary>
        public RuleLeftElement Source
        {
            get { return _source; }
        }

        internal NotElement(RuleLeftElement source)
        {
            _source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitNot(context, this);
        }
    }
}