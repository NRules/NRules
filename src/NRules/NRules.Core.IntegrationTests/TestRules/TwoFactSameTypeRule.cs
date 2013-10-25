using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactSameTypeRule : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            FactType3 fact1 = null;
            FactType3 fact2 = null;

            definition.When()
                .If<FactType3>(() => fact1, f => f.TestProperty == "Valid Value")
                .If<FactType3>(() => fact2, f => f.TestProperty == "Valid Value", f => f.Parent == fact1);

            definition.Then()
                .Do(() => Notifier.RuleActivated());
        }
    }
}
