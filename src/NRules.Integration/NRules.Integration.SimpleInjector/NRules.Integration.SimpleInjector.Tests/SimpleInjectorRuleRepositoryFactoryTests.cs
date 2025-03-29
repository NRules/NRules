using NRules.Integration.SimpleInjector.Tests.TestAssets;
using SimpleInjector;
using Xunit;

namespace NRules.Integration.SimpleInjector.Tests;

public class SimpleInjectorRuleRepositoryFactoryTests
{
    [Fact]
    public void RuleRepositoryFactory_CreateNew_Returns()
    {
        //Arrange
        var container = new Container();
        container.Register<ITestService, TestService>(Lifestyle.Singleton);
        
        var factory = new SimpleInjectorRuleRepositoryFactory(container);
        factory.RegisterNamedRuleRepository("default", x => x.Type(typeof(RuleWithConstructorDependency)));
        factory.RegisterNamedRuleRepository("optional", x => x.Type(typeof(RuleWithActionDependency)));
        
        //Act
        var defaultRepository = factory.CreateNew("default");
        var optionalRepository = factory.CreateNew("optional");
        
        //Assert
        Assert.Single(defaultRepository.GetRules());
        Assert.Single(optionalRepository.GetRules());
    }
}