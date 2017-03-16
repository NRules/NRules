using System;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneEquatableFactRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFact_FiresOnce()
        {
            //Arrange
            var fact = new EquatableFact(1) {TestProperty = "Valid Value 1"};
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
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(2) {TestProperty = "Valid Value 2"};
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Test]
        public void Fire_OneMatchingFactAssertedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            Session.Insert(fact1);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_OneFactUpdatedFromInvalidToMatching_FiresOnce()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Invalid Value 1"};
            var fact2 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            Session.Insert(fact1);
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_OneMatchingFactAssertedAndRetractedAndAssertedAgain_FiresOnce()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact3 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            Session.Insert(fact1);
            Session.Retract(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_OneMatchingFactAssertedAndUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(1) {TestProperty = "Invalid Value 1"};
            Session.Insert(fact1);
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_OneMatchingFactAssertedAndModifiedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(1) {TestProperty = "Invalid Value 1"};
            Session.Insert(fact1);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_DuplicateInsert_Throws()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(1) {TestProperty = "Valid Value 2"};

            //Act - Assert
            Session.Insert(fact1);
            Assert.Throws<ArgumentException>(() => Session.Insert(fact2));
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneEquatableFactRule>();
        }
    }
}