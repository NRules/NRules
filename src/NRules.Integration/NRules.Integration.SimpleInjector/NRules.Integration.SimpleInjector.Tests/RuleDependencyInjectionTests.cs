using NRules.Integration.SimpleInjector.Tests.TestAssets;
using NRules.RuleModel;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace NRules.Integration.SimpleInjector.Tests
{
    public class RuleDependencyInjectionTests
    {
       [Fact]
        public void RuleRepository_Resolved_Returns()
        {
            //Arrange
            var builder = new Container();
            builder.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            builder.Register<ITestService, TestService>(Lifestyle.Singleton);
            builder.RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)));

            builder.Verify();
            var container = builder;

            //Act
            using (var scope = AsyncScopedLifestyle.BeginScope(container))
            {
                var repository = container.GetInstance<IRuleRepository>();
                var rules = repository.GetRules();

                //Assert
                Assert.Single(rules);
            }
        }

        [Fact]
        public void SessionFactory_Resolved_Returns()
        {
            //Arrange
            var builder = new Container();
            builder.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            builder.Register<ITestService, TestService>(Lifestyle.Singleton);
            builder
                .RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)))
                .RegisterSessionFactory();

            builder.Verify();
            var container = builder;

            //Act
            using (var scope = AsyncScopedLifestyle.BeginScope(container))
            {
                var factory = container.GetInstance<ISessionFactory>();

                //Assert
                Assert.NotNull(factory);
            }
        }

        [Fact]
        public void Session_Resolved_Returns()
        {
            //Arrange
            var builder = new Container();
            builder.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            builder.Register<ITestService, TestService>(Lifestyle.Singleton);
            builder
                .RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)))
                .RegisterSessionFactory()
                .RegisterSession();

            builder.Verify();
            var container = builder;

            //Act
            using (var scope = AsyncScopedLifestyle.BeginScope(container))
            {
                var session = container.GetInstance<ISession>();
                
                //Assert
                Assert.NotNull(session);
            }
        }

        [Fact]
        public void Session_ConstructorInjectedServiceCalled_Works()
        {
            //Arrange
            var builder = new Container();
            builder.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            builder.Register<ITestService, TestService>(Lifestyle.Singleton);
            builder
                .RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)))
                .RegisterSessionFactory()
                .RegisterSession();

            builder.Verify();
            var container = builder;
            //Act
            using (var scope = AsyncScopedLifestyle.BeginScope(container))
            {
                var service = container.GetInstance<ITestService>();
                var session = container.GetInstance<ISession>();

                session.Insert(new TestFact1());
                session.Fire();
                //Assert
                Assert.Equal("It's done", service.Status);
            }
        }

        [Fact]
        public void Session_ActionInjectedServiceCalled_Works()
        {
            //Arrange
            var builder = new Container();
            builder.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            builder.Register<ITestService, TestService>(Lifestyle.Singleton);
            builder
                .RegisterRuleRepository(x => x.Type(typeof(RuleWithActionDependency)))
                .RegisterSessionFactory()
                .RegisterSession();

            builder.Verify();
            var container = builder;
            
            //Act
            using (var scope = AsyncScopedLifestyle.BeginScope(container))
            {
                var service = container.GetInstance<ITestService>();
                var session = container.GetInstance<ISession>();

                session.Insert(new TestFact1());
                session.Fire();

                //Assert
                Assert.Equal("It's done", service.Status);
            }
        }

    }
}