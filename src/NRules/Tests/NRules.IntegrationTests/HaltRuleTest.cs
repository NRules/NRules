using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class HaltRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_TwoMatchingFacts_FiresOnceAndHalts()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_TwoMatchingFactsFireCalledTwice_FiresOnceThenHaltsThenResumesAndFiresAgain()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        protected override void SetUpRules()
        {
            SetUpRule<HaltRule>();
        }
    }
}