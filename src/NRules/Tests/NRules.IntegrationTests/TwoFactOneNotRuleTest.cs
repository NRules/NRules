using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactOneNotRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void TwoFactOneNotRule_MatchingNotPatternFacts_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void TwoFactOneNotRule_MatchingFactOneNoFactsMatchingNotPattern_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOneNotRule>();
        }
    }
}