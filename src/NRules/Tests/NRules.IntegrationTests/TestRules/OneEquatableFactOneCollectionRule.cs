using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneEquatableFactOneCollectionRule : BaseRule
    {
        public override void Define()
        {
            IEnumerable<EquatableFact> collection1 = null;

            When()
                .Query(() => collection1, q => q
                    .Match<EquatableFact>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect());
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}