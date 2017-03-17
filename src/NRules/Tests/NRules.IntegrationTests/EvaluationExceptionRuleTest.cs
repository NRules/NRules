using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Diagnostics;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class EvaluationExceptionRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Insert_ErrorInConditionNoErrorHandler_Throws()
        {
            //Arrange
            Expression expression = null;
            IList<FactInfo> facts = null; 
            Session.Events.ConditionFailedEvent += (sender, args) => expression = args.Condition;
            Session.Events.ConditionFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType1 {TestProperty = null};

            //Act - Assert
            var ex = Assert.Throws<RuleConditionEvaluationException>(() => Session.Insert(fact));
            Assert.IsNotNull(expression);
            Assert.AreEqual(1, facts.Count());
            Assert.AreSame(fact, facts.First().Value);
            Assert.IsInstanceOf<NullReferenceException>(ex.InnerException);
        }

        [Test]
        public void Insert_ErrorInConditionErrorHandler_DoesNotThrow()
        {
            //Arrange
            Session.Events.ConditionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType1 { TestProperty = null };
            
            //Act - Assert
            Assert.DoesNotThrow(() => Session.Insert(fact));
        }

        [Test]
        public void Fire_ErrorInActionNoErrorHandler_Throws()
        {
            //Arrange
            Expression expression = null;
            IList<FactInfo> facts = null;
            Session.Events.ActionFailedEvent += (sender, args) => expression = args.Action;
            Session.Events.ActionFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType1 { TestProperty = "Valid value" };
            Session.Insert(fact);

            GetRuleInstance<OneFactRule>().Action = null;

            //Act - Assert
            var ex = Assert.Throws<RuleActionEvaluationException>(() => Session.Fire());
            Assert.IsNotNull(expression);
            Assert.AreEqual(1, facts.Count());
            Assert.AreSame(fact, facts.First().Value);
            Assert.IsInstanceOf<NullReferenceException>(ex.InnerException);
        }

        [Test]
        public void Fire_ErrorInActionErrorHandler_DoesNotThrow()
        {
            //Arrange
            Session.Events.ActionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType1 { TestProperty = "Valid value" };
            Session.Insert(fact);

            GetRuleInstance<OneFactRule>().Action = null;

            //Act - Assert
            Assert.DoesNotThrow(() => Session.Fire());
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactRule>();
        }
    }
}