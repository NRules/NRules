using NRules.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class ForwardChainingSecondRule : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            FactType2 fact2 = null;

            definition.When()
                .If<FactType2>(() => fact2, f => f.TestProperty == "Valid Value");
            definition.Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}