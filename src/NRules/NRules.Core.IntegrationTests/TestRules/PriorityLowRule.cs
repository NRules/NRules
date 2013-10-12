using System;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    [RulePriority(10)]
    public class PriorityLowRule : BaseRule
    {
        public Action<BaseRule> InvocationHandler { get; set; }

        public override void Define(IDefinition definition)
        {
            FactType1 fact1 = null;

            definition.When()
                .If<FactType1>(() => fact1, f => f.TestProperty == "Valid Value");
            definition.Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => InvocationHandler.Invoke(this))
                .Do(ctx => ctx.Insert(new FactType2()
                    {
                        TestProperty = "Valid Value",
                        JoinReference = ctx.Arg<FactType1>()
                    }));
        }
    }
}