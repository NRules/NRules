using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Diagnostics
{
    [Serializable]
    public class SessionSnapshot
    {
        private readonly Dictionary<int, ReteNodeInfo> _idToNodeMap = new Dictionary<int, ReteNodeInfo>();

        private readonly List<ReteNodeInfo> _nodes = new List<ReteNodeInfo>();
        private readonly List<Tuple<int, int>> _links = new List<Tuple<int, int>>();

        public IEnumerable<ReteNodeInfo> Nodes
        {
            get { return _nodes; }
        }

        public IEnumerable<Tuple<ReteNodeInfo, ReteNodeInfo>> Links
        {
            get { return _links.Select(x => Tuple.Create(_idToNodeMap[x.Item1], _idToNodeMap[x.Item2])).ToList(); }
        }

        internal SessionSnapshot()
        {
        }

        public void AddNode(ReteNodeInfo node)
        {
            _nodes.Add(node);
            _idToNodeMap[node.Id] = node;
        }

        public void AddLink(int fromNodeId, int toNodeId)
        {
            _links.Add(Tuple.Create(fromNodeId, toNodeId));
        }
    }
}