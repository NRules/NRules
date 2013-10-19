namespace NRules.Core.Rete
{
    internal class ExistsNode : BetaNode
    {
        public ExistsNode(ITupleSource leftSource, IObjectSource rightSource) : base(leftSource, rightSource)
        {
        }

        protected override void PropagateMatchedAssert(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            int hitCount = leftTuple.GetStateObject<int>() + 1;
            leftTuple.SetStateObject(hitCount);
            if (hitCount == 1)
            {
                MemoryNode.PropagateAssert(workingMemory, leftTuple);
            }
        }

        protected override void PropagateMatchedUpdate(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            MemoryNode.PropagateUpdate(workingMemory, leftTuple);
        }

        protected override void PropagateMatchedRetract(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            int hitCount = leftTuple.GetStateObject<int>() - 1;
            leftTuple.SetStateObject(hitCount);
            if (hitCount == 0)
            {
                MemoryNode.PropagateRetract(workingMemory, leftTuple);
            }
        }
    }
}