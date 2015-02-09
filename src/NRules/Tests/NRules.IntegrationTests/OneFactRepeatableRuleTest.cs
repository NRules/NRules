using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactRepeatableRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFactEligibleForOneIncrement_FiresOnce()
        {
            //Arrange
            var fact = new FactType5 {TestProperty = "Valid Value 1", TestCount = 2};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_OneMatchingFactEligibleForTwoIncrements_FiresTwice()
        {
            //Arrange
            var fact = new FactType5 {TestProperty = "Valid Value 1", TestCount = 1};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }
        
        protected override void SetUpRules()
        {
            SetUpRule<OneFactRepeatableRule>();
        }
    }
}