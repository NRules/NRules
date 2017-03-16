using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class SingleOrDefaultEquatableFactRule : BaseRule
    {
        public override void Define()
        {
            EquatableFact fact1 = null;

            When()
                .Query(() => fact1, q => q
                    .Match<EquatableFact>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect()
                    .Select(x => x.OrderBy(f => f.Id).FirstOrDefault() ?? new EquatableFact(0)));
            Then()
                .Do(ctx => Action());
        }
    }
}
