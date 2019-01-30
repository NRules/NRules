using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a forall element (universal quantifier).
    /// </summary>
    public class ForAllBuilder : RuleElementBuilder, IBuilder<ForAllElement>
    {
        private IBuilder<PatternElement> _basePatternBuilder;
        private readonly List<IBuilder<PatternElement>> _patternBuilders = new List<IBuilder<PatternElement>>();

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
            var basePatternBuilder = new PatternBuilder(declaration);
            _basePatternBuilder = basePatternBuilder;
            return basePatternBuilder;
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
            PatternElement basePatternElement = _basePatternBuilder?.Build();

            var patternElements = new List<PatternElement>();
            foreach (var patternBuilder in _patternBuilders)
            {
                patternElements.Add(patternBuilder.Build());
            }

            var forAllElement = Element.ForAll(basePatternElement, patternElements);
            return forAllElement;
        }
    }
}