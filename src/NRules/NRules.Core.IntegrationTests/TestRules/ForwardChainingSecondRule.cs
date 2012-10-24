using NRules.Core.IntegrationTests.TestAssets;
using NRules.Fluent.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class ForwardChainingSecondRule : BaseRule
    {
        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<FactType2>(f2 => f2.TestProperty == "Valid Value");
            definition.Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}