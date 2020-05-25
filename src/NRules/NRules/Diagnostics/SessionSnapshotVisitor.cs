using NRules.Rete;

namespace NRules.Diagnostics
{
    internal class SessionSnapshotVisitor : ReteNodeVisitor<SnapshotBuilder>
    {
        private readonly IWorkingMemory _workingMemory;

        public SessionSnapshotVisitor(IWorkingMemory workingMemory)
        {
            _workingMemory = workingMemory;
        }

        protected internal override void VisitRootNode(SnapshotBuilder builder, RootNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitRootNode(builder, node);
        }

        protected internal override void VisitTypeNode(SnapshotBuilder builder, TypeNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitTypeNode(builder, node);
        }

        protected internal override void VisitSelectionNode(SnapshotBuilder builder, SelectionNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitSelectionNode(builder, node);
        }

        protected internal override void VisitAlphaMemoryNode(SnapshotBuilder builder, AlphaMemoryNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, n => NodeInfo.Create(n, _workingMemory.GetNodeMemory(n)));
            builder.AddLinks(node, node.Sinks);
            base.VisitAlphaMemoryNode(builder, node);
        }

        protected override void VisitAlphaNode(SnapshotBuilder builder, AlphaNode node)
        {
            builder.AddLinks(node, node.ChildNodes);
            if (node.MemoryNode != null)
            {
                builder.AddLink(node, node.MemoryNode);
            }
            base.VisitAlphaNode(builder, node);
        }

        protected internal override void VisitJoinNode(SnapshotBuilder builder, JoinNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitJoinNode(builder, node);
        }

        protected internal override void VisitNotNode(SnapshotBuilder builder, NotNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitNotNode(builder, node);
        }

        protected internal override void VisitExistsNode(SnapshotBuilder builder, ExistsNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitExistsNode(builder, node);
        }

        protected internal override void VisitAggregateNode(SnapshotBuilder builder, AggregateNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitAggregateNode(builder, node);
        }

        protected internal override void VisitObjectInputAdapter(SnapshotBuilder builder, ObjectInputAdapter node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            builder.AddLinks(node, node.Sinks);
            base.VisitObjectInputAdapter(builder, node);
        }

        protected internal override void VisitBindingNode(SnapshotBuilder builder, BindingNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitBindingNode(builder, node);
        }

        protected internal override void VisitBetaMemoryNode(SnapshotBuilder builder, BetaMemoryNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, n => NodeInfo.Create(n, _workingMemory.GetNodeMemory(n)));
            builder.AddLinks(node, node.Sinks);
            base.VisitBetaMemoryNode(builder, node);
        }

        protected override void VisitBetaNode(SnapshotBuilder builder, BetaNode node)
        {
            builder.AddLink(node, node.MemoryNode);
            base.VisitBetaNode(builder, node);
        }

        protected internal override void VisitRuleNode(SnapshotBuilder builder, RuleNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, NodeInfo.Create);
            base.VisitRuleNode(builder, node);
        }
    }
}