using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
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
            IList<IFact> facts = null; 
            Session.Events.ConditionFailedEvent += (sender, args) => expression = args.Condition;
            Session.Events.ConditionFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType {TestProperty = null};

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

            var fact = new FactType { TestProperty = null };
            
            //Act - Assert
            Assert.DoesNotThrow(() => Session.Insert(fact));
        }

        [Test]
        public void Fire_ErrorInActionNoErrorHandler_Throws()
        {
            //Arrange
            Expression expression = null;
            IList<IFactMatch> facts = null;
            Session.Events.ActionFailedEvent += (sender, args) => expression = args.Action;
            Session.Events.ActionFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType { TestProperty = "Valid value" };
            Session.Insert(fact);

            GetRuleInstance<TestRule>().Action = null;

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

            var fact = new FactType { TestProperty = "Valid value" };
            Session.Insert(fact);

            GetRuleInstance<TestRule>().Action = null;

            //Act - Assert
            Assert.DoesNotThrow(() => Session.Fire());
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public Action Action = () => { };

            public override void Define()
            {
                FactType fact = null;

                When()
                    .Match<FactType>(() => fact, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => Action());
            }
        }
    }
}