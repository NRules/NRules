using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    [Priority(10)]
    public class PriorityLowRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx))
                .Do(ctx => ctx.Insert(new FactType2()
                    {
                        TestProperty = "Valid Value",
                        JoinProperty = fact1.TestProperty
                    }));
        }
    }
}