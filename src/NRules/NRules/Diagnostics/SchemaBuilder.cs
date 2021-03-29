using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Rete;

namespace NRules.Diagnostics
{
    internal class SchemaBuilder
    {
        private readonly List<(INode source, INode target)> _links = new List<(INode, INode)>();
        private readonly Dictionary<INode, ReteNode> _nodeMap = new Dictionary<INode, ReteNode>(); 

        public bool IsVisited(INode node)
        {
            return _nodeMap.ContainsKey(node);
        }

        public void AddNode<TNode>(TNode node, Func<TNode, ReteNode> ctor) where TNode : INode
        {
            var nodeInfo = ctor(node);
            _nodeMap.Add(node, nodeInfo);
        }

        public void AddLink(INode source, INode target)
        {
            _links.Add((source, target));
        }

        public void AddLinks(INode source, IEnumerable<INode> targets)
        {
            foreach (var target in targets)
            {
                AddLink(source, target);
            }
        }

        public ReteGraph Build()
        {
            var nodes = _nodeMap.Values;
            var links = _links.Select(x => new ReteLink(_nodeMap[x.source], _nodeMap[x.target]));
            return new ReteGraph(nodes, links);
        }
    }
}