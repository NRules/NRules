using NRules.Core.Rete;
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

        internal Agenda CreateTarget()
        {
            return new Agenda(_eventAggregator);
        }

        [Test]
        public void Activate_Called_ActivationEndsUpInQueue()
        {
            // Arrange
            var activation = new Activation("rule1", new Tuple(new Tuple(), new Fact(new object())));
            var target = CreateTarget();

            // Act
            _eventAggregator.Activate(activation);
            var queuedItem = target.ActivationQueue.Dequeue();

            // Assert
            Assert.AreEqual(0, target.ActivationQueue.Count());
            Assert.AreEqual("rule1", queuedItem.RuleHandle);
        }

        [Test]
        public void Activate_CalledWithMultipleRules_RulesAreQueuedInOrder()
        {
            // Arrange
            var activation1 = new Activation("rule1", new Tuple(new Tuple(), new Fact(new object())));
            var activation2 = new Activation("rule2", new Tuple(new Tuple(), new Fact(new object())));
            var target = CreateTarget();

            // Act
            _eventAggregator.Activate(activation1);
            _eventAggregator.Activate(activation2);
            var queuedItem1 = target.ActivationQueue.Dequeue();
            var queuedItem2 = target.ActivationQueue.Dequeue();

            // Assert
            Assert.AreEqual(0, target.ActivationQueue.Count());
            Assert.AreEqual("rule1", queuedItem1.RuleHandle);
            Assert.AreEqual("rule2", queuedItem2.RuleHandle);
        }
    }
}