using System.Collections.Generic;
using System.Linq;

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
                    PropagateMatchedAssert(context, tuple, fact);
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
                    PropagateMatchedUpdate(context, tuple, fact);
                }
                else
                {
                    PropagateMatchedRetract(context, tuple, fact);
                }
            }
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            IEnumerable<Fact> facts = RightSource.GetFacts(context);
            foreach (Fact fact in facts)
            {
                PropagateMatchedRetract(context, tuple, fact);
            }
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            IEnumerable<Tuple> tuples = LeftSource.GetTuples(context);
            foreach (Tuple tuple in tuples)
            {
                if (MatchesConditions(tuple, fact))
                {
                    PropagateMatchedAssert(context, tuple, fact);
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
                    PropagateMatchedUpdate(context, tuple, fact);
                }
                else
                {
                    PropagateMatchedRetract(context, tuple, fact);
                }
            }
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            IEnumerable<Tuple> tuples = LeftSource.GetTuples(context);
            foreach (Tuple tuple in tuples)
            {
                PropagateMatchedRetract(context, tuple, fact);
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitJoinNode(context, this);
        }

        private void PropagateMatchedAssert(IExecutionContext context, Tuple tuple, Fact fact)
        {
            var childTuple = new Tuple(tuple, fact, this);
            MemoryNode.PropagateAssert(context, childTuple);
        }

        private void PropagateMatchedUpdate(IExecutionContext context, Tuple tuple, Fact fact)
        {
            Tuple childTuple = tuple.ChildTuples.SingleOrDefault(t => t.Node == this && t.RightFact == fact);
            if (childTuple == null)
            {
                PropagateMatchedAssert(context, tuple, fact);
            }
            else
            {
                MemoryNode.PropagateUpdate(context, childTuple);
            }
        }

        private void PropagateMatchedRetract(IExecutionContext context, Tuple tuple, Fact fact)
        {
            Tuple childTuple = tuple.ChildTuples.SingleOrDefault(t => t.Node == this && t.RightFact == fact);
            if (childTuple != null)
            {
                MemoryNode.PropagateRetract(context, childTuple);
                childTuple.Clear();
            }
        }
    }
}