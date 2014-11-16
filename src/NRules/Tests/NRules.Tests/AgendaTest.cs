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
            var activation = new Activation(ruleMock.Object, new Tuple());
            var target = CreateTarget();

            // Act
            target.Activate(activation);

            // Assert
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(ruleMock.Object, target.NextActivation().Rule);
        }
        
        [Test]
        public void Deactivate_CalledAfterActivation_ActivationQueueEmpty()
        {
            // Arrange
            var ruleMock = new Mock<ICompiledRule>();
            var activation = new Activation(ruleMock.Object, new Tuple());
            var target = CreateTarget();
            target.Activate(activation);

            // Act
            target.Deactivate(activation);

            // Assert
            Assert.False(target.HasActiveRules());
        }

        [Test]
        public void Activate_CalledWithMultipleRules_RulesAreQueuedInOrder()
        {
            // Arrange
            var ruleMock1 = new Mock<ICompiledRule>();
            var ruleMock2 = new Mock<ICompiledRule>();
            var activation1 = new Activation(ruleMock1.Object, new Tuple());
            var activation2 = new Activation(ruleMock2.Object, new Tuple());
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
    }
}