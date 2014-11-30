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
        /// Aggregator for rule session events.
        /// </summary>
        IEventProvider Events { get; }

        /// <summary>
        /// Adds a new fact to the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to add.</param>
        void Insert(object fact);

        /// <summary>
        /// Updates existing fact in the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to update.</param>
        void Update(object fact);

        /// <summary>
        /// Removes existing fact from the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to remove.</param>
        void Retract(object fact);

        /// <summary>
        /// Starts rules execution cycle. This method blocks until there are no more
        /// rules to fire.
        /// </summary>
        void Fire();

        /// <summary>
        /// Creates a LINQ query to retrieve facts of a given type from the rules engine's memory.
        /// </summary>
        /// <typeparam name="TFact">Type of facts to query.</typeparam>
        /// <returns>Queryable engine's memory.</returns>
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