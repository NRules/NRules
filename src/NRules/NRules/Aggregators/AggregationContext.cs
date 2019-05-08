using NRules.Diagnostics;
using NRules.Rete;

namespace NRules.Aggregators
{
    /// <summary>
    /// Context associated with the aggregation operation.
    /// </summary>
    public class AggregationContext
    {
        internal ISessionInternal Session { get; }
        internal IEventAggregator EventAggregator { get; }
        internal NodeDebugInfo NodeInfo { get; }

        internal AggregationContext(ISessionInternal session, IEventAggregator eventAggregator, NodeDebugInfo nodeInfo)
        {
            Session = session;
            EventAggregator = eventAggregator;
            NodeInfo = nodeInfo;
        }
    }
}
