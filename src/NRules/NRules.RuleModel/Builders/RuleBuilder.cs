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
        private int _priority = RuleDefinition.DefaultPriority;
        private RuleRepeatability _repeatability = RuleDefinition.DefaultRepeatability;
        private readonly List<string> _tags = new List<string>();
        private readonly List<RuleProperty> _properties = new List<RuleProperty>();
        private readonly DependencyGroupBuilder _dependencyBuilder;
        private readonly GroupBuilder _groupBuilder;
        private readonly ActionGroupBuilder _actionGroupBuilder;

        /// <summary>
        /// Constructs an empty rule builder.
        /// </summary>
        public RuleBuilder()
        {
            var rootScope = new SymbolTable();
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
        /// Adds rule's tags.
        /// </summary>
        /// <param name="tags">Rule tag values.</param>
        public void Tags(IEnumerable<string> tags)
        {
            _tags.AddRange(tags);
        }

        /// <summary>
        /// Adds rule's tag.
        /// </summary>
        /// <param name="tag">Rule tag value.</param>
        public void Tag(string tag)
        {
            _tags.Add(tag);
        }

        /// <summary>
        /// Adds rule's properties.
        /// </summary>
        /// <param name="properties">Rule property.</param>
        public void Properties(IEnumerable<RuleProperty> properties)
        {
            _properties.AddRange(properties);
        }

        /// <summary>
        /// Adds rule's property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public void Property(string name, object value)
        {
            var property = new RuleProperty(name, value);
            _properties.Add(property);
        }

        /// <summary>
        /// Sets rule's priority.
        /// Default priority is 0.
        /// </summary>
        /// <param name="priority">Rule priority value.</param>
        public void Priority(int priority)
        {
            _priority = priority;
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

            var ruleDefinition = new RuleDefinition(_name, _description, _priority, _repeatability, _tags, _properties, dependencies, conditions, actions);
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