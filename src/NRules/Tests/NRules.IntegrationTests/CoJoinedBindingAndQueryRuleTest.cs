using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class CoJoinedBindingAndQueryRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_MatchingFactOfGroupKindOnly_FiresWithDefaultKey()
    {
        //Arrange
        var fact = new FactType2 { GroupKey = "Group1" };

        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Key == "Group1|0")));
    }

    [Fact]
    public void Fire_MatchingFactsOfBothKinds_FiresWithCorrectKey()
    {
        //Arrange
        var fact1 = new FactType1 { Value = "1" };
        var fact2 = new FactType2 { GroupKey = "Group1" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Key == "Group1|1")));
    }

    [Fact]
    public void Fire_MatchingFactsOfBothKindsReverseInsertOrder_FiresWithCorrectKey()
    {
        //Arrange
        var fact1 = new FactType1 { Value = "1" };
        var fact2 = new FactType2 { GroupKey = "Group1" };

        Session.Insert(fact2);
        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Key == "Group1|1")));
    }

    [Fact]
    public void Fire_MatchingFactsOfBothKindsFirstUpdated_FiresWithCorrectKey()
    {
        //Arrange
        var fact1 = new FactType1 { Value = "1" };
        var fact2 = new FactType2 { GroupKey = "Group1" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        fact1.Value = "2";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Key == "Group1|2")));
    }

    [Fact]
    public void Fire_MatchingFactsOfBothKindsSecondUpdated_FiresWithCorrectKey()
    {
        //Arrange
        var fact1 = new FactType1 { Value = "1" };
        var fact2 = new FactType2 { GroupKey = "Group1" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        fact2.GroupKey = "Group2";
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Key == "Group2|1")));
    }

    [Fact]
    public void Fire_MatchingFactsOfBothKindsFirstRetracted_FiresWithDefaultKey()
    {
        //Arrange
        var fact1 = new FactType1 { Value = "1" };
        var fact2 = new FactType2 { GroupKey = "Group1" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        Session.Retract(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Key == "Group1|0")));
    }

    [Fact]
    public void Fire_MatchingFactsOfBothKindsSecondRetracted_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { Value = "1" };
        var fact2 = new FactType2 { GroupKey = "Group1" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        Session.Retract(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        public string Value { get; set; }
    }

    public class FactType2
    {
        public string GroupKey { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact = null;
            IEnumerable<FactType1> collection = null;
            IGrouping<string, FactType2> group = null;

            When()
                .Query(() => collection, q => q
                    .Match<FactType1>()
                    .Collect())
                .Let(() => fact, () => collection.FirstOrDefault() ?? new FactType1 { Value = "0" })
                .Query(() => group, q => q
                    .Match<FactType2>()
                    .GroupBy(x => x.GroupKey + "|" + fact.Value));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}