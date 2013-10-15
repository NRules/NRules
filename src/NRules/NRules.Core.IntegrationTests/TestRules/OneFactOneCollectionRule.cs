using System.Collections.Generic;
using System.Linq;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
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
                .Do(() => Notifier.RuleActivated())
                .Do(() => SetCount(collection1.Count()));
        }

        private void SetCount(int count)
        {
            FactCount = count;
        }
    }
}