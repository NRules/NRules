using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactOneExistsCheckRule : BaseRule
    {
        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .Exists<FactType1>(f1 => f1.TestProperty.StartsWith("Valid"))
                .If<FactType2>(f2 => f2.TestProperty.StartsWith("Valid"));
            definition.Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}