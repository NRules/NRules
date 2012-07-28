using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal abstract class BetaNode : ITupleSink, ITupleSource, IObjectSink
    {
        private readonly ITupleMemory _leftSource;
        private readonly IObjectMemory _rightSource;
        protected ITupleMemory Memory { get; private set; }

        public IList<JoinConditionAdaptor> Conditions { get; private set; }

        protected BetaNode(ITupleMemory leftSource, IObjectMemory rightSource)
        {
            _leftSource = leftSource;
            _rightSource = rightSource;

            Conditions = new List<JoinConditionAdaptor>();
        }

        protected abstract void PropagateMatchedAssert(Tuple leftTuple, Fact rightFact);
        protected abstract void PropagateMatchedUpdate(Tuple leftTuple, Fact rightFact);
        protected abstract void PropagateMatchedRetract(Tuple leftTuple, Fact rightFact);

        public void PropagateAssert(Tuple leftTuple)
        {
            IEnumerable<Fact> rightFacts = _rightSource.GetFacts();
            foreach (var rightFact in rightFacts)
            {
                if (MatchesConditions(leftTuple, rightFact))
                {
                    PropagateMatchedAssert(leftTuple, rightFact);
                }
            }
        }

        public void PropagateUpdate(Tuple tuple)
        {
            IEnumerable<Fact> rightFacts = _rightSource.GetFacts();
            foreach (var rightFact in rightFacts)
            {
                if (MatchesConditions(tuple, rightFact))
                {
                    PropagateMatchedUpdate(tuple, rightFact);
                }
                else
                {
                    PropagateMatchedRetract(tuple, rightFact);
                }
            }
        }

        public void PropagateRetract(Tuple tuple)
        {
            IEnumerable<Fact> rightFacts = _rightSource.GetFacts();
            foreach (var rightFact in rightFacts)
            {
                PropagateMatchedRetract(tuple, rightFact);
            }
            tuple.Clear();
        }

        public void PropagateAssert(Fact rightFact)
        {
            IEnumerable<Tuple> leftTuples = _leftSource.GetTuples();
            foreach (var leftTuple in leftTuples)
            {
                if (MatchesConditions(leftTuple, rightFact))
                {
                    PropagateMatchedAssert(leftTuple, rightFact);
                }
            }
        }

        public void PropagateUpdate(Fact fact)
        {
            IEnumerable<Tuple> leftTuples = _leftSource.GetTuples();
            foreach (var leftTuple in leftTuples)
            {
                if (MatchesConditions(leftTuple, fact))
                {
                    PropagateMatchedUpdate(leftTuple, fact);
                }
                else
                {
                    PropagateMatchedRetract(leftTuple, fact);
                }
            }
        }

        public void PropagateRetract(Fact fact)
        {
            IEnumerable<Tuple> leftTuples = _leftSource.GetTuples();
            foreach (var leftTuple in leftTuples)
            {
                PropagateMatchedRetract(leftTuple, fact);
            }
        }

        protected Tuple CreateTuple(Tuple left, Fact right)
        {
            var newTuple = new Tuple(left, right, Memory);
            return newTuple;
        }

        private bool MatchesConditions(Tuple left, Fact right)
        {
            if (left == null) return true;

            return Conditions.All(joinCondition => joinCondition.IsSatisfiedBy(left, right));
        }

        public void Attach(ITupleMemory sink)
        {
            Memory = sink;
        }
    }
}