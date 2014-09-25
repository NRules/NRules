using System;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class EvaluationExceptionRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Insert_ErrorInCondition_Throws()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = null};

            //Act - Assert
            var ex = Assert.Throws<RuleConditionEvaluationException>(() => Session.Insert(fact));
            Assert.IsNotNull(ex.Condition);
            Assert.IsInstanceOf<NullReferenceException>(ex.InnerException);
        }
        
        [Test]
        public void Fire_ErrorInActionNoErrorHandler_Throws()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = "Valid value"};
            Session.Insert(fact);

            GetRuleInstance<OneFactRule>().Notifier = null;

            //Act - Assert
            var ex = Assert.Throws<RuleActionEvaluationException>(() => Session.Fire());
            Assert.IsNotNull(ex.Action);
            Assert.IsInstanceOf<NullReferenceException>(ex.InnerException);
        }
        
        [Test]
        public void Fire_ErrorInActionErrorHandler_DoesNotThrow()
        {
            //Arrange
            Session.Events.ActionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType1 {TestProperty = "Valid value"};
            Session.Insert(fact);

            GetRuleInstance<OneFactRule>().Notifier = null;

            //Act - Assert
            Assert.DoesNotThrow(() => Session.Fire());
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactRule>();
        }
    }
}