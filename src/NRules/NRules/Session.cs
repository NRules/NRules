using System;
using System.Linq;
using NRules.Diagnostics;
using NRules.Rete;

namespace NRules
{
    /// <summary>
    /// Represents a rules engine session.
    /// Each session has its own working memory, and exposes operations that 
    /// manipulate facts in it, as well as fire matching rules.
    /// </summary>
    /// <remarks>Session can only be used by a single thread.</remarks>
    public interface ISession
    {
        /// <summary>
        /// Aggregator for rule session events. Use it to subscribe to various rules engine lifecycle events.
        /// </summary>
        IEventProvider Events { get; }

        /// <summary>
        /// Adds a new fact to the rules engine memory.
        /// Raises <see cref="IEventProvider.FactInsertingEvent"/> and <see cref="IEventProvider.FactInsertedEvent"/> events.
        /// </summary>
        /// <param name="fact">Fact to add.</param>
        /// <exception cref="ArgumentException">If fact already exists in working memory.</exception>
        void Insert(object fact);

        /// <summary>
        /// Updates existing fact in the rules engine memory.
        /// Raises <see cref="IEventProvider.FactUpdatingEvent"/> and <see cref="IEventProvider.FactUpdatedEvent"/> events.
        /// </summary>
        /// <param name="fact">Fact to update.</param>
        /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
        void Update(object fact);

        /// <summary>
        /// Removes existing fact from the rules engine memory.
        /// Raises <see cref="IEventProvider.FactRetractingEvent"/> and <see cref="IEventProvider.FactRetractedEvent"/> events.
        /// </summary>
        /// <param name="fact">Fact to remove.</param>
        /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
        void Retract(object fact);

        /// <summary>
        /// Starts rules execution cycle. This method blocks until there are no more
        /// rules to fire.
        /// Raises <see cref="IEventProvider.ActivationCreatedEvent"/> and <see cref="IEventProvider.ActivationDeletedEvent"/> events when activations are created or deleted.
        /// Raises <see cref="IEventProvider.RuleFiringEvent"/> and <see cref="IEventProvider.RuleFiredEvent"/> events when rules are firing.
        /// </summary>
        /// <exception cref="RuleExecutionException">Any fatal failure during rules execution.</exception>
        /// <exception cref="RuleConditionEvaluationException">Failure while evaluating any of the rules' conditions.
        /// This exception can also be observed as an event <see cref="IEventProvider.ConditionFailedEvent"/>.</exception>
        /// <exception cref="RuleActionEvaluationException">Failure while evaluating any of the rules' actions.
        /// This exception can also be observed as an event <see cref="IEventProvider.ActionFailedEvent"/>.</exception>
        void Fire();

        /// <summary>
        /// Creates a LINQ query to retrieve facts of a given type from the rules engine's memory.
        /// </summary>
        /// <typeparam name="TFact">Type of facts to query.</typeparam>
        /// <returns>Queryable working memory of the rules engine.</returns>
        IQueryable<TFact> Query<TFact>();
    }

    public sealed class Session : ISession, ISessionSnapshotProvider
    {
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;
        private readonly IEventAggregator _eventAggregator;
        private readonly IExecutionContext _executionContext;

        internal Session(INetwork network, IAgenda agenda, IWorkingMemory workingMemory, IEventAggregator eventAggregator)
        {
            _network = network;
            _workingMemory = workingMemory;
            _agenda = agenda;
            _eventAggregator = eventAggregator;
            _executionContext = new ExecutionContext(this, _workingMemory, _agenda, _eventAggregator);
            _network.Activate(_executionContext);
        }

        public IEventProvider Events { get { return _eventAggregator; } }

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
            var actionContext = new ActionContext();

            while (_agenda.HasActiveRules() && !actionContext.IsHalted)
            {
                Activation activation = _agenda.NextActivation();
                ICompiledRule rule = activation.Rule;

                _eventAggregator.RaiseRuleFiring(this, activation);
                foreach (IRuleAction action in rule.Actions)
                {
                    action.Invoke(_executionContext, actionContext, activation.Tuple);
                    ApplyActionOperations(actionContext);
                }
                _eventAggregator.RaiseRuleFired(this, activation);
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