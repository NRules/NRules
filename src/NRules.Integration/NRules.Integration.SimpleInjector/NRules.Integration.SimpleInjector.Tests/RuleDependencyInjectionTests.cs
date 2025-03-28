using NRules.Integration.SimpleInjector.Tests.TestAssets;
using NRules.RuleModel;
using SimpleInjector;

namespace NRules.Integration.SimpleInjector.Tests
{
    public class RuleDependencyInjectionTests
    {
        [Fact]
        public void RuleRepository_Resolved_Returns()
        {
            //Arrange
            var builder = new Container();
            builder.Register<ITestService, TestService>(Lifestyle.Singleton);
            builder.RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)));

            builder.Verify();
            var container = builder;

            //Act
            var repository = container.GetInstance<IRuleRepository>();
            var rules = repository.GetRules();

            //Assert
            Assert.Single(rules);
        }
    }
}