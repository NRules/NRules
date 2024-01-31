using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class JoinFollowedByGroupByOnDifferentKeyTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_BulkInsertForMultipleTypes_InsertsOnly_FiresThreeTimesWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Join 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", JoinProperty = "Join 2" };

        var fact21 = new FactType2 { TestProperty = "Valid Group1", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };
        var fact22 = new FactType2 { TestProperty = "Valid Group2", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };
        var fact23 = new FactType2 { TestProperty = "Valid Group3", JoinProperty = fact11.JoinProperty, GroupProperty = "Group2" };

        var fact24 = new FactType2 { TestProperty = "Valid Group4", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };
        var fact25 = new FactType2 { TestProperty = "Valid Group5", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };
        var fact26 = new FactType2 { TestProperty = "Valid Group6", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };

        var facts = new object[] { fact11, fact12, fact21, fact22, fact23, fact24, fact25, fact26 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x =>
        {
            x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 2));
            x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 1));
            x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 3));
        });
    }

    [Fact]
    public void Fire_BulkInsertForMultipleTypes_WithUpdates_FiresThreeTimesWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Join 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", JoinProperty = "Join 2" };

        var fact21 = new FactType2 { TestProperty = "Valid Group1", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };
        var fact22 = new FactType2 { TestProperty = "Valid Group2", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };
        var fact23 = new FactType2 { TestProperty = "Valid Group3", JoinProperty = fact11.JoinProperty, GroupProperty = "Group2" };

        var fact24 = new FactType2 { TestProperty = "Valid Group4", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };
        var fact25 = new FactType2 { TestProperty = "Valid Group5", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };
        var fact26 = new FactType2 { TestProperty = "Valid Group6", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };

        var facts = new object[] { fact11, fact12, fact21, fact22, fact23, fact24, fact25, fact26 };
        Session.InsertAll(facts);

        fact21.GroupProperty = "Group3";
        fact22.GroupProperty = "Group3";
        fact23.GroupProperty = "Group3";
        var factsForUpdate = new[] { fact21, fact22, fact23 };
        Session.UpdateAll(factsForUpdate);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 3)));
    }

    [Fact]
    public void Fire_BulkInsertForMultipleTypes_WithRetracts_FiresThreeTimesWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Join 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", JoinProperty = "Join 2" };

        var fact21 = new FactType2 { TestProperty = "Valid Group1", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };
        var fact22 = new FactType2 { TestProperty = "Valid Group2", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };
        var fact23 = new FactType2 { TestProperty = "Valid Group3", JoinProperty = fact11.JoinProperty, GroupProperty = "Group2" };

        var fact24 = new FactType2 { TestProperty = "Valid Group4", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };
        var fact25 = new FactType2 { TestProperty = "Valid Group5", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };
        var fact26 = new FactType2 { TestProperty = "Valid Group6", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };

        var facts = new object[] { fact11, fact12, fact21, fact22, fact23, fact24, fact25, fact26 };
        Session.InsertAll(facts);

        var factsForRetract = new[] { fact21, fact22, fact23 };
        Session.RetractAll(factsForRetract);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 3)));
    }

    [Fact]
    public void Fire_TwoMatchingSetsFactOfFirstKindUpdated_FiresTwiceThenFiresOnce()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Join 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", JoinProperty = "Join 2" };

        var fact61 = new FactType2 { TestProperty = "Valid Group1", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };
        var fact62 = new FactType2 { TestProperty = "Valid Group2", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };

        var fact63 = new FactType2 { TestProperty = "Valid Group3", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };
        var fact64 = new FactType2 { TestProperty = "Valid Group4", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };

        var facts = new object[] { fact11, fact12, fact61, fact62, fact63, fact64 };
        Session.InsertAll(facts);

        //Act - 1
        Session.Fire();

        //Assert - 1
        Verify(x => x.Rule().Fired(Times.Twice));

        //Act - 2
        Session.Update(fact11);
        Session.Fire();

        //Assert - 2
        Verify(x => x.Rule().Fired(Times.Exactly(3)));
    }

    [Fact]
    public void Fire_TwoMatchingSetsFactOfSecondKindUpdated_FiresTwiceThenFiresOnce()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Join 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", JoinProperty = "Join 2" };

        var fact61 = new FactType2 { TestProperty = "Valid Group1", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };
        var fact62 = new FactType2 { TestProperty = "Valid Group2", JoinProperty = fact11.JoinProperty, GroupProperty = "Group1" };

        var fact63 = new FactType2 { TestProperty = "Valid Group3", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };
        var fact64 = new FactType2 { TestProperty = "Valid Group4", JoinProperty = fact12.JoinProperty, GroupProperty = "Group2" };

        var facts = new object[] { fact11, fact12, fact61, fact62, fact63, fact64 };
        Session.InsertAll(facts);

        //Act - 1
        Session.Fire();

        //Assert - 1
        Verify(x => x.Rule().Fired(Times.Twice));

        //Act - 2
        Session.Update(fact61);
        Session.Fire();

        //Assert - 2
        Verify(x => x.Rule().Fired(Times.Exactly(3)));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        [NotNull]
        public string? TestProperty { get; set; }
        [NotNull]
        public string? JoinProperty { get; set; }
    }

    public class FactType2
    {
        [NotNull]
        public string? TestProperty { get; set; }
        [NotNull]
        public string? JoinProperty { get; set; }
        [NotNull]
        public string? GroupProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact = null!;
            IGrouping<string, FactType2> group = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => group, x => x
                    .Match<FactType2>(
                        f => f.TestProperty.StartsWith("Valid"),
                        f => f.JoinProperty == fact.JoinProperty)
                    .GroupBy(f => f.GroupProperty));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}
