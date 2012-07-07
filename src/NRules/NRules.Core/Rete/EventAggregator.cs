using System;

namespace NRules.Core.Rete
{
    internal class EventAggregator : IEventSink, IEventSource
    {
        public event EventHandler<ActivationEventArgs> RuleActivatedEvent;

        public void Activate(Activation activation)
        {
            var args = new ActivationEventArgs(activation);
            if (RuleActivatedEvent != null)
            {
                RuleActivatedEvent(this, args);
            }
        }
    }
}