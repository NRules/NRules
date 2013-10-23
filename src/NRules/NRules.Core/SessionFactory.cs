using System.Collections.Generic;
using NRules.Core.Rete;

namespace NRules.Core
{
    public interface ISessionFactory
    {
        ISession CreateSession();
    }

    internal class SessionFactory : ISessionFactory
    {
        private readonly INetwork _network;
        private readonly List<ICompiledRule> _rules;

        public SessionFactory(IEnumerable<ICompiledRule> rules, INetwork network)
        {
            _rules = new List<ICompiledRule>(rules);
            _network = network;
        }

        public ISession CreateSession()
        {
            var eventAggregator = new EventAggregator();
            var workingMemory = new WorkingMemory(eventAggregator);
            var agenda = new Agenda(eventAggregator);
            var session = new Session(_rules, _network, agenda, workingMemory);
            return session;
        }
    }
}