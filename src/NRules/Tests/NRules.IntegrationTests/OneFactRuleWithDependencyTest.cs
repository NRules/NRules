using System;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
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
            var fact = new FactType1 {TestProperty = "Valid Value 1"};
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

            var fact = new FactType1 {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(true, serviceCalled);
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactRuleWithDependency>();
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
    }
}