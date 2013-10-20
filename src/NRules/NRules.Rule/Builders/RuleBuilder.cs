using System;

namespace NRules.Rule.Builders
{
    public class RuleBuilder
    {
        private string _name;
        private int _priority = RuleDefinition.DefaultPriority;
        private readonly GroupBuilder _groupBuilder;
        private readonly ActionGroupBuilder _actionGroupBuilder;

        public RuleBuilder()
        {
            var rootScope = new SymbolTable();
            _groupBuilder = new GroupBuilder(rootScope, GroupType.And);
            _actionGroupBuilder = new ActionGroupBuilder(rootScope);
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

        public ActionGroupBuilder RightHandSide()
        {
            return _actionGroupBuilder;
        }

        public IRuleDefinition Build()
        {
            Validate();

            IBuilder<GroupElement> groupBuilder = _groupBuilder;
            GroupElement conditions = groupBuilder.Build();

            IBuilder<ActionGroupElement> actionBuilder = _actionGroupBuilder;
            ActionGroupElement actions = actionBuilder.Build();

            var ruleDefinition = new RuleDefinition(_name, _priority, conditions, actions);
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