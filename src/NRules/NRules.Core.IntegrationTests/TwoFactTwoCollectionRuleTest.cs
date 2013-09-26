using NRules.Core.IntegrationTests.TestAssets;
using NRules.Core.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.Core.IntegrationTests
{
    [TestFixture]
    public class TwoFactTwoCollectionRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void TwoFactTwoCollectionRule_MatchingFactsOfEachKind_FiresOnceWithValidFactsInTwoCollections()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType1() {TestProperty = "Invalid Value"};
            var fact3 = new FactType1() {TestProperty = "Valid Value"};
            var fact4 = new FactType2() {TestProperty = "Valid Value"};
            var fact5 = new FactType2() {TestProperty = "Valid Value"};
            var fact6 = new FactType2() {TestProperty = "Invalid Value"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);
            Session.Insert(fact5);
            Session.Insert(fact6);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetRuleInstance<TwoFactTwoCollectionRule>().Fact1Count);
            Assert.AreEqual(2, GetRuleInstance<TwoFactTwoCollectionRule>().Fact2Count);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactTwoCollectionRule>();
        }
    }
}