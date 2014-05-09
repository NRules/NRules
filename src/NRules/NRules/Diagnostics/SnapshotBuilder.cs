using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Rete;

namespace NRules.Diagnostics
{
    internal class SnapshotBuilder
    {
        private readonly List<Tuple<INode, INode>> _links = new List<Tuple<INode, INode>>();
        private readonly Dictionary<INode, NodeInfo> _nodeMap = new Dictionary<INode, NodeInfo>(); 

        public bool IsVisited(INode node)
        {
            return _nodeMap.ContainsKey(node);
        }

        public void AddNode<TNode>(TNode node, Func<TNode, NodeInfo> ctor) where TNode : INode
        {
            var nodeInfo = ctor(node);
            _nodeMap.Add(node, nodeInfo);
        }

        public void AddLink(INode source, INode target)
        {
            _links.Add(new Tuple<INode, INode>(source, target));
        }

        public void AddLinks(INode source, IEnumerable<INode> targets)
        {
            foreach (var target in targets)
            {
                _links.Add(new Tuple<INode, INode>(source, target));
            }
        }

        public SessionSnapshot Build()
        {
            var nodes = _nodeMap.Values;
            var links = _links.Select(x => new LinkInfo(_nodeMap[x.Item1], _nodeMap[x.Item2]));
            return new SessionSnapshot(nodes, links);
        }
    }
}