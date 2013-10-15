using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactOneExistsCheckRule : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            FactType2 fact2 = null;

            definition.When()
                .Exists<FactType1>(f => f.TestProperty.StartsWith("Valid"))
                .If<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"));
            definition.Then()
                .Do(() => Notifier.RuleActivated());
        }
    }
}