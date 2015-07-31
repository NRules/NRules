using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactRetractingRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFact_FiresOnceAndRetractsFact()
        {
            //Arrange
            var fact = new FactType5 {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(0, Session.Query<FactType5>().Count());
        }
        
        protected override void SetUpRules()
        {
            SetUpRule<OneFactRetractingRule>();
        }
    }
}