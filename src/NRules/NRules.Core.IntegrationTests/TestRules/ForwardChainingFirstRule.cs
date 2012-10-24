using NRules.Core.IntegrationTests.TestAssets;
using NRules.Fluent.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class ForwardChainingFirstRule : BaseRule
    {
        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<FactType1>(f1 => f1.TestProperty == "Valid Value");
            definition.Then()
                .Do(ctx =>
                        {
                            Notifier.RuleActivated();
                            var fact2 = new FactType2()
                                            {
                                                TestProperty = "Valid Value",
                                                JoinReference = ctx.Arg<FactType1>()
                                            };
                            ctx.Insert(fact2);
                        });
        }
    }
}