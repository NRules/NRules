using System;
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

        protected Rule()
        {
            _builder = new RuleBuilder();
            _definition = new Lazy<IRuleDefinition>(BuildDefinition);
        }

        /// <summary>
        /// Returns expression builder for rule's left hand side (conditions).
        /// </summary>
        /// <returns>Left hand side expression builder.</returns>
        protected ILeftHandSideExpression When()
        {
            return new LeftHandSideExpression(_builder);
        }

        /// <summary>
        /// Returns expression builder for rule's right hand side (actions).
        /// </summary>
        /// <returns>Right hand side expression builder.</returns>
        protected IRightHandSideExpression Then()
        {
            return new RightHandSideExpression(_builder);
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

            return _builder.Build();
        }
    }
}