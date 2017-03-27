using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactOneCollectionRule : BaseRule
    {
        public override void Define()
        {
            IEnumerable<FactType1> collection1 = null;

            When()
                .Query(() => collection1, x => x
                    .Match<FactType1>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect());
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}