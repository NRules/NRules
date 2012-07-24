using System.Collections.Generic;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    internal interface IAgenda
    {
        ActivationQueue ActivationQueue { get; }
        void RegisterRule(CompiledRule rule);
        void Subscribe(IEventSource eventSource);
    }

    internal class Agenda : IAgenda
    {
        public ActivationQueue ActivationQueue { get; private set; }
        private readonly Dictionary<string, CompiledRule> _ruleMap = new Dictionary<string, CompiledRule>();

        public Agenda()
        {
            ActivationQueue = new ActivationQueue();
        }

        public void RegisterRule(CompiledRule rule)
        {
            _ruleMap.Add(rule.Handle, rule);
        }

        public void Subscribe(IEventSource eventSource)
        {
            eventSource.RuleActivatedEvent += OnRuleActivated;
            eventSource.RuleDeactivatedEvent += OnRuleDeactivated;
        }

        private void OnRuleActivated(object sender, ActivationEventArgs e)
        {
            ActivationQueue.Enqueue(e.Activation);
        }

        private void OnRuleDeactivated(object sender, ActivationEventArgs e)
        {
            ActivationQueue.Remove(e.Activation);
        }
    }
}