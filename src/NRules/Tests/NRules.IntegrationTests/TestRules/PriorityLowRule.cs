using System;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class PriorityLowRule : BaseRule
    {
        public Action<BaseRule> InvocationHandler { get; set; }

        public override void Define()
        {
            Priority(10);

            FactType1 fact1 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => InvocationHandler.Invoke(this))
                .Do(ctx => ctx.Insert(new FactType2()
                    {
                        TestProperty = "Valid Value",
                        JoinProperty = fact1.TestProperty
                    }));
        }
    }
}