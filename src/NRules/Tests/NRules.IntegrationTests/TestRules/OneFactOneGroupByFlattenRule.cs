using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactOneGroupByFlattenRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .Query(() => fact1, q => q
                    .Match<FactType1>(f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(f => f.TestProperty)
                    .Where(g => g.Count() > 1)
                    .SelectMany(x => x));
            Then()
                .Do(ctx => Action());
        }
    }
}