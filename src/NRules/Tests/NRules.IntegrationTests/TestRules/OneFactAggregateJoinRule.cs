using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactAggregateJoinRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            IEnumerable<FactType1> collection1 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => collection1, x => x
                    .Match<FactType1>()
                    .Collect()
                    .Where(c => c.Contains(fact1)));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}
