using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactOneGroupByRule : BaseRule
    {
        public override void Define()
        {
            IGrouping<string, string> group1 = null;

            When()
                .Query(() => group1, x => x
                    .Match<FactType1>(f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(f => f.TestProperty, f => f.TestProperty)
                    .Where(g => g.Count() > 1));
            Then()
                .Do(ctx => Action());
        }
    }
}