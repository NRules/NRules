using System.Collections.Generic;
using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneEquatableFactOneCollectionRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_TwoMatchingFactsAndOneInvalid_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(2) {TestProperty = "Valid Value 2"};
            var fact3 = new EquatableFact(3) {TestProperty = "Invalid Value 3"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IEnumerable<EquatableFact>>().Count());
        }
        
        [Test]
        public void Fire_OneMatchingFactInsertedThenUpdated_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact11 = new EquatableFact(1) {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);
            Session.Update(fact11);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, GetFiredFact<IEnumerable<EquatableFact>>().Count());
        }

        [Test]
        public void Fire_TwoMatchingFactsInsertedOneUpdated_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(2) {TestProperty = "Valid Value 2"};
            var fact21 = new EquatableFact(2) {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Update(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IEnumerable<EquatableFact>>().Count());
        }

        [Test]
        public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(2) {TestProperty = "Valid Value 2"};
            var fact21 = new EquatableFact(2) {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, GetFiredFact<IEnumerable<EquatableFact>>().Count());
        }

        [Test]
        public void Fire_TwoMatchingFactsInsertedTwoRetracted_FiresOnceWithEmptyCollection()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact11 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(2) {TestProperty = "Valid Value 2"};
            var fact21 = new EquatableFact(2) {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact11);
            Session.Retract(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(0, GetFiredFact<IEnumerable<EquatableFact>>().Count());
        }

        [Test]
        public void Fire_TwoMatchingFactsInsertedOneUpdatedToInvalid_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(2) {TestProperty = "Valid Value 2"};
            var fact21 = new EquatableFact(2) {TestProperty = "Invalid Value"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            Session.Update(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, GetFiredFact<IEnumerable<EquatableFact>>().Count());
        }

        [Test]
        public void Fire_OneMatchingFactsAndOneInvalidInsertedTheInvalidUpdatedToValid_FiresOnceWithTwoFactInCollection()
        {
            //Arrange
            var fact1 = new EquatableFact(1) {TestProperty = "Valid Value 1"};
            var fact2 = new EquatableFact(2) {TestProperty = "Invalid Value"};
            var fact21 = new EquatableFact(2) {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            Session.Update(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IEnumerable<EquatableFact>>().Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneEquatableFactOneCollectionRule>();
        }
    }
}