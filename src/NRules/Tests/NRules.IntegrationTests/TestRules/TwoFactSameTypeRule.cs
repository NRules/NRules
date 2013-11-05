using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactSameTypeRule : BaseRule
    {
        public override void Define()
        {
            FactType3 fact1 = null;
            FactType3 fact2 = null;

            When()
                .If<FactType3>(() => fact1, f => f.TestProperty == "Valid Value")
                .If<FactType3>(() => fact2, f => f.TestProperty == "Valid Value", f => f.Parent == fact1);

            Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}
