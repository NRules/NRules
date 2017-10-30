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
    public class EvaluationExceptionRuleTest : BaseRuleTestFixture
    {
        [Fact]
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
            Assert.NotNull(expression);
            Assert.Equal(1, facts.Count());
            Assert.Same(fact, facts.First().Value);
            Assert.IsType<NullReferenceException>(ex.InnerException);
        }

        [Fact]
        public void Insert_ErrorInConditionErrorHandler_DoesNotThrow()
        {
            //Arrange
            Session.Events.ConditionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = null };
            
            //Act - Assert
            Session.Insert(fact);
        }

        [Fact]
        public void Fire_ErrorInBinding_Throws()
        {
            //Arrange
            GetRuleInstance<TestRule>().BindingExpression = null;

            Expression expression = null;
            IList<IFact> facts = null;
            Session.Events.BindingFailedEvent += (sender, args) => expression = args.Expression;
            Session.Events.BindingFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType { TestProperty = "Valid value" };

            //Act - Assert
            var ex = Assert.Throws<RuleExpressionEvaluationException>(() => Session.Insert(fact));
            Assert.NotNull(expression);
            Assert.Equal(1, facts.Count());
            Assert.Same(fact, facts.First().Value);
            Assert.IsType<NullReferenceException>(ex.InnerException);
        }

        [Fact]
        public void Fire_ErrorInAggregate_Throws()
        {
            //Arrange
            GetRuleInstance<TestRule>().GroupingExpression = null;

            Expression expression = null;
            IList<IFact> facts = null;
            Session.Events.AggregateFailedEvent += (sender, args) => expression = args.Expression;
            Session.Events.AggregateFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType { TestProperty = "Valid value" };

            //Act - Assert
            var ex = Assert.Throws<RuleExpressionEvaluationException>(() => Session.Insert(fact));
            Assert.NotNull(expression);
            Assert.Equal(3, facts.Count());
            Assert.Same(fact, facts.First().Value);
            Assert.Equal("value", facts.Skip(1).First().Value);
            Assert.IsType<NullReferenceException>(ex.InnerException);
        }

        [Fact]
        public void Fire_ErrorInActionNoErrorHandler_Throws()
        {
            //Arrange
            GetRuleInstance<TestRule>().Action = null;

            Expression expression = null;
            IList<IFactMatch> facts = null;
            Session.Events.ActionFailedEvent += (sender, args) => expression = args.Action;
            Session.Events.ActionFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType { TestProperty = "Valid value" };
            Session.Insert(fact);

            //Act - Assert
            var ex = Assert.Throws<RuleActionEvaluationException>(() => Session.Fire());
            Assert.NotNull(expression);
            Assert.Equal(3, facts.Count());
            Assert.Same(fact, facts.First().Value);
            Assert.IsType<NullReferenceException>(ex.InnerException);
        }

        [Fact]
        public void Fire_ErrorInActionErrorHandler_DoesNotThrow()
        {
            //Arrange
            GetRuleInstance<TestRule>().Action = null;

            Session.Events.ActionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = "Valid value" };
            Session.Insert(fact);

            //Act - Assert
            Session.Fire();
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
            public Func<string> BindingExpression = () => "value";
            public Func<FactType, object> GroupingExpression = x => x.TestProperty;

            public override void Define()
            {
                FactType fact = null;
                string binding = null;
                IEnumerable<FactType> factGroup = null;

                When()
                    .Match<FactType>(() => fact, f => f.TestProperty.StartsWith("Valid"))
                    .Let(() => binding, () => BindingExpression())
                    .Query(() => factGroup, q => q
                        .Match<FactType>(f => f == fact)
                        .GroupBy(f => GroupingExpression(f))
                    );
                Then()
                    .Do(ctx => Action());
            }
        }
    }
}