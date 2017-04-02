using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactSimpleGroupByRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_NoMatchingFacts_DoesNotFire()
        {
            //Arrange - Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
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
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType>>().Count());
        }

        [Test]
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
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType>>(0).Count());
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType>>(1).Count());
        }

        [Test]
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
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType>>(0).Count());
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType>>(1).Count());
        }

        [Test]
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
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType>>(0).Count());
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType>>(1).Count());
        }

        [Test]
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

        [Test]
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
            Assert.AreEqual(1, firedGroup.Count());
            Assert.AreEqual("Valid Value Group2", firedGroup.Key);
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