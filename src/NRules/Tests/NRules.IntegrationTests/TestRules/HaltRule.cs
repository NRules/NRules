using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRules.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class HaltRule : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            FactType1 fact1 = null;

            definition.When()
                .If<FactType1>(() => fact1, f => f.TestProperty == "Valid Value");
            definition.Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => ctx.Halt());
        }
    }
}