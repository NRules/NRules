using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class JoinFollowedByGroupByOnDifferentKeyRule : BaseRule
    {
        public override void Define()
        {
            FactType2 fact2 = null;
            IGrouping<string, FactType6> group = null;

            When()
                .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => group, x => x
                    .Match<FactType6>(
                        f => f.TestProperty.StartsWith("Valid"),
                        f => f.JoinProperty == fact2.JoinProperty)
                    .GroupBy(f => f.GroupProperty));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}
