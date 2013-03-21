using System.Linq;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class OneFactOneCollectionRule : BaseRule
    {
        public int FactCount { get; set; }

        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .Collect<FactType1>(f1 => f1.TestProperty.StartsWith("Valid"));
            definition.Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => SetCount(ctx.Collection<FactType1>().Count()));
        }

        private void SetCount(int count)
        {
            FactCount = count;
        }
    }
}