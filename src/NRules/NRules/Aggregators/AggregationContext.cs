using NRules.Diagnostics;

namespace NRules.Aggregators
{
    /// <summary>
    /// Context associated with the aggregation operation.
    /// </summary>
    public class AggregationContext
    {
        internal ISessionInternal Session { get; }
        internal IEventAggregator EventAggregator { get; }

        internal AggregationContext(ISessionInternal session, IEventAggregator eventAggregator)
        {
            Session = session;
            EventAggregator = eventAggregator;
        }
    }
}
