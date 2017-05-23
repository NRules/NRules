namespace NRules.Rete
{
    internal class ReteNodeVisitor<TContext>
    {
        public virtual void Visit(TContext context, INode node)
        {
            node.Accept(context, this);
        }

        protected internal virtual void VisitRootNode(TContext builder, RootNode node)
        {
            VisitAlphaNode(builder, node);
        }

        protected internal virtual void VisitTypeNode(TContext builder, TypeNode node)
        {
            VisitAlphaNode(builder, node);
        }

        protected internal virtual void VisitSelectionNode(TContext builder, SelectionNode node)
        {
            VisitAlphaNode(builder, node);
        }

        protected internal virtual void VisitAlphaMemoryNode(TContext builder, AlphaMemoryNode node)
        {
            foreach (var objectSink in node.Sinks)
            {
                Visit(builder, objectSink);
            }
        }

        protected virtual void VisitAlphaNode(TContext builder, AlphaNode node)
        {
            foreach (var childNode in node.ChildNodes)
            {
                Visit(builder, childNode);
            }
            if (node.MemoryNode != null)
            {
                Visit(builder, node.MemoryNode);
            }
        }

        protected internal virtual void VisitAggregateNode(TContext builder, AggregateNode node)
        {
            VisitBetaNode(builder, node);
        }

        protected internal virtual void VisitNotNode(TContext builder, NotNode node)
        {
            VisitBetaNode(builder, node);
        }

        protected internal virtual void VisitJoinNode(TContext builder, JoinNode node)
        {
            VisitBetaNode(builder, node);
        }

        protected internal virtual void VisitExistsNode(TContext builder, ExistsNode node)
        {
            VisitBetaNode(builder, node);
        }

        protected internal virtual void VisitTerminalNode(TContext builder, TerminalNode node)
        {
            Visit(builder, node.RuleNode);
        }

        protected internal virtual void VisitObjectInputAdapter(TContext builder, ObjectInputAdapter node)
        {
            foreach (var objectSink in node.Sinks)
            {
                Visit(builder, objectSink);
            }
        }

        protected internal virtual void VisitBetaMemoryNode(TContext builder, BetaMemoryNode node)
        {
            foreach (var tupleSink in node.Sinks)
            {
                Visit(builder, tupleSink);
            }
        }

        protected virtual void VisitBetaNode(TContext builder, BetaNode node)
        {
            Visit(builder, node.MemoryNode);
        }

        protected internal virtual void VisitRuleNode(TContext builder, RuleNode node)
        {
        }

        protected internal virtual void VisitDummyNode(TContext context, DummyNode node)
        {
        }
    }
}