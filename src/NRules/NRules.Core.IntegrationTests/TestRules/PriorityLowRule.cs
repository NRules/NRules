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
                .Do(() => Notifier.RuleActivated())
                .Do(() => InvocationHandler.Invoke(this))
                .Do(() => Context.Insert(new FactType2()
                    {
                        TestProperty = "Valid Value",
                        JoinReference = fact1
                    }));
        }
    }
}