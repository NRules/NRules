using NRules.Integration.SimpleInjector.Tests.TestAssets;
using SimpleInjector;
using Xunit;

namespace NRules.Integration.SimpleInjector.Tests
{
    public class RuleDependencyInjectionTests
    {
        [Fact]
        public void Register_Resolve_FiresCorrectly()
        {
            //Arrange
            var builder = new Container();
            builder.Register<ITestService, TestService>(Lifestyle.Singleton);
            builder
                .RegisterRuleRepository<IMyRuleRepositoryA>(
                    x => x.Type(typeof(RuleWithConstructorDependency)))
                .RegisterRuleActivator()
                .RegisterDependencyResolver()
                .RegisterSessionFactory<IMyRuleRepositoryA>();
                
            builder.Verify();
            var container = builder;

            //Act
            var repository = container.ResolveRuleRepository<IMyRuleRepositoryA>();
            var rules = repository.GetRules();
            var sessionFactory = container.ResolveSessionFactory<IMyRuleRepositoryA>();
            var session = sessionFactory.CreateSession();
            session.Insert(new TestFact1());
            int fired = session.Fire();

            //Assert
            Assert.Single(rules);
            Assert.Equal(1, fired);
        }

        public interface IMyRuleRepositoryA : IRuleRepository<IMyRuleRepositoryA>;
    }
}