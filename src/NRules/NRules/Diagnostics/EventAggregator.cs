using System;
using System.Linq.Expressions;
using NRules.Rete;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Diagnostics
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
        /// Raised when action execution threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        event EventHandler<ActionErrorEventArgs> ActionFailedEvent;

        /// <summary>
        /// Raised when condition evaluation threw an exception.
        /// </summary>
        event EventHandler<ConditionErrorEventArgs> ConditionFailedEvent;
    }

    internal interface IEventAggregator : IEventProvider
    {
        void RaiseActivationCreated(ISession session, Activation activation);
        void RaiseActivationDeleted(ISession session, Activation activation);
        void RaiseRuleFiring(ISession session, Activation activation);
        void RaiseRuleFired(ISession session, Activation activation);
        void RaiseFactInserted(ISession session, Fact fact);
        void RaiseFactUpdated(ISession session, Fact fact);
        void RaiseFactRetracted(ISession session, Fact fact);
        void RaiseActionFailed(ISession session, Exception exception, Expression expression, Tuple tuple, out bool isHandled);
        void RaiseConditionFailed(ISession session, Exception exception, Expression expression, Tuple tuple, Fact fact);
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
        public event EventHandler<ConditionErrorEventArgs> ConditionFailedEvent;

        public void RaiseActivationCreated(ISession session, Activation activation)
        {
            var handler = ActivationCreatedEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(session, @event);
            }
        }

        public void RaiseActivationDeleted(ISession session, Activation activation)
        {
            var handler = ActivationDeletedEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(session, @event);
            }
        }

        public void RaiseRuleFiring(ISession session, Activation activation)
        {
            var handler = RuleFiringEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(session, @event);
            }
        }

        public void RaiseRuleFired(ISession session, Activation activation)
        {
            var handler = RuleFiredEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation.Rule, activation.Tuple);
                handler(session, @event);
            }
        }

        public void RaiseFactInserted(ISession session, Fact fact)
        {
            var handler = FactInsertedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
        }

        public void RaiseFactUpdated(ISession session, Fact fact)
        {
            var handler = FactUpdatedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
        }

        public void RaiseFactRetracted(ISession session, Fact fact)
        {
            var handler = FactRetractedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
        }

        public void RaiseActionFailed(ISession session, Exception exception, Expression expression, Tuple tuple, out bool isHandled)
        {
            isHandled = false;
            var handler = ActionFailedEvent;
            if (handler != null)
            {
                var @event = new ActionErrorEventArgs(exception, expression, tuple);
                handler(session, @event);
                isHandled = @event.IsHandled;
            }            
        }

        public void RaiseConditionFailed(ISession session, Exception exception, Expression expression, Tuple tuple, Fact fact)
        {
            var hanlder = ConditionFailedEvent;
            if (hanlder != null)
            {
                var @event = new ConditionErrorEventArgs(exception, expression, tuple, fact);
                hanlder(session, @event);
            }
        }
    }
}