using System;
using NRules.Extensibility;
using NRules.IntegrationTests.TestAssets;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactRuleWithDependencyTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_DefaultResolver_Throws()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            //Act - Assert
            Assert.Throws<InvalidOperationException>(() => Session.Fire());
        }

        [Test]
        public void Fire_OneMatchingFact_FiresOnceAndCallsDependency()
        {
            //Arrange
            bool serviceCalled = false;
            var service = new TestService();
            service.ServiceCalled += (sender, args) => serviceCalled = true;

            Session.DependencyResolver = new TestDependencyResolver(service);

            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(true, serviceCalled);
        }

        [Test]
        public void Fire_OneMatchingFact_CanResolveDependencyFromContext()
        {
            //Arrange
            var service = new TestService();
            Session.DependencyResolver = new TestDependencyResolver(service);

            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            ITestService resolvedService = null;
            GetRuleInstance<TestRule>().Action = ctx =>
            {
                resolvedService = ctx.Resove<ITestService>();
            };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreSame(service, resolvedService);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public interface ITestService
        {
            void Action(string value);
        }

        private class TestService : ITestService
        {
            public event EventHandler ServiceCalled;

            public void Action(string value)
            {
                var handler = ServiceCalled;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        private class TestDependencyResolver : IDependencyResolver
        {
            private readonly TestService _service;

            public TestDependencyResolver(TestService service)
            {
                _service = service;
            }

            public object Resolve(IResolutionContext context, Type serviceType)
            {
                return _service;
            }
        }

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : BaseRule
        {
            public override void Define()
            {
                FactType fact = null;
                ITestService service = null;

                Dependency()
                    .Resolve(() => service);

                When()
                    .Match<FactType>(() => fact, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => Action(ctx))
                    .Do(ctx => service.Action(fact.TestProperty));
            }
        }
    }
}