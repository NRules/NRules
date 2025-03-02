using NRules.Rete;

namespace NRules.Utilities;

internal interface IArgumentMap
{
    IndexMap FactMap { get; }
    int Count { get; }
}

internal class ArgumentMap(IndexMap factMap, int count) : IArgumentMap
{
    public IndexMap FactMap { get; } = factMap;
    public int Count { get; } = count;
}