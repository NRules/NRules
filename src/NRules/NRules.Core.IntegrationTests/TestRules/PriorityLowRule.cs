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
            definition.When()
                .If<FactType1>(f1 => f1.TestProperty == "Valid Value");
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