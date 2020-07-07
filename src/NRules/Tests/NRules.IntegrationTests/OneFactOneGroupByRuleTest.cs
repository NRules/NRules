using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactOneGroupByRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_NoMatchingFacts_DoesNotFire()
        {
            //Arrange - Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndOneForAnother_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnother_FiresTwiceWithTwoFactsInEachGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
            var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(0).Count());
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(1).Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneRetracted_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
            var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            Session.Retract(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToInvalid_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
            var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Invalid Value";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToFirstGroup_FiresOnceWithThreeFactsInOneGroup()
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
            var actual = GetFiredFact<IGrouping<long, string>>().Count();
            Assert.Equal(3, actual);
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndOneForAnotherAndOneInvalidTheInvalidUpdatedToSecondGroup_FiresTwiceWithTwoFactsInEachGroup()
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
            AssertFiredTwice();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(0).Count());
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(1).Count());
        }
        
        [Fact]
        public void Fire_TwoFactGroupsKeyChangedGroupRemovedAndReAddedThenNewFactInserted_FiresTwiceWithFactsInEachGroup()
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
            AssertFiredTwice();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(0).Count());
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(1).Count());
        }

        [Fact]
        public void Fire_TwoFactsOneGroupAnotherFactFilteredOut_FiresOnceAggregateHasSource()
        {
            //Arrange
            IFact matchedGroup = null;
            Session.Events.RuleFiredEvent += (sender, args) =>
            {
                matchedGroup = args.Facts.Single();
            };

            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

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
                IEnumerable<string> group = null;

                When()
                    .Query(() => group, x => x
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .GroupBy(f => f.GroupProperty, f => f.TestProperty)
                        .Where(g => g.Count() > 1));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}