using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    internal interface IAgenda
    {
        bool HasActiveRules();
        RuleActivation NextActivation();
        void RegisterRule(CompiledRule rule);
    }

    internal class Agenda : IAgenda
    {
        private readonly Dictionary<string, CompiledRule> _ruleMap;
        private readonly ActivationQueue _activationQueue;

        public Agenda(IEnumerable<CompiledRule> rules, IEventAggregator eventAggregator)
        {
            _activationQueue = new ActivationQueue();
            _ruleMap = rules.ToDictionary(r => r.Handle);
            Subscribe(eventAggregator);
        }

        public bool HasActiveRules()
        {
            return _activationQueue.HasActive();
        }

        public RuleActivation NextActivation()
        {
            Activation activation = _activationQueue.Dequeue();
            CompiledRule rule = _ruleMap[activation.RuleHandle];
            var ruleActivation = new RuleActivation(rule, activation.Tuple);
            return ruleActivation;
        }

        public void RegisterRule(CompiledRule rule)
        {
            _ruleMap.Add(rule.Handle, rule);
        }

        private void Subscribe(IEventAggregator eventAggregator)
        {
            eventAggregator.RuleActivatedEvent += OnRuleActivated;
            eventAggregator.RuleDeactivatedEvent += OnRuleDeactivated;
        }

        private void OnRuleActivated(object sender, ActivationEventArgs e)
        {
            CompiledRule rule = _ruleMap[e.Activation.RuleHandle];
            _activationQueue.Enqueue(rule.Priority, e.Activation);
        }

        private void OnRuleDeactivated(object sender, ActivationEventArgs e)
        {
            _activationQueue.Remove(e.Activation);
        }
    }
}