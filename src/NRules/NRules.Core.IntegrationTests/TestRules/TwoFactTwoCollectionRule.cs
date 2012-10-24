using System.Linq;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Fluent.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactTwoCollectionRule : BaseRule
    {
        public int Fact1Count { get; set; }
        public int Fact2Count { get; set; }

        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .Collect<FactType1>(f1 => f1.TestProperty.StartsWith("Valid"))
                .Collect<FactType2>(f2 => f2.TestProperty.StartsWith("Valid"));
            definition.Then()
                .Do(ctx =>
                        {
                            Notifier.RuleActivated();
                            Fact1Count = ctx.Collection<FactType1>().Count();
                            Fact2Count = ctx.Collection<FactType2>().Count();
                        });
        }
    }
}