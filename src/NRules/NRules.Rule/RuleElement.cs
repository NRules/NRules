namespace NRules.Rule
{
    public abstract class RuleElement
    {
        protected RuleElement()
        {
            SymbolTable = new SymbolTable();
        }

        internal SymbolTable SymbolTable { get; private set; }
    }
}