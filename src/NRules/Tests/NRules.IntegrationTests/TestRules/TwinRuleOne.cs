using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwinRuleOne : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty == "Valid Value")
                .Match<FactType2>(() => fact2, f => f.TestProperty == "Valid Value", f => fact1 == f.JoinReference);

            Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}