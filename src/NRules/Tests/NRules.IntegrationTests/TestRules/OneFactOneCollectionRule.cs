using System.Collections.Generic;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactOneCollectionRule : BaseRule
    {
        public override void Define()
        {
            IEnumerable<FactType1> collection1 = null;

            When()
                .Match<FactType1>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect(() => collection1);
            Then()
                .Do(ctx => Action());
        }
    }
}