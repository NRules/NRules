using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactSameTypeRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingFacts_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType4 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType4 {TestProperty = "Valid Value 2", Parent = fact1};
            var fact3 = new FactType4 {TestProperty = "Invalid Value 3", Parent = fact1};
            var fact4 = new FactType4 {TestProperty = "Valid Value 4", Parent = null};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_FirstMatchingFactSecondInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType4 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType4 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactSameTypeRule>();
        }
    }
}