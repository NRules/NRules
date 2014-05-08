using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NRules.Rete
{
    internal class JoinNode : BetaNode
    {
        public JoinNode(ITupleSource leftSource, IObjectSource rightSource)
            : base(leftSource, rightSource)
        {
            Conditions = new List<BetaCondition>();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<BetaCondition> Conditions { get; private set; }

        public override void PropagateAssert(IExecutionContext context, Tuple leftTuple)
        {
            IEnumerable<Fact> rightFacts = RightSource.GetFacts(context);
            foreach (Fact rightFact in rightFacts)
            {
                if (MatchesConditions(leftTuple, rightFact))
                {
                    PropagateMatchedAssert(context, leftTuple, rightFact);
                }
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            IEnumerable<Fact> rightFacts = RightSource.GetFacts(context);
            foreach (Fact rightFact in rightFacts)
            {
                if (MatchesConditions(tuple, rightFact))
                {
                    PropagateMatchedUpdate(context, tuple, rightFact);
                }
                else
                {
                    PropagateMatchedRetract(context, tuple, rightFact);
                }
            }
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            IEnumerable<Fact> rightFacts = RightSource.GetFacts(context);
            foreach (Fact rightFact in rightFacts)
            {
                PropagateMatchedRetract(context, tuple, rightFact);
            }
        }

        public override void PropagateAssert(IExecutionContext context, Fact rightFact)
        {
            IEnumerable<Tuple> leftTuples = LeftSource.GetTuples(context);
            foreach (Tuple leftTuple in leftTuples)
            {
                if (MatchesConditions(leftTuple, rightFact))
                {
                    PropagateMatchedAssert(context, leftTuple, rightFact);
                }
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            IEnumerable<Tuple> leftTuples = LeftSource.GetTuples(context);
            foreach (Tuple leftTuple in leftTuples)
            {
                if (MatchesConditions(leftTuple, fact))
                {
                    PropagateMatchedUpdate(context, leftTuple, fact);
                }
                else
                {
                    PropagateMatchedRetract(context, leftTuple, fact);
                }
            }
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            IEnumerable<Tuple> leftTuples = LeftSource.GetTuples(context);
            foreach (Tuple leftTuple in leftTuples)
            {
                PropagateMatchedRetract(context, leftTuple, fact);
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitJoinNode(context, this);
        }

        private void PropagateMatchedAssert(IExecutionContext context, Tuple leftTuple, Fact rightFact)
        {
            var newTuple = new Tuple(leftTuple, rightFact);
            Sink.PropagateAssert(context, newTuple);
        }

        private void PropagateMatchedUpdate(IExecutionContext context, Tuple leftTuple, Fact rightFact)
        {
            Tuple tuple = leftTuple.ChildTuples.FirstOrDefault(t => t.RightFact == rightFact);
            if (tuple == null)
            {
                PropagateMatchedAssert(context, leftTuple, rightFact);
            }
            else
            {
                Sink.PropagateUpdate(context, tuple);
            }
        }

        private void PropagateMatchedRetract(IExecutionContext context, Tuple leftTuple, Fact rightFact)
        {
            Tuple tuple = leftTuple.ChildTuples.FirstOrDefault(t => t.RightFact == rightFact);
            if (tuple != null)
            {
                Sink.PropagateRetract(context, tuple);
                tuple.Clear();
            }
        }

        private bool MatchesConditions(Tuple left, Fact right)
        {
            if (left == null) return true;

            return Conditions.All(joinCondition => joinCondition.IsSatisfiedBy(left, right));
        }
    }
}