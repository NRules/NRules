using System.Collections.Generic;
using NRules.Rete;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.Tests
{
    [TestFixture]
    public class SessionTest
    {
        private IAgenda _agenda;
        private INetwork _network;
        private IWorkingMemory _workingMemory;
        private IList<ICompiledRule> _rules;

        [SetUp]
        public void Setup()
        {
            _rules = new List<ICompiledRule>();
            _agenda = MockRepository.GenerateStub<IAgenda>();
            _network = MockRepository.GenerateStub<INetwork>();
            _workingMemory = MockRepository.GenerateStub<IWorkingMemory>();
        }

        internal Session CreateTarget()
        {
            return new Session(_rules, _network, _agenda, _workingMemory);
        }

        [Test]
        public void Insert_Always_PropagatesAssert()
        {
            // Arrange
            _network = MockRepository.GenerateMock<INetwork>();
            var myFact = new object();
            var target = CreateTarget();

            // Act
            target.Insert(myFact);

            // Assert
            _network.AssertWasCalled(x => x.PropagateAssert(_workingMemory, myFact));
        }
    }
}