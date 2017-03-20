using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NRules.Proxy;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class ActionInterceptorTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_ConditionsMatchNoInterceptor_ExecutesAction()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value"};
            Session.Insert(fact1);

            bool actionExecuted = false;
            GetRuleInstance<OneFactOneCollectionRule>().Action = () => { actionExecuted = true; };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.True(actionExecuted);
        }

        [Test]
        public void Fire_ConditionsMatchInterceptorInvokes_ExecutesAction()
        {
            //Arrange
            Session.ActionInterceptor = new ActionInterceptor(invoke: true);

            var fact1 = new FactType1 {TestProperty = "Valid Value"};
            Session.Insert(fact1);

            bool actionExecuted = false;
            GetRuleInstance<OneFactOneCollectionRule>().Action = () => { actionExecuted = true; };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.True(actionExecuted);
        }

        [Test]
        public void Fire_ConditionsMatchInterceptorDoesNotInvoke_DoesNotExecuteAction()
        {
            //Arrange
            Session.ActionInterceptor = new ActionInterceptor(invoke: false);

            var fact1 = new FactType1 {TestProperty = "Valid Value"};
            Session.Insert(fact1);

            bool actionExecuted = false;
            GetRuleInstance<OneFactOneCollectionRule>().Action = () => { actionExecuted = true; };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.False(actionExecuted);
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactOneCollectionRule>();
        }

        private class ActionInterceptor : IActionInterceptor
        {
            private readonly bool _invoke;

            public ActionInterceptor(bool invoke)
            {
                _invoke = invoke;
            }

            public void Intercept(IActionInvocation action)
            {
                if (_invoke)
                    action.Invoke();
            }
        }
    }
}
