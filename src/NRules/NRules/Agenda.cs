using NRules.Rete;

namespace NRules
{
    internal interface IAgenda
    {
        bool HasActiveRules();
        Activation NextActivation();
        void Activate(Activation activation);
        void Deactivate(Activation activation);
    }

    internal class Agenda : IAgenda
    {
        private readonly IActivationQueue _activationQueue = new ActivationQueue();

        public bool HasActiveRules()
        {
            return _activationQueue.HasActive();
        }

        public Activation NextActivation()
        {
            Activation activation = _activationQueue.Dequeue();
            return activation;
        }

        public void Activate(Activation activation)
        {
            _activationQueue.Enqueue(activation.Rule.Priority, activation);
        }

        public void Deactivate(Activation activation)
        {
            _activationQueue.Remove(activation);
        }
    }
}