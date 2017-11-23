using System;
using System.Collections.Generic;
using Moq;
using NRules.AgendaFilters;
using NRules.Rete;
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
            Assert.True(target.IsEmpty());
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
            var ruleMock1 = new Mock<ICompiledRule>();
            var activation = new Activation(ruleMock1.Object, new Tuple(), null);
            var target = CreateTarget();

            target.Add(_context.Object, activation);

            // Act
            var actualActivation = target.Pop();

            // Assert
            Assert.True(target.IsEmpty());
            Assert.Same(activation, actualActivation);
        }

        [Fact]
        public void Add_Called_ActivationEndsUpInAgenda()
        {
            // Arrange
            var ruleMock = new Mock<ICompiledRule>();
            var factObject = new FactObject {Value = "Test"};
            var tuple = CreateTuple(factObject);
            var activation = new Activation(ruleMock.Object, tuple, null);
            var target = CreateTarget();

            // Act
            target.Add(_context.Object, activation);

            // Assert
            Assert.False(target.IsEmpty());
            var actualActivation = target.Pop();
            Assert.Equal(ruleMock.Object, actualActivation.CompiledRule);
            Assert.Equal(factObject, actualActivation.Tuple.RightFact.Object);
            Assert.True(target.IsEmpty());
        }

        [Fact]
        public void Modify_Called_ActivationUpdatedInQueue()
        {
            // Arrange
            var ruleMock = new Mock<ICompiledRule>();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation = new Activation(ruleMock.Object, tuple, null);
            var target = CreateTarget();
            target.Add(_context.Object, activation);

            // Act
            factObject.Value = "New Value";
            target.Modify(_context.Object, activation);

            // Assert
            var actualActivation = target.Pop();
            Assert.Equal(ruleMock.Object, actualActivation.CompiledRule);
            Assert.Equal(factObject, actualActivation.Tuple.RightFact.Object);
            Assert.True(target.IsEmpty());
        }
        
        [Fact]
        public void Remove_CalledAfterAdd_AgendaEmpty()
        {
            // Arrange
            var ruleMock = new Mock<ICompiledRule>();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation = new Activation(ruleMock.Object, tuple, null);
            var target = CreateTarget();
            target.Add(_context.Object, activation);

            _session.Setup(x => x.GetLinkedKeys(activation)).Returns(new object[0]);

            // Act
            target.Remove(_context.Object, activation);

            // Assert
            Assert.True(target.IsEmpty());
        }

        [Fact]
        public void Add_CalledWithMultipleRules_RulesAreQueuedInOrder()
        {
            // Arrange
            var ruleMock1 = new Mock<ICompiledRule>();
            var ruleMock2 = new Mock<ICompiledRule>();
            var activation1 = new Activation(ruleMock1.Object, new Tuple(), null);
            var activation2 = new Activation(ruleMock2.Object, new Tuple(), null);
            var target = CreateTarget();

            // Act
            target.Add(_context.Object, activation1);
            target.Add(_context.Object, activation2);

            // Assert
            Assert.False(target.IsEmpty());
            Assert.Equal(ruleMock1.Object, target.Pop().CompiledRule);
            Assert.False(target.IsEmpty());
            Assert.Equal(ruleMock2.Object, target.Pop().CompiledRule);
        }

        [Fact]
        public void Peek_AgendaHasActivations_ReturnsActivationAgendaRamainsNonEmpty()
        {
            // Arrange
            var ruleMock1 = new Mock<ICompiledRule>();
            var activation1 = new Activation(ruleMock1.Object, new Tuple(), null);
            var target = CreateTarget();

            target.Add(_context.Object, activation1);

            // Act
            var actualActivation = target.Peek();

            // Assert
            Assert.False(target.IsEmpty());
            Assert.Same(activation1, actualActivation);
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
            var ruleMock1 = new Mock<ICompiledRule>();
            var activation1 = new Activation(ruleMock1.Object, new Tuple(), null);
            var target = CreateTarget();

            target.Add(_context.Object, activation1);

            // Act
            target.Clear();

            // Assert
            Assert.True(target.IsEmpty());
        }

        private Agenda CreateTarget()
        {
            return new Agenda(new Dictionary<ICompiledRule, IAgendaFilter[]>());
        }

        private static Tuple CreateTuple(object factObject)
        {
            return new Tuple(new Tuple(), new Fact(factObject));
        }

        private class FactObject
        {
            public string Value { get; set; }
        }
    }
}