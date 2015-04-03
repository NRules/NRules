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
                .Collect<FactType1>(() => collection1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action());
        }
    }
}