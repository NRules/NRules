using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose an aggregate element.
    /// </summary>
    public class AggregateBuilder : RuleElementBuilder, IBuilder<AggregateElement>
    {
        private string _name;
        private readonly Dictionary<string, LambdaExpression> _expressions = new Dictionary<string, LambdaExpression>();
        private readonly Type _resultType;
        private Type _customFactoryType;
        private IBuilder<PatternElement> _sourceBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateBuilder"/>.
        /// </summary>
        /// <param name="resultType">Type of the aggregation result.</param>
        public AggregateBuilder(Type resultType) 
        {
            _resultType = resultType;
        }

        /// <summary>
        /// Configure a custom aggregator.
        /// </summary>
        /// <param name="name">Name of the aggregator.</param>
        /// <param name="expressions">Named expressions used by the aggregator.</param>
        /// <param name="customFactoryType">The type of the custom aggregate factory</param>
        public void Aggregator(string name, IDictionary<string, LambdaExpression> expressions, Type customFactoryType = null)
        {
            _name = name;
            foreach (var item in expressions)
            {
                _expressions[item.Key] = item.Value;
            }
            _customFactoryType = customFactoryType;
        }
        
        /// <summary>
        /// Configure a collection aggregator.
        /// </summary>
        public void Collect()
        {
            _name = AggregateElement.CollectName;
        }

        /// <summary>
        /// Configure group by aggregator.
        /// </summary>
        /// <param name="keySelector">Key selection expressions.</param>
        /// <param name="elementSelector">Element selection expression.</param>
        public void GroupBy(LambdaExpression keySelector, LambdaExpression elementSelector)
        {
            _name = AggregateElement.GroupByName;
            _expressions["KeySelector"] = keySelector;
            _expressions["ElementSelector"] = elementSelector;
        }

        /// <summary>
        /// Configure projection aggregator.
        /// </summary>
        /// <param name="selector">Projection expression.</param>
        public void Project(LambdaExpression selector)
        {
            _name = AggregateElement.ProjectName;
            _expressions["Selector"] = selector;
        }

        /// <summary>
        /// Configure flattening aggregator.
        /// </summary>
        /// <param name="selector">Projection expression.</param>
        public void Flatten(LambdaExpression selector)
        {
            _name = AggregateElement.FlattenName;
            _expressions["Selector"] = selector;
        }

        /// <summary>
        /// Sets a pattern element as the source of the aggregate element.
        /// </summary>
        /// <param name="element">Element to set as the source.</param>
        public void Pattern(PatternElement element)
        {
            AssertSingleSource();
            _sourceBuilder = BuilderAdapter.Create(element);
        }

        /// <summary>
        /// Sets a pattern builder as the source of the aggregate element.
        /// </summary>
        /// <param name="builder">Element builder to set as the source.</param>
        public void Pattern(PatternBuilder builder)
        {
            AssertSingleSource();
            _sourceBuilder = builder;
        }

        /// <summary>
        /// Creates a pattern builder that builds the source of the aggregate element.
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
        /// Creates a pattern builder that builds the source of the aggregate element.
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

        AggregateElement IBuilder<AggregateElement>.Build()
        {
            PatternElement sourceElement = _sourceBuilder?.Build();
            var aggregateElement = Element.Aggregate(_resultType, _name, _expressions, sourceElement, _customFactoryType);
            return aggregateElement;
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