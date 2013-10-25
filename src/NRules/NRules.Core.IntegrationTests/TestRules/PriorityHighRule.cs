using System;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    [RulePriority(100)]
    public class PriorityHighRule : BaseRule
    {
        public Action<BaseRule> InvocationHandler { get; set; }

        public override void Define(IDefinition definition)
        {
            FactType2 fact2 = null;

            definition.When()
                .If<FactType2>(() => fact2, f => f.TestProperty == "Valid Value");
            definition.Then()
                .Do(() => Notifier.RuleActivated())
                .Do(() => InvocationHandler.Invoke(this));
        }
    }
}