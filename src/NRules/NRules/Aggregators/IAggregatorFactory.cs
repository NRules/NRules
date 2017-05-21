namespace NRules.Aggregators
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
}