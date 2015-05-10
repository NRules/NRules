using System;
using NRules.RuleModel.Aggregators;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose an aggregate element.
    /// </summary>
    public class AggregateBuilder : RuleElementBuilder, IBuilder<AggregateElement>
    {
        private readonly Type _resultType;
        private IAggregateFactory _aggregateFactory; 
        private PatternBuilder _sourceBuilder;

        internal AggregateBuilder(Type resultType, SymbolTable scope) 
            : base(scope.New("Aggregate"))
        {
            _resultType = resultType;
        }

        /// <summary>
        /// Sets aggregate type.
        /// </summary>
        /// <param name="aggregateType">Type that implements <see cref="IAggregate"/> that aggregates facts.</param>
        public void AggregateType(Type aggregateType)
        {
            if (!typeof(IAggregate).IsAssignableFrom(aggregateType))
            {
                throw new InvalidOperationException(
                    "Aggregate type must implement IAggregate interface");
            }
            if (aggregateType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException(
                    "Aggregate type must have a parameterless constructor to be used directly. Provide aggregate factory instead.");
            }
            Type factoryType = typeof(DefaultAggregateFactory<>).MakeGenericType(aggregateType);
            var aggregateFactory = (IAggregateFactory) Activator.CreateInstance(factoryType);
            AggregateFactory(aggregateFactory);
        }

        /// <summary>
        /// Sets aggregate type.
        /// </summary>
        /// <param name="aggregateFactory">Factory to create new aggregates.</param>
        public void AggregateFactory(IAggregateFactory aggregateFactory)
        {
            _aggregateFactory = aggregateFactory;
        }

        /// <summary>
        /// Configure a collection aggregate.
        /// </summary>
        /// <param name="elementType">Type of element to aggregate.</param>
        public void CollectionOf(Type elementType)
        {
            Type aggregateType = typeof (CollectionAggregate<>).MakeGenericType(elementType);
            AggregateType(aggregateType);
        }

        /// <summary>
        /// Configure a collection aggregate.
        /// </summary>
        public void CollectionOf<TElement>()
        {
            var aggregateFactory = new DefaultAggregateFactory<CollectionAggregate<TElement>>();
            AggregateFactory(aggregateFactory);
        }

        /// <summary>
        /// Sets aggregate type to the collection aggregate.
        /// </summary>
        /// <param name="keySelector">Key selection function.</param>
        public void GroupBy<TKey, TElement>(Func<TElement, TKey> keySelector)
        {
            var aggregateFactory = new GroupByAggregateFactory<TKey, TElement>(keySelector);
            AggregateFactory(aggregateFactory);
        }

        /// <summary>
        /// Creates a pattern builder that builds the source of the aggregate.
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
        /// Creates a pattern builder that builds the source of the aggregate.
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

        AggregateElement IBuilder<AggregateElement>.Build()
        {
            Validate();
            IBuilder<PatternElement> sourceBuilder = _sourceBuilder;
            PatternElement sourceElement = sourceBuilder.Build();
            var aggregateElement = new AggregateElement(Scope.VisibleDeclarations, _resultType, _aggregateFactory, sourceElement);
            return aggregateElement;
        }

        private void Validate()
        {
            if (_aggregateFactory == null)
            {
                throw new InvalidOperationException("Aggregate factory is not provided");
            }
            if (_sourceBuilder == null)
            {
                throw new InvalidOperationException("Aggregate source is not provided");
            }
        }

        private void AssertSingleSource()
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("Aggregate element can only have a single source");
            }
        }
    }
}