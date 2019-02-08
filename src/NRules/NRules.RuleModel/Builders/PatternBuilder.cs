using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a rule pattern.
    /// </summary>
    public class PatternBuilder : RuleElementBuilder, IBuilder<PatternElement>
    {
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();
        private IBuilder<PatternSourceElement> _sourceBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternBuilder"/>.
        /// </summary>
        /// <param name="type">Pattern type.</param>
        /// <param name="name">Pattern name.</param>
        public PatternBuilder(Type type, string name)
        {
            Declaration = new Declaration(type, name ?? "$this$");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternBuilder"/>.
        /// </summary>
        /// <param name="declaration">Pattern declaration.</param>
        public PatternBuilder(Declaration declaration)
        {
            Declaration = declaration;
        }

        /// <summary>
        /// Adds a condition expression to the pattern element.
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
        /// Sets an aggregate element as the source of the pattern element.
        /// </summary>
        /// <param name="element">Element to set as the source.</param>
        public void Aggregate(AggregateElement element)
        {
            AssertSingleSource();
            var builder = BuilderAdapter.Create(element);
            _sourceBuilder = builder;
        }

        /// <summary>
        /// Sets an aggregate builder as the source of the pattern element.
        /// </summary>
        /// <param name="builder">Element builder to set as the source.</param>
        public void Aggregate(AggregateBuilder builder)
        {
            AssertSingleSource();
            builder.ResultType(Declaration.Type);
            _sourceBuilder = builder;
        }

        /// <summary>
        /// Creates an aggregate builder that builds the source of the pattern element.
        /// </summary>
        /// <returns>Aggregate builder.</returns>
        public AggregateBuilder Aggregate()
        {
            AssertSingleSource();
            var builder = new AggregateBuilder();
            builder.ResultType(Declaration.Type);
            _sourceBuilder = builder;
            return builder;
        }

        /// <summary>
        /// Sets a binding element as the source of the pattern element.
        /// </summary>
        /// <param name="element">Element to set as the source.</param>
        public void Binding(BindingElement element)
        {
            AssertSingleSource();
            var builder = BuilderAdapter.Create(element);
            _sourceBuilder = builder;
        }

        /// <summary>
        /// Sets a binding builder as the source of the pattern element.
        /// </summary>
        /// <param name="builder">Element builder to set as the source.</param>
        public void Binding(BindingBuilder builder)
        {
            AssertSingleSource();
            builder.ResultType(Declaration.Type);
            _sourceBuilder = builder;
        }

        /// <summary>
        /// Creates a binding builder that builds the source of the pattern element.
        /// </summary>
        /// <returns>Binding builder.</returns>
        public BindingBuilder Binding()
        {
            var builder = new BindingBuilder();
            builder.ResultType(Declaration.Type);
            _sourceBuilder = builder;
            return builder;
        }

        PatternElement IBuilder<PatternElement>.Build()
        {
            var source = _sourceBuilder?.Build();
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