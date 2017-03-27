using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactOneGroupByFlattenRuleWithIdentityRule : BaseRule
    {
        public override void Define()
        {
            IEnumerable<FactType7> facts = null;

            When()
                .Query(() => facts, q => q
                    .Match<FactType7>(f => f.Id != 0)
                    .GroupBy(f => f.GroupingProperty)
                    .Where(x => x.Select(xx => xx.Id).Distinct().Count() > 1)
                    .SelectMany(x => x)
                    .GroupBy(x => x.GroupingProperty2));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}
