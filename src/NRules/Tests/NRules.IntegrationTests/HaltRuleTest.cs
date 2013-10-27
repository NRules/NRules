using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class HaltRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void HaltRule_TwoMatchingFacts_FiresOnceAndHalts()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType1() {TestProperty = "Valid Value"};
            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void HaltRule_TwoMatchingFactsFireCalledTwice_FiresOnceThenHaltsThenResumesAndFiresAgain()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType1() {TestProperty = "Valid Value"};
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