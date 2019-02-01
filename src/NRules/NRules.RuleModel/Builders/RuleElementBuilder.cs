namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Base class for rule element builders.
    /// </summary>
    public abstract class RuleElementBuilder
    {
        private int _declarationCounter = 0;

        protected string DeclarationName(string name)
        {
            _declarationCounter++;
            string declarationName = name ?? $"$var{_declarationCounter}$";
            return declarationName;
        }
    }
}