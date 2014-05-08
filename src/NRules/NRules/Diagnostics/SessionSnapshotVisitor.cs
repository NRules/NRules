using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRules.Rete;

namespace NRules.Diagnostics
{
    internal class SessionSnapshotVisitor : ReteNodeVisitor<SessionSnapshot>
    {
        private readonly IWorkingMemory _workingMemory;
        private readonly HashSet<INode> _visitedNodes = new HashSet<INode>();
        private readonly Dictionary<INode, int> _idMap = new Dictionary<INode, int>();
        private int _lastNodeId;

        public SessionSnapshotVisitor(IWorkingMemory workingMemory)
        {
            _workingMemory = workingMemory;
        }

        protected internal override void VisitRootNode(SessionSnapshot context, RootNode node)
        {
            if (Visited(node)) return;
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Root, string.Empty);
            context.AddNode(nodeInfo);
            base.VisitRootNode(context, node);
        }

        protected internal override void VisitTypeNode(SessionSnapshot context, TypeNode node)
        {
            if (Visited(node)) return;
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Type, node.FilterType.Name);
            context.AddNode(nodeInfo);
            base.VisitTypeNode(context, node);
        }

        protected internal override void VisitSelectionNode(SessionSnapshot context, SelectionNode node)
        {
            if (Visited(node)) return;
            var builder = new StringBuilder();
            foreach (AlphaCondition condition in node.Conditions)
            {
                builder.AppendLine(condition.ExpressionString);
            }

            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Selection, builder.ToString());
            context.AddNode(nodeInfo);
            base.VisitSelectionNode(context, node);
        }

        protected internal override void VisitAlphaMemoryNode(SessionSnapshot context, AlphaMemoryNode node)
        {
            if (Visited(node)) return;

            var builder = new StringBuilder();
            foreach (var fact in _workingMemory.GetNodeMemory(node).Facts)
            {
                builder.AppendLine(fact.Object.ToString());
            }

            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.AlphaMemory, builder.ToString());
            context.AddNode(nodeInfo);
            AddLinks(context, node, node.Sinks);
            base.VisitAlphaMemoryNode(context, node);
        }

        protected override void VisitAlphaNode(SessionSnapshot context, AlphaNode node)
        {
            AddLinks(context, node, node.ChildNodes);
            if (node.MemoryNode != null)
            {
                context.AddLink(GetId(node), GetId(node.MemoryNode));
            }
            base.VisitAlphaNode(context, node);
        }

        protected internal override void VisitJoinNode(SessionSnapshot context, JoinNode node)
        {
            if (Visited(node)) return;
            var builder = new StringBuilder();
            foreach (BetaCondition condition in node.Conditions)
            {
                builder.AppendLine(condition.ExpressionString);
            }

            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Join, builder.ToString());
            context.AddNode(nodeInfo);
            base.VisitJoinNode(context, node);
        }

        protected internal override void VisitNotNode(SessionSnapshot context, NotNode node)
        {
            if (Visited(node)) return;
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Not, string.Empty);
            context.AddNode(nodeInfo);
            base.VisitNotNode(context, node);
        }

        protected internal override void VisitExistsNode(SessionSnapshot context, ExistsNode node)
        {
            if (Visited(node)) return;
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Exists, string.Empty);
            context.AddNode(nodeInfo);
            base.VisitExistsNode(context, node);
        }

        protected internal override void VisitAggregateNode(SessionSnapshot context, AggregateNode node)
        {
            if (Visited(node)) return;
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Aggregate, string.Empty);
            context.AddNode(nodeInfo);
            base.VisitAggregateNode(context, node);
        }

        protected internal override void VisitObjectInputAdapter(SessionSnapshot context, ObjectInputAdapter node)
        {
            if (Visited(node)) return;
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Adapter, string.Empty);
            context.AddNode(nodeInfo);
            context.AddLink(GetId(node), GetId(node.Sink));
            base.VisitObjectInputAdapter(context, node);
        }

        protected internal override void VisitBetaMemoryNode(SessionSnapshot context, BetaMemoryNode node)
        {
            if (Visited(node)) return;
            var builder = new StringBuilder();
            foreach (var tuple in _workingMemory.GetNodeMemory(node).Tuples)
            {
                builder.AppendLine(string.Join(";", tuple.Select(f => f.Object.ToString())));
            }
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.BetaMemory, builder.ToString());
            context.AddNode(nodeInfo);
            AddLinks(context, node, node.Sinks);
            base.VisitBetaMemoryNode(context, node);
        }

        protected override void VisitBetaNode(SessionSnapshot context, BetaNode node)
        {
            context.AddLink(GetId(node), GetId(node.Sink));
            base.VisitBetaNode(context, node);
        }

        protected internal override void VisitTerminalNode(SessionSnapshot context, TerminalNode node)
        {
            if (Visited(node)) return;
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Terminal, string.Empty);
            context.AddNode(nodeInfo);
            context.AddLink(GetId(node), GetId(node.RuleNode));
            base.VisitTerminalNode(context, node);
        }

        protected internal override void VisitRuleNode(SessionSnapshot context, RuleNode node)
        {
            if (Visited(node)) return;
            var nodeInfo = new ReteNodeInfo(GetId(node), ReteNodeType.Rule, node.Rule.Definition.Name);
            context.AddNode(nodeInfo);
            base.VisitRuleNode(context, node);
        }

        private bool Visited(INode node)
        {
            if (_visitedNodes.Contains(node)) return true;
            _visitedNodes.Add(node);
            return false;
        }

        private int GetId(INode node)
        {
            int id;
            if (!_idMap.TryGetValue(node, out id))
            {
                _lastNodeId++;
                id = _lastNodeId;
                _idMap[node] = id;
            }
            return id;
        }

        private void AddLinks(SessionSnapshot context, INode fromNode, IEnumerable<INode> toNodes)
        {
            int fromNodeId = GetId(fromNode);
            foreach (INode toNode in toNodes)
            {
                context.AddLink(fromNodeId, GetId(toNode));
            }
        }
    }
}