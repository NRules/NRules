using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal class JoinNode : ITupleSink, ITupleSource, IObjectSink
    {
        private readonly ITupleMemory _leftSource;
        private readonly IObjectMemory _rightSource;
        private ITupleSink _sink;

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
                PropagateMatchingTuple(leftTuple, rightFact);
            }
        }

        public void PropagateRetract(Tuple tuple)
        {
            var childTuples = tuple.ChildTuples.ToList();
            foreach (var childTuple in childTuples)
            {
                _sink.PropagateRetract(childTuple);
            }
            tuple.Clear();
        }

        public void PropagateAssert(Fact rightFact)
        {
            IEnumerable<Tuple> leftTuples = _leftSource.GetTuples();
            foreach (var leftTuple in leftTuples)
            {
                PropagateMatchingTuple(leftTuple, rightFact);
            }
        }

        public void PropagateUpdate(Fact fact)
        {
            throw new System.NotImplementedException();
        }

        public void PropagateRetract(Fact fact)
        {
            var childTuples = fact.ChildTuples.ToList();
            foreach (var childTuple in childTuples)
            {
                _sink.PropagateRetract(childTuple);
            }
        }

        private void PropagateMatchingTuple(Tuple tuple, Fact rightFact)
        {
            if (!MatchesConditions(tuple, rightFact)) return;

            var newTuple = new Tuple(tuple, rightFact);
            _sink.PropagateAssert(newTuple);
        }

        private bool MatchesConditions(Tuple left, Fact right)
        {
            if (left == null) return true;

            return Conditions.All(joinCondition => joinCondition.IsSatisfiedBy(left, right));
        }

        public void Attach(ITupleSink sink)
        {
            _sink = sink;
        }
    }
}