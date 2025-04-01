using System.Linq;
using NRules.Fluent;
using NRules.Integration.SimpleInjector.Tests.TestAssets;
using Xunit;

namespace NRules.Integration.SimpleInjector.Tests;

public class RuleSetsWithoutDependencyInjectionTests
{
    [Fact]
    public void Load_Compile_Fire_Works()
    {
        //Arrange
        var repo = new RuleRepository();
        repo.Load(s => s.From(sa => sa.AssemblyOf<TestFact1>()).Where(rm => rm.RuleType.Namespace!.EndsWith("SubNamespace01")).To("SubSet01"));
        repo.Load(s => s.From(sa => sa.AssemblyOf<TestFact1>()).Where(rm => rm.RuleType.Namespace!.EndsWith("SubNamespace02")).To("SubSet02"));
        repo.Load(s => s.From(sa => sa.AssemblyOf<TestFact1>()).Where(rm => rm.RuleType.Namespace!.EndsWith("SubNamespace03")).To("SubSet03"));

        var sets = repo.GetRuleSets();
        var ruleDefinitions = sets.SelectMany(s => s.Rules);
        
        var subSet01 = sets.FirstOrDefault(x => x.Name == "SubSet01");
        var subSet02 = sets.FirstOrDefault(x => x.Name == "SubSet02");
        var subSet03 = sets.FirstOrDefault(x => x.Name == "SubSet03");
       
        // Take subSet01 as common set for both and combine...
        var compiler = new RuleCompiler();
        var sessionFactory02 = compiler.Compile([subSet01!, subSet02!]);
        var sessionFactory03 = compiler.Compile([subSet01!, subSet03!]);

        var session02A = sessionFactory02.CreateSession();
        var session03A = sessionFactory03.CreateSession();
        
        //Act
        session02A.Insert(new TestPerson());
        session02A.Fire();
        
        session03A.Insert(new TestPerson());
        session03A.Fire();
        
        //Assert
        Assert.Equal(3, sets.Count());
        Assert.Equal(2, subSet01!.Rules.Count());
        Assert.Single(subSet02!.Rules);
        Assert.Single(subSet03!.Rules);

        var person02A = session02A.Query<TestPerson>().Single();
        Assert.Equal("Brat Pitt", person02A.FullName);
        var person03A = session03A.Query<TestPerson>().Single();
        Assert.Equal("Pitt, Brat", person03A.FullName);
    }
}