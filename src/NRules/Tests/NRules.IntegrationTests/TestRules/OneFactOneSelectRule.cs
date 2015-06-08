using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactOneSelectRule : BaseRule
    {
        public override void Define()
        {
            FactType1Projection factProjection = null;

            When()
                .Query(() => factProjection, x => x
                    .Match<FactType1>()
                    .Select(f => new FactType1Projection(f))
                    .Where(p => p.Value.StartsWith("Valid")));
            Then()
                .Do(ctx => Action());
        }
    }
}