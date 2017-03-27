using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class GroupWithSelectionAfterGroupRule : BaseRule
    {
        public override void Define()
        {
            IGrouping<string, FactType6> group = null;

            When()
                .Query(() => group, x => x
                    .Match<FactType6>(f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(f => f.GroupProperty).Where(z => HasCorrectValue(z)));
            Then()
                .Do(ctx => Action(ctx));
        }

        private static bool HasCorrectValue(IGrouping<string, FactType6> factType6s)
        {
            return factType6s.Any(factType6 => factType6.JoinProperty.Contains("Good"));
        }
    }
}
