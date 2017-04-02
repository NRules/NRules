using System;
using System.Collections.Generic;
using NRules.Extensibility;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
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
            var fact1 = new FactType1();
            var fact2 = new FactType2();
            Session.Insert(fact1);
            Session.Insert(fact2);

            bool actionExecuted = false;
            GetRuleInstance<TestRule>().Action = () => { actionExecuted = true; };

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

            var fact1 = new FactType1();
            var fact2 = new FactType2();
            Session.Insert(fact1);
            Session.Insert(fact2);

            bool actionExecuted = false;
            GetRuleInstance<TestRule>().Action = () => { actionExecuted = true; };

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

            var fact1 = new FactType1();
            var fact2 = new FactType2();
            Session.Insert(fact1);
            Session.Insert(fact2);

            bool actionExecuted = false;
            GetRuleInstance<TestRule>().Action = () => { actionExecuted = true; };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.False(actionExecuted);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
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

        public class FactType1
        {
        }

        public class FactType2
        {
        }

        public class TestRule : Rule
        {
            public Action Action = () => { };

            public override void Define()
            {
                FactType1 fact = null;
                IEnumerable<FactType2> collection = null;

                When()
                    .Match<FactType1>(() => fact)
                    .Query(() => collection, x => x
                        .Match<FactType2>()
                        .Collect());
                Then()
                    .Do(ctx => CallAction(ctx, fact, collection));
            }

            private void CallAction(IContext context, FactType1 fact1, IEnumerable<FactType2> collection2)
            {
                Action();
            }
        }
    }
}
