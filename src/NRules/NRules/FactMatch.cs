using NRules.RuleModel;

namespace NRules
{
    internal class FactMatch : IFactMatch
    {
        public FactMatch(Declaration declaration)
        {
            Declaration = declaration;
        }

        public Declaration Declaration { get; private set; }
        public object Value { get; set; }
    }
}