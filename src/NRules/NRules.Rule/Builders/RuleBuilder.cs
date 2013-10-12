using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Dsl;

namespace NRules.Rule.Builders
{
    public class RuleBuilder : RuleElementBuilder
    {
        private string _name;
        private int _priority; 
        private readonly SymbolTable _rootScope = new SymbolTable();
        private readonly GroupBuilder _groupBuilder;
        private readonly List<RuleAction> _actions = new List<RuleAction>();

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
            _actions.Add(new RuleAction(action));
        }

        public IRuleDefinition Build()
        {
            Validate();
            IRuleElementBuilder<GroupElement> builder = _groupBuilder;
            var ruleDefinition = new RuleDefinition { Name = _name, Priority = _priority };
            ruleDefinition.LeftHandSide = builder.Build();
            _actions.ForEach(ruleDefinition.AddAction);
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