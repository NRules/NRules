using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NRules.Config;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    public interface ISessionFactory
    {
        ISession CreateSession();
    }

    internal class SessionFactory : ISessionFactory
    {
        private readonly INetwork _network;
        private readonly IEnumerable<CompiledRule> _rules;
        private readonly Dictionary<string, CompiledRule> _ruleMap;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public IContainer Container { get; set; }

        public SessionFactory(RuleRepository repository, IReteBuilder reteBuilder)
        {
            _rules = repository.Compile().ToArray();
            _ruleMap = _rules.ToDictionary(r => r.Handle);
            Log.DebugFormat("Compiled rules from repository. Count={0}", _rules.Count());

            _network = BuildReteNetwork(_rules, reteBuilder);
        }

        public ISession CreateSession()
        {
            var eventAggregator = new EventAggregator();
            var workingMemory = new WorkingMemory(eventAggregator);
            var agenda = new Agenda(eventAggregator);
            var session = new Session(_network, agenda, workingMemory, _ruleMap);
            return session;
        }

        private INetwork BuildReteNetwork(IEnumerable<CompiledRule> rules, IReteBuilder reteBuilder)
        {
            foreach (CompiledRule rule in rules)
            {
                reteBuilder.AddRule(rule);
            }

            INetwork network = reteBuilder.GetNetwork();
            return network;
        }
    }
}