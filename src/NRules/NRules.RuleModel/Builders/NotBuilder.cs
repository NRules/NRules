using System;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a negative existential element.
    /// </summary>
    public class NotBuilder : RuleElementBuilder, IBuilder<NotElement>
    {
        private IBuilder<RuleLeftElement> _sourceBuilder;

        internal NotBuilder(SymbolTable scope)
            : base(scope.New("Not"))
        {
        }

        /// <summary>
        /// Creates a pattern builder that builds the source of the negative existential element.
        /// </summary>
        /// <param name="type">Type of the element the pattern matches.</param>
        /// <param name="name">Pattern name (optional).</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Type type, string name = null)
        {
            Declaration declaration = Scope.Declare(type, name);
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
            var sourceBuilder = new PatternBuilder(Scope, declaration);
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
            var sourceBuilder = new GroupBuilder(Scope, groupType);
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
            var sourceBuilder = new ForAllBuilder(Scope);
            _sourceBuilder = sourceBuilder;
            return sourceBuilder;
        }

        NotElement IBuilder<NotElement>.Build()
        {
            Validate();
            RuleLeftElement sourceElement = _sourceBuilder.Build();
            var notElement = new NotElement(Scope.VisibleDeclarations, sourceElement);
            return notElement;
        }

        private void Validate()
        {
            if (_sourceBuilder == null)
            {
                throw new InvalidOperationException("NOT element source is not provided");
            }
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