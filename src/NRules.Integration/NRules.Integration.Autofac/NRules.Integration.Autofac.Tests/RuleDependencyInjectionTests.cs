using Autofac;
using NRules.Integration.Autofac.Tests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.Integration.Autofac.Tests;

public class RuleDependencyInjectionTests
{
    [Fact]
    public void RuleRepository_Resolved_Returns()
    {
        //Arrange
        var builder = new ContainerBuilder();
        builder.RegisterType<TestService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)));

        var container = builder.Build();

        //Act
        var repository = container.Resolve<IRuleRepository>();
        var rules = repository.GetRules();

        //Assert
        Assert.Single(rules);
    }

    [Fact]
    public void SessionFactory_Resolved_Returns()
    {
        //Arrange
        var builder = new ContainerBuilder();
        builder.RegisterType<TestService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)));
        builder.RegisterSessionFactory();

        var container = builder.Build();

        //Act
        var factory = container.Resolve<ISessionFactory>();

        //Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void Session_Resolved_Returns()
    {
        //Arrange
        var builder = new ContainerBuilder();
        builder.RegisterType<TestService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)));
        builder.RegisterSessionFactory();
        builder.RegisterSession();

        var container = builder.Build();

        //Act
        var session = container.Resolve<ISession>();

        //Assert
        Assert.NotNull(session);
    }

    [Fact]
    public void Session_ConstructorInjectedServiceCalled_Works()
    {
        //Arrange
        var builder = new ContainerBuilder();
        builder.RegisterType<TestService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterRuleRepository(x => x.Type(typeof(RuleWithConstructorDependency)));
        builder.RegisterSessionFactory();
        builder.RegisterSession();

        var container = builder.Build();
        var service = container.Resolve<ITestService>();
        var session = container.Resolve<ISession>();

        //Act
        session.Insert(new TestFact1());
        session.Fire();

        //Assert
        Assert.Equal("It's done", service.Status);
    }

    [Fact]
    public void Session_ActionInjectedServiceCalled_Works()
    {
        //Arrange
        var builder = new ContainerBuilder();
        builder.RegisterType<TestService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterRuleRepository(x => x.Type(typeof(RuleWithActionDependency)));
        builder.RegisterSessionFactory();
        builder.RegisterSession();

        var container = builder.Build();
        var service = container.Resolve<ITestService>();
        var session = container.Resolve<ISession>();

        //Act
        session.Insert(new TestFact1());
        session.Fire();

        //Assert
        Assert.Equal("It's done", service.Status);
    }

    [Fact]
    public void Session_ActionInjectedScopedServiceCalled_Works()
    {
        //Arrange
        var builder = new ContainerBuilder();
        builder.RegisterType<TestService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterRuleRepository(x => x.Type(typeof(RuleWithActionDependency)));
        builder.RegisterSessionFactory();

        var container = builder.Build();

        using var scope = container.BeginLifetimeScope(sb =>
        {
            sb.RegisterType<TestService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            sb.RegisterSession();
        });

        var rootService = container.Resolve<ITestService>();
        var scopedService = scope.Resolve<ITestService>();
        var session = scope.Resolve<ISession>();

        //Act
        session.Insert(new TestFact1());
        session.Fire();

        //Assert
        Assert.Null(rootService.Status);
        Assert.Equal("It's done", scopedService.Status);
    }
}
