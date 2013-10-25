using NRules.Core.IntegrationTests.TestAssets;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class ForwardChainingFirstRule : BaseRule
    {
        public override void Define(IDefinition definition)
        {
            FactType1 fact1 = null;

            definition.When()
                .If<FactType1>(() => fact1, f => f.TestProperty == "Valid Value");
            definition.Then()
                .Do(() => Notifier.RuleActivated())
                .Do(() => Context.Insert(new FactType2()
                    {
                        TestProperty = "Valid Value",
                        JoinReference = fact1
                    }));
        }
    }
}