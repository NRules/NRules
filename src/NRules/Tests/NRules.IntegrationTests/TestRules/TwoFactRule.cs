using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactRule : BaseRule
    {
        public FactType1 Fact1 { get; set; }
        public FactType2 Fact2;

        public override void Define()
        {
            When()
                .Match<FactType1>(() => Fact1, f => f.TestProperty.StartsWith("Valid"))
                .Match<FactType2>(() => Fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == Fact1.TestProperty);

            Then()
                .Do(ctx => Action(ctx));
        }
    }
}