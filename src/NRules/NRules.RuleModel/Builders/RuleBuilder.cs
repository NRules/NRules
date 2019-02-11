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

        private DependencyGroupBuilder _dependencyGroupBuilder;
        private GroupBuilder _lhsBuilder;
        private FilterGroupBuilder _filterGroupBuilder;
        private ActionGroupBuilder _rhsBuilder;

        /// <summary>
        /// Constructs an empty rule builder.
        /// </summary>
        public RuleBuilder()
        {
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
            if (_dependencyGroupBuilder == null)
                _dependencyGroupBuilder = new DependencyGroupBuilder();

            return _dependencyGroupBuilder;
        }

        /// <summary>
        /// Sets dependencies builder.
        /// </summary>
        /// <param name="builder">Builder to set.</param>
        public void Dependencies(DependencyGroupBuilder builder)
        {
            if (_dependencyGroupBuilder != null)
                throw new ArgumentException("Builder for dependencies is already set", nameof(builder));

            _dependencyGroupBuilder = builder;
        }

        /// <summary>
        /// Retrieves left-hand side builder (conditions).
        /// </summary>
        /// <returns>Left hand side builder.</returns>
        public GroupBuilder LeftHandSide()
        {
            if (_lhsBuilder == null)
                _lhsBuilder = new GroupBuilder();

            return _lhsBuilder;
        }

        /// <summary>
        /// Sets left-hand side builder (conditions).
        /// </summary>
        /// <param name="builder">Builder to set.</param>
        public void LeftHandSide(GroupBuilder builder)
        {
            if (_lhsBuilder != null)
                throw new ArgumentException("Builder for left-hand side is already set", nameof(builder));

            _lhsBuilder = builder;
        }

        /// <summary>
        /// Retrieves filters builder.
        /// </summary>
        /// <returns>Filters builder.</returns>
        public FilterGroupBuilder Filters()
        {
            if (_filterGroupBuilder == null)
                _filterGroupBuilder = new FilterGroupBuilder();

            return _filterGroupBuilder;
        }

        /// <summary>
        /// Sets filters builder.
        /// </summary>
        /// <param name="builder">Builder to set.</param>
        public void Filters(FilterGroupBuilder builder)
        {
            if (_filterGroupBuilder != null)
                throw new ArgumentException($"Builder for filters is already set", nameof(builder));

            _filterGroupBuilder = builder;
        }

        /// <summary>
        /// Retrieves right-hand side builder (actions).
        /// </summary>
        /// <returns>Right hand side builder.</returns>
        public ActionGroupBuilder RightHandSide()
        {
            if (_rhsBuilder == null)
                _rhsBuilder = new ActionGroupBuilder();

            return _rhsBuilder;
        }

        /// <summary>
        /// Sets right-hand side builder.
        /// </summary>
        /// <param name="builder">Builder to set.</param>
        public void RightHandSide(ActionGroupBuilder builder)
        {
            if (_rhsBuilder != null)
                throw new ArgumentException($"Builder for right-hand side is already set", nameof(builder));

            _rhsBuilder = builder;
        }

        /// <summary>
        /// Creates rule definition using current state of the builder.
        /// </summary>
        /// <returns>Rule definition.</returns>
        public IRuleDefinition Build()
        {
            IBuilder<DependencyGroupElement> dependencyGroupBuilder = _dependencyGroupBuilder;
            DependencyGroupElement dependencies = dependencyGroupBuilder?.Build()
                ?? Element.DependencyGroup();

            IBuilder<FilterGroupElement> filterGroupBuilder = _filterGroupBuilder;
            FilterGroupElement filters = filterGroupBuilder?.Build()
                ?? Element.FilterGroup();

            IBuilder<GroupElement> lhsBuilder = _lhsBuilder;
            GroupElement lhs = lhsBuilder?.Build()
                ?? Element.AndGroup();

            IBuilder<ActionGroupElement> rhsBuilder = _rhsBuilder;
            ActionGroupElement rhs = rhsBuilder?.Build()
                ?? Element.ActionGroup();

            var ruleDefinition = Element.RuleDefinition(_name, _description, _priority, _repeatability, _tags, _properties, dependencies, lhs, filters, rhs);
            return ruleDefinition;
        }
    }
}