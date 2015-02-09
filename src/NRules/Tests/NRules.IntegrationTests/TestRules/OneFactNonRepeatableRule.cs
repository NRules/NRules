using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactNonRepeatableRule : BaseRule
    {
        public override void Define()
        {
            FactType5 fact = null;

            Repeatability(RuleRepeatability.NonRepeatable);

            When()
                .Match<FactType5>(() => fact, f => f.TestProperty.StartsWith("Valid"), f => f.TestCount <= 2);
            Then()
                .Do(ctx => fact.IncrementCount())
                .Do(ctx => ctx.Update(fact))
                .Do(ctx => Notifier.RuleActivated());
        }
    }
}