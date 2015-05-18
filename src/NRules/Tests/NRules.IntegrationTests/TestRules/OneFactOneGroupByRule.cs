using System.Linq;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactOneGroupByRule : BaseRule
    {
        public override void Define()
        {
            IGrouping<string, FactType1> group1 = null;

            When()
                .Match<FactType1>(f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(() => group1, f => f.TestProperty)
                    .Where(x => x.Count() > 1);
            Then()
                .Do(ctx => Action());
        }
    }
}