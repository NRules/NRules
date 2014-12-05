using System;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Type of quantifier.
    /// </summary>
    public enum QuantifierType
    {
        /// <summary>
        /// Existential quantifier.
        /// </summary>
        Exists = 0,

        /// <summary>
        /// Negative existential.
        /// </summary>
        Not = 1,

        /// <summary>
        /// Universal quantifier.
        /// </summary>
        ForAll = 2,
    }

    /// <summary>
    /// Builder to compose a quantification element.
    /// </summary>
    public class QuantifierBuilder : RuleElementBuilder, IBuilder<QuantifierElement>
    {
        private readonly QuantifierType _quantifierType;
        private PatternBuilder _sourceBuilder;

        internal QuantifierBuilder(SymbolTable scope, QuantifierType quantifierType)
            : base(scope)
        {
            _quantifierType = quantifierType;
        }

        /// <summary>
        /// Creates a pattern builder that builds the source of the quantifier.
        /// </summary>
        /// <param name="type">Type of the element the pattern matches.</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder SourcePattern(Type type)
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("Quantifier can only have a single source pattern");
            }

            SymbolTable scope = Scope.New();
            Declaration declaration = scope.Declare(type, null);

            _sourceBuilder = new PatternBuilder(scope, declaration);

            return _sourceBuilder;
        }

        QuantifierElement IBuilder<QuantifierElement>.Build()
        {
            Validate();
            IBuilder<PatternElement> sourceBuilder = _sourceBuilder;
            PatternElement sourceElement = sourceBuilder.Build();
            QuantifierElement quantifierElement;
            switch (_quantifierType)
            {
                case QuantifierType.Exists:
                    quantifierElement = new ExistsElement(sourceElement);
                    break;
                case QuantifierType.Not:
                    quantifierElement = new NotElement(sourceElement);
                    break;
                case QuantifierType.ForAll:
                    quantifierElement = new ForAllElement(sourceElement);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unrecognized quantifier type. QuantifierType={0}", _quantifierType));
            }
            return quantifierElement;
        }

        private void Validate()
        {
            if (_sourceBuilder == null)
            {
                throw new InvalidOperationException("Quantifier source is not provided");
            }
        }
    }
}
