using NRules.Fluent.Dsl;

namespace NRules.Integration.Autofac.Tests.TestAssets
{
    public class RuleWithConstructorDependency : Rule
    {
        public ITestService Service { get; }

        public RuleWithConstructorDependency(ITestService service)
        {
            Service = service;
        }

        public override void Define()
        {
            TestFact1 fact1 = default;

            When()
                .Match(() => fact1);

            Then()
                .Do(_ => Service.DoIt());
        }
    }
}
