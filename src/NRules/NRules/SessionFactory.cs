using NRules.Diagnostics;
using NRules.Extensibility;
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
    /// <event cref="IEventProvider.FactInsertingEvent">Before processing fact insertion.</event>
    /// <event cref="IEventProvider.FactInsertedEvent">After processing fact insertion.</event>
    /// <event cref="IEventProvider.FactUpdatingEvent">Before processing fact update.</event>
    /// <event cref="IEventProvider.FactUpdatedEvent">After processing fact update.</event>
    /// <event cref="IEventProvider.FactRetractingEvent">Before processing fact retraction.</event>
    /// <event cref="IEventProvider.FactRetractedEvent">After processing fact retraction.</event>
    /// <event cref="IEventProvider.ActivationCreatedEvent">When a set of facts matches a rule.</event>
    /// <event cref="IEventProvider.ActivationUpdatedEvent">When a set of facts is updated and re-matches a rule.</event>
    /// <event cref="IEventProvider.ActivationDeletedEvent">When a set of facts no longer matches a rule.</event>
    /// <event cref="IEventProvider.RuleFiringEvent">Before rule's actions are executed.</event>
    /// <event cref="IEventProvider.RuleFiredEvent">After rule's actions are executed.</event>
    /// <event cref="IEventProvider.ConditionFailedEvent">When there is an error during condition evaluation,
    /// before throwing exception to the client.</event>
    /// <event cref="IEventProvider.ActionFailedEvent">When there is an error during action evaluation,
    /// before throwing exception to the client.</event>
    /// <seealso cref="ISession"/>
    /// <threadsafety instance="true" />
    public interface ISessionFactory
    {
        /// <summary>
        /// Provider of events aggregated across all rule sessions. 
        /// Event sender is used to convey the session instance responsible for the event.
        /// Use it to subscribe to various rules engine lifecycle events.
        /// </summary>
        IEventProvider Events { get; }

        /// <summary>
        /// Rules dependency resolver for all rules sessions.
        /// </summary>
        IDependencyResolver DependencyResolver { get; set; }

        /// <summary>
        /// Action interceptor for all rules sessions.
        /// If provided, invocation of rule actions is delegated to the interceptor.
        /// </summary>
        IActionInterceptor ActionInterceptor { get; set; }

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
            DependencyResolver = new DependencyResolver();
        }

        public IEventProvider Events { get { return _eventAggregator; } }
        public IDependencyResolver DependencyResolver { get; set; }
        public IActionInterceptor ActionInterceptor { get; set; }

        public ISession CreateSession()
        {
            var agenda = new Agenda();
            var workingMemory = new WorkingMemory();
            var eventAggregator = new EventAggregator(_eventAggregator);
            var actionExecutor = new ActionExecutor();
            var session = new Session(_network, agenda, workingMemory, eventAggregator, actionExecutor, DependencyResolver, ActionInterceptor);
            return session;
        }
    }
}