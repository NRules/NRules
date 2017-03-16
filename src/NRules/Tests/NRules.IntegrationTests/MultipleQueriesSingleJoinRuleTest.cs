using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class MultipleQueriesSingleJoinRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFactSet_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact4 = new FactType4 {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_OneMatchingFactSetOneNotMatching_FiresOnce()
        {
            //Arrange
            var fact1A = new FactType1 {TestProperty = "Valid Value 1"};
            var fact1B = new FactType1 {TestProperty = "Valid Value 2"};
            var fact4A = new FactType4 {TestProperty = "Valid Value 1"};
            var fact4B = new FactType4 {TestProperty = "Valid Value 3"};

            Session.Insert(fact1A);
            Session.Insert(fact1B);
            Session.Insert(fact4A);
            Session.Insert(fact4B);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_TwoMatchingFactSets_FiresTwice()
        {
            //Arrange
            var fact1A = new FactType1 {TestProperty = "Valid Value 1"};
            var fact1B = new FactType1 {TestProperty = "Valid Value 2"};
            var fact4A = new FactType4 {TestProperty = "Valid Value 1"};
            var fact4B = new FactType4 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1A);
            Session.Insert(fact1B);
            Session.Insert(fact4A);
            Session.Insert(fact4B);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        protected override void SetUpRules()
        {
            SetUpRule<MultipleQueriesSingleJoinRule>();
        }
    }
}