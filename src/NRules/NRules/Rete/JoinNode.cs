using System.Collections.Generic;
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

        public IList<BetaCondition> Conditions { get; private set; }

        public override void PropagateAssert(IWorkingMemory workingMemory, Tuple leftTuple)
        {
            IEnumerable<Fact> rightFacts = RightSource.GetFacts(workingMemory);
            foreach (Fact rightFact in rightFacts)
            {
                if (MatchesConditions(leftTuple, rightFact))
                {
                    PropagateMatchedAssert(workingMemory, leftTuple, rightFact);
                }
            }
        }

        public override void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            IEnumerable<Fact> rightFacts = RightSource.GetFacts(workingMemory);
            foreach (Fact rightFact in rightFacts)
            {
                if (MatchesConditions(tuple, rightFact))
                {
                    PropagateMatchedUpdate(workingMemory, tuple, rightFact);
                }
                else
                {
                    PropagateMatchedRetract(workingMemory, tuple, rightFact);
                }
            }
        }

        public override void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            IEnumerable<Fact> rightFacts = RightSource.GetFacts(workingMemory);
            foreach (Fact rightFact in rightFacts)
            {
                PropagateMatchedRetract(workingMemory, tuple, rightFact);
            }
        }

        public override void PropagateAssert(IWorkingMemory workingMemory, Fact rightFact)
        {
            IEnumerable<Tuple> leftTuples = LeftSource.GetTuples(workingMemory);
            foreach (Tuple leftTuple in leftTuples)
            {
                if (MatchesConditions(leftTuple, rightFact))
                {
                    PropagateMatchedAssert(workingMemory, leftTuple, rightFact);
                }
            }
        }

        public override void PropagateUpdate(IWorkingMemory workingMemory, Fact fact)
        {
            IEnumerable<Tuple> leftTuples = LeftSource.GetTuples(workingMemory);
            foreach (Tuple leftTuple in leftTuples)
            {
                if (MatchesConditions(leftTuple, fact))
                {
                    PropagateMatchedUpdate(workingMemory, leftTuple, fact);
                }
                else
                {
                    PropagateMatchedRetract(workingMemory, leftTuple, fact);
                }
            }
        }

        public override void PropagateRetract(IWorkingMemory workingMemory, Fact fact)
        {
            IEnumerable<Tuple> leftTuples = LeftSource.GetTuples(workingMemory);
            foreach (Tuple leftTuple in leftTuples)
            {
                PropagateMatchedRetract(workingMemory, leftTuple, fact);
            }
        }

        private void PropagateMatchedAssert(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            var newTuple = new Tuple(leftTuple, rightFact);
            Sink.PropagateAssert(workingMemory, newTuple);
        }

        private void PropagateMatchedUpdate(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            Tuple tuple = leftTuple.ChildTuples.FirstOrDefault(t => t.RightFact == rightFact);
            if (tuple == null)
            {
                PropagateMatchedAssert(workingMemory, leftTuple, rightFact);
            }
            else
            {
                Sink.PropagateUpdate(workingMemory, tuple);
            }
        }

        private void PropagateMatchedRetract(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            Tuple tuple = leftTuple.ChildTuples.FirstOrDefault(t => t.RightFact == rightFact);
            if (tuple != null)
            {
                Sink.PropagateRetract(workingMemory, tuple);
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