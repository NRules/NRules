using System.Linq;
using NRules.Diagnostics;
using NRules.Events;
using NRules.Rete;

namespace NRules
{
    /// <summary>
    /// Represents a rules engine session.
    /// Each session has its own working memory, and exposes operations that 
    /// manipulate facts in it, as well as run the rules.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Hub for rule session events.
        /// </summary>
        IEventProvider EventProvider { get; }

        /// <summary>
        /// Adds a new fact to the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to add.</param>
        void Insert(object fact);

        /// <summary>
        /// Updates an existing fact in the rules engine memory.
        /// </summary>
        /// <param name="fact">Fact to update.</param>
        void Update(object fact);

        /// <summary>
        /// Removes an existing fact from the rules engine memory.
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

        /// <summary>
        /// Returns a snapshot of session state for diagnostics.
        /// </summary>
        /// <returns>Session snapshot.</returns>
        SessionSnapshot GetSnapshot();
    }

    internal class Session : ISession
    {
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;
        private readonly IEventAggregator _eventAggregator;
        private readonly IExecutionContext _executionContext;

        public Session(INetwork network, IAgenda agenda, IWorkingMemory workingMemory, IEventAggregator eventAggregator)
        {
            _network = network;
            _workingMemory = workingMemory;
            _agenda = agenda;
            _eventAggregator = eventAggregator;
            _executionContext = new ExecutionContext(_workingMemory, _agenda, _eventAggregator);
            _network.Activate(_executionContext);
        }

        public IEventProvider EventProvider { get { return _eventAggregator; } }

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
            var actionContext = new ActionContext(this);

            while (_agenda.HasActiveRules() && !actionContext.IsHalted)
            {
                Activation activation = _agenda.NextActivation();
                ICompiledRule rule = activation.Rule;

                _eventAggregator.BeforeRuleFired(activation);
                foreach (IRuleAction action in rule.Actions)
                {
                    action.Invoke(actionContext, activation.Tuple);
                }
                _eventAggregator.AfterRuleFired(activation);
            }
        }

        public IQueryable<TFact> Query<TFact>()
        {
            return _workingMemory.Facts.Select(x => x.Object).OfType<TFact>().AsQueryable();
        }

        public SessionSnapshot GetSnapshot()
        {
            var builder = new SnapshotBuilder();
            var visitor = new SessionSnapshotVisitor(_workingMemory);
            _network.Visit(builder, visitor);
            return builder.Build();
        }
    }
}