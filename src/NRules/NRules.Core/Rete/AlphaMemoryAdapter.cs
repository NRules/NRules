namespace NRules.Core.Rete
{
    internal class AlphaMemoryAdapter : AlphaNode
    {
        private readonly AlphaMemory _memory;

        public AlphaMemoryAdapter(AlphaMemory memory)
        {
            _memory = memory;
        }

        public override void PropagateAssert(Fact fact)
        {
            _memory.PropagateAssert(fact);
        }

        public override void PropagateUpdate(Fact fact)
        {
            _memory.PropagateUpdate(fact);
        }

        public override void PropagateRetract(Fact fact)
        {
            _memory.PropagateRetract(fact);
        }
    }
}