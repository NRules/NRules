using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Rule.Builders
{
    public class RuleBuilder
    {
        private string _name;
        private int _priority = RuleDefinition.DefaultPriority; 
        private readonly GroupBuilder _groupBuilder = new GroupBuilder();
        private readonly List<ActionElement> _actions = new List<ActionElement>();

        public void Name(string name)
        {
            _name = name;
        }

        public void Priority(int priority)
        {
            _priority = priority;
        }

        public GroupBuilder LeftHandSide()
        {
            return _groupBuilder;
        }

        public void Action(LambdaExpression action)
        {
            var actionElement = new ActionElement(action);
            _actions.Add(actionElement);
        }

        public IRuleDefinition Build()
        {
            Validate();

            IRuleElementBuilder<GroupElement> groupBuilder = _groupBuilder;
            var conditions = groupBuilder.Build();

            var ruleDefinition = new RuleDefinition(_name, _priority, conditions, _actions);
            return ruleDefinition;
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("Rule name not specified");
            }
        }
    }
}