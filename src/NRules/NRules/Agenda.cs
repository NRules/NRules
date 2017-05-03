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
        void Add(Activation activation);
        void Modify(Activation activation);
        void Remove(Activation activation);
    }

    internal class Agenda : IAgendaInternal
    {
        private readonly ActivationQueue _activationQueue = new ActivationQueue();

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

        public void Add(Activation activation)
        {
            _activationQueue.Enqueue(activation.CompiledRule.Priority, activation);
        }

        public void Modify(Activation activation)
        {
            _activationQueue.Enqueue(activation.CompiledRule.Priority, activation);
        }

        public void Remove(Activation activation)
        {
            _activationQueue.Remove(activation);
        }
    }
}