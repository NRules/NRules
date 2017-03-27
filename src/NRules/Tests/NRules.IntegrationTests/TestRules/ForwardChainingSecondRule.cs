using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class ForwardChainingSecondRule : BaseRule
    {
        public override void Define()
        {
            FactType3 fact3 = null;

            When()
                .Match<FactType3>(() => fact3, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}