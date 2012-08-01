namespace NRules.Core.Rete
{
    internal interface ITupleSink
    {
        void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple);
        void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple);
        void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple);
    }
}