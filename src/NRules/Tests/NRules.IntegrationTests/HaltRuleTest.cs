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
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

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
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

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