using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;

namespace NRules.IntegrationTests.TestRules
{
    [Repeatability(RuleRepeatability.Repeatable)]
    public class OneFactRepeatableRule : BaseRule
    {
        public override void Define()
        {
            FactType5 fact = null;

            When()
                .Match<FactType5>(() => fact, f => f.TestProperty.StartsWith("Valid"), f => f.TestCount <= 2);
            Then()
                .Do(ctx => fact.IncrementCount())
                .Do(ctx => ctx.Update(fact))
                .Do(ctx => Action());
        }
    }
}