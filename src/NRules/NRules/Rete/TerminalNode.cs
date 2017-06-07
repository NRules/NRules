using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface ITerminalNode
    {
        void Attach(IRuleNode ruleNode);
    }

    internal class TerminalNode : ITerminalNode, ITupleSink
    {
        public IndexMap FactIndexMap { get; }
        public IRuleNode RuleNode { get; private set; }

        public TerminalNode(ITupleSource source, IndexMap factIndexMap)
        {
            FactIndexMap = factIndexMap;
            source.Attach(this);
        }

        public void PropagateAssert(IExecutionContext context, IList<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                RuleNode.Activate(context, tuple, FactIndexMap);
            }
        }

        public void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                RuleNode.Reactivate(context, tuple, FactIndexMap);
            }
        }

        public void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                RuleNode.Deactivate(context, tuple, FactIndexMap);
            }
        }

        public void Attach(IRuleNode ruleNode)
        {
            RuleNode = ruleNode;
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitTerminalNode(context, this);
        }
    }
}