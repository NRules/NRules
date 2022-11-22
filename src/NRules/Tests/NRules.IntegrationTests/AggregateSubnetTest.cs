using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class AggregateSubnetTest : BaseRuleTestFixture
{
    [Fact]
    public void Fire_OneMatchingFactInsertedThenUpdatedNoFactsOfSecondKind_UpdatePropagatesFiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);

        fact1.TestProperty = "Valid Value 2";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        var calculatedFact = GetFiredFact<CalculatedFact>();
        Assert.Equal("Valid Value 2", calculatedFact.Value);
    }

    [Fact]
    public void Fire_OneMatchingFactInsertedThenUpdatedHasFactsOfSecondKind_UpdatePropagatesFiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);

        var fact2 = new FactType2 { TestProperty = "Valid Value 1" };
        Session.Insert(fact2);

        fact1.TestProperty = "Valid Value 2";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        var calculatedFact = GetFiredFact<CalculatedFact>();
        Assert.Equal("Valid Value 2", calculatedFact.Value);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        public string TestProperty { get; set; }
    }

    public class FactType2
    {
        public string TestProperty { get; set; }
    }

    public class CalculatedFact
    {
        public string Value { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            IEnumerable<FactType2> query = null;
            CalculatedFact calculatedFact = null;

            When()
                .Match(() => fact1)
                .Query(() => query, q => q
                    .Match<FactType2>(f => f.TestProperty.StartsWith("Valid"))
                    .Select(x => x)
                    .Collect())
                .Let(() => calculatedFact, () => new CalculatedFact { Value = fact1.TestProperty });

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}