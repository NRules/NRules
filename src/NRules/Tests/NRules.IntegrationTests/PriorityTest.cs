using System;
using System.Collections.Generic;
using Moq;
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
            var invokedRules = new List<BaseRule>();

            var mock = new Mock<Action<BaseRule>>();
            mock.Setup(x => x(It.IsAny<BaseRule>())).Callback<BaseRule>(invokedRules.Add);

            GetRuleInstance<PriorityLowRule>().InvocationHandler = mock.Object;
            GetRuleInstance<PriorityHighRule>().InvocationHandler = mock.Object;

            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
                //low priority activates twice
                //it runs once, activates high priority rule, which preempts low priority and fires once
                //low priority fires second time, which activates high priority which also fires second time
            Assert.AreEqual(4, invokedRules.Count);
            Assert.IsInstanceOf<PriorityLowRule>(invokedRules[0]);
            Assert.IsInstanceOf<PriorityHighRule>(invokedRules[1]);
            Assert.IsInstanceOf<PriorityLowRule>(invokedRules[2]);
            Assert.IsInstanceOf<PriorityHighRule>(invokedRules[3]);
        }

        protected override void SetUpRules()
        {
            SetUpRule<PriorityLowRule>();
            SetUpRule<PriorityHighRule>();
        }
    }
}