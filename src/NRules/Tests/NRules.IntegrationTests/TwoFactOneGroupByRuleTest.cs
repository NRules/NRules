using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactOneGroupByRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFactOfOneKindAndTwoOfAnother_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = null};
            var fact4 = new FactType2 {TestProperty = "Invalid Value", JoinProperty = fact1.TestProperty};
            var fact5 = new FactType2 {TestProperty = "Valid Value Group 3", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);
            Session.Insert(fact5);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>(0).Count());
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>(1).Count());
        }

        [Test]
        public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherInsertedInOppositeOrder_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = null};
            var fact4 = new FactType2 {TestProperty = "Invalid Value", JoinProperty = fact1.TestProperty};
            var fact5 = new FactType2 {TestProperty = "Valid Value Group 3", JoinProperty = fact1.TestProperty};

            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);
            Session.Insert(fact5);
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>(0).Count());
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>(1).Count());
        }

        [Test]
        public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherThenFireThenAnotherMatchingFactForSecondGroupThenFire_FiresTwiceWithOneFactInEachGroupThenFiresAgainWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact21 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = fact1.TestProperty};
            var fact23 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact21);
            Session.Insert(fact22);

            //Act
            Session.Fire();
            var actualCount11 = GetFiredFact<IGrouping<string, FactType2>>(0).Count();
            var actualCount12 = GetFiredFact<IGrouping<string, FactType2>>(1).Count();

            Session.Insert(fact23);
            Session.Fire();
            var actualCount2 = GetFiredFact<IGrouping<string, FactType2>>(2).Count();

            //Assert
            AssertFiredTimes(3);
            Assert.AreEqual(1, actualCount11);
            Assert.AreEqual(1, actualCount12);
            Assert.AreEqual(2, actualCount2);
        }

        [Test]
        public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenOneRetracted_FiresOnceWithOneFactInGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            Session.Retract(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>().Count());
        }

        [Test]
        public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};

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
            var fact1 = new FactType1 {TestProperty = "Invalid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Invalid Value", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = fact1.TestProperty};

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
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Invalid Value", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = fact1.TestProperty};

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
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Invalid Value", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact1.TestProperty = "Invalid Value 1";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FactOfOneKindIsInvalidThenUpdatedToValidAndTwoOfAnotherKindAreValid_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Invalid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = "Valid Value 1"};
            var fact3 = new FactType2 {TestProperty = "Invalid Value", JoinProperty = "Valid Value 1"};
            var fact4 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = "Valid Value 1"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact1.TestProperty = "Valid Value 1";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>(0).Count());
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>(1).Count());
        }

        [Test]
        public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingOneOfTheFacts_FiresOnceWithTwoFactsInGroups()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact12 = new FactType1 {TestProperty = "Valid Value 2"};
            var fact21 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact11.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact11.TestProperty};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);
            Session.Insert(fact22);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType2>>().Count());
        }

        [Test]
        public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingBothOfTheFacts_FiresThreeTimesWithCorrectCounts()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact12 = new FactType1 {TestProperty = "Valid Value 2"};
            var fact21 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact11.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact11.TestProperty};
            var fact23 = new FactType2 {TestProperty = "Valid Value Group 2", JoinProperty = fact11.TestProperty};
            var fact24 = new FactType2 {TestProperty = "Valid Value Group 1", JoinProperty = fact12.TestProperty};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);
            Session.Insert(fact22);
            Session.Insert(fact23);
            Session.Insert(fact24);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType2>>(0).Count());
            Assert.AreSame(fact11, GetFiredFact<FactType1>(0));
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>(1).Count());
            Assert.AreSame(fact11, GetFiredFact<FactType1>(1));
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType2>>(2).Count());
            Assert.AreSame(fact12, GetFiredFact<FactType1>(2));
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOneGroupByRule>();
        }
    }
}