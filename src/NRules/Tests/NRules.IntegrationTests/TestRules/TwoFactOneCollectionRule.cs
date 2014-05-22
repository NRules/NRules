using System.Collections.Generic;
using System.Linq;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactOneCollectionRule : BaseRule
    {
        private readonly Dictionary<FactType1, int> _factCount = new Dictionary<FactType1, int>();
        public IDictionary<FactType1, int> FactCount { get { return _factCount; } }

        public override void Define()
        {
            FactType1 fact1 = null;
            IEnumerable<FactType2> collection2 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Collect<FactType2>(() => collection2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty);
            Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => FactCount.Add(fact1, collection2.Count()));
        }
    }
}