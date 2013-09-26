using System.Linq;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactTwoCollectionRule : BaseRule
    {
        public int Fact1Count { get; set; }
        public int Fact2Count { get; set; }

        public override void Define(IDefinition definition)
        {
            definition.When()
                .Collect<FactType1>(f1 => f1.TestProperty.StartsWith("Valid"))
                .Collect<FactType2>(f2 => f2.TestProperty.StartsWith("Valid"));
            definition.Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => SetCount1(ctx.Collection<FactType1>().Count()))
                .Do(ctx => SetCount2(ctx.Collection<FactType2>().Count()));
        }

        private void SetCount1(int count)
        {
            Fact1Count = count;
        }

        private void SetCount2(int count)
        {
            Fact2Count = count;
        }
    }
}