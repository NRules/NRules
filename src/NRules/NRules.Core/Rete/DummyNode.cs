namespace NRules.Core.Rete
{
    internal class DummyNode : IBetaMemoryNode
    {
        public void InitializeMemory(IBetaMemory memory)
        {
            memory.Tuples.Add(new Tuple());
        }

        public void Attach(ITupleSink sink)
        {
            //Do nothing
        }
    }
}