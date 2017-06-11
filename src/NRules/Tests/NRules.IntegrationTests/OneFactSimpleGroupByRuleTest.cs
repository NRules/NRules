using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactSimpleGroupByRuleTest : BaseRuleTestFixture
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
        public void Fire_TwoFactsWithNullKey_FiresOnceWithBothFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = null};
            var fact2 = new FactType {TestProperty = null};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<string, FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsWithNullKeyOneKeyUpdatedToValue_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = null};
            var fact2 = new FactType {TestProperty = null};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            fact2.TestProperty = "Value";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(1, GetFiredFact<IGrouping<string, FactType>>(0).Count());
            Assert.Equal(1, GetFiredFact<IGrouping<string, FactType>>(1).Count());
        }

        [Fact]
        public void Fire_TwoFactsWithValueKeyOneKeyUpdatedToNull_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Value"};
            var fact2 = new FactType {TestProperty = "Value"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            fact2.TestProperty = null;
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(1, GetFiredFact<IGrouping<string, FactType>>(0).Count());
            Assert.Equal(1, GetFiredFact<IGrouping<string, FactType>>(1).Count());
        }

        [Fact]
        public void Fire_OneFactWithValueAnotherWithNullThenNullUpdated_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Value"};
            var fact2 = new FactType {TestProperty = null};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(1, GetFiredFact<IGrouping<string, FactType>>(0).Count());
            Assert.Equal(1, GetFiredFact<IGrouping<string, FactType>>(1).Count());
        }

        [Fact]
        public void Fire_TwoFactsWithNullBothRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = null};
            var fact2 = new FactType {TestProperty = null};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            Session.Retract(fact1);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_OneFactInsertedThenUpdatedToAnotherGroup_FiresOnceWithOneFactInSecondGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            Session.Insert(fact1);
            
            fact1.TestProperty = "Valid Value Group2";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var firedGroup = GetFiredFact<IGrouping<string, FactType>>();
            Assert.Equal(1, firedGroup.Count());
            Assert.Equal("Valid Value Group2", firedGroup.Key);
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
                IGrouping<string, FactType> group = null;

                When()
                    .Query(() => group, x => x
                        .Match<FactType>()
                        .GroupBy(f => f.TestProperty));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}