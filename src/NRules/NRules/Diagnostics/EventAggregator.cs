using System;
using System.Linq.Expressions;
using NRules.AgendaFilters;
using NRules.Aggregators;
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
        /// <seealso cref="IAggregator"/>
        event EventHandler<AggregateErrorEventArgs> AggregateFailedEvent;

        /// <summary>
        /// Raised when agenda filter execution threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        /// <seealso cref="IAgendaFilter"/>
        event EventHandler<AgendaFilterErrorEventArgs> AgendaFilterFailedEvent;

        /// <summary>
        /// Raised when action execution threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        /// <remarks>This event is not raised when actions are invoked via <see cref="IActionInterceptor"/>.</remarks>
        /// <seealso cref="IActionInterceptor"/>
        event EventHandler<ActionErrorEventArgs> ActionFailedEvent;

        /// <summary>
        /// Raised when condition expression is evaluated.
        /// </summary>
        event EventHandler<ConditionEventArgs> ConditionEvaluatedEvent;

        /// <summary>
        /// Raised when binding expression is evaluated.
        /// </summary>
        event EventHandler<BindingEventArgs> BindingEvaluatedEvent;

        /// <summary>
        /// Raised when aggregate expression is evaluated.
        /// </summary>
        /// <seealso cref="IAggregator"/>
        event EventHandler<AggregateEventArgs> AggregateEvaluatedEvent;

        /// <summary>
        /// Raised when agenda filter expression is evaluated.
        /// </summary>
        /// <seealso cref="IAgendaFilter"/>
        event EventHandler<AgendaFilterEventArgs> AgendaFilterEvaluatedEvent;

        /// <summary>
        /// Raised when action expression is evaluated.
        /// </summary>
        event EventHandler<ActionEventArgs> ActionEvaluatedEvent;
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
        void RaiseConditionFailed(ISession session, Exception exception, Expression expression, object argument, Fact fact, ref bool isHandled);
        void RaiseConditionFailed(ISession session, Exception exception, Expression expression, object[] arguments, Tuple tuple, Fact fact, ref bool isHandled);
        void RaiseConditionEvaluated(ISession session, Exception exception, Expression expression, object argument, object result, Fact fact);
        void RaiseConditionEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, object result, Tuple tuple, Fact fact);
        void RaiseBindingFailed(ISession session, Exception exception, Expression expression, object[] arguments, Tuple tuple, ref bool isHandled);
        void RaiseBindingEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, object result, Tuple tuple);
        void RaiseAggregateFailed(ISession session, Exception exception, Expression expression, object argument, ITuple tuple, IFact fact, ref bool isHandled);
        void RaiseAggregateFailed(ISession session, Exception exception, Expression expression, object[] arguments, ITuple tuple, IFact fact, ref bool isHandled);
        void RaiseAggregateEvaluated(ISession session, Exception exception, Expression expression, object argument, object result, ITuple tuple, IFact fact);
        void RaiseAggregateEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, object result, ITuple tuple, IFact fact);
        void RaiseAgendaFilterFailed(ISession session, Exception exception, Expression expression, object[] arguments, Activation activation, ref bool isHandled);
        void RaiseAgendaFilterEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, object result, Activation activation);
        void RaiseActionFailed(ISession session, Exception exception, Expression expression, object[] arguments, Activation activation, ref bool isHandled);
        void RaiseActionEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, Activation activation);
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
        public event EventHandler<AgendaFilterErrorEventArgs> AgendaFilterFailedEvent;
        public event EventHandler<ActionErrorEventArgs> ActionFailedEvent;
        public event EventHandler<ConditionEventArgs> ConditionEvaluatedEvent;
        public event EventHandler<BindingEventArgs> BindingEvaluatedEvent;
        public event EventHandler<AggregateEventArgs> AggregateEvaluatedEvent;
        public event EventHandler<AgendaFilterEventArgs> AgendaFilterEvaluatedEvent;
        public event EventHandler<ActionEventArgs> ActionEvaluatedEvent;

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

        public void RaiseConditionFailed(ISession session, Exception exception, Expression expression, object argument, Fact fact, ref bool isHandled)
        {
            var hanlder = ConditionFailedEvent;
            if (hanlder != null)
            {
                var @event = new ConditionErrorEventArgs(expression, exception, argument, fact);
                hanlder(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseConditionFailed(session, exception, expression, argument, fact, ref isHandled);
        }

        public void RaiseConditionFailed(ISession session, Exception exception, Expression expression, object[] arguments, Tuple tuple, Fact fact, ref bool isHandled)
        {
            var hanlder = ConditionFailedEvent;
            if (hanlder != null)
            {
                var @event = new ConditionErrorEventArgs(expression, exception, arguments, tuple, fact);
                hanlder(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseConditionFailed(session, exception, expression, arguments, tuple, fact, ref isHandled);
        }

        public void RaiseConditionEvaluated(ISession session, Exception exception, Expression expression, object argument, object result, Fact fact)
        {
            var hanlder = ConditionEvaluatedEvent;
            if (hanlder != null)
            {
                var @event = new ConditionEventArgs(expression, exception, argument, result, fact);
                hanlder(session, @event);
            }
            _parent?.RaiseConditionEvaluated(session, exception, expression, argument, result, fact);
        }

        public void RaiseConditionEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, object result, Tuple tuple, Fact fact)
        {
            var hanlder = ConditionEvaluatedEvent;
            if (hanlder != null)
            {
                var @event = new ConditionEventArgs(expression, exception, arguments, result, tuple, fact);
                hanlder(session, @event);
            }
            _parent?.RaiseConditionEvaluated(session, exception, expression, arguments, result, tuple, fact);
        }

        public void RaiseBindingFailed(ISession session, Exception exception, Expression expression, object[] arguments, Tuple tuple, ref bool isHandled)
        {
            var hanlder = BindingFailedEvent;
            if (hanlder != null)
            {
                var @event = new BindingErrorEventArgs(expression, exception, arguments, tuple);
                hanlder(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseBindingFailed(session, exception, expression, arguments, tuple, ref isHandled);
        }

        public void RaiseBindingEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, object result, Tuple tuple)
        {
            var hanlder = BindingEvaluatedEvent;
            if (hanlder != null)
            {
                var @event = new BindingEventArgs(expression, exception, arguments, result, tuple);
                hanlder(session, @event);
            }
            _parent?.RaiseBindingEvaluated(session, exception, expression, arguments, result, tuple);
        }

        public void RaiseAggregateFailed(ISession session, Exception exception, Expression expression, object argument, ITuple tuple, IFact fact, ref bool isHandled)
        {
            var hanlder = AggregateFailedEvent;
            if (hanlder != null)
            {
                var @event = new AggregateErrorEventArgs(expression, exception, argument, tuple, fact);
                hanlder(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseAggregateFailed(session, exception, expression, argument, tuple, fact, ref isHandled);
        }

        public void RaiseAggregateFailed(ISession session, Exception exception, Expression expression, object[] arguments, ITuple tuple, IFact fact, ref bool isHandled)
        {
            var hanlder = AggregateFailedEvent;
            if (hanlder != null)
            {
                var @event = new AggregateErrorEventArgs(expression, exception, arguments, tuple, fact);
                hanlder(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseAggregateFailed(session, exception, expression, arguments, tuple, fact, ref isHandled);
        }

        public void RaiseAggregateEvaluated(ISession session, Exception exception, Expression expression, object argument, object result, ITuple tuple, IFact fact)
        {
            var hanlder = AggregateEvaluatedEvent;
            if (hanlder != null)
            {
                var @event = new AggregateEventArgs(expression, exception, argument, result, tuple, fact);
                hanlder(session, @event);
            }
            _parent?.RaiseAggregateEvaluated(session, exception, expression, argument, result, tuple, fact);
        }

        public void RaiseAggregateEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, object result, ITuple tuple, IFact fact)
        {
            var hanlder = AggregateEvaluatedEvent;
            if (hanlder != null)
            {
                var @event = new AggregateEventArgs(expression, exception, arguments, result, tuple, fact);
                hanlder(session, @event);
            }
            _parent?.RaiseAggregateEvaluated(session, exception, expression, arguments, result, tuple, fact);
        }

        public void RaiseAgendaFilterFailed(ISession session, Exception exception, Expression expression, object[] arguments, Activation activation, ref bool isHandled)
        {
            var handler = AgendaFilterFailedEvent;
            if (handler != null)
            {
                var @event = new AgendaFilterErrorEventArgs(expression, exception, arguments, activation);
                handler(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseAgendaFilterFailed(session, exception, expression, arguments, activation, ref isHandled);
        }

        public void RaiseAgendaFilterEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, object result, Activation activation)
        {
            var hanlder = AgendaFilterEvaluatedEvent;
            if (hanlder != null)
            {
                var @event = new AgendaFilterEventArgs(expression, exception, arguments, result, activation);
                hanlder(session, @event);
            }
            _parent?.RaiseAgendaFilterEvaluated(session, exception, expression, arguments, result, activation);
        }

        public void RaiseActionFailed(ISession session, Exception exception, Expression expression, object[] arguments, Activation activation, ref bool isHandled)
        {
            var handler = ActionFailedEvent;
            if (handler != null)
            {
                var @event = new ActionErrorEventArgs(expression, exception, arguments, activation);
                handler(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseActionFailed(session, exception, expression, arguments, activation, ref isHandled);
        }

        public void RaiseActionEvaluated(ISession session, Exception exception, Expression expression, object[] arguments, Activation activation)
        {
            var hanlder = ActionEvaluatedEvent;
            if (hanlder != null)
            {
                var @event = new ActionEventArgs(expression, exception, arguments, activation);
                hanlder(session, @event);
            }
            _parent?.RaiseActionEvaluated(session, exception, expression, arguments, activation);
        }
    }
}