using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty == "Valid Value")
                .Match<FactType2>(() => fact2, f => f.TestProperty == "Valid Value", f => f.JoinReference == fact1);

            Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}