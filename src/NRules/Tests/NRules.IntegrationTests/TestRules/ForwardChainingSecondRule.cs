using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class ForwardChainingSecondRule : BaseRule
    {
        public override void Define()
        {
            FactType2 fact2 = null;

            When()
                .If<FactType2>(() => fact2, f => f.TestProperty == "Valid Value");
            Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}