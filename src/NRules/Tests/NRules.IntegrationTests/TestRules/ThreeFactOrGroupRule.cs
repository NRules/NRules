using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class ThreeFactOrGroupRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;
            FactType3 fact3 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Or(x => x
                    .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                    .And(xx => xx
                        .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Invalid"), f => f.JoinProperty == fact1.TestProperty)
                        .Match<FactType3>(() => fact3, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)));

            Then()
                .Do(ctx => Action());
        }
    }
}