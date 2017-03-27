using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NRules.RuleModel;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactNoBindingRuleTest : BaseRuleTestFixture
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

        [Test]
        public void Fire_OneMatchingFact_FactInContext()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            IFactMatch[] matches = null;
            GetRuleInstance<OneFactNoBindingRule>().Action = ctx =>
            {
                matches = ctx.Facts.ToArray();
            };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, matches.Length);
            Assert.AreSame(fact, matches[0].Value);
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactNoBindingRule>();
        }
    }
}