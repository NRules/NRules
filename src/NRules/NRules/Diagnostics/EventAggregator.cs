using System;
using System.Linq.Expressions;
using System.Threading;
using NRules.AgendaFilters;
using NRules.Extensibility;
using NRules.Rete;
using NRules.Utilities;
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
        /// Raised when left-hand side expression is evaluated.
        /// This event is raised on both, successful expression evaluations, and on exceptions.
        /// </summary>
        event EventHandler<LhsExpressionEventArgs> LhsExpressionEvaluatedEvent;

        /// <summary>
        /// Raised when left-hand side expression evaluation threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        event EventHandler<LhsExpressionErrorEventArgs> LhsExpressionFailedEvent;

        /// <summary>
        /// Raised when agenda expression is evaluated.
        /// This event is raised on both, successful expression evaluations, and on exceptions.
        /// </summary>
        /// <seealso cref="IAgendaFilter"/>
        event EventHandler<AgendaExpressionEventArgs> AgendaExpressionEvaluatedEvent;

        /// <summary>
        /// Raised when agenda expression evaluation threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        /// <seealso cref="IAgendaFilter"/>
        event EventHandler<AgendaExpressionErrorEventArgs> AgendaExpressionFailedEvent;

        /// <summary>
        /// Raised when right-hand side expression is evaluated.
        /// This event is raised on both, successful expression evaluations, and on exceptions.
        /// </summary>
        /// <seealso cref="IActionInterceptor"/>
        event EventHandler<RhsExpressionEventArgs> RhsExpressionEvaluatedEvent;

        /// <summary>
        /// Raised when right-hand side expression evaluation threw an exception.
        /// Gives observer of the event control over handling of the exception.
        /// </summary>
        /// <seealso cref="IActionInterceptor"/>
        event EventHandler<RhsExpressionErrorEventArgs> RhsExpressionFailedEvent;
    }

    internal interface IEventAggregator : IEventProvider
    {
        bool TraceEnabled { get; }
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
        void RaiseLhsExpressionFailed(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, Tuple tuple, Fact fact, NodeInfo nodeInfo, ref bool isHandled);
        void RaiseLhsExpressionEvaluated(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, object result, Tuple tuple, Fact fact, NodeInfo nodeInfo);
        void RaiseAgendaExpressionFailed(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, Activation activation, ref bool isHandled);
        void RaiseAgendaExpressionEvaluated(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, object result, Activation activation);
        void RaiseRhsExpressionFailed(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, Activation activation, ref bool isHandled);
        void RaiseRhsExpressionEvaluated(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, Activation activation);
    }

    internal class EventAggregator : IEventAggregator
    {
        private readonly IEventAggregator _parent;
        private volatile int _traceSubscriberCount = 0;

        private event EventHandler<LhsExpressionEventArgs> LhsExpressionEvaluatedEvent;
        private event EventHandler<AgendaExpressionEventArgs> AgendaExpressionEvaluatedEvent;
        private event EventHandler<RhsExpressionEventArgs> RhsExpressionEvaluatedEvent;

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
        public event EventHandler<LhsExpressionErrorEventArgs> LhsExpressionFailedEvent;
        public event EventHandler<AgendaExpressionErrorEventArgs> AgendaExpressionFailedEvent;
        public event EventHandler<RhsExpressionErrorEventArgs> RhsExpressionFailedEvent;

        event EventHandler<LhsExpressionEventArgs> IEventProvider.LhsExpressionEvaluatedEvent
        {
            add
            {
                Interlocked.Increment(ref _traceSubscriberCount);
                LhsExpressionEvaluatedEvent += value;
            }
            remove
            {
                LhsExpressionEvaluatedEvent -= value;
                Interlocked.Decrement(ref _traceSubscriberCount);
            }
        }

        event EventHandler<AgendaExpressionEventArgs> IEventProvider.AgendaExpressionEvaluatedEvent
        {
            add
            {
                Interlocked.Increment(ref _traceSubscriberCount);
                AgendaExpressionEvaluatedEvent += value;
            }
            remove
            {
                AgendaExpressionEvaluatedEvent -= value;
                Interlocked.Decrement(ref _traceSubscriberCount);
            }
        }

        event EventHandler<RhsExpressionEventArgs> IEventProvider.RhsExpressionEvaluatedEvent
        {
            add
            {
                Interlocked.Increment(ref _traceSubscriberCount);
                RhsExpressionEvaluatedEvent += value;
            }
            remove
            {
                RhsExpressionEvaluatedEvent -= value;
                Interlocked.Decrement(ref _traceSubscriberCount);
            }
        }

        public EventAggregator()
        {
        }

        public EventAggregator(IEventAggregator eventAggregator)
        {
            _parent = eventAggregator;
        }

        public bool TraceEnabled => _traceSubscriberCount > 0 || (_parent != null && _parent.TraceEnabled);

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

        public void RaiseLhsExpressionEvaluated(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, object result, Tuple tuple, Fact fact, NodeInfo nodeInfo)
        {
            var handler = LhsExpressionEvaluatedEvent;
            if (handler != null)
            {
                var arguments = new LhsExpressionArguments(argumentMap, tuple, fact);
                var @event = new LhsExpressionEventArgs(expression, exception, arguments, result, tuple, fact, nodeInfo.Rules);
                handler(session, @event);
            }
            _parent?.RaiseLhsExpressionEvaluated(session, exception, expression, argumentMap, result, tuple, fact, nodeInfo);
        }

        public void RaiseLhsExpressionFailed(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, Tuple tuple, Fact fact, NodeInfo nodeInfo, ref bool isHandled)
        {
            var handler = LhsExpressionFailedEvent;
            if (handler != null)
            {
                var arguments = new LhsExpressionArguments(argumentMap, tuple, fact);
                var @event = new LhsExpressionErrorEventArgs(expression, exception, arguments, tuple, fact, nodeInfo.Rules);
                handler(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseLhsExpressionFailed(session, exception, expression, argumentMap, tuple, fact, nodeInfo, ref isHandled);
        }

        public void RaiseAgendaExpressionEvaluated(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, object result, Activation activation)
        {
            var handler = AgendaExpressionEvaluatedEvent;
            if (handler != null)
            {
                var arguments = new ActivationExpressionArguments(argumentMap, activation);
                var @event = new AgendaExpressionEventArgs(expression, exception, arguments, result, activation);
                handler(session, @event);
            }
            _parent?.RaiseAgendaExpressionEvaluated(session, exception, expression, argumentMap, result, activation);
        }

        public void RaiseAgendaExpressionFailed(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, Activation activation, ref bool isHandled)
        {
            var handler = AgendaExpressionFailedEvent;
            if (handler != null)
            {
                var arguments = new ActivationExpressionArguments(argumentMap, activation);
                var @event = new AgendaExpressionErrorEventArgs(expression, exception, arguments, activation);
                handler(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseAgendaExpressionFailed(session, exception, expression, argumentMap, activation, ref isHandled);
        }
        
        public void RaiseRhsExpressionEvaluated(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, Activation activation)
        {
            var handler = RhsExpressionEvaluatedEvent;
            if (handler != null)
            {
                var arguments = new ActivationExpressionArguments(argumentMap, activation);
                var @event = new RhsExpressionEventArgs(expression, exception, arguments, activation);
                handler(session, @event);
            }
            _parent?.RaiseRhsExpressionEvaluated(session, exception, expression, argumentMap, activation);
        }

        public void RaiseRhsExpressionFailed(ISession session, Exception exception, Expression expression, IArgumentMap argumentMap, Activation activation, ref bool isHandled)
        {
            var handler = RhsExpressionFailedEvent;
            if (handler != null)
            {
                var arguments = new ActivationExpressionArguments(argumentMap, activation);
                var @event = new RhsExpressionErrorEventArgs(expression, exception, arguments, activation);
                handler(session, @event);
                isHandled |= @event.IsHandled;
            }
            _parent?.RaiseRhsExpressionFailed(session, exception, expression, argumentMap, activation, ref isHandled);
        }
    }
}