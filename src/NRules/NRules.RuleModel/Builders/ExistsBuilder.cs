using System;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose an existential element.
    /// </summary>
    public class ExistsBuilder : RuleElementBuilder, IBuilder<ExistsElement>
    {
        private IBuilder<RuleLeftElement> _sourceBuilder;

        internal ExistsBuilder(SymbolTable scope)
            : base(scope.New("Exists"))
        {
        }

        /// <summary>
        /// Creates a pattern builder that builds the source of the existential element.
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
        /// Creates a pattern builder that builds the source of the existential element.
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

        ExistsElement IBuilder<ExistsElement>.Build()
        {
            Validate();
            RuleLeftElement sourceElement = _sourceBuilder.Build();
            var existsElement = new ExistsElement(Scope.VisibleDeclarations, sourceElement);
            return existsElement;
        }

        private void Validate()
        {
            if (_sourceBuilder == null)
            {
                throw new InvalidOperationException("EXISTS element source is not provided");
            }
        }

        private void AssertSingleSource()
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("EXISTS element can only have a single source");
            }
        }
    }
}
