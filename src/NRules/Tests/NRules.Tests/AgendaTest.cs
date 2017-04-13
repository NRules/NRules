using System;
using Moq;
using NRules.Rete;
using Xunit;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Tests
{
    public class AgendaTest
    {
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
            var activation1 = new Activation(ruleMock1.Object, new Tuple(), null);
            var target = CreateTarget();

            target.Add(activation1);

            // Act
            var actualActivation = target.Pop();

            // Assert
            Assert.True(target.IsEmpty());
            Assert.Same(activation1, actualActivation);
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
            target.Add(activation);

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
            var activation1 = new Activation(ruleMock.Object, tuple, null);
            var target = CreateTarget();
            target.Add(activation1);

            // Act
            factObject.Value = "New Value";
            var activation2 = new Activation(ruleMock.Object, tuple, null);
            target.Modify(activation2);

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
            var activation1 = new Activation(ruleMock.Object, tuple, null);
            var target = CreateTarget();
            target.Add(activation1);

            // Act
            var activation2 = new Activation(ruleMock.Object, tuple, null);
            target.Remove(activation2);

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
            target.Add(activation1);
            target.Add(activation2);

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

            target.Add(activation1);

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

            target.Add(activation1);

            // Act
            target.Clear();

            // Assert
            Assert.True(target.IsEmpty());
        }

        private Agenda CreateTarget()
        {
            return new Agenda();
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