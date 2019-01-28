using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly DependencyGroupBuilder _dependencyGroupBuilder;
        private readonly FilterGroupBuilder _filterGroupBuilder;
        private readonly GroupBuilder _conditionGroupBuilder;
        private readonly ActionGroupBuilder _actionGroupBuilder;

        /// <summary>
        /// Constructs an empty rule builder.
        /// </summary>
        public RuleBuilder()
        {
            _dependencyGroupBuilder = new DependencyGroupBuilder();
            _filterGroupBuilder = new FilterGroupBuilder();
            _conditionGroupBuilder = new GroupBuilder(GroupType.And);
            _actionGroupBuilder = new ActionGroupBuilder();
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
        /// <returns>Dependencies builder.</returns>
        public DependencyGroupBuilder Dependencies()
        {
            return _dependencyGroupBuilder;
        }

        /// <summary>
        /// Retrieves filters builder.
        /// </summary>
        /// <returns>Filters builder.</returns>
        public FilterGroupBuilder Filters()
        {
            return _filterGroupBuilder;
        }

        /// <summary>
        /// Retrieves left-hand side builder (conditions).
        /// </summary>
        /// <returns>Left hand side builder.</returns>
        public GroupBuilder LeftHandSide()
        {
            return _conditionGroupBuilder;
        }

        /// <summary>
        /// Retrieves right-hand side builder (actions).
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

            IBuilder<DependencyGroupElement> dependencyGroupBuilder = _dependencyGroupBuilder;
            DependencyGroupElement dependencies = dependencyGroupBuilder.Build();

            IBuilder<FilterGroupElement> filterGroupBuilder = _filterGroupBuilder;
            FilterGroupElement filters = filterGroupBuilder.Build();

            IBuilder<GroupElement> conditionGroupBuilder = _conditionGroupBuilder;
            GroupElement conditions = conditionGroupBuilder.Build();

            IBuilder<ActionGroupElement> actionGroupBuilder = _actionGroupBuilder;
            ActionGroupElement actions = actionGroupBuilder.Build();

            var ruleDefinition = new RuleDefinition(_name, _description, _priority, _repeatability, _tags, _properties, dependencies, filters, conditions, actions);
            Validate(ruleDefinition);

            return ruleDefinition;
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("Rule name not specified");
            }
        }

        private void Validate(RuleDefinition definition)
        {
            ElementValidator.ValidateUniqueDeclarations(
                definition.LeftHandSide, definition.DependencyGroup);
            
            var exports = definition.LeftHandSide.Exports
                .Concat(definition.DependencyGroup.Exports).ToArray();

            var undefinedLhs = definition.LeftHandSide.Imports
                .Except(exports).ToArray();
            if (undefinedLhs.Any())
            {
                var variables = string.Join(",", undefinedLhs.Select(x => x.Name));
                throw new InvalidOperationException($"Undefined variables in rule match conditions. Variables={variables}");
            }

            var undefinedFilter = definition.FilterGroup.Imports
                .Except(exports).ToArray();
            if (undefinedFilter.Any())
            {
                var variables = string.Join(",", undefinedFilter.Select(x => x.Name));
                throw new InvalidOperationException($"Undefined variables in rule filter. Variables={variables}");
            }

            var undefinedRhs = definition.RightHandSide.Imports
                .Except(exports).ToArray();
            if (undefinedRhs.Any())
            {
                var variables = string.Join(",", undefinedRhs.Select(x => x.Name));
                throw new InvalidOperationException($"Undefined variables in rule actions. Variables={variables}");
            }

            var lhsDependencyRefs = definition.LeftHandSide.Imports
                .Intersect(definition.DependencyGroup.Exports).ToArray();
            if (lhsDependencyRefs.Any())
            {
                var variables = string.Join(",", lhsDependencyRefs.Select(x => x.Name));
                throw new InvalidOperationException($"Rule match conditions cannot reference injected dependencies. Variables={variables}");
            }
        }
    }
}