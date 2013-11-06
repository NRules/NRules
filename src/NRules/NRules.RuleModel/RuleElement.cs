namespace NRules.RuleModel
{
    /// <summary>
    /// Base class for rule elements.
    /// </summary>
    public abstract class RuleElement
    {
        internal abstract void Accept(RuleElementVisitor visitor);
    }
}