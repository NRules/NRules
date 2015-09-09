using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Fluent.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Base class for inline rule definitions.
    /// To create a rule using internal DSL, create a class that inherits from <c>NRules.Fluent.Dsl.Rule</c>
    /// and override <see cref="Define"/> method.
    /// Use <see cref="When"/> and <see cref="Then"/> methods to define rule's conditions and actions correspondingly.
    /// A rule can also be decorated with attributes to add relevant metadata:
    /// <see cref="NameAttribute"/>, <see cref="DescriptionAttribute"/>, <see cref="TagAttribute"/>, 
    /// <see cref="PriorityAttribute"/>, <see cref="RepeatabilityAttribute"/>.
    /// </summary>
    public abstract class Rule
    {
        private readonly Lazy<IRuleDefinition> _definition;
        private readonly RuleBuilder _builder;
        private readonly LeftHandSideExpression _lhsExpression;
        private readonly RightHandSideExpression _rhsExpression;
        private readonly List<Action<RuleBuilder>> _delayedBuildActions = new List<Action<RuleBuilder>>();

        protected Rule()
        {
            _builder = new RuleBuilder();
            _lhsExpression = new LeftHandSideExpression(_builder);
            _rhsExpression = new RightHandSideExpression(_builder);
            _definition = new Lazy<IRuleDefinition>(BuildDefinition);
        }

        /// <summary>
        /// Returns expression builder for rule's dependencies.
        /// </summary>
        /// <returns>Dependencies expression builder.</returns>
        protected IDependencyExpression Dependency()
        {
            return new DependencyExpression(_builder);
        }

        /// <summary>
        /// Sets rule's name.
        /// Name value set at this level overrides the values specified via <see cref="NameAttribute"/> attribute.
        /// </summary>
        /// <param name="value">Rule name value.</param>
        protected void Name(string value)
        {
            _builder.Name(value);
        }

        /// <summary>
        /// Sets rule's priority.
        /// Priority value set at this level overrides the value specified via <see cref="PriorityAttribute"/> attribute.
        /// </summary>
        /// <param name="value">Priority value.</param>
        protected void Priority(int value)
        {
            Action<RuleBuilder> action = b => b.Priority().PriorityValue(value);
            _delayedBuildActions.Add(action);
        }

        /// <summary>
        /// Sets rule's priority.
        /// Priority value set at this level overrides the value specified via <see cref="PriorityAttribute"/> attribute.
        /// </summary>
        /// <param name="expression">Priority expression.</param>
        protected void Priority(Expression<Func<int>> expression)
        {
            Action<RuleBuilder> action = b => b.Priority().DslExpression(expression);
            _delayedBuildActions.Add(action);
        }

        /// <summary>
        /// Returns expression builder for rule's left hand side (conditions).
        /// </summary>
        /// <returns>Left hand side expression builder.</returns>
        protected ILeftHandSideExpression When()
        {
            return _lhsExpression;
        }

        /// <summary>
        /// Returns expression builder for rule's right hand side (actions).
        /// </summary>
        /// <returns>Right hand side expression builder.</returns>
        protected IRightHandSideExpression Then()
        {
            return _rhsExpression;
        }

        /// <summary>
        /// Method called by the rules engine to define the rule.
        /// </summary>
        public abstract void Define();

        internal IRuleDefinition GetDefinition()
        {
            return _definition.Value;
        }

        private IRuleDefinition BuildDefinition()
        {
            var metadata = new RuleMetadata(GetType());
            _builder.Name(metadata.Name);
            _builder.Description(metadata.Description);
            _builder.Tags(metadata.Tags);

            if (metadata.Priority.HasValue)
            {
                _builder.Priority(metadata.Priority.Value);
            }
            if (metadata.Repeatability.HasValue)
            {
                _builder.Repeatability(metadata.Repeatability.Value);
            }

            Define();

            foreach (var delayedBuildAction in _delayedBuildActions)
            {
                delayedBuildAction(_builder);
            }

            return _builder.Build();
        }
    }
}