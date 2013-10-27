using NRules.Rete;
using NUnit.Framework;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class AgendaTest
    {
        private EventAggregator _eventAggregator;

        [SetUp]
        public void Setup()
        {
            _eventAggregator = new EventAggregator();
        }

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
            var activation = new Activation("rule1", 0, new Tuple());
            var target = CreateTarget();

            // Act
            _eventAggregator.Activate(activation);

            // Assert
            Assert.True(target.HasActiveRules());
            Assert.AreEqual("rule1", target.NextActivation().RuleHandle);
        }

        [Test]
        public void Activate_CalledWithMultipleRules_RulesAreQueuedInOrder()
        {
            // Arrange
            var activation1 = new Activation("rule1", 0, new Tuple());
            var activation2 = new Activation("rule2", 0, new Tuple());
            var target = CreateTarget();

            // Act
            _eventAggregator.Activate(activation1);
            _eventAggregator.Activate(activation2);

            // Assert
            Assert.True(target.HasActiveRules());
            Assert.AreEqual("rule1", target.NextActivation().RuleHandle);
            Assert.True(target.HasActiveRules());
            Assert.AreEqual("rule2", target.NextActivation().RuleHandle);
        }

        private Agenda CreateTarget()
        {
            return new Agenda(_eventAggregator);
        }
    }
}