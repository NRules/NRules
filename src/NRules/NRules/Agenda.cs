using System.Collections.Generic;
using System.Linq;
using NRules.AgendaFilters;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules
{
    /// <summary>
    /// Agenda stores matches between rules and facts. These matches are called activations.
    /// Multiple activations are ordered according to the conflict resolution strategy.
    /// </summary>
    /// <seealso cref="IMatch"/>
    /// <seealso cref="IAgendaFilter"/>
    public interface IAgenda
    {
        /// <summary>
        /// Indicates whether there are any activations in the agenda.
        /// </summary>
        /// <value>If agenda is empty then <c>true</c> otherwise <c>false</c>.</value>
        bool IsEmpty { get; }

        /// <summary>
        /// Retrieves the next match, without removing it from agenda.
        /// </summary>
        /// <remarks>Throws <c>InvalidOperationException</c> if agenda is empty.</remarks>
        /// <returns>Next match.</returns>
        IMatch Peek();

        /// <summary>
        /// Removes all matches from agenda.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds a global filter to the agenda.
        /// </summary>
        /// <param name="filter">Filter to be applied to all activations before they are placed on the agenda.</param>
        void AddFilter(IAgendaFilter filter);

        /// <summary>
        /// Adds a rule-level filter to the agenda.
        /// </summary>
        /// <param name="rule">Rule, whose activations are to be filtered before placing them on the agenda.</param>
        /// <param name="filter">Filter to be applied to all activations for a given rule before they are placed on the agenda.</param>
        void AddFilter(IRuleDefinition rule, IAgendaFilter filter);
    }

    internal interface IAgendaInternal : IAgenda
    {
        Activation Pop();
        void Add(IExecutionContext context, Activation activation);
        void Modify(IExecutionContext context, Activation activation);
        void Remove(IExecutionContext context, Activation activation);
    }

    internal class Agenda : IAgendaInternal
    {
        private readonly ActivationQueue _activationQueue = new ActivationQueue();
        private readonly List<IAgendaFilter> _globalFilters = new List<IAgendaFilter>();
        private readonly Dictionary<IRuleDefinition, List<IAgendaFilter>> _ruleFilters = new Dictionary<IRuleDefinition, List<IAgendaFilter>>();

        public bool IsEmpty => !_activationQueue.HasActive();

        public IMatch Peek()
        {
            Activation activation = _activationQueue.Peek();
            return activation;
        }

        public void Clear()
        {
            _activationQueue.Clear();
        }

        public void AddFilter(IAgendaFilter filter)
        {
            _globalFilters.Add(filter);
        }

        public void AddFilter(IRuleDefinition rule, IAgendaFilter filter)
        {
            if (!_ruleFilters.TryGetValue(rule, out var filters))
            {
                filters = new List<IAgendaFilter>();
                _ruleFilters.Add(rule, filters);
            }
            filters.Add(filter);
        }

        public Activation Pop()
        {
            Activation activation = _activationQueue.Dequeue();
            activation.RuleFiring();
            return activation;
        }

        public void Add(IExecutionContext context, Activation activation)
        {
            activation.Insert();
            if (Accept(context, activation))
            {
                _activationQueue.Enqueue(activation.CompiledRule.Priority, activation);
            }
            else
            {
                _activationQueue.Remove(activation);
            }
        }

        public void Modify(IExecutionContext context, Activation activation)
        {
            if (activation.CompiledRule.Repeatability == RuleRepeatability.NonRepeatable &&
                activation.HasFired)
            {
                return;
            }

            activation.Update();
            if (Accept(context, activation))
            {
                _activationQueue.Enqueue(activation.CompiledRule.Priority, activation);
            }
            else
            {
                _activationQueue.Remove(activation);
            }
        }

        public void Remove(IExecutionContext context, Activation activation)
        {
            activation.Remove();
            if (activation.Trigger.Matches(activation.CompiledRule.ActionTriggers) &&
                activation.HasFired)
            {
                _activationQueue.Enqueue(activation.CompiledRule.Priority, activation);
            }
            else
            {
                _activationQueue.Remove(activation);
            }

            if (context.Session.GetLinkedKeys(activation).Any())
            {
                context.UnlinkQueue.Enqueue(activation);
            }
        }

        private bool Accept(IExecutionContext context, Activation activation)
        {
            var agendaContext = new AgendaContext(context.Session, context.EventAggregator);
            try
            {
                return AcceptActivation(agendaContext, activation);
            }
            catch (ExpressionEvaluationException e)
            {
                if (!e.IsHandled)
                {
                    throw new AgendaExpressionEvaluationException("Failed to evaluate agenda filter expression",
                        activation.Rule.Name, e.Expression.ToString(), e.InnerException);
                }
                return false;
            }
        }

        private bool AcceptActivation(AgendaContext context, Activation activation)
        {
            foreach (var filter in _globalFilters)
            {
                if (!filter.Accept(context, activation)) return false;
            }
            if (!_ruleFilters.TryGetValue(activation.Rule, out var ruleFilters)) return true;
            foreach (var filter in ruleFilters)
            {
                if (!filter.Accept(context, activation)) return false;
            }
            return true;
        }
    }
}