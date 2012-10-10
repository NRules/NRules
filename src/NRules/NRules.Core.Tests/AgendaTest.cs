using System.Collections.Generic;
using NRules.Core.Rete;
using NRules.Core.Rules;
using NUnit.Framework;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class AgendaTest
    {
        private EventAggregator _eventAggregator;
        private IList<CompiledRule> _rules;

        [SetUp]
        public void Setup()
        {
            _eventAggregator = new EventAggregator();
            _rules = new List<CompiledRule>
                         {
                             new CompiledRule() {Name = "rule1"},
                             new CompiledRule() {Name = "rule2"},
                             new CompiledRule() {Name = "rule3"}
                         };
        }

        internal Agenda CreateTarget()
        {
            return new Agenda(_rules, _eventAggregator);
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
            var activation = new Activation(_rules[0].Handle, new Tuple());
            var target = CreateTarget();

            // Act
            _eventAggregator.Activate(activation);

            // Assert
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(_rules[0], target.NextActivation().Rule);
        }

        [Test]
        public void Activate_CalledWithMultipleRules_RulesAreQueuedInOrder()
        {
            // Arrange
            var activation1 = new Activation(_rules[0].Handle, new Tuple());
            var activation2 = new Activation(_rules[1].Handle, new Tuple());
            var target = CreateTarget();

            // Act
            _eventAggregator.Activate(activation1);
            _eventAggregator.Activate(activation2);

            // Assert
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(_rules[0], target.NextActivation().Rule);
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(_rules[1], target.NextActivation().Rule);
        }
    }
}