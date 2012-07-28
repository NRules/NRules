using System.Linq;

namespace NRules.Core.Rete
{
    internal class JoinNode : BetaNode
    {
        public JoinNode(ITupleMemory leftSource, IObjectMemory rightSource)
            : base(leftSource, rightSource)
        {
        }

        protected override void PropagateMatchedAssert(Tuple leftTuple, Fact rightFact)
        {
            var newTuple = CreateTuple(leftTuple, rightFact);
            Memory.PropagateAssert(newTuple);
        }

        protected override void PropagateMatchedUpdate(Tuple leftTuple, Fact rightFact)
        {
            Tuple tuple = leftTuple.ChildTuples.FirstOrDefault(t => t.RightFact == rightFact);
            if (tuple == null)
            {
                PropagateMatchedAssert(leftTuple, rightFact);
            }
            else
            {
                Memory.PropagateUpdate(tuple);
            }
        }

        protected override void PropagateMatchedRetract(Tuple leftTuple, Fact rightFact)
        {
            Tuple tuple = leftTuple.ChildTuples.FirstOrDefault(t => t.RightFact == rightFact);
            if (tuple != null)
            {
                Memory.PropagateRetract(tuple);
            }
        }
    }
}