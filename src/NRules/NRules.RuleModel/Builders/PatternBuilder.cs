using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a rule pattern.
    /// </summary>
    public class PatternBuilder : RuleLeftElementBuilder, IBuilder<PatternElement>
    {
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();
        private PatternSourceElementBuilder _sourceBuilder;

        internal PatternBuilder(Declaration declaration)
        {
            Declaration = declaration;
        }

        /// <summary>
        /// Builder for the source of this element.
        /// </summary>
        public PatternSourceElementBuilder SourceBuilder => _sourceBuilder;

        /// <summary>
        /// Adds a condition expression to the pattern.
        /// </summary>
        /// <param name="expression">Condition expression.
        /// Names and types of the expression parameters must match the names and types defined in the pattern declarations.</param>
        public void Condition(LambdaExpression expression)
        {
            var condition = Element.Condition(expression);
            _conditions.Add(condition);
        }

        /// <summary>
        /// Pattern declaration.
        /// </summary>
        public Declaration Declaration { get; }

        /// <summary>
        /// Creates an aggregate builder that builds the source of the pattern.
        /// </summary>
        /// <returns>Aggregate builder.</returns>
        public AggregateBuilder Aggregate()
        {
            AssertSingleSource();
            var builder = new AggregateBuilder(Declaration.Type);
            _sourceBuilder = builder;
            return builder;
        }

        /// <summary>
        /// Creates a binding builder that builds the source of the pattern.
        /// </summary>
        /// <returns>Binding builder.</returns>
        public BindingBuilder Binding()
        {
            var builder = new BindingBuilder(Declaration.Type);
            _sourceBuilder = builder;
            return builder;
        }

        PatternElement IBuilder<PatternElement>.Build()
        {
            var sourceBuilder = (IBuilder<PatternSourceElement>)_sourceBuilder;
            var source = sourceBuilder?.Build();
            var patternElement = Element.Pattern(Declaration, _conditions, source);
            return patternElement;
        }

        private void AssertSingleSource()
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("Pattern element can only have a single source");
            }
        }
    }
}