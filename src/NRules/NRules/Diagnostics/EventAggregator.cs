using System;
using System.Linq.Expressions;
using NRules.AgendaFilters;
using NRules.Extensibility;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Provider of rules session events.
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
        /// Raised when an existing activation is updated.
        /// An activation is updated when a previously matching set of facts (tuple) is updated 
        /// and it still matches the rule.
        /// The activation is updated in the agenda and remains a candidate for firing.
        /// </summary>
        event EventHandler<AgendaEventArgs> ActivationUpdatedEvent;

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
        /// Raised before a new fact is inserted into working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactInsertingEvent;

        /// <summary>
        /// Raised after a new fact is inserted into working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactInsertedEvent;

        /// <summary>
        /// Raised before an existing fact is updated in the working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactUpdatingEvent;

        /// <summary>
        /// Raised after an existing fact is updated in the working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactUpdatedEvent;

        /// <summary>
        /// Raised before an existing fact is retracted from the working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactRetractingEvent;

        /// <summary>
        /// Raised after an existing fact is retracted from the working memory.
        /// </summary>
        event EventHandler<WorkingMemoryEventArgs> FactRetractedEvent;

        /// <summary>
        /// Raised when condition evaluation threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        event EventHandler<ConditionErrorEventArgs> ConditionFailedEvent;

        /// <summary>
        /// Raised when binding expression evaluation threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        event EventHandler<BindingErrorEventArgs> BindingFailedEvent;

        /// <summary>
        /// Raised when aggregate expression evaluation threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        event EventHandler<AggregateErrorEventArgs> AggregateFailedEvent;

        /// <summary>
        /// Raised when agenda filter execution threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        /// <seealso cref="IAgendaFilter"/>
        event EventHandler<AgendaErrorEventArgs> AgendaFilterFailedEvent;

        /// <summary>
        /// Raised when action execution threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        /// <remarks>This event is not raised when actions are invoked via <see cref="IActionInterceptor"/>.</remarks>
        /// <seealso cref="IActionInterceptor"/>
        event EventHandler<ActionErrorEventArgs> ActionFailedEvent;
    }

    internal interface IEventAggregator : IEventProvider
    {
        void RaiseActivationCreated(ISession session, Activation activation);
        void RaiseActivationUpdated(ISession session, Activation activation);
        void RaiseActivationDeleted(ISession session, Activation activation);
        void RaiseRuleFiring(ISession session, Activation activation);
        void RaiseRuleFired(ISession session, Activation activation);
        void RaiseFactInserting(ISession session, Fact fact);
        void RaiseFactInserted(ISession session, Fact fact);
        void RaiseFactUpdating(ISession session, Fact fact);
        void RaiseFactUpdated(ISession session, Fact fact);
        void RaiseFactRetracting(ISession session, Fact fact);
        void RaiseFactRetracted(ISession session, Fact fact);
        void RaiseConditionFailed(ISession session, Exception exception, Expression expression, Tuple tuple, Fact fact, ref bool isHandled);
        void RaiseBindingFailed(ISession session, Exception exception, Expression expression, Tuple tuple, ref bool isHandled);
        void RaiseAggregateFailed(ISession session, Exception exception, Expression expression, ITuple tuple, IFact fact, ref bool isHandled);
        void RaiseAgendaFilterFailed(ISession session, Exception exception, Expression expression, Activation activation, ref bool isHandled);
        void RaiseActionFailed(ISession session, Exception exception, Expression expression, Activation activation, ref bool isHandled);
    }

    internal class EventAggregator : IEventAggregator
    {
        private readonly IEventAggregator _parent;

        public event EventHandler<AgendaEventArgs> ActivationCreatedEvent;
        public event EventHandler<AgendaEventArgs> ActivationUpdatedEvent;
        public event EventHandler<AgendaEventArgs> ActivationDeletedEvent;
        public event EventHandler<AgendaEventArgs> RuleFiringEvent;
        public event EventHandler<AgendaEventArgs> RuleFiredEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactInsertingEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactInsertedEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactUpdatingEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactUpdatedEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactRetractingEvent;
        public event EventHandler<WorkingMemoryEventArgs> FactRetractedEvent;
        public event EventHandler<ConditionErrorEventArgs> ConditionFailedEvent;
        public event EventHandler<BindingErrorEventArgs> BindingFailedEvent;
        public event EventHandler<AggregateErrorEventArgs> AggregateFailedEvent;
        public event EventHandler<AgendaErrorEventArgs> AgendaFilterFailedEvent;
        public event EventHandler<ActionErrorEventArgs> ActionFailedEvent;

        public EventAggregator()
        {
        }

        public EventAggregator(IEventAggregator eventAggregator)
        {
            _parent = eventAggregator;
        }

        public void RaiseActivationCreated(ISession session, Activation activation)
        {
            var handler = ActivationCreatedEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation);
                handler(session, @event);
            }
            _parent?.RaiseActivationCreated(session, activation);
        }

        public void RaiseActivationUpdated(ISession session, Activation activation)
        {
            var handler = ActivationUpdatedEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation);
                handler(session, @event);
            }
            _parent?.RaiseActivationUpdated(session, activation);
        }

        public void RaiseActivationDeleted(ISession session, Activation activation)
        {
            var handler = ActivationDeletedEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation);
                handler(session, @event);
            }
            _parent?.RaiseActivationDeleted(session, activation);
        }

        public void RaiseRuleFiring(ISession session, Activation activation)
        {
            var handler = RuleFiringEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation);
                handler(session, @event);
            }
            _parent?.RaiseRuleFiring(session, activation);
        }

        public void RaiseRuleFired(ISession session, Activation activation)
        {
            var handler = RuleFiredEvent;
            if (handler != null)
            {
                var @event = new AgendaEventArgs(activation);
                handler(session, @event);
            }
            _parent?.RaiseRuleFired(session, activation);
        }

        public void RaiseFactInserting(ISession session, Fact fact)
        {
            var handler = FactInsertingEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
            _parent?.RaiseFactInserting(session, fact);
        }

        public void RaiseFactInserted(ISession session, Fact fact)
        {
            var handler = FactInsertedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
            _parent?.RaiseFactInserted(session, fact);
        }

        public void RaiseFactUpdating(ISession session, Fact fact)
        {
            var handler = FactUpdatingEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
            _parent?.RaiseFactUpdating(session, fact);
        }

        public void RaiseFactUpdated(ISession session, Fact fact)
        {
            var handler = FactUpdatedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
            _parent?.RaiseFactUpdated(session, fact);
        }

        public void RaiseFactRetracting(ISession session, Fact fact)
        {
            var handler = FactRetractingEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
            _parent?.RaiseFactRetracting(session, fact);
        }

        public void RaiseFactRetracted(ISession session, Fact fact)
        {
            var handler = FactRetractedEvent;
            if (handler != null)
            {
                var @event = new WorkingMemoryEventArgs(fact);
                handler(session, @event);
            }
            _parent?.RaiseFactRetracted(session, fact);
        }

        public void RaiseConditionFailed(ISession session, Exception exception, Expression expression, Tuple tuple, Fact fact, ref bool isHandled)
        {
            var hanlder = ConditionFailedEvent;
            if (hanlder != null)
            {
                var @event = new ConditionErrorEventArgs(exception, expression, tuple, fact);
                hanlder(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseConditionFailed(session, exception, expression, tuple, fact, ref isHandled);
        }

        public void RaiseBindingFailed(ISession session, Exception exception, Expression expression, Tuple tuple, ref bool isHandled)
        {
            var hanlder = BindingFailedEvent;
            if (hanlder != null)
            {
                var @event = new BindingErrorEventArgs(exception, expression, tuple);
                hanlder(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseBindingFailed(session, exception, expression, tuple, ref isHandled);
        }

        public void RaiseAggregateFailed(ISession session, Exception exception, Expression expression, ITuple tuple, IFact fact, ref bool isHandled)
        {
            var hanlder = AggregateFailedEvent;
            if (hanlder != null)
            {
                var @event = new AggregateErrorEventArgs(exception, expression, tuple, fact);
                hanlder(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseAggregateFailed(session, exception, expression, tuple, fact, ref isHandled);
        }

        public void RaiseAgendaFilterFailed(ISession session, Exception exception, Expression expression, Activation activation, ref bool isHandled)
        {
            var handler = AgendaFilterFailedEvent;
            if (handler != null)
            {
                var @event = new AgendaErrorEventArgs(exception, expression, activation);
                handler(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseAgendaFilterFailed(session, exception, expression, activation, ref isHandled);
        }

        public void RaiseActionFailed(ISession session, Exception exception, Expression expression, Activation activation, ref bool isHandled)
        {
            var handler = ActionFailedEvent;
            if (handler != null)
            {
                var @event = new ActionErrorEventArgs(exception, expression, activation);
                handler(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseActionFailed(session, exception, expression, activation, ref isHandled);
        }
    }
}