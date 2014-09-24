using System;
using NRules.Exceptions;
using NRules.Rete;

namespace NRules.Events
{
    /// <summary>
    /// Aggregator of rules session events.
    /// </summary>
    public interface IEventProvider
    {
        /// <summary>
        /// Raised when a new rule activation is created.
        /// A new activation is created when a new set of facts (tuple) matches a rule.
        /// The activation is placed on the agenda and becomes a candidate for firing.
        /// </summary>
        event EventHandler<AgendaEventArgs> ActivationCreatedEvent;

        /// <summary>
        /// Raised when an existing activation is deleted.
        /// An activation is deleted when a previously matching set of facts (tuple) no longer 
        /// matches the rule due to updated or retracted facts.
        /// The activation is removed from the agenda and is no longer a candidate for firing.
        /// </summary>
        event EventHandler<AgendaEventArgs> ActivationDeletedEvent;

        /// <summary>
        /// Raised before a rule is about to fire.
        /// </summary>
        event EventHandler<AgendaEventArgs> RuleFiringEvent;

        /// <summary>
        /// Raised after a rule has fired and all its actions executed.
        /// </summary>
        event EventHandler<AgendaEventArgs> RuleFiredEvent;

        /// <summary>
        /// Raised when a new fact is inserted into working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactInsertedEvent;

        /// <summary>
        /// Raised when an existing fact is updated in the working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactUpdatedEvent;

        /// <summary>
        /// Raised when an existing fact is retracted from the working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactRetractedEvent;

        /// <summary>
        /// Raised when action threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        event EventHandler<ActionErrorEventArgs> ActionFailedEvent;
    }

    internal interface IEventAggregator : IEventProvider
    {
        void ActivationCreated(Activation activation);
        void ActivationDeleted(Activation activation);
        void RuleFiring(Activation activation);
        void RuleFired(Activation activation);
        void FactInserted(Fact fact);
        void FactUpdated(Fact fact);
        void FactRetracted(Fact fact);
        void ActionFailed(ActionEvaluationException exception, out bool isHandled);
    }

    internal class EventAggregator : IEventAggregator
    {
        public event EventHandler<AgendaEventArgs> ActivationCreatedEvent;
        public event EventHandler<AgendaEventArgs> ActivationDeletedEvent;
        public event EventHandler<AgendaEventArgs> RuleFiringEvent;
        public event EventHandler<AgendaEventArgs> RuleFiredEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactInsertedEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactUpdatedEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactRetractedEvent;
        public event EventHandler<ActionErrorEventArgs> ActionFailedEvent;

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

        public void RuleFiring(Activation activation)
        {
            var handler = RuleFiringEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(this, @event);
            }
        }

        public void RuleFired(Activation activation)
        {
            var handler = RuleFiredEvent;
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

        public void ActionFailed(ActionEvaluationException exception, out bool isHandled)
        {
            isHandled = false;
            var handler = ActionFailedEvent;
            if (handler != null)
            {
                var @event = new ActionErrorEventArgs(exception);
                handler(this, @event);
                isHandled = @event.IsHandled;
            }            
        }
    }
}