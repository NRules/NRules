using System;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class PriorityHighRule : BaseRule
    {
        public Action<BaseRule> InvocationHandler { get; set; }

        public override void Define()
        {
            Priority(100);

            FactType2 fact2 = null;

            When()
                .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => InvocationHandler.Invoke(this));
        }
    }
}