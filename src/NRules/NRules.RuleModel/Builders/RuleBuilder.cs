using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a rule definition.
    /// Contains methods to specify rule's metadata, as well as create child builders for rule's left-hand side and right-hand side.
    /// Creates <see cref="IRuleDefinition"/>.
    /// </summary>
    /// <threadsafety instance="false" />
    public class RuleBuilder
    {
        private string _name;
        private string _description = string.Empty;
        private RuleRepeatability _repeatability = RuleDefinition.DefaultRepeatability;
        private readonly List<string> _tags = new List<string>();
        private readonly PriorityBuilder _priorityBuilder;
        private readonly DependencyGroupBuilder _dependencyBuilder;
        private readonly GroupBuilder _groupBuilder;
        private readonly ActionGroupBuilder _actionGroupBuilder;

        /// <summary>
        /// Constructs an empty rule builder.
        /// </summary>
        public RuleBuilder()
        {
            var rootScope = new SymbolTable();
            _priorityBuilder = new PriorityBuilder(rootScope);
            _dependencyBuilder = new DependencyGroupBuilder(rootScope);
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
            _tags.AddRange(tags);
        }

        /// <summary>
        /// Sets rule's priority.
        /// Default priority is 0.
        /// </summary>
        /// <param name="priority">Rule priority value.</param>
        public void Priority(int priority)
        {
            _priorityBuilder.PriorityValue(priority);
        }

        /// <summary>
        /// Retrieves priority expression builder.
        /// </summary>
        /// <returns>Priority builder.</returns>
        public PriorityBuilder Priority()
        {
            return _priorityBuilder;
        }

        /// <summary>
        /// Sets rule's repeatability.
        /// Default repeatability is <see cref="RuleRepeatability.Repeatable"/>.
        /// </summary>
        public void Repeatability(RuleRepeatability repeatability)
        {
            _repeatability = repeatability;
        }

        /// <summary>
        /// Retrieves dependencies builder.
        /// </summary>
        /// <returns>Left hand side builder.</returns>
        public DependencyGroupBuilder Dependencies()
        {
            return _dependencyBuilder;
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

            IBuilder<DependencyGroupElement> dependencyBuilder = _dependencyBuilder;
            DependencyGroupElement dependencies = dependencyBuilder.Build();

            IBuilder<GroupElement> groupBuilder = _groupBuilder;
            GroupElement conditions = groupBuilder.Build();

            IBuilder<ActionGroupElement> actionBuilder = _actionGroupBuilder;
            ActionGroupElement actions = actionBuilder.Build();

            PriorityElement priority = _priorityBuilder.Build();

            var ruleDefinition = new RuleDefinition(_name, _description, _repeatability, _tags, priority, dependencies, conditions, actions);
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