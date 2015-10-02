using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class ThreeFactNestedOrGroupRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;
            FactType3 fact3 = null;

            When()
                .Or(x => x
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Or(xx => xx
                        .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"))
                        .Match<FactType3>(() => fact3, f => f.TestProperty.StartsWith("Valid"))));

            Then()
                .Do(ctx => Action());
        }
    }
}