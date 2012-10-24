using System;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Fluent.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    [RulePriority(10)]
    public class PriorityLowRule : BaseRule
    {
        public Action<BaseRule> InvocationHandler { get; set; }

        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<FactType1>(f1 => f1.TestProperty == "Valid Value");
            definition.Then()
                .Do(ctx =>
                        {
                            Notifier.RuleActivated();
                            InvocationHandler.Invoke(this);
                            var fact2 = new FactType2()
                                            {
                                                TestProperty = "Valid Value",
                                                JoinReference = ctx.Arg<FactType1>()
                                            };
                            ctx.Insert(fact2);
                        });
        }
    }
}