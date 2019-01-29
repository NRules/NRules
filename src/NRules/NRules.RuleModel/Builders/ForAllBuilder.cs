using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a forall element (universal quantifier).
    /// </summary>
    public class ForAllBuilder : RuleLeftElementBuilder, IBuilder<ForAllElement>
    {
        private PatternBuilder _basePatternBuilder;
        private readonly List<PatternBuilder> _patternBuilders = new List<PatternBuilder>();

        internal ForAllBuilder()
        {
        }

        /// <summary>
        /// Creates a pattern builder that builds the base pattern of the forall element.
        /// </summary>
        /// <param name="type">Type of the element the pattern matches.</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder BasePattern(Type type)
        {
            if (_basePatternBuilder != null)
            {
                throw new InvalidOperationException("FORALL element can only have a single source");
            }

            var declaration = new Declaration(type, DeclarationName(null));
            _basePatternBuilder = new PatternBuilder(declaration);
            return _basePatternBuilder;
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
            IBuilder<PatternElement> basePatternBuilder = _basePatternBuilder;
            PatternElement basePatternElement = basePatternBuilder?.Build();

            var patternElements = new List<PatternElement>();
            foreach (IBuilder<PatternElement> patternBuilder in _patternBuilders)
            {
                patternElements.Add(patternBuilder.Build());
            }

            var forAllElement = Element.ForAll(basePatternElement, patternElements);
            return forAllElement;
        }
    }
}