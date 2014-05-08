using System.Collections.Generic;
using NRules.Diagnostics;
using QuickGraph;

namespace NRules.Debug.Visualizer.Model
{
    public class ReteGraph : BidirectionalGraph<ReteNode, ReteEdge>
    {
        internal static ReteGraph Create(SessionSnapshot snapshot)
        {
            var graph = new ReteGraph();

            var nodes = new Dictionary<int, ReteNode>();
            foreach (var nodeInfo in snapshot.Nodes)
            {
                var node = new ReteNode(nodeInfo);
                nodes[node.Id] = node;
                graph.AddVertex(node);
            }

            foreach (var link in snapshot.Links)
            {
                graph.AddEdge(new ReteEdge(nodes[link.Item1.Id], nodes[link.Item2.Id]));
            }

            return graph;
        }
    }
}
