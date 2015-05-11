using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactOneGroupByRuleTest : BaseRuleTestFixture
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
        public void Fire_TwoFactsForOneGroupAndOneForAnother_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType1 {TestProperty = "Valid Value Group2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndTwoForAnother_FiresTwiceWithTwoFactsInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType1 {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType1 {TestProperty = "Valid Value Group2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType1>>(0).Count());
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType1>>(1).Count());
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneRetracted_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact3 = new FactType1 { TestProperty = "Valid Value Group2" };
            var fact4 = new FactType1 { TestProperty = "Valid Value Group2" };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            Session.Retract(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToInvalid_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact3 = new FactType1 { TestProperty = "Valid Value Group2" };
            var fact4 = new FactType1 { TestProperty = "Valid Value Group2" };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact4.TestProperty = "Invalid Value";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToFirstGroup_FiresOnceWithThreeFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact3 = new FactType1 { TestProperty = "Valid Value Group2" };
            var fact4 = new FactType1 { TestProperty = "Valid Value Group2" };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact4.TestProperty = "Valid Value Group1";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(3, GetFiredFact<IGrouping<string, FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndOneForAnotherAndOneInvalidTheInvalidUpdatedToSecondGroup_FiresTwiceWithTwoFactsInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact3 = new FactType1 { TestProperty = "Valid Value Group2" };
            var fact4 = new FactType1 { TestProperty = "Invalid Value" };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact4.TestProperty = "Valid Value Group2";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType1>>(0).Count());
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType1>>(1).Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactOneGroupByRule>();
        }
    }
}