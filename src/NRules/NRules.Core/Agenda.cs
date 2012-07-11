using System.Collections.Generic;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    internal interface IAgenda
    {
        Queue<Activation> ActivationQueue { get; }
        void Subscribe(IEventSource eventSource);
    }

    internal class Agenda : IAgenda
    {
        public Queue<Activation> ActivationQueue { get; private set; }

        public Agenda()
        {
            ActivationQueue = new Queue<Activation>();
        }

        public void Subscribe(IEventSource eventSource)
        {
            eventSource.RuleActivatedEvent += OnRuleActivated;
        }

        private void OnRuleActivated(object sender, ActivationEventArgs e)
        {
            ActivationQueue.Enqueue(e.Activation);
        }
    }
}