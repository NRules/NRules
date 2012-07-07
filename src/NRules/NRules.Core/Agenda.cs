using System.Collections.Generic;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    internal class Agenda
    {
        public Queue<Activation> ActivationQueue { get; private set; }
        private readonly Dictionary<string, Rule> _ruleMap = new Dictionary<string, Rule>();

        public Agenda()
        {
            ActivationQueue = new Queue<Activation>();
        }

        public void RegisterRule(Rule rule)
        {
            _ruleMap.Add(rule.Handle, rule);
        }

        internal void Subscribe(IEventSource eventSource)
        {
            eventSource.RuleActivatedEvent += OnRuleActivated;
        }

        private void OnRuleActivated(object sender, ActivationEventArgs e)
        {
            ActivationQueue.Enqueue(e.Activation);
        }
    }
}