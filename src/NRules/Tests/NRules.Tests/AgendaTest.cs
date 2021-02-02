using System;
using Moq;
using NRules.AgendaFilters;
using NRules.Rete;
using NRules.RuleModel;
using Xunit;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Tests
{
    public class AgendaTest
    {
        private readonly Mock<IExecutionContext> _context;
        private readonly Mock<ISessionInternal> _session;

        public AgendaTest()
        {
            _context = new Mock<IExecutionContext>();
            _session = new Mock<ISessionInternal>();

            _context.Setup(x => x.Session).Returns(_session.Object);
        }

        [Fact]
        public void Agenda_Created_Empty()
        {
            // Arrange
            // Act
            var target = CreateTarget();

            // Assert
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Pop_AgendaEmpty_Throws()
        {
            // Arrange
            var target = CreateTarget();

            // Act - Assert
            Assert.Throws<InvalidOperationException>(() => target.Pop());
        }

        [Fact]
        public void Peek_AgendaHasOneActivation_ReturnsActivationAgendaEmpty()
        {
            // Arrange
            var rule = MockRule();
            var activation = new Activation(rule, new Tuple(0));
            var target = CreateTarget();

            target.Add(activation);

            // Act
            var actualActivation = target.Pop();

            // Assert
            Assert.True(target.IsEmpty);
            Assert.Same(activation, actualActivation);
        }

        [Fact]
        public void Add_Called_ActivationInAgenda()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject {Value = "Test"};
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();

            // Act
            target.Add(activation);

            // Assert
            Assert.False(target.IsEmpty);
            var actualActivation = target.Pop();
            Assert.Equal(rule, actualActivation.CompiledRule);
            Assert.Equal(factObject, actualActivation.Tuple.RightFact.Object);
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Add_AcceptingGlobalFilter_ActivationInAgenda()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject {Value = "Test"};
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.AddFilter(new AcceptingFilter());

            // Act
            target.Add(activation);

            // Assert
            Assert.False(target.IsEmpty);
        }

        [Fact]
        public void Add_RejectingGlobalFilter_ActivationNotInAgenda()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject {Value = "Test"};
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.AddFilter(new RejectingFilter());

            // Act
            target.Add(activation);

            // Assert
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Add_AcceptingAndRejectingGlobalFilter_ActivationNotInAgenda()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject {Value = "Test"};
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.AddFilter(new AcceptingFilter());
            target.AddFilter(new RejectingFilter());

            // Act
            target.Add(activation);

            // Assert
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Add_AcceptingRuleFilter_ActivationInAgenda()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject {Value = "Test"};
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.AddFilter(rule.Definition, new AcceptingFilter());

            // Act
            target.Add(activation);

            // Assert
            Assert.False(target.IsEmpty);
        }

        [Fact]
        public void Add_RejectingRuleFilter_ActivationNotInAgenda()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject {Value = "Test"};
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.AddFilter(rule.Definition, new RejectingFilter());

            // Act
            target.Add(activation);

            // Assert
            Assert.True(target.IsEmpty);
        }


        [Fact]
        public void Add_RejectingRuleFilterForDifferentRule_ActivationInAgenda()
        {
            // Arrange
            var rule1 = MockRule();
            var rule2 = MockRule();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule1, tuple);
            var target = CreateTarget();
            target.AddFilter(rule2.Definition, new RejectingFilter());

            // Act
            target.Add(activation);

            // Assert
            Assert.False(target.IsEmpty);
        }

        [Fact]
        public void Modify_ActivationAlreadyInQueue_ActivationUpdatedInQueue()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.Add(activation);

            // Act
            factObject.Value = "New Value";
            target.Modify(activation);

            // Assert
            Assert.False(target.IsEmpty);
            var actualActivation = target.Pop();
            Assert.Equal(rule, actualActivation.CompiledRule);
            Assert.Equal(factObject.Value, ((FactObject)actualActivation.Tuple.RightFact.Object).Value);
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Modify_ActivationNotInQueue_ActivationReAddedToQueue()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.Add(activation);
            target.Pop();
 
            // Act
            target.Modify(activation);

            // Assert
            Assert.False(target.IsEmpty);
            var actualActivation = target.Pop();
            Assert.Equal(rule, actualActivation.CompiledRule);
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Modify_ActivationAlreadyInQueueRejectingFilter_ActivationRemovedFromQueue()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.Add(activation);

            target.AddFilter(new RejectingFilter());

            // Act
            factObject.Value = "New Value";
            target.Modify(activation);

            // Assert
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Modify_ActivationNotInQueue_ActivationNotReAddedToQueue()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.Add(activation);
            target.Pop();

            target.AddFilter(new RejectingFilter());

            // Act
            target.Modify(activation);

            // Assert
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Remove_CalledAfterAdd_AgendaEmpty()
        {
            // Arrange
            var rule = MockRule();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation = new Activation(rule, tuple);
            var target = CreateTarget();
            target.Add(activation);

            _session.Setup(x => x.GetLinkedKeys(activation)).Returns(new object[0]);

            // Act
            target.Remove(activation);

            // Assert
            Assert.True(target.IsEmpty);
        }

        [Fact]
        public void Add_CalledWithMultipleRules_RulesAreQueuedInOrder()
        {
            // Arrange
            var rule1 = MockRule();
            var rule2 = MockRule();
            var activation1 = new Activation(rule1, new Tuple(0));
            var activation2 = new Activation(rule2, new Tuple(0));
            var target = CreateTarget();

            // Act
            target.Add(activation1);
            target.Add(activation2);

            // Assert
            Assert.False(target.IsEmpty);
            Assert.Equal(rule1, target.Pop().CompiledRule);
            Assert.False(target.IsEmpty);
            Assert.Equal(rule2, target.Pop().CompiledRule);
        }

        [Fact]
        public void Peek_AgendaHasActivations_ReturnsActivationAgendaRemainsNonEmpty()
        {
            // Arrange
            var rule = MockRule();
            var activation = new Activation(rule, new Tuple(0));
            var target = CreateTarget();

            target.Add(activation);

            // Act
            var actualActivation = target.Peek();

            // Assert
            Assert.False(target.IsEmpty);
            Assert.Same(activation, actualActivation);
        }

        [Fact]
        public void Peek_AgendaEmpty_Throws()
        {
            // Arrange
            var target = CreateTarget();

            // Act - Assert
            Assert.Throws<InvalidOperationException>(() => target.Peek());
        }

        [Fact]
        public void Clear_CalledAfterActivation_AgendaEmpty()
        {
            // Arrange
            var rule = MockRule();
            var activation = new Activation(rule, new Tuple(0));
            var target = CreateTarget();

            target.Add(activation);

            // Act
            target.Clear();

            // Assert
            Assert.True(target.IsEmpty);
        }

        private Agenda CreateTarget()
        {
            var agenda = new Agenda();
            agenda.Initialize(_context.Object);
            return agenda;
        }

        private static ICompiledRule MockRule()
        {
            var compiledRuleMock = new Mock<ICompiledRule>();
            var ruleDefinitionMock = new Mock<IRuleDefinition>();
            compiledRuleMock.Setup(x => x.Definition).Returns(ruleDefinitionMock.Object);
            var actionTrigger = ActionTrigger.Activated | ActionTrigger.Reactivated | ActionTrigger.Deactivated;
            compiledRuleMock.Setup(x => x.ActionTriggers).Returns(actionTrigger);
            return compiledRuleMock.Object;
        }

        private static Tuple CreateTuple(object factObject)
        {
            return new Tuple(0, new Tuple(0), new Fact(factObject));
        }

        private class FactObject
        {
            public string Value { get; set; }
        }

        private class RejectingFilter : IAgendaFilter
        {
            public bool Accept(AgendaContext context, Activation activation)
            {
                return false;
            }
        }

        private class AcceptingFilter : IAgendaFilter
        {
            public bool Accept(AgendaContext context, Activation activation)
            {
                return true;
            }
        }
    }
}