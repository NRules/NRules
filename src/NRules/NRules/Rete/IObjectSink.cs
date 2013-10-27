namespace NRules.Rete
{
    internal interface IObjectSink
    {
        void PropagateAssert(IWorkingMemory workingMemory, Fact fact);
        void PropagateUpdate(IWorkingMemory workingMemory, Fact fact);
        void PropagateRetract(IWorkingMemory workingMemory, Fact fact);
    }
}