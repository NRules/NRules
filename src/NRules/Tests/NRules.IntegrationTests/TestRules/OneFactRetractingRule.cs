using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactRetractingRule : BaseRule
    {
        public override void Define()
        {
            FactType5 fact = null;

            When()
                .Match<FactType5>(() => fact, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.Retract(fact))
                .Do(ctx => Action(ctx));
        }
    }
}