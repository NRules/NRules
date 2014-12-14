using System;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose existential element.
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
        /// <returns>Pattern builder.</returns>
        public PatternBuilder Pattern(Type type)
        {
            Declaration declaration = Scope.Declare(type, null);
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
        /// <returns>Group builder.</returns>
        public GroupBuilder Group()
        {
            AssertSingleSource();
            var sourceBuilder = new GroupBuilder(Scope, GroupType.And);
            _sourceBuilder = sourceBuilder;
            return sourceBuilder;
        }

        ExistsElement IBuilder<ExistsElement>.Build()
        {
            Validate();
            RuleLeftElement sourceElement = _sourceBuilder.Build();
            var existsElement = new ExistsElement(sourceElement);
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
