using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactOneExistsCheckRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .If<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Exists<FactType2>(f => f.TestProperty.StartsWith("Valid"), f => f.JoinReference == fact1);
            Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}