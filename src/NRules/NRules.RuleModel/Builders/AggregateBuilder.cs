using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.RuleModel.Aggregators;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose an aggregate element.
    /// </summary>
    public class AggregateBuilder : RuleElementBuilder, IBuilder<AggregateElement>, IPatternContainerBuilder
    {
        private const string CollectName = "Collect";
        private const string GroupByName = "GroupBy";
        private const string ProjectName = "Project";
        private const string FlattenName = "Flatten";

        private readonly Type _resultType;
        private IAggregatorFactory _aggregatorFactory; 
        private PatternBuilder _sourceBuilder;
        private string _name;
        private readonly Dictionary<string, LambdaExpression> _expressions = new Dictionary<string, LambdaExpression>();

        internal AggregateBuilder(Type resultType, SymbolTable scope) 
            : base(scope.New("Aggregate"))
        {
            _resultType = resultType;
        }

        /// <summary>
        /// Sets aggregator.
        /// </summary>
        /// <param name="name">Name of the aggregator.</param>
        /// <param name="aggregatorType">Type that implements <see cref="IAggregator"/> that aggregates facts.</param>
        public void Aggregator(string name, Type aggregatorType)
        {
            var aggregatorTypeInfo = aggregatorType.GetTypeInfo();
            var interfaceType = typeof(IAggregator).GetTypeInfo();
            if (!interfaceType.IsAssignableFrom(aggregatorTypeInfo))
            {
                throw new InvalidOperationException(
                    "Aggregator type must implement IAggregator interface");
            }
            if (aggregatorTypeInfo.DeclaredConstructors.All(x => x.GetParameters().Length != 0))
            {
                throw new InvalidOperationException(
                    "Aggregator type must have a parameterless constructor to be used directly. Provide aggregator factory instead.");
            }
            Type factoryType = typeof(DefaultAggregatorFactory<>).MakeGenericType(aggregatorType);
            var aggregatorFactory = (IAggregatorFactory) Activator.CreateInstance(factoryType);
            AggregatorFactory(name, aggregatorFactory);
        }

        /// <summary>
        /// Sets aggregator factory.
        /// </summary>
        /// <param name="name">Name of the aggregator.</param>
        /// <param name="aggregatorFactory">Factory to create new aggregators.</param>
        public void AggregatorFactory(string name, IAggregatorFactory aggregatorFactory)
        {
            _name = name;
            _aggregatorFactory = aggregatorFactory;
        }

        /// <summary>
        /// Configure a collection aggregator.
        /// </summary>
        /// <param name="elementType">Type of elements to aggregate.</param>
        public void CollectionOf(Type elementType)
        {
            Type aggregateType = typeof (CollectionAggregator<>).MakeGenericType(elementType);
            Aggregator(CollectName, aggregateType);
        }

        /// <summary>
        /// Configure a collection aggregate.
        /// </summary>
        /// <typeparam name="TElement">Type of elements to aggregate.</typeparam>
        public void CollectionOf<TElement>()
        {
            CollectionOf(typeof(TElement));
        }

        /// <summary>
        /// Configure group by aggregator.
        /// </summary>
        /// <param name="keySelector">Key selection expressions.</param>
        /// <param name="elementSelector">Element selection expression.</param>
        /// <typeparam name="TSource">Type of source elements to aggregate.</typeparam>
        /// <typeparam name="TKey">Type of grouping key.</typeparam>
        /// <typeparam name="TElement">Type of grouping element.</typeparam>
        public void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        {
            var aggregateFactory = new GroupByAggregatorFactory<TSource, TKey, TElement>(keySelector.Compile(), elementSelector.Compile());
            _expressions["KeySelector"] = keySelector;
            _expressions["ElementSelector"] = keySelector;
            AggregatorFactory(GroupByName, aggregateFactory);
        }

        /// <summary>
        /// Configure projection aggregator.
        /// </summary>
        /// <param name="selector">Projection expression.</param>
        /// <typeparam name="TSource">Type of source elements to aggregate.</typeparam>
        /// <typeparam name="TResult">Type of result elements to aggregate.</typeparam>
        public void Project<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
        {
            var aggregateFactory = new ProjectionAggregatorFactory<TSource, TResult>(selector.Compile());
            _expressions["Selector"] = selector;
            AggregatorFactory(ProjectName, aggregateFactory);
        }

        /// <summary>
        /// Configure flattening aggregator.
        /// </summary>
        /// <param name="selector">Projection expression.</param>
        /// <typeparam name="TSource">Type of source elements to aggregate.</typeparam>
        /// <typeparam name="TResult">Type of result elements to aggregate.</typeparam>
        public void Flatten<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            var aggregateFactory = new FlatteningAggregatorFactory<TSource, TResult>(selector.Compile());
            _expressions["Selector"] = selector;
            AggregatorFactory(FlattenName, aggregateFactory);
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
        /// Creates a pattern builder that builds the source of the aggregate element.
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
            var namedExpressions = _expressions.Select(x => new NamedExpression(x.Key, x.Value));
            var expressionMap = new ExpressionMap(namedExpressions);
            var aggregateElement = new AggregateElement(Scope.VisibleDeclarations, _resultType, _name, expressionMap, _aggregatorFactory, sourceElement);
            return aggregateElement;
        }

        private void Validate()
        {
            if (_aggregatorFactory == null)
            {
                throw new InvalidOperationException("Aggregator factory is not provided");
            }
            if (_sourceBuilder == null)
            {
                throw new InvalidOperationException("Aggregate element source is not provided");
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