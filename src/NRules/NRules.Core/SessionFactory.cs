using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    public class SessionFactory
    {
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IEnumerable<CompiledRule> _rules;
        private readonly Dictionary<string, CompiledRule> _ruleMap;

        public SessionFactory(RuleRepository repository)
        {
            _rules = repository.Compile().ToArray();
            _ruleMap = _rules.ToDictionary(r => r.Handle);

            _network = BuildReteNetwork(_rules, () => new ReteBuilder());
            _agenda = new Agenda();
            _agenda.Subscribe(_network.EventSource);
        }

        public ISession CreateSession()
        {
            var session = new Session(_network, _agenda, _ruleMap);
            return session;
        }

        private INetwork BuildReteNetwork(IEnumerable<CompiledRule> rules, Func<IReteBuilder> builderFactory)
        {
            IReteBuilder reteBuilder = builderFactory.Invoke();
            foreach (CompiledRule rule in rules)
            {
                reteBuilder.AddRule(rule);
            }

            INetwork network = reteBuilder.GetNetwork();
            return network;
        }
    }
}