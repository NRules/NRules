using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class ThreeFactNestedOrGroupRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingOuterFact_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_MatchingInnerFact_FiresOnce()
        {
            //Arrange
            var fact2 = new FactType2 {TestProperty = "Valid Value 2"};

            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_MatchingInnerAndOuterFacts_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        protected override void SetUpRules()
        {
            SetUpRule<ThreeFactNestedOrGroupRule>();
        }
    }
}