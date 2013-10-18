using System;

namespace NRules.Rule.Builders
{
    public class AggregateBuilder : RuleElementBuilder, IRuleElementBuilder<AggregateElement>
    {
        private readonly Type _resultType;
        private Type _aggregateType;
        private PatternBuilder _sourceBuilder;

        internal AggregateBuilder(Declaration declaration, SymbolTable scope) : base(scope)
        {
            _resultType = declaration.Type;
        }

        public void AggregateType(Type aggregateType)
        {
            _aggregateType = aggregateType;
        }

        public void CollectionOf(Type elementType)
        {
            Type aggregateType = typeof (CollectionAggregate<>).MakeGenericType(elementType);
            AggregateType(aggregateType);
        }

        public PatternBuilder SourcePattern(Declaration declaration)
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("Aggregate can only have a single source pattern");
            }

            var builder = new PatternBuilder(declaration, Scope);
            _sourceBuilder = builder;
            return builder;   
        }

        AggregateElement IRuleElementBuilder<AggregateElement>.Build()
        {
            Validate();
            IRuleElementBuilder<PatternElement> sourceBuilder = _sourceBuilder;
            var sourceElement = sourceBuilder.Build();
            var aggregateElement = new AggregateElement(_resultType, _aggregateType, sourceElement);
            return aggregateElement;
        }

        private void Validate()
        {
            if (_aggregateType == null)
            {
                throw new InvalidOperationException("Aggregate type not specified");
            }
            if (!typeof(IAggregate).IsAssignableFrom(_aggregateType))
            {
                throw new InvalidOperationException("Aggregate type must implement IAggregate interface");
            }
            if (_sourceBuilder == null)
            {
                throw new InvalidOperationException("Aggregate source builder is not provided");
            }
        }
    }
}