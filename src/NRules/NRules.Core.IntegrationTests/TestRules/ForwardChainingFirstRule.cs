using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class ForwardChainingFirstRule : BaseRule
    {
        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<FactType1>(f1 => f1.TestProperty == "Valid Value");
            definition.Then()
                .Do(ctx => Notifier.RuleActivated())
                .Do(ctx => ctx.Insert(new FactType2()
                    {
                        TestProperty = "Valid Value",
                        JoinReference = ctx.Arg<FactType1>()
                    }));
        }
    }
}