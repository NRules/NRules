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

        private List<Rule> _rules;
        private Dictionary<string, Rule> _ruleMap;

        [SetUp]
        public void Setup()
        {
            _agenda = MockRepository.GenerateStub<IAgenda>();
            _network = MockRepository.GenerateStub<INetwork>();

            _rules = new List<Rule>
                         {
                             new Rule("rule1"),
                             new Rule("rule2"),
                             new Rule("rule3")
                         };
            _ruleMap = _rules.ToDictionary(x => x.Handle);
        }

        internal Session CreateTarget()
        {
            return new Session(_network, _agenda, _ruleMap);
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
            _network.AssertWasCalled(x => x.PropagateAssert(myFact));
        }
    }
}