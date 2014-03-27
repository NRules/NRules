using NRules.RuleModel.Builders;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Base class for inline rule definitions.
    /// </summary>
    public abstract class Rule
    {
        protected Rule()
        {
            Builder = new RuleBuilder();
            var metadataReader = new RuleMetadataReader(GetType());
            Builder.Name(metadataReader.Name);
            Builder.Description(metadataReader.Description);
            Builder.Tags(metadataReader.Tags);
        }

        internal RuleBuilder Builder { get; private set; }

        /// <summary>
        /// Sets rule priority.
        /// </summary>
        /// <param name="priority">Priority value.</param>
        protected void Priority(int priority)
        {
            Builder.Priority(priority);
        }

        /// <summary>
        /// Returns expression builder for rule's left hand side (conditions).
        /// </summary>
        /// <returns>Left hand side expression builder.</returns>
        protected ILeftHandSide When()
        {
            return new ExpressionBuilder(Builder);
        }

        /// <summary>
        /// Returns expression builder for rule's right hand side (actions).
        /// </summary>
        /// <returns>Right hand side expression builder.</returns>
        protected IRightHandSide Then()
        {
            return new ExpressionBuilder(Builder);
        }

        /// <summary>
        /// Method called by the rules engine to define the rule.
        /// </summary>
        public abstract void Define();
    }
}