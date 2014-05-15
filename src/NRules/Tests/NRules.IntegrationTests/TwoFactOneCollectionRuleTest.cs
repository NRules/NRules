using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactOneCollectionRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFactOfOneKindAndTwoOfAnother_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};
            var fact3 = new FactType2() {TestProperty = "Valid Value", JoinReference = null};
            var fact4 = new FactType2() {TestProperty = "Invalid Value", JoinReference = fact1};
            var fact5 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);
            Session.Insert(fact5);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetRuleInstance<TwoFactOneCollectionRule>().FactCount[fact1]);
        }

        [Test]
        public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenOneRetracted_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};
            var fact3 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            Session.Retract(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, GetRuleInstance<TwoFactOneCollectionRule>().FactCount[fact1]);
        }

        [Test]
        public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};
            var fact3 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            Session.Retract(fact2);
            Session.Retract(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FactOfOneKindIsInvalidAndTwoOfAnotherKindAreValid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Invalid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};
            var fact3 = new FactType2() {TestProperty = "Invalid Value", JoinReference = fact1};
            var fact4 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FactOfOneKindIsAssertedThenRetractedAndTwoOfAnotherKindAreValid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};
            var fact3 = new FactType2() {TestProperty = "Invalid Value", JoinReference = fact1};
            var fact4 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            Session.Retract(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FactOfOneKindIsAssertedThenUpdatedToInvalidAndTwoOfAnotherKindAreValid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};
            var fact3 = new FactType2() {TestProperty = "Invalid Value", JoinReference = fact1};
            var fact4 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact1.TestProperty = "Invalid Value";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingOneOfTheFacts_FiresOnce()
        {
            //Arrange
            var fact11 = new FactType1() {TestProperty = "Valid Value"};
            var fact12 = new FactType1() {TestProperty = "Valid Value"};
            var fact21 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact11};
            var fact22 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact11};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);
            Session.Insert(fact22);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingBothOfTheFacts_FiresTwiceWithCorrectCounts()
        {
            //Arrange
            var fact11 = new FactType1() {TestProperty = "Valid Value"};
            var fact12 = new FactType1() {TestProperty = "Valid Value"};
            var fact21 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact11};
            var fact22 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact11};
            var fact23 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact12};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);
            Session.Insert(fact22);
            Session.Insert(fact23);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(2, GetRuleInstance<TwoFactOneCollectionRule>().FactCount[fact11]);
            Assert.AreEqual(1, GetRuleInstance<TwoFactOneCollectionRule>().FactCount[fact12]);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOneCollectionRule>();
        }
    }
}