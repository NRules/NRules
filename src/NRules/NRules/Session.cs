using System.Linq;
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
    }

    internal class Session : ISession
    {
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;

        public Session(INetwork network, IAgenda agenda, IWorkingMemory workingMemory)
        {
            _network = network;
            _agenda = agenda;
            _workingMemory = workingMemory;
        }

        public void Insert(object fact)
        {
            _network.PropagateAssert(_workingMemory, fact);
        }

        public void Update(object fact)
        {
            _network.PropagateUpdate(_workingMemory, fact);
        }

        public void Retract(object fact)
        {
            _network.PropagateRetract(_workingMemory, fact);
        }

        public void Fire()
        {
            var context = new ActionContext(_network, _workingMemory);

            while (_agenda.HasActiveRules() && !context.IsHalted)
            {
                Activation activation = _agenda.NextActivation();
                ICompiledRule rule = activation.Rule;

                foreach (IRuleAction action in rule.Actions)
                {
                    action.Invoke(context, activation.Tuple);
                }
            }
        }

        public IQueryable<TFact> Query<TFact>()
        {
            return _workingMemory.Facts.Select(x => x.Object).OfType<TFact>().AsQueryable();
        }
    }
}