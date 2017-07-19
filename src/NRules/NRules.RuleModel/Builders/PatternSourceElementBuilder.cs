namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Base class for pattern source rule element builders.
    /// </summary>
    public abstract class PatternSourceElementBuilder : RuleLeftElementBuilder
    {
        internal PatternSourceElementBuilder(SymbolTable scope) : base(scope)
        {
        }
    }
}