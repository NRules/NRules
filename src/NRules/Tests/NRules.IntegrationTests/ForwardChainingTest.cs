using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class ForwardChainingTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFact_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce<ForwardChainingFirstRule>();
            AssertFiredOnce<ForwardChainingSecondRule>();
        }

        [Test]
        public void Fire_OneMatchingFactOfOneKindAndOneOfAnotherKind_FiresSecondRuleDirectlyAndChained()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce<ForwardChainingFirstRule>();
            AssertFiredTwice<ForwardChainingSecondRule>();
        }

        protected override void SetUpRules()
        {
            SetUpRule<ForwardChainingFirstRule>();
            SetUpRule<ForwardChainingSecondRule>();
        }
    }
}