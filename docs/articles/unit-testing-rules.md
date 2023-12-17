# Unit Testing Rules

NRules is a production rules engine, which means rules are defined in a form of `When <conditions> Then <actions>`. This makes the rules easy to unit test, by creating an isolated rules session with a single rule in it, supplying test inputs by inserting facts into the session, calling the `Fire` method and asserting that the rule fired or didn't fire given those inputs.  

Below is an example of a test fixture that uses [xUnit](https://xunit.net/) that bootstraps the rules session for any single rule, and exposes methods for asserting the rule's firing.
```c#
using NRules;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using Xunit;

public abstract class RuleTestFixture
{
    protected readonly ISession Session;
    private int _fireCount = 0;

    protected abstract Rule SetUpRule();
    
    protected RuleTestFixture()
    {
        var rule = SetUpRule();
        var definitionFactory = new RuleDefinitionFactory();
        var ruleDefinition = definitionFactory.Create(rule);

        var compiler = new RuleCompiler();
        ISessionFactory factory = compiler.Compile(new []{ruleDefinition});
        Session = factory.CreateSession();
        Session.Events.RuleFiredEvent += (sender, args) => _fireCount++;
    }
    
    protected void AssertFiredOnce()
    {
        AssertFiredTimes(1);
    }

    protected void AssertFiredTimes(int expected)
    {
        Assert.Equal(expected, _fireCount);
    }

    protected void AssertDidNotFire()
    {
        AssertFiredTimes(0);
    }
}
```

With the above base test fixture, a rule can be tested in the following way:
```c#
public class MyRuleTest : RuleTestFixture
{
    [Fact]
    public void Fire_InsertedOneMatchingFact_FiredOnce()
    {
        //Arrange
        var fact = new MyFact();
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
    }
    
        
    [Fact]
    public void Fire_NoMatchingFacts_DidNotFire()
    {
        //Arrange - Act
        Session.Fire();

        //Assert
        AssertDidNotFire();
    }

    protected override Rule SetUpRule()
    {
        return new MyRule();
    }

    public class MyFact { }

    public class MyRule : Rule
    {
        public override void Define()
        {
            MyFact fact = default;

            When()
                .Match(() => fact);

            Then()
                .Do(_ => NoOp());
        }

        private void NoOp() { }
    }
}
```

If a rule uses injected dependencies, those can be mocked using any mocking framework, and the mocks can be injected into the rule during construction.

This of course is just a simple example that can be extended by collecting more information about the rule's firings, including the actual matched facts. It can also be generalized for testing cooperations of multiple rules to unit test constructs such as forward chaining.
See NRules integration tests on GitHub for examples of how to do that.