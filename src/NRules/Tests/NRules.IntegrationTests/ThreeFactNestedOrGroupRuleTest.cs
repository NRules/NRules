using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class ThreeFactNestedOrGroupRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_MatchingOuterFact_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_MatchingInnerFact_FiresOnce()
    {
        //Arrange
        var fact2 = new FactType2 { TestProperty = "Valid Value 2" };

        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_MatchingInnerAndOuterFacts_FiresTwice()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
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

    public class FactType3
    {
        public string TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;
            FactType3 fact3 = null;

            When()
                .Or(x => x
                    .Match(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Or(xx => xx
                        .Match(() => fact2, f => f.TestProperty.StartsWith("Valid"))
                        .Match(() => fact3, f => f.TestProperty.StartsWith("Valid"))));

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}