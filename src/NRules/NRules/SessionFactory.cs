using NRules.Events;
using NRules.Rete;

namespace NRules
{
    /// <summary>
    /// Represents compiled production rules that can be used to create rules sessions.
    /// Session factory is expensive to create (because rules need to be compiled into an executable form).
    /// Therefore there needs to be only a single instance of session factory for a given set of rules for the lifetime of the application.
    /// If repeatedly running rules for different sets of facts, don't create a new session factory for each rules run.
    /// Instead, have a single session factory and create a new rules session for each independent universe of facts.
    /// <seealso cref="ISession"/>
    /// </summary>
    /// <remarks>Session factory is safe to use from multiple threads.</remarks>
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
            var agenda = new Agenda();
            var workingMemory = new WorkingMemory();
            var eventAggregator = new EventAggregator();
            var session = new Session(_network, agenda, workingMemory, eventAggregator);
            return session;
        }
    }
}