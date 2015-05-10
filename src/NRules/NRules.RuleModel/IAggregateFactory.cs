namespace NRules.RuleModel
{
    /// <summary>
    /// Base interface for aggregate factories.
    /// </summary>
    public interface IAggregateFactory
    {
        /// <summary>
        /// Creates a new instance of an aggregate.
        /// </summary>
        /// <returns>Aggregate instance.</returns>
        IAggregate Create();
    }

    internal class DefaultAggregateFactory<T> : IAggregateFactory where T : IAggregate, new()
    {
        public IAggregate Create()
        {
            return new T();
        }
    }
}