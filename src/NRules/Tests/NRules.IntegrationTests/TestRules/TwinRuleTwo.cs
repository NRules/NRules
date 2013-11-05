using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwinRuleTwo : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;

            When()
                .If<FactType1>(() => fact1, f => f.TestProperty == "Valid Value")
                .If<FactType2>(() => fact2, f => f.TestProperty == "Valid Value", f => fact1 == f.JoinReference);

            Then()
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}