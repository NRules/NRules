using System.Collections.Generic;
using NRules.Diagnostics;
using QuickGraph;

namespace NRules.Debugger.Visualizer.Model
{
    public class ReteGraph : BidirectionalGraph<ReteNode, ReteEdge>
    {
        internal static ReteGraph Create(SessionSnapshot snapshot)
        {
            var graph = new ReteGraph();

            var nodes = new Dictionary<NodeInfo, ReteNode>();
            foreach (var nodeInfo in snapshot.Nodes)
            {
                var node = new ReteNode(nodeInfo);
                nodes[nodeInfo] = node;
                graph.AddVertex(node);
            }

            foreach (var link in snapshot.Links)
            {
                graph.AddEdge(new ReteEdge(nodes[link.Source], nodes[link.Target]));
            }

            return graph;
        }
    }
}
