using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class CoJoinedCollectAndExistsRulesTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingFactOfFirstKindNoFactsOfOtherKind_FiresCollect()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce<TwoFactOneCollectionRule>();
            AssertDidNotFire<TwoFactOneExistsCheckRule>();
        }

        [Test]
        public void Fire_MatchingFactOfFirstKindAndMatchingFactOfOtherKind_EachFiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce<TwoFactOneCollectionRule>();
            AssertFiredOnce<TwoFactOneExistsCheckRule>();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOneExistsCheckRule>();
            SetUpRule<TwoFactOneCollectionRule>();
        }
    }
}
