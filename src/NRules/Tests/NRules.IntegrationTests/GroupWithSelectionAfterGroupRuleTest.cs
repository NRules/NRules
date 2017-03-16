using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class GroupWithSelectionAfterGroupRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MakeAllFactsInelligible_DoesNotFire()
        {
            //Arrange
            var fact11 = new FactType6 {TestProperty = "Valid Test Property 1", GroupProperty = "GP1", JoinProperty = "Good"};
            var fact12 = new FactType6 {TestProperty = "Valid Test Property 1", GroupProperty = "GP1", JoinProperty = "Good" };
            var fact13 = new FactType6 {TestProperty = "Valid Test Property 1", GroupProperty = "GP2", JoinProperty = "Bad" };
            var fact14 = new FactType6 {TestProperty = "Valid Test Property 2", GroupProperty = "GP2", JoinProperty = "Good" };

            var facts = new object[] {fact11, fact12, fact13, fact14};
            Session.InsertAll(facts);

            fact11.TestProperty = "Bad Test Poroperty";
            fact12.TestProperty = "Bad Test Poroperty";
            fact13.TestProperty = "Bad Test Poroperty";
            fact14.TestProperty = "Bad Test Poroperty";
            var factsToUpdate = new object[] { fact11, fact12, fact13, fact14 };
            Session.UpdateAll(factsToUpdate);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }


        protected override void SetUpRules()
        {
            SetUpRule<GroupWithSelectionAfterGroupRule>();
        }
    }
}
