using NRules.Rete;

namespace NRules
{
    /// <summary>
    /// Represents compiled production rules that can be used to create rules sessions.
    /// </summary>
    public interface ISessionFactory
    {
        /// <summary>
        /// Creates a new rules session.
        /// </summary>
        /// <returns>New rules session.</returns>
        ISession CreateSession();
    }

    internal class SessionFactory : ISessionFactory
    {
        private readonly INetwork _network;

        public SessionFactory(INetwork network)
        {
            _network = network;
        }

        public ISession CreateSession()
        {
            var eventAggregator = new EventAggregator();
            var workingMemory = new WorkingMemory(eventAggregator);
            var agenda = new Agenda(eventAggregator);
            var session = new Session(_network, agenda, workingMemory);
            return session;
        }
    }
}