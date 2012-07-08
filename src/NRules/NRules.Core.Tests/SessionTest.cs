using System.Collections.Generic;
using NRules.Core.Rete;
using NRules.Core.Rules;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class SessionTest
    {
        private IReteBuilder _reteBuilder;
        private IAgenda _agenda;
        private INetwork _network;
        private IEventSource _eventSource;

        private List<Rule> _rules;

        [SetUp]
        public void Setup()
        {
            _reteBuilder = MockRepository.GenerateStub<IReteBuilder>();
            _agenda = MockRepository.GenerateStub<IAgenda>();
            _network = MockRepository.GenerateStub<INetwork>();
            _eventSource = MockRepository.GenerateStub<IEventSource>();

            _rules = new List<Rule> { new Rule("rule1"), 
                                      new Rule("rule2"), 
                                      new Rule("rule3") };
            
        }

        internal Session CreateTarget()
        {
            return new Session(_reteBuilder, _agenda);
        }

        [Test]
        public void Constructor_Always_DefaultsToNotConfigured()
        {
            // Arrange
            var target = CreateTarget();

            // Act
            bool configured = target.IsConfigured;

            // Assert
            Assert.False(configured);
        }

        [Test]
        public void SetRules_ValidRules_AreAllAddedToReteBuilder()
        {
            // Arrange
            _reteBuilder = MockRepository.GenerateMock<IReteBuilder>();
            _reteBuilder.Stub(x => x.GetNetwork()).Return(_network);
            var target = CreateTarget();

            // Act
            target.SetRules(_rules);

            // Assert
            _reteBuilder.AssertWasCalled(x => x.AddRule(_rules[0]));
            _reteBuilder.AssertWasCalled(x => x.AddRule(_rules[1]));
            _reteBuilder.AssertWasCalled(x => x.AddRule(_rules[2]));
        }

        [Test]
        public void SetRules_ValidRules_AgendaSubscribesToEventSource()
        {
            // Arrange
            _agenda = MockRepository.GenerateMock<IAgenda>();
            _network.Stub(x => x.EventSource).Return(_eventSource);
            _reteBuilder.Stub(x => x.GetNetwork()).Return(_network);
            var target = CreateTarget();

            // Act
            target.SetRules(_rules);

            // Assert
            _agenda.AssertWasCalled(x => x.Subscribe(_eventSource));
        }
    }
}
