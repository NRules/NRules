using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactSimpleGroupByRule : BaseRule
    {
        public override void Define()
        {
            IGrouping<string, FactType1> group1 = null;

            When()
                .Query(() => group1, x => x
                    .Match<FactType1>()
                    .GroupBy(f => f.TestProperty));
            Then()
                .Do(ctx => Action());
        }
    }
}