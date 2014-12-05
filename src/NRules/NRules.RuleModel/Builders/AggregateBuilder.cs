using System;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose an aggregate element.
    /// </summary>
    public class AggregateBuilder : RuleElementBuilder, IBuilder<AggregateElement>
    {
        private readonly Type _resultType;
        private Type _aggregateType;
        private PatternBuilder _sourceBuilder;

        internal AggregateBuilder(Type resultType, SymbolTable scope) : base(scope)
        {
            _resultType = resultType;
        }

        /// <summary>
        /// Sets aggregate type.
        /// </summary>
        /// <param name="aggregateType"></param>
        public void AggregateType(Type aggregateType)
        {
            _aggregateType = aggregateType;
        }

        /// <summary>
        /// Sets aggregate type to the collection aggregate.
        /// </summary>
        /// <param name="elementType">Type of element to aggregate.</param>
        public void CollectionOf(Type elementType)
        {
            Type aggregateType = typeof (CollectionAggregate<>).MakeGenericType(elementType);
            AggregateType(aggregateType);
        }

        /// <summary>
        /// Creates a pattern builder that builds the source of the aggregate.
        /// </summary>
        /// <param name="type">Type of the element the pattern matches.</param>
        /// <returns>Pattern builder.</returns>
        public PatternBuilder SourcePattern(Type type)
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("Aggregate can only have a single source pattern");
            }

            SymbolTable scope = Scope.New();
            Declaration declaration = scope.Declare(type, null);

            _sourceBuilder = new PatternBuilder(scope, declaration);

            return _sourceBuilder;
        }

        AggregateElement IBuilder<AggregateElement>.Build()
        {
            Validate();
            IBuilder<PatternElement> sourceBuilder = _sourceBuilder;
            PatternElement sourceElement = sourceBuilder.Build();
            var aggregateElement = new AggregateElement(_resultType, _aggregateType, sourceElement);
            return aggregateElement;
        }

        private void Validate()
        {
            if (_aggregateType == null)
            {
                throw new InvalidOperationException("Aggregate type not specified");
            }
            if (!typeof (IAggregate).IsAssignableFrom(_aggregateType))
            {
                throw new InvalidOperationException("Aggregate type must implement IAggregate interface");
            }
            if (_sourceBuilder == null)
            {
                throw new InvalidOperationException("Aggregate source is not provided");
            }
        }
    }
}