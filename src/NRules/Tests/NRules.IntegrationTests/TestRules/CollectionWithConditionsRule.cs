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
                .Collect<FactType1>(() => collection1, f => f.TestProperty.StartsWith("Valid")).Where(x => x.Count() > 2);
            Then()
                .Do(ctx => Action());
        }
    }
}