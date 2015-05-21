using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactOneGroupByRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            IGrouping<string, FactType2> group2 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => group2, x => x
                    .From<FactType2>()
                    .Where(f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                    .GroupBy(f => f.TestProperty));
            Then()
                .Do(ctx => Action());
        }
    }
}