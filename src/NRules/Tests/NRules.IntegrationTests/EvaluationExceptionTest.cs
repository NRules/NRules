using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class EvaluationExceptionTest : BaseRuleTestFixture
    {
        [Fact]
        public void Insert_ConditionErrorNoErrorHandler_Throws()
        {
            //Arrange
            GetRuleInstance<TestRule>().Condition = ThrowCondition;

            Expression expression = null;
            IList<IFact> facts = null; 
            Session.Events.LhsExpressionFailedEvent += (sender, args) => expression = args.Expression;
            Session.Events.LhsExpressionFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType {TestProperty = "Valid Value" };

            //Act - Assert
            var ex = Assert.Throws<RuleLhsExpressionEvaluationException>(() => Session.Insert(fact));
            Assert.NotNull(expression);
            Assert.Equal(1, facts.Count);
            Assert.Same(fact, facts.First().Value);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public void Fire_ConditionFailedInsert_DoesNotFire()
        {
            //Arrange
            GetRuleInstance<TestRule>().Condition = ThrowCondition;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = "Valid Value" };
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_ConditionFailedInsertThenUpdate_Fires()
        {
            //Arrange
            GetRuleInstance<TestRule>().Condition = ThrowCondition;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = "Valid Value" };
            Session.Insert(fact);

            GetRuleInstance<TestRule>().Condition = SuccessfulCondition;

            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_ConditionInsertThenFailedUpdate_DoesNotFire()
        {
            //Arrange
            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = "Valid Value" };
            Session.Insert(fact);

            GetRuleInstance<TestRule>().Condition = ThrowCondition;

            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_ConditionFailedInsertThenRetract_DoesNotFire()
        {
            //Arrange
            GetRuleInstance<TestRule>().Condition = ThrowCondition;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = "Valid Value" };
            Session.Insert(fact);

            GetRuleInstance<TestRule>().Condition = SuccessfulCondition;

            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_ConditionFailedInsertThenUpdateThenRetract_DoesNotFire()
        {
            //Arrange
            GetRuleInstance<TestRule>().Condition = ThrowCondition;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = "Valid Value" };
            Session.Insert(fact);

            GetRuleInstance<TestRule>().Condition = SuccessfulCondition;

            Session.Update(fact);
            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Insert_FilterErrorNoErrorHandler_Throws()
        {
            //Arrange
            GetRuleInstance<TestRule>().FilterCondition = ThrowFilter;

            Expression expression = null;
            IList<IFactMatch> facts = null;
            Session.Events.AgendaExpressionFailedEvent += (sender, args) => expression = args.Expression;
            Session.Events.AgendaExpressionFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType { TestProperty = "Valid Value" };

            //Act - Assert
            var ex = Assert.Throws<AgendaExpressionEvaluationException>(() => Session.Insert(fact));
            Assert.NotNull(expression);
            Assert.Equal(1, facts.Count);
            Assert.Same(fact, facts.First().Value);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public void Insert_FilterErrorHasErrorHandler_DoesNotFire()
        {
            //Arrange
            GetRuleInstance<TestRule>().FilterCondition = ThrowFilter;

            Session.Events.AgendaExpressionFailedEvent += (sender, args) => args.IsHandled = true;
            var fact = new FactType { TestProperty = "Valid Value" };

            //Act
            Session.Insert(fact);
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_ActionErrorNoErrorHandler_Throws()
        {
            //Arrange
            GetRuleInstance<TestRule>().Action = ThrowAction;

            Expression expression = null;
            IList<IFactMatch> facts = null;
            Session.Events.RhsExpressionFailedEvent += (sender, args) => expression = args.Expression;
            Session.Events.RhsExpressionFailedEvent += (sender, args) => facts = args.Match.Facts.ToList();

            var fact = new FactType { TestProperty = "Valid Value" };
            Session.Insert(fact);

            //Act - Assert
            var ex = Assert.Throws<RuleRhsExpressionEvaluationException>(() => Session.Fire());
            Assert.NotNull(expression);
            Assert.Equal(1, facts.Count());
            Assert.Same(fact, facts.First().Value);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public void Fire_ActionErrorHasErrorHandler_DoesNotThrow()
        {
            //Arrange
            GetRuleInstance<TestRule>().Action = ThrowAction;

            Session.Events.RhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = "Valid Value" };
            Session.Insert(fact);

            //Act - Assert
            Session.Fire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        private static readonly Action SuccessfulAction = () => { };
        private static readonly Action ThrowAction = () => throw new InvalidOperationException("Action failed");
        private static readonly Func<FactType, bool> SuccessfulCondition = f => true;
        private static readonly Func<FactType, bool> ThrowCondition = f => throw new InvalidOperationException("Condition failed");
        private static readonly Func<FactType, bool> SuccessfulFilter = f => true;
        private static readonly Func<FactType, bool> ThrowFilter = f => throw new InvalidOperationException("Filter failed");

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public Action Action = SuccessfulAction;
            public Func<FactType, bool> Condition = SuccessfulCondition;
            public Func<FactType, bool> FilterCondition = SuccessfulFilter;

            public override void Define()
            {
                FactType fact = null;

                When()
                    .Match<FactType>(() => fact, f => f.TestProperty.StartsWith("Valid") && Condition(f));

                Filter()
                    .Where(() => FilterCondition(fact));

                Then()
                    .Do(ctx => Action());
            }
        }
    }
}