using System;
using NRules.Rete;

namespace NRules.Events
{
    public interface IEventProvider
    {
        event EventHandler<AgendaEventArgs> ActivationCreatedEvent;
        event EventHandler<AgendaEventArgs> ActivationDeletedEvent;
        event EventHandler<AgendaEventArgs> BeforeRuleFiredEvent;
        event EventHandler<AgendaEventArgs> AfterRuleFiredEvent;
        event EventHandler<WorkingMemoryEventArgs> FactInsertedEvent;
        event EventHandler<WorkingMemoryEventArgs> FactUpdatedEvent;
        event EventHandler<WorkingMemoryEventArgs> FactRetractedEvent;
    }

    internal interface IEventAggregator : IEventProvider
    {
        void ActivationCreated(Activation activation);
        void ActivationDeleted(Activation activation);
        void BeforeRuleFired(Activation activation);
        void AfterRuleFired(Activation activation);
        void FactInserted(Fact fact);
        void FactUpdated(Fact fact);
        void FactRetracted(Fact fact);
    }

    internal class EventAggregator : IEventAggregator
    {
        public event EventHandler<AgendaEventArgs> ActivationCreatedEvent;
        public event EventHandler<AgendaEventArgs> ActivationDeletedEvent;
        public event EventHandler<AgendaEventArgs> BeforeRuleFiredEvent;
        public event EventHandler<AgendaEventArgs> AfterRuleFiredEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactInsertedEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactUpdatedEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactRetractedEvent;

        public void ActivationCreated(Activation activation)
        {
            var handler = ActivationCreatedEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(this, @event);
            }
        }

        public void ActivationDeleted(Activation activation)
        {
            var handler = ActivationDeletedEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(this, @event);
            }
        }

        public void BeforeRuleFired(Activation activation)
        {
            var handler = BeforeRuleFiredEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(this, @event);
            }
        }

        public void AfterRuleFired(Activation activation)
        {
            var handler = AfterRuleFiredEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(this, @event);
            }
        }

        public void FactInserted(Fact fact)
        {
            var handler = FactInsertedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(this, @event);
            }
        }

        public void FactUpdated(Fact fact)
        {
            var handler = FactUpdatedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(this, @event);
            }
        }

        public void FactRetracted(Fact fact)
        {
            var handler = FactRetractedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(this, @event);
            }
        }
    }
}