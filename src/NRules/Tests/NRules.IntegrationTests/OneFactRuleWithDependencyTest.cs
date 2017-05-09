using System;
using NRules.Extensibility;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactRuleWithDependencyTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_DefaultResolver_Throws()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            //Act - Assert
            Assert.Throws<InvalidOperationException>(() => Session.Fire());
        }

        [Fact]
        public void Fire_OneMatchingFact_FiresOnceAndCallsDependency()
        {
            //Arrange
            var service1 = new TestService1();
            bool service1Called = false;
            service1.ServiceCalled += (sender, args) => service1Called = true;

            var service2 = new TestService2();
            bool service2Called = false;
            service2.ServiceCalled += (sender, args) => service2Called = true;

            Session.DependencyResolver = new TestDependencyResolver(service1, service2);

            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(true, service1Called);
            Assert.Equal(true, service2Called);
        }

        [Fact]
        public void Fire_OneMatchingFact_CanResolveDependencyFromContext()
        {
            //Arrange
            var service1 = new TestService1();
            var service2 = new TestService2();
            Session.DependencyResolver = new TestDependencyResolver(service1, service2);

            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            ITestService1 resolvedService1 = null;
            GetRuleInstance<TestRule>().Action = ctx =>
            {
                resolvedService1 = ctx.Resolve<ITestService1>();
            };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Same(service1, resolvedService1);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public interface ITestService1
        {
            void Action(string value);
        }

        private class TestService1 : ITestService1
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

        public interface ITestService2
        {
            void Action(string value);
        }

        private class TestService2 : ITestService2
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
            private readonly TestService1 _service1;
            private readonly TestService2 _service2;

            public TestDependencyResolver(TestService1 service1, TestService2 service2)
            {
                _service1 = service1;
                _service2 = service2;
            }

            public object Resolve(IResolutionContext context, Type serviceType)
            {
                if (serviceType == typeof(ITestService1))
                    return _service1;
                if (serviceType == typeof(ITestService2))
                    return _service2;
                throw new ArgumentException();
            }
        }

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public Action<IContext> Action = ctx => { };

            public override void Define()
            {
                FactType fact = null;
                ITestService1 service1 = null;
                ITestService2 service2 = null;

                Dependency()
                    .Resolve(() => service1)
                    .Resolve(() => service2);

                When()
                    .Match<FactType>(() => fact, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => Action(ctx))
                    .Do(ctx => service1.Action(fact.TestProperty))
                    .Do(ctx => service2.Action(fact.TestProperty))
                    .Do(ctx => SomeAction(fact, service1, service2));
            }

            private void SomeAction(FactType fact, ITestService1 service1, ITestService2 service2)
            {
                service1.Action(fact.TestProperty);
                service2.Action(fact.TestProperty);
            }
        }
    }
}