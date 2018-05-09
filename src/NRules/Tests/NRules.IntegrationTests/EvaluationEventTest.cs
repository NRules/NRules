using System.Collections.Generic;
using NRules.Diagnostics;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class EvaluationEventTest
    {
        [Fact]
        public void Insert_Fact_RaisesAlphaConditionExpressionEvalEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var handledEvents = new List<ExpressionEventArgs>();
            factory.Events.LhsExpressionEvaluatedEvent += (sender, args) =>
            {
                handledEvents.Add(args);
            };

            var fact1 = new FactType1 { TestProperty = "Valid Value" };

            //Act
            session.Insert(fact1);

            //Assert
            Assert.Equal(1, handledEvents.Count);
            var eventArgs = handledEvents[0];
            Assert.Collection(eventArgs.Arguments, x => Assert.Same(fact1, x));
            Assert.Equal(true, eventArgs.Result);
            Assert.Null(eventArgs.Exception);
        }
        
        [Fact]
        public void Insert_Fact_RaisesAlphaConditionExpressionEvalEventWithException()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var handledEvents = new List<ExpressionEventArgs>();
            factory.Events.LhsExpressionEvaluatedEvent += (sender, args) =>
            {
                handledEvents.Add(args);
            };

            var fact1 = new FactType1 { TestProperty = null };

            //Act - Assert
            var ex = Assert.Throws<RuleLhsExpressionEvaluationException>(() => session.Insert(fact1));
            Assert.Equal(1, handledEvents.Count);
            var eventArgs = handledEvents[0];
            Assert.Collection(eventArgs.Arguments, x => Assert.Same(fact1, x));
            Assert.Equal(false, eventArgs.Result);
            Assert.Same(ex.InnerException, eventArgs.Exception);
        }
        
        [Fact]
        public void Insert_Fact_RaisesBetaConditionExpressionEvalEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var handledEvents = new List<ExpressionEventArgs>();
            factory.Events.LhsExpressionEvaluatedEvent += (sender, args) =>
            {
                handledEvents.Add(args);
            };

            var fact1 = new FactType1 { TestProperty = "Valid Value" };
            var fact2 = new FactType2 { JoinProperty = "Invalid Value", SelectProperty = "12345"};

            //Act
            session.Insert(fact1);
            session.Insert(fact2);

            //Assert
            Assert.Equal(2, handledEvents.Count);
            var eventArgs = handledEvents[1];
            Assert.Collection(eventArgs.Arguments, x => Assert.Same(fact2, x), x => Assert.Same(fact1, x));
            Assert.Equal(false, eventArgs.Result);
            Assert.Null(eventArgs.Exception);
        }
        
        [Fact]
        public void Insert_Fact_RaisesAggregateExpressionEvalEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var handledEvents = new List<ExpressionEventArgs>();
            factory.Events.LhsExpressionEvaluatedEvent += (sender, args) =>
            {
                handledEvents.Add(args);
            };

            var fact1 = new FactType1 { TestProperty = "Valid Value" };
            var fact2 = new FactType2 { JoinProperty = "Valid Value", SelectProperty = "12345"};

            //Act
            session.Insert(fact1);
            session.Insert(fact2);

            //Assert
            Assert.Equal(5, handledEvents.Count);
            var eventArgs = handledEvents[2];
            Assert.Collection(eventArgs.Arguments, x => Assert.Same(fact2, x));
            Assert.Equal("12345", eventArgs.Result);
            Assert.Null(eventArgs.Exception);
        }
        
        [Fact]
        public void Insert_Fact_RaisesBindingExpressionEvalEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var handledEvents = new List<ExpressionEventArgs>();
            factory.Events.LhsExpressionEvaluatedEvent += (sender, args) =>
            {
                handledEvents.Add(args);
            };

            var fact1 = new FactType1 { TestProperty = "Valid Value" };
            var fact2 = new FactType2 { JoinProperty = "Valid Value", SelectProperty = "12345"};

            //Act
            session.Insert(fact1);
            session.Insert(fact2);

            //Assert
            Assert.Equal(5, handledEvents.Count);
            var eventArgs = handledEvents[3];
            Assert.Collection(eventArgs.Arguments, x => Assert.Equal("12345", x));
            Assert.Equal(5, eventArgs.Result);
            Assert.Null(eventArgs.Exception);
        }
        
        [Fact]
        public void Insert_Fact_RaisesFilterExpressionEvalEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var handledEvents = new List<AgendaExpressionEventArgs>();
            factory.Events.AgendaExpressionEvaluatedEvent += (sender, args) =>
            {
                handledEvents.Add(args);
            };

            var fact1 = new FactType1 { TestProperty = "Valid Value" };
            var fact2 = new FactType2 { JoinProperty = "Valid Value", SelectProperty = "123456"};

            //Act
            session.Insert(fact1);
            session.Insert(fact2);
            session.Fire();

            //Assert
            Assert.Equal(1, handledEvents.Count);
            var eventArgs = handledEvents[0];
            Assert.Collection(eventArgs.Arguments, x => Assert.Equal(6, x));
            Assert.Equal(false, eventArgs.Result);
            Assert.Null(eventArgs.Exception);
            Assert.Equal("Test Rule", eventArgs.Rule.Name);
        }

        [Fact]
        public void Insert_Fact_RaisesActionExpressionEvalEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var handledEvents = new List<RhsExpressionEventArgs>();
            factory.Events.RhsExpressionEvaluatedEvent += (sender, args) =>
            {
                handledEvents.Add(args);
            };

            var fact1 = new FactType1 { TestProperty = "Valid Value" };
            var fact2 = new FactType2 { JoinProperty = "Valid Value", SelectProperty = "1234567890A"};

            //Act
            session.Insert(fact1);
            session.Insert(fact2);
            session.Fire();

            //Assert
            Assert.Equal(1, handledEvents.Count);
            var eventArgs = handledEvents[0];
            Assert.Collection(eventArgs.Arguments);
            Assert.Null(eventArgs.Result);
            Assert.Null(eventArgs.Exception);
            Assert.Equal("Test Rule", eventArgs.Rule.Name);
        }

        private ISessionFactory CreateTarget()
        {
            var repository = new RuleRepository();
            repository.Load(x => x.NestedTypes().From(typeof(TestRule)));
            return repository.Compile();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
        }

        public class FactType2
        {
            public string JoinProperty { get; set; }
            public string SelectProperty { get; set; }
        }

        [Name("Test Rule")]
        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType1 fact = null;
                string value = null;
                int length = 0;

                When()
                    .Match<FactType1>(() => fact,
                        f => f.TestProperty.StartsWith("Valid"))
                    .Query(() => value, q => q
                        .Match<FactType2>(
                            f => f.JoinProperty == fact.TestProperty)
                        .Select(x => x.SelectProperty))
                    .Let(() => length, () => value.Length)
                    .Having(() => length > 5);

                Filter()
                    .Where(() => length > 10);

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}