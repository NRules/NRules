using System.Collections.Generic;
using System.Linq;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactOneCollectionRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            IEnumerable<FactType2> collection2 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Collect<FactType2>(() => collection2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty);
            Then()
                .Do(ctx => Action())
                .Do(ctx => collection2.ToList().ForEach(x => x.TestProperty.Normalize()));
        }
    }
}
