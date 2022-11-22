using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactOrGroupBindingRuleTest : BaseRuleTestFixture
{
    [Fact]
    public void Fire_FactMatchingFirstPartOfOrGroup_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", Value = "Fact1" };

        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal("Fact1", Fixture.GetFiredFact<string>());
    }

    [Fact]
    public void Fire_FactsMatchingSecondPartOfOrGroup_FiresOnce()
    {
        //Arrange
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", Value = "Fact2" };

        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal("Fact2", Fixture.GetFiredFact<string>());
    }

    [Fact]
    public void Fire_FactsMatchingBothPartsOfOrGroup_FiresTwice()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", Value = "Fact1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", Value = "Fact2" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(2);
        Assert.Equal("Fact1", Fixture.GetFiredFact<string>(0));
        Assert.Equal("Fact2", Fixture.GetFiredFact<string>(1));
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        public string TestProperty { get; set; }
        public string Value { get; set; }
    }

    public class FactType2
    {
        public string TestProperty { get; set; }
        public string Value { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;
            string value = null;

            When()
                .Or(x => x
                    .Match(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Match(() => fact2, f => f.TestProperty.StartsWith("Valid")))
                .Let(() => value, () => GetValue(fact1, fact2));

            Then()
                .Do(ctx => ctx.NoOp());
        }

        private static string GetValue(FactType1 fact1, FactType2 fact2)
        {
            return fact1 != null ? fact1.Value : fact2.Value;
        }
    }
}