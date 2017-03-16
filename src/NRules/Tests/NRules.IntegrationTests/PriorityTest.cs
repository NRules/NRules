using System.Collections.Generic;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class PriorityTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_LowPriorityActivatesTwiceTriggersHighPriority_HighPriorityPreemptsLowPriority()
        {
            //Arrange
            var invokedRules = new List<string>();

            Session.Events.RuleFiredEvent += (sender, args) => invokedRules.Add(args.Rule.Name);

            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
                //low priority activates twice
                //it runs once, activates high priority rule, which preempts low priority and fires once
                //low priority fires second time, which activates high priority which also fires second time
            Assert.AreEqual(4, invokedRules.Count);
            Assert.AreEqual("NRules.IntegrationTests.TestRules.PriorityLowRule", invokedRules[0]);
            Assert.AreEqual("NRules.IntegrationTests.TestRules.PriorityHighRule", invokedRules[1]);
            Assert.AreEqual("NRules.IntegrationTests.TestRules.PriorityLowRule", invokedRules[2]);
            Assert.AreEqual("NRules.IntegrationTests.TestRules.PriorityHighRule", invokedRules[3]);
        }

        protected override void SetUpRules()
        {
            SetUpRule<PriorityLowRule>();
            SetUpRule<PriorityHighRule>();
        }
    }
}