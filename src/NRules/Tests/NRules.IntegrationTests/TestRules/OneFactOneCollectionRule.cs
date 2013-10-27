using System.Collections.Generic;
using System.Linq;
using NRules.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactOneCollectionRule : BaseRule
    {
        public int FactCount { get; set; }

        public override void Define(IDefinition definition)
        {
            IEnumerable<FactType1> collection1 = null;

            definition.When()
                .Collect<FactType1>(() => collection1, f => f.TestProperty.StartsWith("Valid"));
            definition.Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => SetCount(collection1.Count()));
        }

        private void SetCount(int count)
        {
            FactCount = count;
        }
    }
}