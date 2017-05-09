using System;
using NRules.Rete;
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
        public Type Type { get; private set; }
        public object Value { get; private set; }

        public void SetFact(Fact fact)
        {
            Type = fact.FactType.AsType();
            Value = fact.Object;
        }
    }
}