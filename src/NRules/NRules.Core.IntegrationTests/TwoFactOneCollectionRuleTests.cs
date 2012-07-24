using NRules.Core.IntegrationTests.TestAssets;
using NRules.Core.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.Core.IntegrationTests
{
    [TestFixture]
    public class TwoFactOneCollectionRuleTests : BaseRuleTestFixture
    {
        [Test]
        public void TwoFactOneCollectionRule_OneMatchingFactOfOneKindAndTwoOfAnother_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value"};
            var fact3 = new FactType2() {TestProperty = "Invalid Value"};
            var fact4 = new FactType2() {TestProperty = "Valid Value"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetRuleInstance<TwoFactOneCollectionRule>().FactCount);
        }

        [Test]
        public void TwoFactOneCollectionRule_FactOfOneKindIsInvalidAndTwoOfAnotherKindAreValid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Invalid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value"};
            var fact3 = new FactType2() {TestProperty = "Invalid Value"};
            var fact4 = new FactType2() {TestProperty = "Valid Value"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOneCollectionRule>();
        }
    }
}