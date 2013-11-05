using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class ForwardChainingFirstRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .If<FactType1>(() => fact1, f => f.TestProperty == "Valid Value");
            Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => ctx.Insert(new FactType2()
                    {
                        TestProperty = "Valid Value",
                        JoinReference = fact1
                    }));
        }
    }
}