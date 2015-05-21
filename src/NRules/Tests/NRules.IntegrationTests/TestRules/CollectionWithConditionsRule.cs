using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class CollectionWithConditionsRule : BaseRule
    {
        public override void Define()
        {
            IEnumerable<FactType1> collection1 = null;

            When()
                .Query(() => collection1, x => x
                    .From<FactType1>()
                    .Where(f => f.TestProperty.StartsWith("Valid"))
                    .Collect()
                    .Where(c => c.Count() > 2));
            Then()
                .Do(ctx => Action());
        }
    }
}