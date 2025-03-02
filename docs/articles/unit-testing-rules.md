# Unit Testing Rules

NRules is a production rules engine, which means rules are defined in a form of `When <conditions> Then <actions>`. This makes the rules easy to unit test, by creating an isolated rules session with a single rule in it (or a small number of coupled rules), supplying test inputs by inserting facts into the session, calling the [Fire](xref:NRules.ISession.Fire) method and asserting that the rule fired or didn't fire given those inputs.

While it's possible to create your own test fixture to set up the rules engine session and assert rule firings, the [NRules.Testing](xref:NRules.Testing) library provides a set of tools to help set up the rules under test, compile them into a rules session, record rule firings, as well as configure and verify rule firing expectations.

## Setting Up a Test Fixture
[NRules.Testing](xref:NRules.Testing) library defines a [RulesTestFixture](xref:NRules.Testing.RulesTestFixture) that can be used to create test fixtures to house rules tests.

The following code uses [xUnit](https://xunit.net/) to unit test the rules.

```c#
public class MyRuleTest : RulesTestFixture
{
    public class MyFact { }

    public class MyRule : Rule
    {
        public override void Define()
        {
            MyFact fact = default!;

            When()
                .Match(() => fact);

            Then()
                .Do(_ => NoOp());
        }

        private void NoOp() { }
    }

    public MyRuleTest()
    {
        Setup.Rule<MyRule>();
    }
    
    [Fact]
    public void Fire_InsertedOneMatchingFact_FiredOnce()
    {
        //Arrange
        var fact = new MyFact();
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Once));
    }
    
    [Fact]
    public void Fire_NoMatchingFacts_DidNotFire()
    {
        //Arrange - Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}
```

## Verifying Matched Facts
When configuring rule firing expectations, it's possible to set up constraints on the facts matched by the rule, to make expectations more specific. To do this, use corresponding methods on the [Matched](xref:NRules.Testing.Matched) class.

For example, the following code verifies that the rule under test has fired, matching a `Customer` fact, where `IsPreferred` is `true`.
```c#
Verify(x => x.Rule().Fired(Times.Exactly(3), Matched.Fact<Customer>(c => c.IsPreferred)));
```

## Testing Cooperations of Multiple Rules
To test multiple rules together in the same test fixture, more than one rule can be registered during the fixture setup. The rule firing expectations with multiple rules can be set by specifying the actual rule type for which the expectations are verified.

```c#
public class MyRuleTest : RulesTestFixture
{
    public MyRuleTest()
    {
        Setup.Rule<MyRule1>();
        Setup.Rule<MyRule2>();
    }
    
    [Fact]
    public void Fire_InsertedOneMatchingFact_FiredBothRules()
    {
        //Arrange
        var fact = new MyFact();
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x =>
        {
            x.Rule<MyRule1>().Fired(Times.Once);
            x.Rule<MyRule2>().Fired(Times.Twice);
        });
    }
}
```

## Testing Rules with Dependencies
If a rule uses injected dependencies, those can be mocked using any mocking framework, and the mocks can be passed into the rule during construction as part of the test setup.

```c#
var serviceMock = new Mock<IMyService>();
var rule = new MyRule(serviceMock.Object);
Setup.Rule(rule);
```

## Creating a Custom Asserter
[NRules.Testing](xref:NRules.Testing) library does not depend on any specific unit testing or assertion framework, so by default it just throws a [RuleAssertionException](xref:NRules.Testing.RuleAssertionException) when any assertion fails. To better tailor assertions to the specific unit testing framework, implement an [asserter](xref:NRules.Testing.IRuleAsserter) that uses the specific assertion mechanism.

Below we are defining a custom asserter for the [xUnit](https://xunit.net/) framework.

```c#
using NRules.Testing;
using Xunit.Sdk;

public class XUnitRuleAsserter : IRuleAsserter
{
    public void Assert(RuleAssertResult result)
    {
        if (result.Status == RuleAssertStatus.Failed)
        {
            throw new XunitException(result.GetMessage());
        }
    }
}
```

We can set the asserter as part of a base test fixture and use it for all our rule tests.

```c#
using NRules.Testing;

public abstract class BaseRulesTestFixture : RulesTestFixture
{
    protected BaseRulesTestFixture()
    {
        Asserter = new XUnitRuleAsserter();
    }
}
```

[NRules.Testing](xref:NRules.Testing) is a very capable library; see NRules integration tests on GitHub for a wide range of examples.
