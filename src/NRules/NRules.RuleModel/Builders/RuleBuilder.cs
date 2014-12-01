using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a rule definition.
    /// Contains methods to specify rule's metadata, as well as create child builders for rule's left-hand side and right-hand side.
    /// Creates <see cref="IRuleDefinition"/>.
    /// </summary>
    public class RuleBuilder
    {
        private string _name;
        private string _description = string.Empty;
        private IList<string> _tags; 
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
        /// Sets rule's description.
        /// </summary>
        /// <param name="description">Rule description value.</param>
        public void Description(string description)
        {
            _description = description;
        }

        /// <summary>
        /// Sets rule's tags.
        /// </summary>
        /// <param name="tags">Rule tag values.</param>
        public void Tags(IEnumerable<string> tags)
        {
            _tags = new List<string>(tags);
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
        /// Creates rule definition using current state of the builder.
        /// </summary>
        /// <returns>Rule definition.</returns>
        public IRuleDefinition Build()
        {
            Validate();

            IBuilder<GroupElement> groupBuilder = _groupBuilder;
            GroupElement conditions = groupBuilder.Build();

            IBuilder<ActionGroupElement> actionBuilder = _actionGroupBuilder;
            ActionGroupElement actions = actionBuilder.Build();

            var ruleDefinition = new RuleDefinition(_name, _description, _tags, _priority, conditions, actions);
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