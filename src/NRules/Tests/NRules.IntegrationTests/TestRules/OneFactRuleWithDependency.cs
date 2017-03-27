using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactRuleWithDependency : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            ITestService service = null;

            Dependency()
                .Resolve(() => service);

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx))
                .Do(ctx => service.Action(fact1.TestProperty));
        }
    }
}