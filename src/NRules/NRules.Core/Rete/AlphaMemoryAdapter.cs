namespace NRules.Core.Rete
{
    internal class AlphaMemoryAdapter : AlphaNode
    {
        public AlphaMemory AlphaMemory { get; private set; }

        public AlphaMemoryAdapter(AlphaMemory memory)
        {
            AlphaMemory = memory;
        }

        public override void PropagateAssert(Fact fact)
        {
            AlphaMemory.PropagateAssert(fact);
        }

        public override void PropagateUpdate(Fact fact)
        {
            AlphaMemory.PropagateUpdate(fact);
        }

        public override void PropagateRetract(Fact fact)
        {
            AlphaMemory.PropagateRetract(fact);
        }

        public override void ForceRetract(Fact fact)
        {
            PropagateRetract(fact);
        }
    }
}