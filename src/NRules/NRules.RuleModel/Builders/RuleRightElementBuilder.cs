namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Base class for rule right-hand side element builders.
    /// </summary>
    public abstract class RuleRightElementBuilder : RuleElementBuilder
    {
        internal RuleRightElementBuilder(SymbolTable scope) : base(scope)
        {
        }
    }
}