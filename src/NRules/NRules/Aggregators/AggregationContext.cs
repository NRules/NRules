using NRules.Diagnostics;
using NRules.Rete;

namespace NRules.Aggregators
{
    /// <summary>
    /// Context associated with the aggregation operation.
    /// </summary>
    public class AggregationContext
    {
        internal IExecutionContext ExecutionContext { get; }
        internal NodeInfo NodeInfo { get; }

        internal AggregationContext(IExecutionContext executionContext, NodeInfo nodeInfo)
        {
            ExecutionContext = executionContext;
            NodeInfo = nodeInfo;
        }
    }
}
