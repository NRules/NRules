using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal class JoinNode : ITupleSink, ITupleSource, IObjectSink
    {
        private readonly ITupleMemory _leftSource;
        private readonly IObjectMemory _rightSource;
        private ITupleMemory _memory;

        public IList<JoinConditionAdaptor> Conditions { get; private set; }

        public JoinNode(ITupleMemory leftSource, IObjectMemory rightSource)
        {
            _leftSource = leftSource;
            _rightSource = rightSource;

            leftSource.Attach(this);
            rightSource.Attach(this);

            Conditions = new List<JoinConditionAdaptor>();
        }

        public void PropagateAssert(Tuple leftTuple)
        {
            IEnumerable<Fact> rightFacts = _rightSource.GetFacts();
            foreach (var rightFact in rightFacts)
            {
                PropagateAssertedMatchingTuple(leftTuple, rightFact);
            }
        }

        public void PropagateUpdate(Tuple tuple)
        {
            var childTuples = tuple.ChildTuples.Where(t => t.Origin == _memory).ToList();
            if (childTuples.Any())
            {
                foreach (var childTuple in childTuples)
                {
                    PropagateUpdatedMatchingTuple(childTuple);
                }
            }
            else
            {
                PropagateAssert(tuple);
            }
        }

        public void PropagateRetract(Tuple tuple)
        {
            var childTuples = tuple.ChildTuples.Where(t => t.Origin == _memory).ToList();
            foreach (var childTuple in childTuples)
            {
                _memory.PropagateRetract(childTuple);
            }
            tuple.Clear();
        }

        public void PropagateAssert(Fact rightFact)
        {
            IEnumerable<Tuple> leftTuples = _leftSource.GetTuples();
            foreach (var leftTuple in leftTuples)
            {
                PropagateAssertedMatchingTuple(leftTuple, rightFact);
            }
        }

        public void PropagateUpdate(Fact fact)
        {
            var childTuples = fact.ChildTuples.Where(t => t.Origin == _memory).ToList();
            if (childTuples.Any())
            {
                foreach (var childTuple in childTuples)
                {
                    PropagateUpdatedMatchingTuple(childTuple);
                }
            }
            else
            {
                PropagateAssert(fact);
            }
        }

        public void PropagateRetract(Fact fact)
        {
            var childTuples = fact.ChildTuples.Where(t => t.Origin == _memory).ToList();
            foreach (var childTuple in childTuples)
            {
                _memory.PropagateRetract(childTuple);
            }
        }

        private void PropagateAssertedMatchingTuple(Tuple tuple, Fact rightFact)
        {
            if (!MatchesConditions(tuple, rightFact)) return;

            var newTuple = new Tuple(tuple, rightFact, _memory);
            _memory.PropagateAssert(newTuple);
        }

        private void PropagateUpdatedMatchingTuple(Tuple tuple)
        {
            if (MatchesConditions(tuple.LeftTuple, tuple.RightFact))
            {
                _memory.PropagateUpdate(tuple);
            }
            else
            {
                _memory.PropagateRetract(tuple);
            }
        }

        private bool MatchesConditions(Tuple left, Fact right)
        {
            if (left == null) return true;

            return Conditions.All(joinCondition => joinCondition.IsSatisfiedBy(left, right));
        }

        public void Attach(ITupleMemory sink)
        {
            _memory = sink;
        }
    }
}