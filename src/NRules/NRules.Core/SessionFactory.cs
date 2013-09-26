using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NRules.Config;
using NRules.Core.Rete;
using NRules.Rule;

namespace NRules.Core
{
    public interface ISessionFactory
    {
        ISession CreateSession();
    }

    internal class SessionFactory : ISessionFactory
    {
        private readonly INetwork _network;
        private readonly IList<IRuleDefinition> _rules;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public IContainer Container { get; set; }

        public SessionFactory(IRuleBase ruleBase, IReteBuilder reteBuilder)
        {
            _rules = ruleBase.Rules.ToList();
            Log.DebugFormat("Loaded rules from repository. Count={0}", _rules.Count());

            _network = BuildReteNetwork(_rules, reteBuilder);
        }

        public ISession CreateSession()
        {
            var eventAggregator = new EventAggregator();
            var workingMemory = new WorkingMemory(eventAggregator);
            var agenda = new Agenda(_rules, eventAggregator);
            var session = new Session(_network, agenda, workingMemory);
            return session;
        }

        private INetwork BuildReteNetwork(IEnumerable<IRuleDefinition> rules, IReteBuilder reteBuilder)
        {
            foreach (IRuleDefinition rule in rules)
            {
                reteBuilder.AddRule(rule);
            }

            INetwork network = reteBuilder.GetNetwork();
            return network;
        }
    }
}