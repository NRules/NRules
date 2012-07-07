using System;

namespace NRules.Core.Rete
{
    internal interface IEventSource
    {
        event EventHandler<ActivationEventArgs> RuleActivatedEvent;
    }
}