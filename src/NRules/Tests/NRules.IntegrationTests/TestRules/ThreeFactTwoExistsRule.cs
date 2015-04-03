using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class ThreeFactTwoExistsRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Exists<FactType2>(f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                .Exists<FactType3>(f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty);
            Then()
                .Do(ctx => Action());
        }
    }
}