using System.Collections.Generic;
using System.Linq;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class CollectionWithConditionsRule : BaseRule
    {
        public override void Define()
        {
            IEnumerable<FactType1> collection1 = null;

            When()
                .Match<FactType1>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect(() => collection1)
                    .Where(x => x.Count() > 2);
            Then()
                .Do(ctx => Action());
        }
    }
}