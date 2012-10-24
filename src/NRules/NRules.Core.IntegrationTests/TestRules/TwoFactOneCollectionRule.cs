using System.Linq;
using NRules.Core.IntegrationTests.TestAssets;
using NRules.Fluent.Dsl;

namespace NRules.Core.IntegrationTests.TestRules
{
    public class TwoFactOneCollectionRule : BaseRule
    {
        public int FactCount { get; set; }

        public override void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<FactType1>(f1 => f1.TestProperty == "Valid Value")
                .Collect<FactType2>(f2 => f2.TestProperty.StartsWith("Valid"));
            definition.Then()
                .Do(ctx =>
                        {
                            Notifier.RuleActivated();
                            FactCount = ctx.Collection<FactType2>().Count();
                        });
        }
    }
}