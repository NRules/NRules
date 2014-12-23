using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class ThreeFactOrGroupRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingMainFactAndNoneOfOrGroup_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_MatchingMainFactAndFirstPartOfOrGroup_FiresOnce()
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
        public void Fire_MatchingMainFactAndSecondPartOfOrGroup_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 { TestProperty = "Invalid Value 2", JoinProperty = fact1.TestProperty };
            var fact3 = new FactType3 { TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_MatchingMainFactAndBothPartsOfOrGroup_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Invalid Value 3", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType3 {TestProperty = "Valid Value 4", JoinProperty = fact1.TestProperty};

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
        public void Fire_MatchingMainFactAndOnePartOfOrGroupAndMainFactRetracted_DoesNotFire()
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
        public void Fire_MatchingMainFactAndOnePartOfOrGroupAndMainFactUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact1.TestProperty = "Invalid Value";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_MatchingMainFactAndOnePartOfOrGroupAndGroupFactRetracted_DoesNotFire()
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
        public void Fire_MatchingMainFactAndOnePartOfOrGroupAndGroupFactUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Invalid Value";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<ThreeFactOrGroupRule>();
        }
    }
}