using Moq;
using NRules.Rete;
using NUnit.Framework;

namespace NRules.Tests
{
    [TestFixture]
    public class AgendaTest
    {
        [Test]
        public void Activate_NotCalled_ActivationQueueEmpty()
        {
            // Arrange
            // Act
            var target = CreateTarget();

            // Assert
            Assert.False(target.HasActiveRules());
        }

        [Test]
        public void Activate_Called_ActivationEndsUpInQueue()
        {
            // Arrange
            var ruleMock = new Mock<ICompiledRule>();
            var factObject = new FactObject {Value = "Test"};
            var tuple = CreateTuple(factObject);
            var activation = new Activation(ruleMock.Object, 1, tuple, null);
            var target = CreateTarget();

            // Act
            target.Activate(activation);

            // Assert
            Assert.True(target.HasActiveRules());
            var actualActivation = target.NextActivation();
            Assert.AreEqual(ruleMock.Object, actualActivation.Rule);
            Assert.AreEqual(factObject, actualActivation.Tuple.RightFact.Object);
            Assert.False(target.HasActiveRules());
        }

        [Test]
        public void Reactivate_Called_ActivationUpdatedInQueue()
        {
            // Arrange
            var ruleMock = new Mock<ICompiledRule>();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation1 = new Activation(ruleMock.Object, 1, tuple, null);
            var target = CreateTarget();
            target.Activate(activation1);

            // Act
            factObject.Value = "New Value";
            var activation2 = new Activation(ruleMock.Object, 1, tuple, null);
            target.Reactivate(activation2);

            // Assert
            var actualActivation = target.NextActivation();
            Assert.AreEqual(ruleMock.Object, actualActivation.Rule);
            Assert.AreEqual(factObject, actualActivation.Tuple.RightFact.Object);
            Assert.False(target.HasActiveRules());
        }
        
        [Test]
        public void Deactivate_CalledAfterActivation_ActivationQueueEmpty()
        {
            // Arrange
            var ruleMock = new Mock<ICompiledRule>();
            var factObject = new FactObject { Value = "Test" };
            var tuple = CreateTuple(factObject);
            var activation1 = new Activation(ruleMock.Object, 1, tuple, null);
            var target = CreateTarget();
            target.Activate(activation1);

            // Act
            var activation2 = new Activation(ruleMock.Object, 1, tuple, null);
            target.Deactivate(activation2);

            // Assert
            Assert.False(target.HasActiveRules());
        }

        [Test]
        public void Activate_CalledWithMultipleRules_RulesAreQueuedInOrder()
        {
            // Arrange
            var ruleMock1 = new Mock<ICompiledRule>();
            var ruleMock2 = new Mock<ICompiledRule>();
            var activation1 = new Activation(ruleMock1.Object, 1, new Tuple(), null);
            var activation2 = new Activation(ruleMock2.Object, 1, new Tuple(), null);
            var target = CreateTarget();

            // Act
            target.Activate(activation1);
            target.Activate(activation2);

            // Assert
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(ruleMock1.Object, target.NextActivation().Rule);
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(ruleMock2.Object, target.NextActivation().Rule);
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