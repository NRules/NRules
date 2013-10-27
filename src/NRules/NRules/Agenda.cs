using NRules.Rete;

namespace NRules
{
    internal interface IAgenda
    {
        bool HasActiveRules();
        Activation NextActivation();
    }

    internal class Agenda : IAgenda
    {
        private readonly ActivationQueue _activationQueue;

        public Agenda(IEventAggregator eventAggregator)
        {
            _activationQueue = new ActivationQueue();
            Subscribe(eventAggregator);
        }

        public bool HasActiveRules()
        {
            return _activationQueue.HasActive();
        }

        public Activation NextActivation()
        {
            Activation activation = _activationQueue.Dequeue();
            return activation;
        }

        private void Subscribe(IEventAggregator eventAggregator)
        {
            eventAggregator.RuleActivatedEvent += OnRuleActivated;
            eventAggregator.RuleDeactivatedEvent += OnRuleDeactivated;
        }

        private void OnRuleActivated(object sender, ActivationEventArgs e)
        {
            _activationQueue.Enqueue(e.Activation.RulePriority, e.Activation);
        }

        private void OnRuleDeactivated(object sender, ActivationEventArgs e)
        {
            _activationQueue.Remove(e.Activation);
        }
    }
}