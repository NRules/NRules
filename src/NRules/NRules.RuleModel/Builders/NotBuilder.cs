using System;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a negative existential element.
    /// </summary>
    public class NotBuilder : RuleLeftElementBuilder, IBuilder<NotElement>
    {
        private RuleLeftElementBuilder _sourceBuilder;

        internal NotBuilder()
        {
        }

        /// <summary>
        /// Builder for the source of this element.
        /// </summary>
        public RuleLeftElementBuilder SourceBuilder => _sourceBuilder;

        /// <summary>
        /// Creates a pattern builder that builds the source of the negative existential element.
        /// </summary>
        /// <param name="type">Type of the element the pattern matches.</param>
        /// <param name="name">Pattern name (optional).</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Type type, string name = null)
        {
            var declaration = new Declaration(type, DeclarationName(name));
            return Pattern(declaration);
        }

        /// <summary>
        /// Creates a pattern builder that builds the source of the negative existential element.
        /// </summary>
        /// <param name="declaration">Pattern declaration.</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Declaration declaration)
        {
            AssertSingleSource();
            var sourceBuilder = new PatternBuilder(declaration);
            _sourceBuilder = sourceBuilder;
            return sourceBuilder;
        }

        /// <summary>
        /// Creates a group builder that builds a group as part of the current element.
        /// </summary>
        /// <param name="groupType">Group type.</param>
        /// <returns>Group builder.</returns>
        public GroupBuilder Group(GroupType groupType)
        {
            AssertSingleSource();
            var sourceBuilder = new GroupBuilder(groupType);
            _sourceBuilder = sourceBuilder;
            return sourceBuilder;
        }

        /// <summary>
        /// Creates a builder for a forall element as part of the current element.
        /// </summary>
        /// <returns>Forall builder.</returns>
        public ForAllBuilder ForAll()
        {
            AssertSingleSource();
            var sourceBuilder = new ForAllBuilder();
            _sourceBuilder = sourceBuilder;
            return sourceBuilder;
        }

        NotElement IBuilder<NotElement>.Build()
        {
            var sourceBuilder = (IBuilder<RuleLeftElement>)_sourceBuilder;
            var sourceElement = sourceBuilder?.Build();
            var notElement = Element.Not(sourceElement);
            return notElement;
        }

        private void AssertSingleSource()
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("NOT element can only have a single source");
            }
        }
    }
}