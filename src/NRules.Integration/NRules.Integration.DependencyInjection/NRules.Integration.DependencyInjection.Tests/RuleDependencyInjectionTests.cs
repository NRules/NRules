using Microsoft.Extensions.DependencyInjection;
using NRules.Integration.DependencyInjection.Tests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.Integration.DependencyInjection.Tests;

public class RuleDependencyInjectionTests
{
    [Fact]
    public void Activate_Default_Returns()
    {
        //Arrange
        var services = new ServiceCollection();
        var container = services.BuildServiceProvider();
        var activator = new RuleActivator(container);
        
        //Act
        var rule = activator.Activate(typeof(RuleWithoutDependencies));
        
        //Assert
        Assert.NotNull(rule);
    }
    
    [Fact]
    public void RuleRepository_Resolved_Returns()
    {
        //Arrange
        var services = new ServiceCollection();
        services.AddRules(x => x.Type(typeof(RuleWithConstructorDependency)));
        services.AddSingleton<ITestService, TestService>();

        var container = services.BuildServiceProvider();

        //Act
        var repository = container.GetRequiredService<IRuleRepository>();
        var rules = repository.GetRules();

        //Assert
        Assert.Single(rules);
    }

    [Fact]
    public void SessionFactory_Resolved_Returns()
    {
        //Arrange
        var services = new ServiceCollection();
        services.AddRules(x => x.Type(typeof(RuleWithConstructorDependency)));
        services.AddSingleton<ITestService, TestService>();

        var container = services.BuildServiceProvider();

        //Act
        var factory = container.GetRequiredService<ISessionFactory>();

        //Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void Session_Resolved_Returns()
    {
        //Arrange
        var services = new ServiceCollection();
        services.AddRules(x => x.Type(typeof(RuleWithConstructorDependency)));
        services.AddSingleton<ITestService, TestService>();

        var container = services.BuildServiceProvider();

        //Act
        var session = container.GetRequiredService<ISession>();

        //Assert
        Assert.NotNull(session);
    }

    [Fact]
    public void Session_ConstructorInjectedServiceCalled_Works()
    {
        //Arrange
        var services = new ServiceCollection();
        services.AddRules(x => x.Type(typeof(RuleWithConstructorDependency)));
        services.AddSingleton<ITestService, TestService>();

        var container = services.BuildServiceProvider();
        var service = container.GetRequiredService<ITestService>();
        var session = container.GetRequiredService<ISession>();

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
        var services = new ServiceCollection();
        services.AddRules(x => x.Type(typeof(RuleWithActionDependency)));
        services.AddSingleton<ITestService, TestService>();

        var container = services.BuildServiceProvider();
        var service = container.GetRequiredService<ITestService>();
        var session = container.GetRequiredService<ISession>();

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
        var services = new ServiceCollection();
        services.AddRules(x => x.Type(typeof(RuleWithActionDependency)));
        services.AddScoped<ITestService, TestService>();

        var container = services.BuildServiceProvider();
        using var scope = container.CreateScope();
        
        var rootService = container.GetRequiredService<ITestService>();
        var scopedService = scope.ServiceProvider.GetRequiredService<ITestService>();
        var session = scope.ServiceProvider.GetRequiredService<ISession>();

        //Act
        session.Insert(new TestFact1());
        session.Fire();

        //Assert
        Assert.Null(rootService.Status);
        Assert.Equal("It's done", scopedService.Status);
    }
}