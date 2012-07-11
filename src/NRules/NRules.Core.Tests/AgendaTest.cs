using System.Collections.Generic;
using NRules.Core.Rete;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class AgendaTest
    {
        private EventAggregator _eventAggregator;
        private EventAggregator _eventAggregator2;
        private EventAggregator _eventAggregator3;

        [SetUp]
        public void Setup()
        {
            _eventAggregator = new EventAggregator();
            _eventAggregator2 = new EventAggregator();
            _eventAggregator3 = new EventAggregator();
        }

        internal static Agenda CreateTarget()
        {
            return new Agenda();
        }

        [Test]
        public void Subscribe_RuleActivated_ActivationEndsUpInQueue()
        {
            // Arrange
            Activation activation = new Activation("rule1", new Tuple(new Fact(new object())));
            var target = CreateTarget();

            // Act
            target.Subscribe(_eventAggregator);
            target.Subscribe(_eventAggregator2);
            _eventAggregator.Activate(activation);
            Queue<Activation> activationQueue = target.ActivationQueue;

            // Assert
            Assert.AreEqual(1, activationQueue.Count);
            Assert.AreEqual("rule1", activationQueue.Peek().RuleHandle);
        }
        
        [Test]
        public void Subscribe_MultipleRulesActivated_RulesAreQueuedInOrder()
        {
            // Arrange
            Activation activation = new Activation("rule1", new Tuple(new Fact(new object())));
            Activation activation2 = new Activation("rule2", new Tuple(new Fact(new object())));
            var target = CreateTarget();

            // Act
            target.Subscribe(_eventAggregator);
            target.Subscribe(_eventAggregator2);
            target.Subscribe(_eventAggregator3);
            _eventAggregator.Activate(activation);
            _eventAggregator2.Activate(activation2);
            Queue<Activation> activationQueue = target.ActivationQueue;

            // Assert
            Assert.AreEqual(2, activationQueue.Count);
            Assert.AreEqual("rule1", activationQueue.Dequeue().RuleHandle);
            Assert.AreEqual("rule2", activationQueue.Dequeue().RuleHandle);
        }
    }
}
