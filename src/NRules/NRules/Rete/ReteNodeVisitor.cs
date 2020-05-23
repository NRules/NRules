namespace NRules.Rete
{
    internal class ReteNodeVisitor<TContext>
    {
        public virtual void Visit(TContext context, INode node)
        {
            node.Accept(context, this);
        }

        protected internal virtual void VisitRootNode(TContext context, RootNode node)
        {
            VisitAlphaNode(context, node);
        }

        protected internal virtual void VisitTypeNode(TContext context, TypeNode node)
        {
            VisitAlphaNode(context, node);
        }

        protected internal virtual void VisitSelectionNode(TContext context, SelectionNode node)
        {
            VisitAlphaNode(context, node);
        }

        protected internal virtual void VisitAlphaMemoryNode(TContext context, AlphaMemoryNode node)
        {
            foreach (var objectSink in node.Sinks)
            {
                Visit(context, objectSink);
            }
        }

        protected virtual void VisitAlphaNode(TContext context, AlphaNode node)
        {
            foreach (var childNode in node.ChildNodes)
            {
                Visit(context, childNode);
            }
            if (node.MemoryNode != null)
            {
                Visit(context, node.MemoryNode);
            }
        }

        protected internal virtual void VisitAggregateNode(TContext context, AggregateNode node)
        {
            VisitBetaNode(context, node);
        }

        protected internal virtual void VisitNotNode(TContext context, NotNode node)
        {
            VisitBetaNode(context, node);
        }

        protected internal virtual void VisitJoinNode(TContext context, JoinNode node)
        {
            VisitBetaNode(context, node);
        }

        protected internal virtual void VisitExistsNode(TContext context, ExistsNode node)
        {
            VisitBetaNode(context, node);
        }
        
        protected internal virtual void VisitObjectInputAdapter(TContext context, ObjectInputAdapter node)
        {
            foreach (var objectSink in node.Sinks)
            {
                Visit(context, objectSink);
            }
        }

        protected internal virtual void VisitBindingNode(TContext context, BindingNode node)
        {
            VisitBetaNode(context, node);
        }

        protected internal virtual void VisitBetaMemoryNode(TContext context, BetaMemoryNode node)
        {
            foreach (var tupleSink in node.Sinks)
            {
                Visit(context, tupleSink);
            }
        }

        protected virtual void VisitBetaNode(TContext context, BetaNode node)
        {
            Visit(context, node.MemoryNode);
        }

        protected internal virtual void VisitRuleNode(TContext context, RuleNode node)
        {
        }

        protected internal virtual void VisitDummyNode(TContext context, DummyNode node)
        {
        }
    }
}