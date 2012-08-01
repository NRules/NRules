using System;

namespace NRules.Core.Rete
{
    internal interface IEventAggregator
    {
        event EventHandler<ActivationEventArgs> RuleActivatedEvent;
        event EventHandler<ActivationEventArgs> RuleDeactivatedEvent;

        void Activate(Activation activation);
        void Deactivate(Activation activation);
    }

    internal class EventAggregator : IEventAggregator
    {
        public event EventHandler<ActivationEventArgs> RuleActivatedEvent;
        public event EventHandler<ActivationEventArgs> RuleDeactivatedEvent;

        public void Activate(Activation activation)
        {
            var args = new ActivationEventArgs(activation);
            if (RuleActivatedEvent != null)
            {
                RuleActivatedEvent(this, args);
            }
        }

        public void Deactivate(Activation activation)
        {
            var args = new ActivationEventArgs(activation);
            if (RuleDeactivatedEvent != null)
            {
                RuleDeactivatedEvent(this, args);
            }
        }
    }
}