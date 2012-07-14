using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestAssets
{
    public class OneFactRule : BaseRule
    {
        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<FactType1>(f => f.TestProperty == "Valid Value");
            definition.Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}