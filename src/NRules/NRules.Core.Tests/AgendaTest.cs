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

        internal static Agenda CreateTarget()
        {
            return new Agenda();
        }

        [Test]
        public void Subscribe_RuleActivated_ActivationEndsUpInQueue()
        {
            // Arrange
            var activation = new Activation("rule1", new Tuple(new Fact(new object()), null));
            var target = CreateTarget();
            target.Subscribe(_eventAggregator);

            // Act
            _eventAggregator.Activate(activation);
            var queuedItem = target.ActivationQueue.Dequeue();

            // Assert
            Assert.AreEqual(0, target.ActivationQueue.Count());
            Assert.AreEqual("rule1", queuedItem.RuleHandle);
        }

        [Test]
        public void Subscribe_MultipleRulesActivated_RulesAreQueuedInOrder()
        {
            // Arrange
            var activation1 = new Activation("rule1", new Tuple(new Fact(new object()), null));
            var activation2 = new Activation("rule2", new Tuple(new Fact(new object()), null));
            var target = CreateTarget();
            target.Subscribe(_eventAggregator);

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