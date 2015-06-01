using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class ForwardChainingFirstRule : BaseRule
    {
        public override void Define()
        {
            FactType2 fact2 = null;

            When()
                .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action())
                .Do(ctx => ctx.Insert(new FactType3()
                    {
                        TestProperty = fact2.JoinProperty,
                        JoinProperty = fact2.TestProperty
                    }));
        }
    }
}