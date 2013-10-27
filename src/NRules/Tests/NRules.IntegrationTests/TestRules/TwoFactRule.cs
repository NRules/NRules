using NRules.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactRule : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;

            definition.When()
                .If<FactType1>(() => fact1, f => f.TestProperty == "Valid Value")
                .If<FactType2>(() => fact2, f => f.TestProperty == "Valid Value", f => f.JoinReference == fact1);

            definition.Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}