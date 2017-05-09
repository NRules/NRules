using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactOneGroupByFlattenRuleTest : BaseRuleTestFixture
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
        public void Fire_TwoFactsForOneGroup_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(fact1, GetFiredFact<FactType>(0));
            Assert.Equal(fact2, GetFiredFact<FactType>(1));
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupInsertedThenOneUpdated_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(fact1, GetFiredFact<FactType>(0));
            Assert.Equal(fact2, GetFiredFact<FactType>(1));
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndOneForAnother_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(fact1, GetFiredFact<FactType>(0));
            Assert.Equal(fact2, GetFiredFact<FactType>(1));
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnother_FiresWithEachFactFromEachGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(4);
            Assert.Equal(fact1, GetFiredFact<FactType>(0));
            Assert.Equal(fact2, GetFiredFact<FactType>(1));
            Assert.Equal(fact3, GetFiredFact<FactType>(2));
            Assert.Equal(fact4, GetFiredFact<FactType>(3));
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneRetracted_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            Session.Retract(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(fact1, GetFiredFact<FactType>(0));
            Assert.Equal(fact2, GetFiredFact<FactType>(1));
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToInvalid_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Invalid Value";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(fact1, GetFiredFact<FactType>(0));
            Assert.Equal(fact2, GetFiredFact<FactType>(1));
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToFirstGroup_FiresThreeTimesWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Valid Value Group1";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            var firedFacts = new[] {GetFiredFact<FactType>(0), GetFiredFact<FactType>(1), GetFiredFact<FactType>(2)};
            var valid1 = firedFacts.Any(x => Equals(fact1, x));
            var valid2 = firedFacts.Any(x => Equals(fact2, x));
            var valid4 = firedFacts.Any(x => Equals(fact4, x));
            Assert.True(valid1 && valid2 && valid4);
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndOneForAnotherAndOneInvalidTheInvalidUpdatedToSecondGroup_FiresWithEachFactFromEachGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType { TestProperty = "Invalid Value" };

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Valid Value Group2";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(4);
            Assert.Equal(fact1, GetFiredFact<FactType>(0));
            Assert.Equal(fact2, GetFiredFact<FactType>(1));
            Assert.Equal(fact3, GetFiredFact<FactType>(2));
            Assert.Equal(fact4, GetFiredFact<FactType>(3));
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAssertedThenOneRetractedAnotherUpdatedThenOneAssertedBack_FiresTwiceWithFactsFromOneGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            Session.Retract(fact2);
            
            Session.Update(fact1);
            Session.Insert(fact2);
            
            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(2);
            Assert.Equal(fact1, GetFiredFact<FactType>(0));
            Assert.Equal(fact2, GetFiredFact<FactType>(1));
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType fact = null;

                When()
                    .Query(() => fact, q => q
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .GroupBy(f => f.TestProperty)
                        .Where(g => g.Count() > 1)
                        .SelectMany(x => x));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}