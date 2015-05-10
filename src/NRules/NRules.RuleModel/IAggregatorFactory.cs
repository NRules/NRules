namespace NRules.RuleModel
{
    /// <summary>
    /// Base interface for aggregator factories.
    /// </summary>
    public interface IAggregatorFactory
    {
        /// <summary>
        /// Creates a new aggregator instance.
        /// </summary>
        /// <returns>Aggregator instance.</returns>
        IAggregator Create();
    }

    internal class DefaultAggregatorFactory<T> : IAggregatorFactory where T : IAggregator, new()
    {
        public IAggregator Create()
        {
            return new T();
        }
    }
}