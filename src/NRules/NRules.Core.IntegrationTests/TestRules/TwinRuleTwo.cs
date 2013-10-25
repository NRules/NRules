using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwinRuleTwo : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;

            definition.When()
                .If<FactType1>(() => fact1, f => f.TestProperty == "Valid Value")
                .If<FactType2>(() => fact2, f => f.TestProperty == "Valid Value", f => fact1 == f.JoinReference);

            definition.Then()
                .Do(() => Notifier.RuleActivated());
        }
    }
}