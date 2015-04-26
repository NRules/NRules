using System;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactRuleWithDependency : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            ITestService service = null;

            Dependency()
                .Required(() => service);

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action())
                .Do(ctx => Test(service, fact1));
        }

        private static void Test(ITestService service, FactType1 fact1)
        {
            Console.WriteLine(fact1.TestProperty);
            //service.DoSomething(fact1.TestProperty);
        }
    }
}