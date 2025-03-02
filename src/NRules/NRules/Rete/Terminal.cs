namespace NRules.Rete;

internal interface ITerminal
{
    ITupleSource Source { get; }
    IndexMap FactMap { get; }
}

internal class Terminal(ITupleSource source, IndexMap factMap) : ITerminal
{
    public ITupleSource Source { get; } = source;
    public IndexMap FactMap { get; } = factMap;
}