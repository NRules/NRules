using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Dsl;

namespace NRules.Rule.Builders
{
    public class RuleBuilder : RuleElementBuilder
    {
        private string _name;
        private int _priority = RuleDefinition.DefaultPriority; 
        private readonly SymbolTable _rootScope = new SymbolTable();
        private readonly GroupBuilder _groupBuilder;
        private readonly List<ActionElement> _actions = new List<ActionElement>();

        public RuleBuilder()
        {
            _groupBuilder = new GroupBuilder(_rootScope);
        }

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

        public void Action(Expression<Action<IActionContext>> action)
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