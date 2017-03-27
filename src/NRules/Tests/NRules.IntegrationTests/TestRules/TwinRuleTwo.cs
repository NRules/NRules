using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwinRuleTwo : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;
            IEnumerable<FactType5> group = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                .Not<FactType3>(f => f.TestProperty.StartsWith("Invalid"))
                .Exists<FactType3>(f => f.TestProperty.StartsWith("Valid"))
                .Query(() => group, q => q
                    .Match<FactType5>()
                    .Where(f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(f => f.TestProperty)
                    .SelectMany(x => x)
                    .Collect()
                    .Where(c => c.Any()));

            Then()
                .Do(ctx => Action(ctx));
        }
    }
}