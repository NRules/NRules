using NRules.Rete;

namespace NRules.Diagnostics
{
    internal class SchemaReteVisitor : ReteNodeVisitor<SchemaBuilder>
    {
        protected internal override void VisitRootNode(SchemaBuilder builder, RootNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitRootNode(builder, node);
        }

        protected internal override void VisitTypeNode(SchemaBuilder builder, TypeNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitTypeNode(builder, node);
        }

        protected internal override void VisitSelectionNode(SchemaBuilder builder, SelectionNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitSelectionNode(builder, node);
        }

        protected internal override void VisitAlphaMemoryNode(SchemaBuilder builder, AlphaMemoryNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            builder.AddLinks(node, node.Sinks);
            base.VisitAlphaMemoryNode(builder, node);
        }

        protected override void VisitAlphaNode(SchemaBuilder builder, AlphaNode node)
        {
            builder.AddLinks(node, node.ChildNodes);
            if (node.MemoryNode != null)
            {
                builder.AddLink(node, node.MemoryNode);
            }
            base.VisitAlphaNode(builder, node);
        }

        protected internal override void VisitJoinNode(SchemaBuilder builder, JoinNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitJoinNode(builder, node);
        }

        protected internal override void VisitNotNode(SchemaBuilder builder, NotNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitNotNode(builder, node);
        }

        protected internal override void VisitExistsNode(SchemaBuilder builder, ExistsNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitExistsNode(builder, node);
        }

        protected internal override void VisitAggregateNode(SchemaBuilder builder, AggregateNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitAggregateNode(builder, node);
        }

        protected internal override void VisitObjectInputAdapter(SchemaBuilder builder, ObjectInputAdapter node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            builder.AddLinks(node, node.Sinks);
            base.VisitObjectInputAdapter(builder, node);
        }

        protected internal override void VisitBindingNode(SchemaBuilder builder, BindingNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitBindingNode(builder, node);
        }

        protected internal override void VisitBetaMemoryNode(SchemaBuilder builder, BetaMemoryNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            builder.AddLinks(node, node.Sinks);
            base.VisitBetaMemoryNode(builder, node);
        }

        protected override void VisitBetaNode(SchemaBuilder builder, BetaNode node)
        {
            builder.AddLink(node, node.MemoryNode);
            base.VisitBetaNode(builder, node);
        }

        protected internal override void VisitRuleNode(SchemaBuilder builder, RuleNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            base.VisitRuleNode(builder, node);
        }

        protected internal override void VisitDummyNode(SchemaBuilder builder, DummyNode node)
        {
            if (builder.IsVisited(node)) return;
            builder.AddNode(node, ReteNode.Create);
            foreach (var sink in node.Sinks)
            {
                if (!builder.IsVisited(sink))
                    builder.AddLink(node, sink);
            }
            base.VisitDummyNode(builder, node);
        }
    }
}