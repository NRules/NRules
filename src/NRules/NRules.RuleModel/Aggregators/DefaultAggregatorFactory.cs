namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator factory that creates new instances of the aggregator via a default constructor.
    /// </summary>
    /// <typeparam name="TAggregator">Type of aggregator.</typeparam>
    internal class DefaultAggregatorFactory<TAggregator> : IAggregatorFactory where TAggregator : IAggregator, new()
    {
        public IAggregator Create()
        {
            return new TAggregator();
        }
    }
}