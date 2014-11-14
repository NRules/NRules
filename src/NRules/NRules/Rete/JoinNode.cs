using System.Collections.Generic;

namespace NRules.Rete
{
    internal class JoinNode : BetaNode
    {
        public JoinNode(ITupleSource leftSource, IObjectSource rightSource)
            : base(leftSource, rightSource)
        {
        }

        public override void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            IEnumerable<Fact> facts = RightSource.GetFacts(context);
            foreach (Fact fact in facts)
            {
                if (MatchesConditions(tuple, fact))
                {
                    MemoryNode.PropagateAssert(context, tuple, fact);
                }
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            IEnumerable<Fact> facts = RightSource.GetFacts(context);
            foreach (Fact fact in facts)
            {
                if (MatchesConditions(tuple, fact))
                {
                    MemoryNode.PropagateUpdate(context, tuple, fact);
                }
                else
                {
                    MemoryNode.PropagateRetract(context, tuple, fact);
                }
            }
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            IEnumerable<Fact> facts = RightSource.GetFacts(context);
            foreach (Fact fact in facts)
            {
                MemoryNode.PropagateRetract(context, tuple, fact);
            }
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            IEnumerable<Tuple> tuples = LeftSource.GetTuples(context);
            foreach (Tuple tuple in tuples)
            {
                if (MatchesConditions(tuple, fact))
                {
                    MemoryNode.PropagateAssert(context, tuple, fact);
                }
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            IEnumerable<Tuple> tuples = LeftSource.GetTuples(context);
            foreach (Tuple tuple in tuples)
            {
                if (MatchesConditions(tuple, fact))
                {
                    MemoryNode.PropagateUpdate(context, tuple, fact);
                }
                else
                {
                    MemoryNode.PropagateRetract(context, tuple, fact);
                }
            }
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            IEnumerable<Tuple> tuples = LeftSource.GetTuples(context);
            foreach (Tuple tuple in tuples)
            {
                MemoryNode.PropagateRetract(context, tuple, fact);
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitJoinNode(context, this);
        }
    }
}