using System;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class PriorityTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_LowPriorityActivatesTwiceTriggersHighPriority_HighPriorityPreemptsLowPriority()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType1() {TestProperty = "Valid Value"};
            Session.Insert(fact1);
            Session.Insert(fact2);

            var lowRule = GetRuleInstance<PriorityLowRule>();
            var highRule = GetRuleInstance<PriorityHighRule>();

            var invocationHandler = MockRepository.GenerateMock<Action<BaseRule>>();
            invocationHandler.GetMockRepository().Ordered();

            lowRule.InvocationHandler = invocationHandler;
            highRule.InvocationHandler = invocationHandler;

            //low priority activates twice
            //it runs once, activates high priority rule, which preempts low priority and fires once
            //low priority fires second time, which activates high priority which also fires second time
            invocationHandler.Expect(x => x.Invoke(lowRule)).Repeat.Once();
            invocationHandler.Expect(x => x.Invoke(highRule)).Repeat.Once();
            invocationHandler.Expect(x => x.Invoke(lowRule)).Repeat.Once();
            invocationHandler.Expect(x => x.Invoke(highRule)).Repeat.Once();

            //Act
            Session.Fire();

            //Assert
            invocationHandler.VerifyAllExpectations();
        }

        protected override void SetUpRules()
        {
            SetUpRule<PriorityLowRule>();
            SetUpRule<PriorityHighRule>();
        }
    }
}