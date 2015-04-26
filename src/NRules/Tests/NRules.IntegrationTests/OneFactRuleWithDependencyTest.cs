using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactRuleWithDependencyTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFact_FiresOnce()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactRuleWithDependency>();
        }
    }
}