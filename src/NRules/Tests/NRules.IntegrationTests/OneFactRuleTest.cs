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
            var fact = new FactType1 {TestProperty = "Valid Value 1"};
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
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
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
            var fact = new FactType1 {TestProperty = "Invalid Value 1"};
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
            var fact = new FactType1 {TestProperty = "Valid Value 1"};
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
            var fact = new FactType1 {TestProperty = "Invalid Value 1"};
            Session.Insert(fact);

            fact.TestProperty = "Valid Value 1";
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
            var fact = new FactType1 {TestProperty = "Valid Value 1"};
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
            var fact = new FactType1 {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            fact.TestProperty = "Invalid Value 1";
            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test, Ignore("TODO: Rework retract logic to not re-evaluate conditions")]
        public void Fire_OneMatchingFactAssertedAndModifiedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            fact.TestProperty = "Invalid Value 1";
            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_DuplicateInsert_Throws()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = "Valid Value 1"};

            //Act - Assert
            Session.Insert(fact);
            Assert.Throws<ArgumentException>(() => Session.Insert(fact));
        }

        [Test]
        public void Fire_UpdateWithoutInsert_Throws()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = "Valid Value 1"};

            //Act - Assert
            Assert.Throws<ArgumentException>(() => Session.Update(fact));
        }

        [Test]
        public void Fire_RetractWithoutInsert_Throws()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = "Valid Value 1"};

            //Act - Assert
            Assert.Throws<ArgumentException>(() => Session.Retract(fact));
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactRule>();
        }
    }
}