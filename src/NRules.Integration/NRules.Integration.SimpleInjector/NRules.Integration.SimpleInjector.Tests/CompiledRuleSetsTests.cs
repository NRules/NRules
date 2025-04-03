
using NRules.Integration.SimpleInjector.Tests.TestAssets;
using SimpleInjector;
using Xunit;
using System.Linq;
using SimpleInjector.Advanced;
using SimpleInjector.Lifestyles;

namespace NRules.Integration.SimpleInjector.Tests;

public class CompiledRuleSetsTests
{

    [Fact]
    public void Resolve_MyCompiledRuleSets02_Works()
    {
        //Arrange
        var container = new Container();
        container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        container.RegisterCompiledRuleSets<MyCompiledRuleSets02>();

        container.Verify();
        
        //Act
        ISession? session02A = null;
        using (var scope = AsyncScopedLifestyle.BeginScope(container))
        {
            var instance = container.GetCompiledRuleSets<MyCompiledRuleSets02>();
            session02A = instance.GetOrCreate();
        }

        //Assert
        session02A.Insert(new TestPerson());
        session02A.Fire();
        
        //Assert
        var person02A = session02A.Query<TestPerson>().Single();
        Assert.Equal("Brat Pitt", person02A.FullName);
    }
}