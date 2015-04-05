using NRules.Diagnostics;
using NRules.Rete;

namespace NRules
{
    /// <summary>
    /// Represents compiled production rules that can be used to create rules sessions.
    /// Created by <see cref="RuleCompiler"/> by compiling rule model into an executable form.
    /// </summary>
    /// <remarks>
    /// Session factory is expensive to create (because rules need to be compiled into an executable form).
    /// Therefore there needs to be only a single instance of session factory for a given set of rules for the lifetime of the application.
    /// If repeatedly running rules for different sets of facts, don't create a new session factory for each rules run.
    /// Instead, have a single session factory and create a new rules session for each independent universe of facts.
    /// </remarks>
    /// <seealso cref="ISession"/>
    /// <threadsafety instance="true" />
    public interface ISessionFactory
    {
        /// <summary>
        /// Provider of rule session events. Use it to subscribe to various rules engine lifecycle events.
        /// </summary>
        IEventProvider Events { get; }

        /// <summary>
        /// Creates a new rules session.
        /// </summary>
        /// <returns>New rules session.</returns>
        ISession CreateSession();
    }

    internal class SessionFactory : ISessionFactory
    {
        private readonly INetwork _network;
        private readonly IEventAggregator _eventAggregator = new EventAggregator();

        public SessionFactory(INetwork network)
        {
            _network = network;
        }

        public IEventProvider Events { get { return _eventAggregator; } }

        public ISession CreateSession()
        {
            var agenda = new Agenda();
            var workingMemory = new WorkingMemory();
            var eventAggregator = new EventAggregator(_eventAggregator);
            var session = new Session(_network, agenda, workingMemory, eventAggregator);
            return session;
        }
    }
}