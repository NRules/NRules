using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal abstract class BetaNode : ITupleSink, IObjectSink
    {
        private readonly IBetaMemoryNode _leftSource;
        private readonly IAlphaMemoryNode _rightSource;
        public BetaMemoryNode MemoryNode { get; set; }

        public IList<BetaCondition> Conditions { get; private set; }

        protected BetaNode(IBetaMemoryNode leftSource, IAlphaMemoryNode rightSource)
        {
            _leftSource = leftSource;
            _rightSource = rightSource;

            Conditions = new List<BetaCondition>();
        }

        protected abstract void PropagateMatchedAssert(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact);
        protected abstract void PropagateMatchedUpdate(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact);
        protected abstract void PropagateMatchedRetract(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact);

        public void PropagateAssert(IWorkingMemory workingMemory, Tuple leftTuple)
        {
            IAlphaMemory memory = workingMemory.GetNodeMemory(_rightSource);
            IEnumerable<Fact> rightFacts = memory.Facts;
            foreach (var rightFact in rightFacts)
            {
                if (MatchesConditions(leftTuple, rightFact))
                {
                    PropagateMatchedAssert(workingMemory, leftTuple, rightFact);
                }
            }
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            IAlphaMemory memory = workingMemory.GetNodeMemory(_rightSource);
            IEnumerable<Fact> rightFacts = memory.Facts;
            foreach (var rightFact in rightFacts)
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

        public void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            IAlphaMemory memory = workingMemory.GetNodeMemory(_rightSource);
            IEnumerable<Fact> rightFacts = memory.Facts;
            foreach (var rightFact in rightFacts)
            {
                PropagateMatchedRetract(workingMemory, tuple, rightFact);
            }
            tuple.Clear();
        }

        public void PropagateAssert(IWorkingMemory workingMemory, Fact rightFact)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(_leftSource);
            IEnumerable<Tuple> leftTuples = memory.Tuples;
            foreach (var leftTuple in leftTuples)
            {
                if (MatchesConditions(leftTuple, rightFact))
                {
                    PropagateMatchedAssert(workingMemory, leftTuple, rightFact);
                }
            }
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Fact fact)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(_leftSource);
            IEnumerable<Tuple> leftTuples = memory.Tuples;
            foreach (var leftTuple in leftTuples)
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

        public void PropagateRetract(IWorkingMemory workingMemory, Fact fact)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(_leftSource);
            IEnumerable<Tuple> leftTuples = memory.Tuples;
            foreach (var leftTuple in leftTuples)
            {
                PropagateMatchedRetract(workingMemory, leftTuple, fact);
            }
        }

        protected Tuple CreateTuple(Tuple left, Fact right)
        {
            var newTuple = new Tuple(left, right);
            return newTuple;
        }

        private bool MatchesConditions(Tuple left, Fact right)
        {
            if (left == null) return true;

            return Conditions.All(joinCondition => joinCondition.IsSatisfiedBy(left, right));
        }
    }
}