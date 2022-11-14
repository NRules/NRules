using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactOneCollectToLookupRuleTest : BaseRuleTestFixture
{
    [Fact]
    public void Fire_NoMatchingFacts_FiresOnceWithEmptyLookup()
    {
        //Arrange - Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.Empty(GetFiredFact<ILookup<long, string>>());
    }
    
    [Fact]
    public void Fire_TwoFactsForOneGroup_FiresOnceWithFactsInOneGroup()
    {
        //Arrange
        var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};

        var facts = new[] {fact1, fact2};
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.Single(GetFiredFact<IKeyedLookup<long, string>>());
        Assert.Equal(2, GetFiredFact<IKeyedLookup<long, string>>()[1].Count());
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndThreeForAnother_FiresOnceWithFactsInCorrectGroups()
    {
        //Arrange
        var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
        var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
        var fact5 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

        var facts = new[] {fact1, fact2, fact3, fact4, fact5};
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>().Count());
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>()[1].Count());
        Assert.Equal(3, GetFiredFact<ILookup<long, string>>()[2].Count());
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndThreeForAnotherOneRetracted_FiresOnceWithTwoFactsInEachGroup()
    {
        //Arrange
        var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
        var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
        var fact5 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

        var facts = new[] {fact1, fact2, fact3, fact4, fact5};
        Session.InsertAll(facts);

        Session.Retract(fact4);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>().Count());
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>()[1].Count());
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>()[2].Count());
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndOneForAnotherOneUpdatedToInvalid_FiresOnceWithTwoFactsInOneGroup()
    {
        //Arrange
        var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

        var facts = new[] {fact1, fact2, fact3};
        Session.InsertAll(facts);

        fact3.TestProperty = "Invalid Value";
        Session.Update(fact3);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.Single(GetFiredFact<ILookup<long, string>>());
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>()[1].Count());
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToFirstGroup_FiresOnceWithThreeFactsInOneGroupAndOneInAnother()
    {
        //Arrange
        var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
        var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

        var facts = new[] {fact1, fact2, fact3, fact4};
        Session.InsertAll(facts);

        fact4.GroupProperty = 1;
        Session.Update(fact4);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>().Count());
        Assert.Equal(3, GetFiredFact<ILookup<long, string>>()[1].Count());
        Assert.Single(GetFiredFact<ILookup<long, string>>()[2]);
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndOneForAnotherAndOneInvalidTheInvalidUpdatedToValid_FiresOnceWithTwoFactsInEachGroup()
    {
        //Arrange
        var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
        var fact4 = new FactType {GroupProperty = 2, TestProperty = "Invalid Value"};

        var facts = new[] {fact1, fact2, fact3, fact4};
        Session.InsertAll(facts);

        fact4.TestProperty = "Valid Value";
        Session.Update(fact4);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>().Count());
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>()[1].Count());
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>()[2].Count());
    }
    
    [Fact]
    public void Fire_TwoFactGroupsKeyChangedGroupRemovedAndReAddedThenNewFactInserted_FiresOnceWithFactsInEachGroup()
    {
        //Arrange
        var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact2 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
        var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

        var facts = new[] {fact1, fact2, fact3};
        Session.InsertAll(facts);

        fact1.GroupProperty = 2;
        fact3.GroupProperty = 1;
        Session.UpdateAll(new object[] {fact1, fact3});

        var fact4 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        Session.Insert(fact4);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>().Count());
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>()[1].Count());
        Assert.Equal(2, GetFiredFact<ILookup<long, string>>()[2].Count());
    }

    [Fact]
    public void Fire_TwoFactsOneGroupAnotherFactFilteredOut_FiresOnceAggregateHasSource()
    {
        //Arrange
        IFact matchedGroup = null;
        Session.Events.RuleFiredEvent += (_, args) =>
        {
            matchedGroup = args.Facts.Single();
        };

        var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
        var fact3 = new FactType {GroupProperty = 2, TestProperty = "Invalid Value"};

        var facts = new[] {fact1, fact2, fact3};
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        AssertFiredOnce();
        Assert.NotNull(matchedGroup);
        Assert.NotNull(matchedGroup.Source);
        Assert.Equal(FactSourceType.Aggregate, matchedGroup.Source.SourceType);
        Assert.Collection(matchedGroup.Source.Facts,
            item => Assert.Same(fact1, item.Value),
            item => Assert.Same(fact2, item.Value));
    }

    protected override void SetUpRules()
    {
        SetUpRule<TestRule>();
    }

    public class FactType
    {
        public long GroupProperty { get; set; }
        public string TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            IKeyedLookup<long, string> lookup = null;

            When()
                .Query(() => lookup, x => x
                    .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect()
                    .ToLookup(f => f.GroupProperty, f => f.TestProperty));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}