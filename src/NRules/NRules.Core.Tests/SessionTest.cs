using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using NRules.Core.Rules;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class SessionTest
    {
        private IAgenda _agenda;
        private INetwork _network;
        private IWorkingMemory _workingMemory;

        private List<CompiledRule> _rules;
        private Dictionary<string, CompiledRule> _ruleMap;

        [SetUp]
        public void Setup()
        {
            _agenda = MockRepository.GenerateStub<IAgenda>();
            _network = MockRepository.GenerateStub<INetwork>();
            _workingMemory = MockRepository.GenerateStub<IWorkingMemory>();

            _rules = new List<CompiledRule>
                         {
                             new CompiledRule("rule1"),
                             new CompiledRule("rule2"),
                             new CompiledRule("rule3")
                         };
            _ruleMap = _rules.ToDictionary(x => x.Handle);
        }

        internal Session CreateTarget()
        {
            return new Session(_network, _agenda, _workingMemory, _ruleMap);
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