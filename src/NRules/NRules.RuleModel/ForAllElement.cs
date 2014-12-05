namespace NRules.RuleModel
{
    /// <summary>
    /// Universal quantifier.
    /// </summary>
    public class ForAllElement : QuantifierElement
    {
        internal ForAllElement(PatternElement source) 
            : base(source)
        {
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitForAll(context, this);
        }
    }
}
