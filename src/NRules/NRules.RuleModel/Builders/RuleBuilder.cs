using System;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a rule definition.
    /// </summary>
    public class RuleBuilder
    {
        private string _name;
        private int _priority = RuleDefinition.DefaultPriority;
        private readonly GroupBuilder _groupBuilder;
        private readonly ActionGroupBuilder _actionGroupBuilder;

        /// <summary>
        /// Constructs an empty rule builder.
        /// </summary>
        public RuleBuilder()
        {
            var rootScope = new SymbolTable();
            _groupBuilder = new GroupBuilder(rootScope, GroupType.And);
            _actionGroupBuilder = new ActionGroupBuilder(rootScope);
        }

        /// <summary>
        /// Sets rule's name.
        /// </summary>
        /// <param name="name">Rule name value.</param>
        public void Name(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Sets rule's priority.
        /// </summary>
        /// <param name="priority">Rule priority value.</param>
        public void Priority(int priority)
        {
            _priority = priority;
        }

        /// <summary>
        /// Retrieves left hand side builder (conditions).
        /// </summary>
        /// <returns>Left hand side builder.</returns>
        public GroupBuilder LeftHandSide()
        {
            return _groupBuilder;
        }

        /// <summary>
        /// Retrieves right hand side builder (actions).
        /// </summary>
        /// <returns>Right hand side builder.</returns>
        public ActionGroupBuilder RightHandSide()
        {
            return _actionGroupBuilder;
        }

        /// <summary>
        /// Builds rule definition.
        /// </summary>
        /// <returns>Rule definition.</returns>
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