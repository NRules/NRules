using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingFactsInsertAndFireThenUpdateAndFire_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();
            Session.Update(fact1);
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }
        
        [Test]
        public void Fire_MatchingFactsInsertThenUpdateThenFire_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_MatchingFacts_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_MatchingFactsReverseOrder_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact2);
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_TwoMatchingPairsOfFacts_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            var fact3 = new FactType1 {TestProperty = "Valid Value 3"};
            var fact4 = new FactType2 {TestProperty = "Valid Value 4", JoinProperty = fact3.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Test]
        public void Fire_TwoMatchingFactsOfOneKindOneMatchingFactOfAnotherKind_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Test]
        public void Fire_FirstMatchingFactSecondInvalidFact_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Invalid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FirstInvalidFactSecondMatchingFact_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Invalid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_TwoMatchingFactsUnsatisfiedJoin_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = null};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_TwoMatchingFactsFirstRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_TwoMatchingFactsSecondRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_TwoMatchingFactsFirstUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact1.TestProperty = "Invalid Value 1";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_TwoMatchingFactsSecondUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Invalid Value 2";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactRule>();
        }
    }
}