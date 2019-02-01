using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a forall element (universal quantifier).
    /// </summary>
    public class ForAllBuilder : RuleElementBuilder, IBuilder<ForAllElement>
    {
        private IBuilder<PatternElement> _sourceBuilder;
        private readonly List<IBuilder<PatternElement>> _patternBuilders = new List<IBuilder<PatternElement>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ForAllBuilder"/>.
        /// </summary>
        public ForAllBuilder()
        {
        }

        /// <summary>
        /// Sets the base pattern of the forall element.
        /// </summary>
        /// <param name="element">Element to set as the base pattern.</param>
        public void BasePattern(PatternElement element)
        {
            AssertSingleSource();
            _sourceBuilder = BuilderAdapter.Create(element);
        }

        /// <summary>
        /// Sets the base pattern builder of the forall element.
        /// </summary>
        /// <param name="builder">Element builder to set as the base pattern.</param>
        public void BasePattern(PatternBuilder builder)
        {
            AssertSingleSource();
            _sourceBuilder = builder;
        }

        /// <summary>
        /// Creates a pattern builder that builds the base pattern of the forall element.
        /// </summary>
        /// <param name="type">Type of the element the pattern matches.</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder BasePattern(Type type)
        {
            AssertSingleSource();
            var declaration = new Declaration(type, DeclarationName(null));
            var builder = new PatternBuilder(declaration);
            _sourceBuilder = builder;
            return builder;
        }
        
        /// <summary>
        /// Adds a pattern to the forall element.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Pattern(PatternElement element)
        {
            var builder = BuilderAdapter.Create(element);
            _patternBuilders.Add(builder);
        }

        /// <summary>
        /// Adds a pattern builder to the forall element.
        /// </summary>
        /// <param name="builder">Element builder to add.</param>
        public void Pattern(PatternBuilder builder)
        {
            _patternBuilders.Add(builder);
        }

        /// <summary>
        /// Creates a pattern builder that builds a pattern of the forall element.
        /// </summary>
        /// <param name="type">Type of the element the pattern matches.</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Type type)
        {
            var declaration = new Declaration(type, DeclarationName(null));
            var patternBuilder = new PatternBuilder(declaration);
            _patternBuilders.Add(patternBuilder);
            return patternBuilder;
        }

        ForAllElement IBuilder<ForAllElement>.Build()
        {
            PatternElement basePatternElement = _sourceBuilder?.Build();

            var patternElements = new List<PatternElement>();
            foreach (var patternBuilder in _patternBuilders)
            {
                patternElements.Add(patternBuilder.Build());
            }

            var forAllElement = Element.ForAll(basePatternElement, patternElements);
            return forAllElement;
        }

        private void AssertSingleSource()
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("FORALL element can only have a single source");
            }
        }
    }
}