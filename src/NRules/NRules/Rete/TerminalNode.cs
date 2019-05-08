using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface ITerminalNode
    {
        void Attach(IRuleNode ruleNode);
    }

    internal class TerminalNode : ITerminalNode, ITupleSink
    {
        public IndexMap FactMap { get; }
        public IRuleNode RuleNode { get; private set; }

        public TerminalNode(ITupleSource source, IndexMap factMap)
        {
            FactMap = factMap;
            source.Attach(this);
        }

        public void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                RuleNode.PropagateAssert(context, tuple, FactMap);
            }
        }

        public void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                RuleNode.PropagateUpdate(context, tuple, FactMap);
            }
        }

        public void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                RuleNode.PropagateRetract(context, tuple, FactMap);
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