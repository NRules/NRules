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
                .Do(ctx => SetCount(fact1, collection2))
                .Do(ctx => collection2.ToList().ForEach(x => x.TestProperty.Normalize()));
        }

        private void SetCount(FactType1 fact1, IEnumerable<FactType2> collection2)
        {
            FactCount[fact1] = collection2.Count();
        }
    }
}
