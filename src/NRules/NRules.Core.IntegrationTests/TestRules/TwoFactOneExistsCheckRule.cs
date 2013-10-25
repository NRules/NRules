using NRules.Core.IntegrationTests.TestAssets;
using NRules.Fluent.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactOneExistsCheckRule : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            FactType1 fact1 = null;

            definition.When()
                .If<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Exists<FactType2>(f => f.TestProperty.StartsWith("Valid"), f => f.JoinReference == fact1);
            definition.Then()
                .Do(() => Notifier.RuleActivated());
        }
    }
}