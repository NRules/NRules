using System;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Base class for inline rule definitions.
    /// To create a rule using internal DSL, create a class that inherits from <c>NRules.Fluent.Dsl.Rule</c>
    /// and override <see cref="Define"/> method.
    /// Use <see cref="When"/> and <see cref="Then"/> methods to define rule's conditions and actions correspondingly.
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
        /// Sets rule's priority.
        /// If multiple rules get activated at the same time, rules with higher priority get executed first.
        /// Priority value can be positive, negative or zero.
        /// Default priority is zero.
        /// </summary>
        /// <param name="priority">Priority value.</param>
        protected void Priority(int priority)
        {
            _builder.Priority(priority);
        }

        /// <summary>
        /// Sets rule's repeatability, that is, how it behaves when it is activated with the same set of facts multiple times, 
        /// which is important for recursion control. By default rules are <see cref="RuleRepeatability.Repeatable"/>, 
        /// which means a rule will fire every time it is activated with the same set of facts.
        /// If repeatability is set to <see cref="RuleRepeatability.NonRepeatable"/> then the rule will not fire with the same combination of facts, 
        /// unless that combination was previously deactivated (i.e. through retraction).
        /// </summary>
        protected void Repeatability(RuleRepeatability repeatability)
        {
            _builder.Repeatability(repeatability);
        }

        /// <summary>
        /// Returns expression builder for rule's left hand side (conditions).
        /// </summary>
        /// <returns>Left hand side expression builder.</returns>
        protected ILeftHandSide When()
        {
            return new ExpressionBuilder(_builder);
        }

        /// <summary>
        /// Returns expression builder for rule's right hand side (actions).
        /// </summary>
        /// <returns>Right hand side expression builder.</returns>
        protected IRightHandSide Then()
        {
            return new ExpressionBuilder(_builder);
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

            Define();

            return _builder.Build();
        }
    }
}