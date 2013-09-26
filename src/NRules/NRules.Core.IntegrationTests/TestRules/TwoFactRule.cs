using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactRule : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            definition.When()
                .If<FactType1>(f1 => f1.TestProperty == "Valid Value")
                .If<FactType2>(f2 => f2.TestProperty == "Valid Value")
                .If<FactType1, FactType2>((f1, f2) => f1 == f2.JoinReference);

            definition.Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}