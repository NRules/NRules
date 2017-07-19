namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Base class for rule left-hand side element builders.
    /// </summary>
    public abstract class RuleLeftElementBuilder : RuleElementBuilder
    {
        internal RuleLeftElementBuilder(SymbolTable scope) : base(scope)
        {
        }
    }
}