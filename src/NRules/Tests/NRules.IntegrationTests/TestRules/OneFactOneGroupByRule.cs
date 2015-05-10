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
                .GroupBy(() => group1, f => f.TestProperty, f => f.TestProperty.StartsWith("Valid"))
                    .Where(x => x.Count() > 1);
            Then()
                .Do(ctx => Action());
        }
    }
}