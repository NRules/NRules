using System;
using System.Linq;
using NRules.Diagnostics;
using NRules.Rete;

namespace NRules
{
    /// <summary>
    /// Represents a rules engine session. Created by <see cref="ISessionFactory"/>.
    /// Each session has its own working memory, and exposes operations that 
    /// manipulate facts in it, as well as fire matching rules.
    /// </summary>
    /// <event cref="IEventProvider.FactInsertingEvent">Before processing fact insertion.</event>
    /// <event cref="IEventProvider.FactInsertedEvent">After processing fact insertion.</event>
    /// <event cref="IEventProvider.FactUpdatingEvent">Before processing fact update.</event>
    /// <event cref="IEventProvider.FactUpdatedEvent">After processing fact update.</event>
    /// <event cref="IEventProvider.FactRetractingEvent">Before processing fact retraction.</event>
    /// <event cref="IEventProvider.FactRetractedEvent">After processing fact retraction.</event>
    /// <event cref="IEventProvider.ActivationCreatedEvent">When a set of facts matches a rule.</event>
    /// <event cref="IEventProvider.ActivationDeletedEvent">When a set of facts no longer matches a rule.</event>
    /// <event cref="IEventProvider.RuleFiringEvent">Before rule's actions are executed.</event>
    /// <event cref="IEventProvider.RuleFiredEvent">After rule's actions are executed.</event>
    /// <event cref="IEventProvider.ConditionFailedEvent">When there is an error during condition evaluation,
    /// before throwing exception to the client.</event>
    /// <event cref="IEventProvider.ActionFailedEvent">When there is an error during action evaluation,
    /// before throwing exception to the client.</event>
    /// <exception cref="RuleConditionEvaluationException">Error while evaluating any of the rules' conditions.
    /// This exception can also be observed as an event <see cref="IEventProvider.ConditionFailedEvent"/>.</exception>
    /// <exception cref="RuleActionEvaluationException">Error while evaluating any of the rules' actions.
    /// This exception can also be observed as an event <see cref="IEventProvider.ActionFailedEvent"/>.</exception>
    /// <threadsafety instance="false" />
    public interface ISession
    {
        /// <summary>
        /// Provider of events from the current rule session. Use it to subscribe to various rules engine lifecycle events.
        /// </summary>
        IEventProvider Events { get; }

        /// <summary>
        /// Rules dependency resolver.
        /// </summary>
        IDependencyResolver DependencyResolver { get; set; }

        /// <summary>
        /// Adds a new fact to the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to add.</param>
        /// <exception cref="ArgumentException">If fact already exists in working memory.</exception>
        void Insert(object fact);

        /// <summary>
        /// Updates existing fact in the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to update.</param>
        /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
        void Update(object fact);

        /// <summary>
        /// Removes existing fact from the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to remove.</param>
        /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
        void Retract(object fact);

        /// <summary>
        /// Starts rules execution cycle.
        /// This method blocks until there are no more rules to fire.
        /// </summary>
        void Fire();

        /// <summary>
        /// Creates a LINQ query to retrieve facts of a given type from the rules engine's memory.
        /// </summary>
        /// <typeparam name="TFact">Type of facts to query. Use <see cref="object"/> to query all facts.</typeparam>
        /// <returns>Queryable working memory of the rules engine.</returns>
        IQueryable<TFact> Query<TFact>();
    }

    /// <summary>
    /// See <see cref="ISession"/>.
    /// </summary>
    public sealed class Session : ISession, ISessionSnapshotProvider
    {
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;
        private readonly IEventAggregator _eventAggregator;
        private readonly IExecutionContext _executionContext;

        internal Session(INetwork network, IAgenda agenda, IWorkingMemory workingMemory, IEventAggregator eventAggregator, IDependencyResolver dependencyResolver)
        {
            _network = network;
            _workingMemory = workingMemory;
            _agenda = agenda;
            _eventAggregator = eventAggregator;
            _executionContext = new ExecutionContext(this, _workingMemory, _agenda, _eventAggregator);
            DependencyResolver = dependencyResolver;
            _network.Activate(_executionContext);
        }

        public IEventProvider Events { get { return _eventAggregator; } }

        public IDependencyResolver DependencyResolver { get; set; }

        public void Insert(object fact)
        {
            _network.PropagateAssert(_executionContext, fact);
        }

        public void Update(object fact)
        {
            _network.PropagateUpdate(_executionContext, fact);
        }

        public void Retract(object fact)
        {
            _network.PropagateRetract(_executionContext, fact);
        }

        public void Fire()
        {
            while (_agenda.HasActiveRules())
            {
                Activation activation = _agenda.NextActivation();
                ICompiledRule rule = activation.Rule;
                var actionContext = new ActionContext(rule.Definition);

                _eventAggregator.RaiseRuleFiring(this, activation);
                foreach (IRuleAction action in rule.Actions)
                {
                    action.Invoke(_executionContext, actionContext, activation.Tuple, activation.TupleFactMap, rule.Dependencies);
                    ApplyActionOperations(actionContext);
                }
                _eventAggregator.RaiseRuleFired(this, activation);

                if (actionContext.IsHalted) break;
            }
        }

        private void ApplyActionOperations(ActionContext actionContext)
        {
            while (actionContext.Operations.Count > 0)
            {
                var operation = actionContext.Operations.Dequeue();
                switch (operation.OperationType)
                {
                    case ActionOperationType.Insert:
                        Insert(operation.Fact);
                        break;
                    case ActionOperationType.Update:
                        Update(operation.Fact);
                        break;
                    case ActionOperationType.Retract:
                        Retract(operation.Fact);
                        break;
                }
            }
        }

        public IQueryable<TFact> Query<TFact>()
        {
            return _workingMemory.Facts.Select(x => x.Object).OfType<TFact>().AsQueryable();
        }

        SessionSnapshot ISessionSnapshotProvider.GetSnapshot()
        {
            var builder = new SnapshotBuilder();
            var visitor = new SessionSnapshotVisitor(_workingMemory);
            _network.Visit(builder, visitor);
            return builder.Build();
        }
    }
}