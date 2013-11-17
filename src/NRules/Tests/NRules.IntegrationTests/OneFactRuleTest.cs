using System;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFact_FiresOnce()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Valid Value"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_TwoMatchingFacts_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType1() {TestProperty = "Valid Value"};
            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Test]
        public void Fire_ConditionDoesNotMatch_DoesNotFire()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Invalid Value"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_OneMatchingFactAssertedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Valid Value"};
            Session.Insert(fact);
            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_OneFactUpdatedFromInvalidToMatching_FiresOnce()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Invalid Value"};
            Session.Insert(fact);

            fact.TestProperty = "Valid Value";
            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_OneMatchingFactAssertedAndRetractedAndAssertedAgain_FiresOnce()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Valid Value"};
            Session.Insert(fact);
            Session.Retract(fact);
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_OneMatchingFactAssertedAndUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Valid Value"};
            Session.Insert(fact);

            fact.TestProperty = "Invalid Value";
            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_DuplicateInsert_Throws()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Valid Value"};

            //Act - Assert
            Session.Insert(fact);
            Assert.Throws<ArgumentException>(() => Session.Insert(fact));
        }

        [Test]
        public void Fire_UpdateWithoutInsert_Throws()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Valid Value"};

            //Act - Assert
            Assert.Throws<ArgumentException>(() => Session.Update(fact));
        }

        [Test]
        public void Fire_RetractWithoutInsert_Throws()
        {
            //Arrange
            var fact = new FactType1() {TestProperty = "Valid Value"};

            //Act - Assert
            Assert.Throws<ArgumentException>(() => Session.Retract(fact));
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactRule>();
        }
    }
}