using System;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a Collect rule pattern.
    /// </summary>
    public class CollectPatternBuilder : PatternBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectPatternBuilder"/>.
        /// </summary>
        /// <param name="type">Pattern type.</param>
        /// <param name="name">Pattern name.</param>
        public CollectPatternBuilder(Type type, string name)
            : base(type, name)
        {
        }

        /// <summary>
        /// Creates and configures a collection aggregator.
        /// </summary>
        /// <param name="sourceBuilder"></param>
        public void Collect(PatternBuilder sourceBuilder)
        {
            var aggregateBuilder = Aggregate();
            aggregateBuilder.Collect();
            aggregateBuilder.Pattern(sourceBuilder);

            SourceBuilder = sourceBuilder;
            AggregateBuilder = aggregateBuilder;
        }

		public AggregateBuilder AggregateBuilder { get; private set; }

		public PatternBuilder SourceBuilder { get; private set; }
	}
}