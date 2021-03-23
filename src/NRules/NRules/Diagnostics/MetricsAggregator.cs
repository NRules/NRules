using System.Collections.Generic;
using NRules.Rete;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Provides access to performance metrics associated with individual nodes
    /// in the Rete network used to execute the rules.
    /// </summary>
    public interface IMetricsProvider
    {
        /// <summary>
        /// Retrieves performance metrics for a given Rete network node by the node id.
        /// </summary>
        /// <param name="nodeId">Id of the node for which to retrieve the metrics.</param>
        /// <returns>Rete network node performance metrics or <c>null</c>.</returns>
        INodeMetrics FindByNodeId(int nodeId);

        /// <summary>
        /// Retries performance metrics for all nodes in the Rete network.
        /// </summary>
        /// <returns>Collection of Rete network node metrics.</returns>
        IEnumerable<INodeMetrics> GetAll();

        /// <summary>
        /// Resets cumulative performance metrics associated with all nodes in the network.
        /// </summary>
        void Reset();
    }

    internal interface IMetricsAggregator : IMetricsProvider
    {
        NodeMetrics GetMetrics(INode node);
    }

    internal class MetricsAggregator : IMetricsAggregator
    {
        private readonly Dictionary<int, NodeMetrics> _metrics = new Dictionary<int, NodeMetrics>();

        public INodeMetrics FindByNodeId(int nodeId)
        {
            _metrics.TryGetValue(nodeId, out var nodeMetrics);
            return nodeMetrics;
        }

        public IEnumerable<INodeMetrics> GetAll()
        {
            return _metrics.Values;
        }

        public NodeMetrics GetMetrics(INode node)
        {
            if (!_metrics.TryGetValue(node.Id, out var nodeMetrics))
            {
                nodeMetrics = new NodeMetrics(node.Id);
                _metrics[node.Id] = nodeMetrics;
            }

            return nodeMetrics;
        }

        public void Reset()
        {
            foreach (var nodeMetrics in _metrics.Values)
            {
                nodeMetrics.Reset();
            }
        }
    }
}
