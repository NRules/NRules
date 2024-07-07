using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class TestingFixtureTest : RulesTestFixture
{
    [Fact]
    public void Verify_FiresNoneExpectedNone_Passes()
    {
        //Arrange
        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule1>().Fired(Times.Never));

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Passed, result.Status);
        Assert.Equal(0, result.Expected);
        Assert.Equal(0, result.Actual);
    }

    [Fact]
    public void Verify_FiresOnceExpectedOnce_Passes()
    {
        //Arrange
        var fact = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule1>().Fired());

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Passed, result.Status);
        Assert.Equal(1, result.Expected);
        Assert.Equal(1, result.Actual);
    }
    
    [Fact]
    public void Verify_FiresDynamicRuleOnceExpectedOnce_Passes()
    {
        //Arrange
        var fact = new FactType3 { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        Session.Fire();

        //Act
        Verify(x => x.Rule("Test Rule 3").Fired());

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Passed, result.Status);
        Assert.Equal(1, result.Expected);
        Assert.Equal(1, result.Actual);
    }
    
    [Fact]
    public void Verify_FiresTwiceExpectedTwice_Passes()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);
        
        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule1>().Fired(Times.Twice));

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Passed, result.Status);
        Assert.Equal(2, result.Expected);
        Assert.Equal(2, result.Actual);
    }
    
    [Fact]
    public void Verify_FiresThreeTimesExpectedThreeTimes_Passes()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact3 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Insert(fact3);

        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule1>().Fired(Times.Exactly(3)));

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Passed, result.Status);
        Assert.Equal(3, result.Expected);
        Assert.Equal(3, result.Actual);
    }
    
    [Fact]
    public void Verify_FiresTwiceExpectedOnce_Fails()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule1>().Fired());

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Failed, result.Status);
        Assert.Equal(1, result.Expected);
        Assert.Equal(2, result.Actual);
        Assert.Equal(
            @"Rule 'Test Rule 1': Fired; Expected: 1; Actual: 2",
            result.GetMessage());
    }
    
    [Fact]
    public void Verify_FiresOnceExpectedOnceWithDifferentRule_Fails()
    {
        //Arrange
        var fact = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule2>().Fired());

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Failed, result.Status);
        Assert.Equal(1, result.Expected);
        Assert.Equal(0, result.Actual);
        Assert.Equal(
            @"Rule 'Test Rule 2': Fired; Expected: 1; Actual: 0",
            result.GetMessage());
    }

    [Fact]
    public void Verify_FiresOnceExpectedOnceDifferentFact_Fails()
    {
        //Arrange
        var fact = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule1>().Fired(Matched.Fact<FactType1>(f => f.TestProperty == "Valid Value 2")));

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Failed, result.Status);
        Assert.Equal(1, result.Expected);
        Assert.Equal(0, result.Actual);
        Assert.Equal(
            @"Rule 'Test Rule 1': Fired With: Fact NRules.IntegrationTests.TestingFixtureTest+FactType1 where f => (f.TestProperty == ""Valid Value 2""); Expected: 1; Actual: 0",
            result.GetMessage());
    }

    [Fact]
    public void Verify_FiresOnceExpectedNone_Fails()
    {
        //Arrange
        var fact = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule1>().Fired(Times.Never));

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Failed, result.Status);
        Assert.Equal(0, result.Expected);
        Assert.Equal(1, result.Actual);
        Assert.Equal("Rule 'Test Rule 1': Fired; Expected: 0; Actual: 1", result.GetMessage());
    }
    
    [Fact]
    public void Verify_FiresNoneExpectedNoneOtherRuleFires_Passes()
    {
        //Arrange
        var fact = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule2>().Fired(Times.Never));

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Passed, result.Status);
    }
    
    [Fact]
    public void Verify_FiresOnceExpectedOnceExtraRuleFired_Fails()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        Session.Fire();

        //Act
        Verify(x => x.Rule<TestRule1>().Fired());

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Failed, result.Status);
        Assert.Equal("Fired; Expected: 1; Actual: 2", result.GetMessage());
    }

    [Fact]
    public void VerifySequence_Rule1FiresOnceRule2FiresTwiceAsExpected_Passes()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 1" };
        var fact22 = new FactType2 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);
        Session.Insert(fact21);
        Session.Insert(fact22);

        Session.Fire();

        //Act
        VerifySequence(x =>
        {
            x.Rule<TestRule1>().Fired();
            x.Rule<TestRule2>().Fired();
            x.Rule<TestRule2>().Fired();
        });

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Passed, result.Status);
        Assert.Equal(3, result.Expected);
        Assert.Equal(3, result.Actual);
    }
    
    [Fact]
    public void VerifySequence_Rule1FiresOnceRule2FiresTwiceExpectedRule1OnceRule2Once_Fails()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);
        Session.Insert(fact21);

        Session.Fire();

        //Act
        VerifySequence(x =>
        {
            x.Rule<TestRule1>().Fired();
            x.Rule<TestRule2>().Fired();
            x.Rule<TestRule2>().Fired();
        });

        //Assert
        var result = _asserter.GetLastResult();
        Assert.NotNull(result);
        Assert.Equal(RuleAssertStatus.Failed, result.Status);
        Assert.Equal(1, result.Expected);
        Assert.Equal(0, result.Actual);
    }

    protected void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule1>();
        setup.Rule<TestRule2>();
        setup.Rule(BuildTestRule3());
    }

    public class FactType1
    {
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class FactType2
    {
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class FactType3
    {
        [NotNull]
        public string? TestProperty { get; set; }
    }
    
    [Name("Test Rule 1")]
    public class TestRule1 : Rule
    {
        public override void Define()
        {
            FactType1 fact = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }

    [Name("Test Rule 2")]
    public class TestRule2 : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null!;
            FactType2 fact2 = null!;

            When()
                .Match(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Match(() => fact2, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
    
    private IRuleDefinition BuildTestRule3()
    {
        //rule "Test Rule 3"
        //when
        //    fact = FactType3(x => x.TestProperty.StartsWith("Valid"));
        //then
        //    Context.NoOp();

        var builder = new RuleBuilder();
        builder.Name("Test Rule 3");

        PatternBuilder factPattern = builder.LeftHandSide().Pattern(typeof (FactType3), "fact");
        Expression<Func<FactType3, bool>> customerCondition = fact => fact.TestProperty.StartsWith("Valid");
        factPattern.Condition(customerCondition);

        Expression<Action<IContext>> action = ctx => ctx.NoOp();
        builder.RightHandSide().Action(action);

        return builder.Build();
    }

    private readonly RecordingAsserter _asserter;

    public TestingFixtureTest()
    {
        _asserter = new RecordingAsserter();
        Asserter = _asserter;
        SetUpRules(Setup);
    }

    public class RecordingAsserter : IRuleAsserter
    {
        private readonly List<RuleAssertResult> _results = new ();

        public IReadOnlyCollection<RuleAssertResult> Results => _results;
        public RuleAssertResult? GetLastResult() => _results.Any() ? _results.Last() : null;

        public void Assert(RuleAssertResult result)
        {
            _results.Add(result);
        }
    }
}
