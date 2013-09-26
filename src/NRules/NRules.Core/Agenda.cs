using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using NRules.Rule;

namespace NRules.Core
{
    internal interface IAgenda
    {
        bool HasActiveRules();
        RuleActivation NextActivation();
        void RegisterRule(IRuleDefinition ruleDefinition);
    }

    internal class Agenda : IAgenda
    {
        private readonly Dictionary<string, IRuleDefinition> _ruleMap;
        private readonly ActivationQueue _activationQueue;

        public Agenda(IEnumerable<IRuleDefinition> rules, IEventAggregator eventAggregator)
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
            IRuleDefinition ruleDefinition = _ruleMap[activation.RuleHandle];
            var ruleActivation = new RuleActivation(ruleDefinition, activation.Tuple);
            return ruleActivation;
        }

        public void RegisterRule(IRuleDefinition ruleDefinition)
        {
            _ruleMap.Add(ruleDefinition.Handle, ruleDefinition);
        }

        private void Subscribe(IEventAggregator eventAggregator)
        {
            eventAggregator.RuleActivatedEvent += OnRuleActivated;
            eventAggregator.RuleDeactivatedEvent += OnRuleDeactivated;
        }

        private void OnRuleActivated(object sender, ActivationEventArgs e)
        {
            IRuleDefinition ruleDefinition = _ruleMap[e.Activation.RuleHandle];
            _activationQueue.Enqueue(ruleDefinition.Priority, e.Activation);
        }

        private void OnRuleDeactivated(object sender, ActivationEventArgs e)
        {
            _activationQueue.Remove(e.Activation);
        }
    }
}