using System.Linq;

namespace NRules.Rete
{
    internal class JoinNode : BetaNode
    {
        public JoinNode(ITupleSource leftSource, IObjectSource rightSource)
            : base(leftSource, rightSource)
        {
        }

        protected override void PropagateMatchedAssert(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            var newTuple = CreateTuple(leftTuple, rightFact);
            MemoryNode.PropagateAssert(workingMemory, newTuple);
        }

        protected override void PropagateMatchedUpdate(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            Tuple tuple = leftTuple.ChildTuples.FirstOrDefault(t => t.RightFact == rightFact);
            if (tuple == null)
            {
                PropagateMatchedAssert(workingMemory, leftTuple, rightFact);
            }
            else
            {
                MemoryNode.PropagateUpdate(workingMemory, tuple);
            }
        }

        protected override void PropagateMatchedRetract(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            Tuple tuple = leftTuple.ChildTuples.FirstOrDefault(t => t.RightFact == rightFact);
            if (tuple != null)
            {
                MemoryNode.PropagateRetract(workingMemory, tuple);
            }
        }
    }
}