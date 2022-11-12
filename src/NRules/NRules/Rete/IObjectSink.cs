namespace NRules.Rete;

internal interface IObjectSink : INode
{
    void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Fact> facts);
    void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Fact> facts);
    void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Fact> facts);
}