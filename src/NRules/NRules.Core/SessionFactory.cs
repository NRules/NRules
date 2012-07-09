using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    public class SessionFactory
    {
        private readonly IReteBuilder _reteBuilder;
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IEnumerable<Rule> _rules;
        private readonly Dictionary<string, Rule> _ruleMap;

        public SessionFactory(RuleRepository repository)
        {
            _rules = repository.Compile().ToArray();

            //todo: use di container
            _reteBuilder = new ReteBuilder();
            _network = GetNetwork(_rules);
            _ruleMap = GetRuleMap(_rules);
            _agenda = new Agenda();
            _agenda.Subscribe(_network.EventSource);
        }

        public ISession CreateSession()
        {
            var session = new Session(_network, _agenda, _ruleMap);
            return session;
        }

        private INetwork GetNetwork(IEnumerable<Rule> rules)
        {
            foreach (Rule rule in rules)
            {
                _reteBuilder.AddRule(rule);
            }

            INetwork network = _reteBuilder.GetNetwork();
            return network;
        }

        private static Dictionary<string, Rule> GetRuleMap(IEnumerable<Rule> rules)
        {
            Dictionary<string, Rule> ruleMap = rules.ToDictionary(r => r.Handle);
            return ruleMap;
        }
    }
}