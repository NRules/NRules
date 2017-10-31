using System.Collections.Generic;
using System.Linq;

namespace NRules
{
    /// <summary>
    /// Agenda stores matches between rules and facts. These matches are called activations.
    /// Multiple activations are ordered according to the conflict resolution strategy.
    /// </summary>
    public interface IAgenda
    {
        /// <summary>
        /// Indicates whether there are any activations in the agenda.
        /// </summary>
        /// <returns>If agenda is empty then <c>true</c> otherwise <c>false</c>.</returns>
        bool IsEmpty();

        /// <summary>
        /// Retrieves the next match, without removing it from agenda.
        /// </summary>
        /// <remarks>Throws <c>InvalidOperationException</c> if agenda is empty.</remarks>
        /// <returns>Next match.</returns>
        IActivation Peek();

        /// <summary>
        /// Removes all matches from agenda.
        /// </summary>
        void Clear();
    }

    internal interface IAgendaInternal : IAgenda
    {
        Activation Pop();
        void Add(IExecutionContext context, Activation activation);
        void Modify(IExecutionContext context, Activation activation);
        void Remove(IExecutionContext context, Activation activation);
    }

    internal class Agenda : IAgendaInternal
    {
        private readonly ActivationQueue _activationQueue = new ActivationQueue();
        private readonly IDictionary<ICompiledRule, IActivationFilter[]> _filters;

        public Agenda(IDictionary<ICompiledRule, IActivationFilter[]> filters)
        {
            _filters = filters;
        }

        public bool IsEmpty()
        {
            return !_activationQueue.HasActive();
        }

        public IActivation Peek()
        {
            Activation activation = _activationQueue.Peek();
            return activation;
        }

        public void Clear()
        {
            _activationQueue.Clear();
        }

        public Activation Pop()
        {
            Activation activation = _activationQueue.Dequeue();
            return activation;
        }

        public void Add(IExecutionContext context, Activation activation)
        {
            if (!Accept(activation)) return;
            _activationQueue.Enqueue(activation.CompiledRule.Priority, activation);
        }
        //I apologize, but the analyzer issued a warning here. Methods Add and Modify Implement the same code, is this normal behavior? Unfortunately I can not go deep into the project to find out.
        public void Modify(IExecutionContext context, Activation activation) 
        {
            if (!Accept(activation)) return;
            _activationQueue.Enqueue(activation.CompiledRule.Priority, activation);
        }

        public void Remove(IExecutionContext context, Activation activation)
        {
            _activationQueue.Remove(activation);
            UnlinkFacts(context.Session, activation);
            Remove(activation);
        }

        private bool Accept(Activation activation)
        {
            if (!_filters.TryGetValue(activation.CompiledRule, out var filters)) return true;
            foreach (var filter in filters)
            {
                if (!filter.Accept(activation)) return false;
            }
            return true;
        }

        private void Remove(Activation activation)
        {
            if (!_filters.TryGetValue(activation.CompiledRule, out var filters)) return;
            foreach (var filter in filters)
            {
                filter.Remove(activation);
            }
        }

        private static void UnlinkFacts(ISessionInternal session, Activation activation)
        {
            var linkedKeys = session.GetLinkedKeys(activation).ToList();
            foreach (var key in linkedKeys)
            {
                var linkedFact = session.GetLinked(activation, key);
                session.RetractLinked(activation, key, linkedFact);
            }
        }
    }
}