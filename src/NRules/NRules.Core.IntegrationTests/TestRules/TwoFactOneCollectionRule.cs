using System.Linq;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactOneCollectionRule : BaseRule
    {
        public int FactCount { get; set; }

        public override void Define(IDefinition definition)
        {
            definition.When()
                .If<FactType1>(f1 => f1.TestProperty == "Valid Value")
                .Collect<FactType2>(f2 => f2.TestProperty.StartsWith("Valid"));
            definition.Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => SetCount(ctx.Collection<FactType2>().Count()));
        }

        private void SetCount(int count)
        {
            FactCount = count;
        }
    }
}